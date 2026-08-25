Option Explicit On
Option Strict On
Option Infer On

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.IO.Ports
Imports System.Threading

Friend NotInheritable Class RobustModbusRtuMaster
    Implements IDisposable

    Private Const CompactBatchFunction As Byte = &H42
    Private Const ProtocolVersionV1 As Byte = 1
    Private Const ProtocolVersionV2 As Byte = 2
    Private Const MaxBatchRecords As Integer = 10

    Private Const MaxAttempts As Integer = 3

    Private ReadOnly _port As SerialPort
    Private ReadOnly _slaveId As Byte
    Private ReadOnly _sync As New Object()

    Private _successStreak As Integer
    Private _failureStreak As Integer
    Private _averageRttMs As Double
    Private _lastRttMs As Integer
    Private _successfulTransactions As Long
    Private _failedTransactions As Long
    Private _retryCount As Long
    Private _adaptiveBatch As Integer = 5

    Public Sub New(portName As String, baud As Integer, slaveId As Byte)

        _slaveId = slaveId

        _port = New SerialPort(portName, baud, Parity.None, 8, StopBits.One) With {
            .Handshake = Handshake.None,
            .ReadTimeout = 100,
            .WriteTimeout = 3000,
            .DtrEnable = False,
            .RtsEnable = False,
            .ReadBufferSize = 16384,
            .WriteBufferSize = 4096
        }

    End Sub

    Public Sub Open()

        SyncLock _sync

            If _port.IsOpen Then
                Return
            End If

            _port.Open()
            Thread.Sleep(300)
            DrainInput(100)

        End SyncLock

    End Sub

    Public ReadOnly Property AdaptiveBatchSize As Integer
        Get
            Return _adaptiveBatch
        End Get
    End Property

    Public ReadOnly Property LastRttMs As Integer
        Get
            Return _lastRttMs
        End Get
    End Property

    Public ReadOnly Property AverageRttMs As Double
        Get
            Return _averageRttMs
        End Get
    End Property

    Public ReadOnly Property SuccessRatePercent As Double
        Get
            Dim total As Long = _successfulTransactions + _failedTransactions

            If total = 0 Then
                Return 100.0
            End If

            Return CDbl(_successfulTransactions) * 100.0 / CDbl(total)
        End Get
    End Property

    Public ReadOnly Property RetryCount As Long
        Get
            Return _retryCount
        End Get
    End Property

    Public Function GetLinkStatusText() As String

        Return "Batch=" & _adaptiveBatch.ToString() &
               " RTT=" & _lastRttMs.ToString() & "ms" &
               " Avg=" & _averageRttMs.ToString("F0") & "ms" &
               " Success=" & SuccessRatePercent.ToString("F1") & "%" &
               " Retries=" & _retryCount.ToString()

    End Function

    Public Function ReadTelemetryBatch(
        requestedRecords As Byte,
        cancellationToken As CancellationToken) As TelemetryBatchResponse

        If requestedRecords < 1 OrElse requestedRecords > MaxBatchRecords Then
            Throw New ArgumentOutOfRangeException(
                NameOf(requestedRecords),
                "Το compact batch επιτρέπει 1 έως 10 records.")
        End If

        SyncLock _sync

            Dim maxRequested As Integer =
                Math.Min(CInt(requestedRecords), _adaptiveBatch)

            Dim lastError As Exception = Nothing

            For attempt As Integer = 1 To MaxAttempts

                cancellationToken.ThrowIfCancellationRequested()

                Dim thisRequest As Integer = maxRequested

                If attempt = 2 Then
                    thisRequest = Math.Max(1, maxRequested \ 2)
                ElseIf attempt >= 3 Then
                    thisRequest = 1
                End If

                Dim request(7) As Byte

                request(0) = _slaveId
                request(1) = CompactBatchFunction
                PutU16(request, 2, CUShort(thisRequest))
                PutU16(request, 4, 0US)
                AppendCrc(request)

                Try
                    ' Important:
                    ' Drain only stale bytes BEFORE a brand-new transaction.
                    ' Do not wait for a long "quiet" period on LoRa.
                    If attempt = 1 Then
                        DrainInput(20)
                    End If

                    cancellationToken.ThrowIfCancellationRequested()

                    Dim timer As Stopwatch = Stopwatch.StartNew()

                    _port.Write(request, 0, request.Length)

                    Dim timeoutMs As Integer = GetTimeoutMs(attempt)

                    Dim frame As Byte() =
                        ReadCompactFrame(timeoutMs, cancellationToken)

                    timer.Stop()

                    RegisterSuccess(CInt(timer.ElapsedMilliseconds), attempt)

                    Return DecodeCompactFrame(frame)

                Catch ex As OperationCanceledException
                    Throw

                Catch ex As Exception
                    lastError = ex

                    RegisterAttemptFailure()

                    If attempt < MaxAttempts Then
                        _retryCount += 1
                        WaitWithCancellation(80 * attempt, cancellationToken)
                    End If
                End Try

            Next

            RegisterTransactionFailure()

            Throw New IOException(
                "Compact LoRa batch απέτυχε μετά από retries.",
                lastError)

        End SyncLock

    End Function

    Public Sub WriteMultipleRegisters(
        startAddress As UShort,
        values As UShort(),
        cancellationToken As CancellationToken)

        If values Is Nothing Then
            Throw New ArgumentNullException(NameOf(values))
        End If

        If values.Length < 1 OrElse values.Length > 123 Then
            Throw New ArgumentOutOfRangeException(NameOf(values))
        End If

        SyncLock _sync

            Dim dataBytes As Integer = values.Length * 2
            Dim request(9 + dataBytes - 1) As Byte

            request(0) = _slaveId
            request(1) = &H10
            PutU16(request, 2, startAddress)
            PutU16(request, 4, CUShort(values.Length))
            request(6) = CByte(dataBytes)

            For i As Integer = 0 To values.Length - 1
                PutU16(request, 7 + i * 2, values(i))
            Next

            AppendCrc(request)

            Dim lastError As Exception = Nothing

            For attempt As Integer = 1 To MaxAttempts

                cancellationToken.ThrowIfCancellationRequested()

                Try
                    If attempt = 1 Then
                        DrainInput(20)
                    End If

                    _port.Write(request, 0, request.Length)

                    Dim response As Byte() =
                        ReadFixedFrame(&H10, 8, GetTimeoutMs(attempt), cancellationToken)

                    If GetU16(response, 2) <> startAddress OrElse
                       GetU16(response, 4) <> CUShort(values.Length) Then

                        Throw New IOException("Λάθος FC16 echo.")
                    End If

                    Return

                Catch ex As OperationCanceledException
                    Throw

                Catch ex As Exception
                    lastError = ex

                    If attempt < MaxAttempts Then
                        _retryCount += 1
                        WaitWithCancellation(80 * attempt, cancellationToken)
                    End If
                End Try

            Next

            Throw New IOException(
                "FC16 ACK απέτυχε μετά από retries.",
                lastError)

        End SyncLock

    End Sub

    Private Function DecodeCompactFrame(frame As Byte()) As TelemetryBatchResponse

        If frame.Length < 26 Then
            Throw New IOException("Πολύ μικρό compact frame.")
        End If

        Dim protocolVersion As Byte = frame(2)

        If protocolVersion <> ProtocolVersionV1 AndAlso
           protocolVersion <> ProtocolVersionV2 Then

            Throw New IOException(
                "Άγνωστη έκδοση compact protocol: " & protocolVersion.ToString())
        End If

        Dim count As Integer = CInt(frame(3))

        If count < 0 OrElse count > MaxBatchRecords Then
            Throw New IOException("Λάθος record count.")
        End If

        Dim result As New TelemetryBatchResponse()

        result.DeviceStatus = GetU16(frame, 4)
        result.FifoCount = GetU16(frame, 6)
        result.FifoCapacity = GetU16(frame, 8)
        result.LastAck = GetU32(frame, 10)
        result.FirstSequence = GetU32(frame, 14)
        result.BaseTimestampMs = GetU64(frame, 18)
        result.EncodedBytes = frame.Length

        If count = 0 Then
            Return result
        End If

        Dim valueCount As Integer = If(protocolVersion = ProtocolVersionV2, 22, 15)
        Dim offset As Integer = 26
        Dim previous(valueCount - 1) As UShort

        For recordIndex As Integer = 0 To count - 1

            EnsureAvailable(frame, offset, 2)
            Dim timeDeltaMs As UShort = GetU16(frame, offset)
            offset += 2

            Dim values(valueCount - 1) As UShort

            If recordIndex = 0 Then

                EnsureAvailable(frame, offset, valueCount * 2)

                For i As Integer = 0 To valueCount - 1
                    values(i) = GetU16(frame, offset)
                    offset += 2
                Next

            Else

                Dim changeMask As UInteger

                If protocolVersion = ProtocolVersionV2 Then
                    EnsureAvailable(frame, offset, 4)
                    changeMask = GetU32(frame, offset)
                    offset += 4
                Else
                    EnsureAvailable(frame, offset, 2)
                    changeMask = GetU16(frame, offset)
                    offset += 2
                End If

                Array.Copy(previous, values, previous.Length)

                For i As Integer = 0 To valueCount - 1

                    Dim bit As UInteger = 1UI << i

                    If (changeMask And bit) = 0UI Then
                        Continue For
                    End If

                    EnsureAvailable(frame, offset, 2)

                    If i = 0 Then

                        values(i) = GetU16(frame, offset)
                        offset += 2

                    Else

                        Dim delta As Short = GetI16(frame, offset)
                        offset += 2

                        If delta = Short.MinValue Then

                            EnsureAvailable(frame, offset, 2)
                            values(i) = GetU16(frame, offset)
                            offset += 2

                        Else

                            Dim reconstructed As Integer =
                                CInt(previous(i)) + CInt(delta)

                            If reconstructed < 0 OrElse reconstructed > 65535 Then
                                Throw New IOException("Delta εκτός ορίων.")
                            End If

                            values(i) = CUShort(reconstructed)

                        End If

                    End If

                Next

            End If

            Dim r As New TelemetryRecord()

            r.Sequence = result.FirstSequence + CUInt(recordIndex)
            r.TimestampMs = result.BaseTimestampMs + CULng(timeDeltaMs)
            r.DeviceStatus = result.DeviceStatus
            r.FifoCount = result.FifoCount
            r.FifoCapacity = result.FifoCapacity
            r.LastAck = result.LastAck

            r.ValidFlags = CType(values(0), ValidFlags)

            r.SenPm1 = CSng(values(1)) / 10.0F
            r.SenPm25 = CSng(values(2)) / 10.0F
            r.SenPm4 = CSng(values(3)) / 10.0F
            r.SenPm10 = CSng(values(4)) / 10.0F

            r.SenTemperature = CSng(U16ToI16(values(5))) / 100.0F
            r.SenHumidity = CSng(values(6)) / 100.0F
            r.SenVoc = CSng(values(7)) / 10.0F
            r.SenNox = CSng(values(8)) / 10.0F
            r.SenCo2 = values(9)

            r.BmpTemperature = CSng(U16ToI16(values(10))) / 100.0F
            r.BmpPressurePa = CSng(CInt(values(11)) + 50000)
            r.BmpAltitude = CSng(U16ToI16(values(12)))

            r.ShtTemperature = CSng(U16ToI16(values(13))) / 100.0F
            r.ShtHumidity = CSng(values(14)) / 100.0F

            If protocolVersion = ProtocolVersionV2 Then
                Dim latRaw As Integer = U16PairToI32(values(15), values(16))
                Dim lonRaw As Integer = U16PairToI32(values(17), values(18))
                Dim altRaw As Integer = U16PairToI32(values(19), values(20))

                r.GpsLatitude = CDbl(latRaw) / 10000000.0R
                r.GpsLongitude = CDbl(lonRaw) / 10000000.0R
                r.GpsAltitudeM = CSng(altRaw) / 100.0F
                r.GpsSatellites = values(21)
            End If

            result.Records.Add(r)
            Array.Copy(values, previous, values.Length)

        Next

        Return result

    End Function

    Private Function ReadCompactFrame(
        timeoutMs As Integer,
        cancellationToken As CancellationToken) As Byte()

        Dim received As New List(Of Byte)()
        Dim timer As Stopwatch = Stopwatch.StartNew()

        While timer.ElapsedMilliseconds < timeoutMs

            cancellationToken.ThrowIfCancellationRequested()

            ReadAvailableBytes(received)

            Dim frame As Byte() = TryExtractCompactFrame(received)

            If frame IsNot Nothing Then
                Return frame
            End If

            WaitWithCancellation(2, cancellationToken)

        End While

        Throw New TimeoutException(
            "LoRa compact response timeout (" & timeoutMs.ToString() & " ms).")

    End Function

    Private Function TryExtractCompactFrame(
        received As List(Of Byte)) As Byte()

        Dim start As Integer = 0

        While start <= received.Count - 4

            If received(start) <> _slaveId OrElse
               received(start + 1) <> CompactBatchFunction Then

                start += 1
                Continue While
            End If

            If received.Count - start < 26 Then
                Exit While
            End If

            Dim protocolVersion As Byte = received(start + 2)

            If protocolVersion <> ProtocolVersionV1 AndAlso
               protocolVersion <> ProtocolVersionV2 Then

                start += 1
                Continue While
            End If

            Dim count As Integer = CInt(received(start + 3))

            If count < 0 OrElse count > MaxBatchRecords Then
                start += 1
                Continue While
            End If

            Dim valueCount As Integer = If(protocolVersion = ProtocolVersionV2, 22, 15)
            Dim maskBytes As Integer = If(protocolVersion = ProtocolVersionV2, 4, 2)
            Dim pos As Integer = start + 26
            Dim complete As Boolean = True

            If count > 0 Then

                Dim firstRecordBytes As Integer = 2 + valueCount * 2

                If received.Count < pos + firstRecordBytes Then
                    complete = False
                Else
                    pos += firstRecordBytes

                    For recordIndex As Integer = 1 To count - 1

                        If received.Count < pos + 2 + maskBytes Then
                            complete = False
                            Exit For
                        End If

                        pos += 2 ' time delta

                        Dim mask As UInteger

                        If protocolVersion = ProtocolVersionV2 Then
                            mask =
                                (CUInt(received(pos)) << 24) Or
                                (CUInt(received(pos + 1)) << 16) Or
                                (CUInt(received(pos + 2)) << 8) Or
                                CUInt(received(pos + 3))
                            pos += 4
                        Else
                            mask =
                                (CUInt(received(pos)) << 8) Or
                                CUInt(received(pos + 1))
                            pos += 2
                        End If

                        For i As Integer = 0 To valueCount - 1

                            If (mask And (1UI << i)) = 0UI Then
                                Continue For
                            End If

                            If received.Count < pos + 2 Then
                                complete = False
                                Exit For
                            End If

                            If i = 0 Then
                                pos += 2
                            Else
                                Dim raw As UShort =
                                    CUShort(
                                        (CUInt(received(pos)) << 8) Or
                                        CUInt(received(pos + 1)))

                                pos += 2

                                If raw = &H8000US Then
                                    If received.Count < pos + 2 Then
                                        complete = False
                                        Exit For
                                    End If

                                    pos += 2
                                End If
                            End If

                        Next

                        If Not complete Then
                            Exit For
                        End If

                    Next
                End If

            End If

            If Not complete Then
                Exit While
            End If

            Dim expectedLength As Integer = (pos - start) + 2

            If received.Count - start < expectedLength Then
                Exit While
            End If

            Dim candidate(expectedLength - 1) As Byte

            For i As Integer = 0 To expectedLength - 1
                candidate(i) = received(start + i)
            Next

            If IsValidCrc(candidate) Then

                If start > 0 Then
                    received.RemoveRange(0, start)
                End If

                Return candidate
            End If

            start += 1

        End While

        If start > 0 Then
            received.RemoveRange(0, Math.Min(start, received.Count))
        End If

        Return Nothing

    End Function

    Private Function ReadFixedFrame(
        expectedFunction As Byte,
        expectedLength As Integer,
        timeoutMs As Integer,
        cancellationToken As CancellationToken) As Byte()

        Dim received As New List(Of Byte)()
        Dim timer As Stopwatch = Stopwatch.StartNew()

        While timer.ElapsedMilliseconds < timeoutMs

            cancellationToken.ThrowIfCancellationRequested()
            ReadAvailableBytes(received)

            Dim start As Integer = 0

            While start <= received.Count - expectedLength

                If received(start) = _slaveId AndAlso
                   received(start + 1) = expectedFunction Then

                    Dim candidate(expectedLength - 1) As Byte

                    For i As Integer = 0 To expectedLength - 1
                        candidate(i) = received(start + i)
                    Next

                    If IsValidCrc(candidate) Then
                        Return candidate
                    End If
                End If

                start += 1
            End While

            WaitWithCancellation(2, cancellationToken)

        End While

        Throw New TimeoutException("Modbus response timeout.")

    End Function

    Private Sub ReadAvailableBytes(received As List(Of Byte))

        Dim available As Integer = _port.BytesToRead

        If available <= 0 Then
            Return
        End If

        Dim buffer(available - 1) As Byte
        Dim read As Integer = _port.Read(buffer, 0, buffer.Length)

        For i As Integer = 0 To read - 1
            received.Add(buffer(i))
        Next

    End Sub

    Private Sub RegisterSuccess(rttMs As Integer, attempt As Integer)

        _lastRttMs = rttMs
        _successfulTransactions += 1
        _successStreak += 1
        _failureStreak = 0

        If _averageRttMs <= 0.0 Then
            _averageRttMs = CDbl(rttMs)
        Else
            _averageRttMs = (_averageRttMs * 0.8) + (CDbl(rttMs) * 0.2)
        End If

        If attempt > 1 Then
            _successStreak = Math.Min(_successStreak, 2)
        End If

        ' Slow upgrade, fast fallback.
        If _successStreak >= 8 Then

            If _adaptiveBatch < 10 Then

                If _adaptiveBatch < 3 Then
                    _adaptiveBatch = 3
                ElseIf _adaptiveBatch < 5 Then
                    _adaptiveBatch = 5
                ElseIf _adaptiveBatch < 7 Then
                    _adaptiveBatch = 7
                Else
                    _adaptiveBatch = 10
                End If

            End If

            _successStreak = 0
        End If

    End Sub

    Private Sub RegisterAttemptFailure()

        _failureStreak += 1
        _successStreak = 0

        If _adaptiveBatch >= 10 Then
            _adaptiveBatch = 7
        ElseIf _adaptiveBatch >= 7 Then
            _adaptiveBatch = 5
        ElseIf _adaptiveBatch >= 5 Then
            _adaptiveBatch = 3
        ElseIf _adaptiveBatch >= 3 Then
            _adaptiveBatch = 1
        Else
            _adaptiveBatch = 1
        End If

    End Sub

    Private Sub RegisterTransactionFailure()

        _failedTransactions += 1
        _failureStreak += 1
        _adaptiveBatch = 1

    End Sub

    Private Function GetTimeoutMs(attempt As Integer) As Integer

        ' Compact packets are much smaller, so recovery can be fast.
        If attempt <= 1 Then
            Return 2500
        End If

        If attempt = 2 Then
            Return 3500
        End If

        Return 5000

    End Function

    Private Sub DrainInput(quietMs As Integer)

        Dim timer As Stopwatch = Stopwatch.StartNew()
        Dim lastData As Long = timer.ElapsedMilliseconds

        While timer.ElapsedMilliseconds < 250

            Dim available As Integer = _port.BytesToRead

            If available > 0 Then

                Dim buffer(available - 1) As Byte
                _port.Read(buffer, 0, buffer.Length)
                lastData = timer.ElapsedMilliseconds

            ElseIf timer.ElapsedMilliseconds - lastData >= quietMs Then

                Exit While

            Else

                Thread.Sleep(1)

            End If

        End While

    End Sub

    Private Shared Sub EnsureAvailable(
        frame As Byte(),
        offset As Integer,
        count As Integer)

        ' Last two bytes are CRC, so payload must end before them.
        If offset < 0 OrElse
           count < 0 OrElse
           offset + count > frame.Length - 2 Then

            Throw New IOException("Κομμένο compact frame.")
        End If

    End Sub

    Private Shared Sub WaitWithCancellation(
       milliseconds As Integer,
    cancellationToken As CancellationToken)

        Try
            If milliseconds <= 0 Then
                Return
            End If

            If cancellationToken.WaitHandle.WaitOne(milliseconds) Then
                cancellationToken.ThrowIfCancellationRequested()
            End If

        Catch ex As OperationCanceledException
            ' Κανονικό cancellation - δεν θεωρείται σφάλμα
            Return

        Catch ex As Exception
            ' Προαιρετικά γράψε εδώ log
            Debug.WriteLine(
            "SafeDelay error: " & ex.Message
        )
        End Try
    End Sub

    Private Shared Function U16PairToI32(high As UShort, low As UShort) As Integer

        Dim raw As UInteger =
            (CUInt(high) << 16) Or CUInt(low)

        If raw <= &H7FFFFFFFUI Then
            Return CInt(raw)
        End If

        Return CInt(CLng(raw) - 4294967296L)

    End Function

    Private Shared Sub AppendCrc(frame As Byte())

        Dim crc As UShort = Crc16(frame, 0, frame.Length - 2)

        frame(frame.Length - 2) = CByte(crc And &HFFUS)
        frame(frame.Length - 1) = CByte((crc >> 8) And &HFFUS)

    End Sub

    Private Shared Function IsValidCrc(frame As Byte()) As Boolean

        If frame Is Nothing OrElse frame.Length < 4 Then
            Return False
        End If

        Dim receivedCrc As UShort =
            CUShort(
                CUInt(frame(frame.Length - 2)) Or
                (CUInt(frame(frame.Length - 1)) << 8))

        Return receivedCrc = Crc16(frame, 0, frame.Length - 2)

    End Function

    Private Shared Function Crc16(
        data As Byte(),
        offset As Integer,
        count As Integer) As UShort

        Dim crc As UShort = &HFFFFUS

        For i As Integer = offset To offset + count - 1

            crc = CUShort(crc Xor data(i))

            For bit As Integer = 0 To 7

                If (crc And 1US) <> 0US Then
                    crc = CUShort((crc >> 1) Xor &HA001US)
                Else
                    crc = CUShort(crc >> 1)
                End If

            Next

        Next

        Return crc

    End Function

    Private Shared Sub PutU16(
        buffer As Byte(),
        offset As Integer,
        value As UShort)

        buffer(offset) = CByte((value >> 8) And &HFFUS)
        buffer(offset + 1) = CByte(value And &HFFUS)

    End Sub

    Private Shared Function GetU16(
        buffer As Byte(),
        offset As Integer) As UShort

        Return CUShort(
            (CUInt(buffer(offset)) << 8) Or
            CUInt(buffer(offset + 1)))

    End Function

    Private Shared Function GetI16(
        buffer As Byte(),
        offset As Integer) As Short

        Dim raw As UShort = GetU16(buffer, offset)

        If raw <= &H7FFFUS Then
            Return CShort(raw)
        End If

        Return CShort(CInt(raw) - 65536)

    End Function

    Private Shared Function U16ToI16(value As UShort) As Short

        If value <= &H7FFFUS Then
            Return CShort(value)
        End If

        Return CShort(CInt(value) - 65536)

    End Function

    Private Shared Function GetU32(
    buffer As Byte(),
    offset As Integer) As UInteger

        Dim high As UInteger = CUInt(GetU16(buffer, offset))
        Dim low As UInteger = CUInt(GetU16(buffer, offset + 2))

        Return (high << 16) Or low

    End Function

    Private Shared Function GetU64(
        buffer As Byte(),
        offset As Integer) As ULong

        Dim high As ULong = CULng(GetU32(buffer, offset))
        Dim low As ULong = CULng(GetU32(buffer, offset + 4))

        Return (high << 32) Or low

    End Function

    Public Sub Dispose() Implements IDisposable.Dispose

        SyncLock _sync

            If _port.IsOpen Then
                _port.Close()
            End If

            _port.Dispose()

        End SyncLock

    End Sub

End Class
