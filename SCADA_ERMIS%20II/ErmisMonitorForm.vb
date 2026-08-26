Imports System.Diagnostics
Imports System.IO.Ports
Imports System.Text
Imports System.Threading
Imports ErmisSensorStatusTool

Partial Public Class ErmisMonitorForm
    Inherits Form

    Private Const AckSequenceRegister As UShort = 36US
    Private Const CatchUpFifoThreshold As UShort = 20US
    Private Const CatchUpBatchSize As Integer = 10
    Private Const MaxConsoleCharacters As Integer = 500000

    Private _modbus As RobustModbusRtuMaster
    Private _cancel As CancellationTokenSource
    Private _pollTask As Task
    Private _lastDisplayedSequence As UInteger
    Private _lastAckedSequence As UInteger
    Private _lastKnownFifo As UShort = CatchUpFifoThreshold

    Private _ermisRecorder As ErmisFileRecorder
    Private _missionControl As MissionControlClient

    ' Spectrogram is read by the SAME PollLoopAsync that reads telemetry.
    ' This prevents two Modbus transactions from competing for the same serial link.
    Private _spectrogramRequested As Boolean = False
    Private _lastSpectrumFrame As UInteger = 0UI
    Private _spectrumRxCounter As UInteger = 0UI

    Private _closingAfterDisconnect As Boolean = False

    Private ReadOnly _rateTimer As Stopwatch = Stopwatch.StartNew()
    Private _rateBytes As Long
    Private _receiveKbps As Double
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ErmisMonitorForm_Load(
        sender As Object,
        e As EventArgs) Handles MyBase.Load

        RefreshPorts()
        AppendConsole("ERMIS II Compact LoRa monitor έτοιμο.")

        Try
            Dim logDirectory As String =
                System.IO.Path.Combine(
                    Application.StartupPath,
                    "ERMIS_Data")

            System.IO.Directory.CreateDirectory(logDirectory)

            Dim fileName As String =
                "Flight_" &
                DateTime.Now.ToString("yyyy-MM-dd") &
                ".ERMIS"

            Dim filePath As String =
                System.IO.Path.Combine(
                    logDirectory,
                    fileName)

            _ermisRecorder =
                New ErmisFileRecorder(filePath)

            AppendConsole(
                "ERMIS recording: " &
                fileName)

        Catch ex As Exception
            AppendConsole(
                "ERMIS recorder error: " &
                ex.Message)
        End Try

    End Sub

    Private Sub RefreshButton_Click(
        sender As Object,
        e As EventArgs) Handles refreshButton.Click

        RefreshPorts()

    End Sub

    Private Sub ClearButton_Click(
        sender As Object,
        e As EventArgs) Handles clearButton.Click

        outputConsole.Clear()

    End Sub

    Private Async Sub ConnectButton_Click(
        sender As Object,
        e As EventArgs) Handles connectButton.Click

        If _modbus IsNot Nothing Then
            Await DisconnectAsync()
            Return
        End If

        Dim portName As String = TryCast(comList.SelectedItem, String)

        If String.IsNullOrWhiteSpace(portName) Then
            MessageBox.Show("Επίλεξε πρώτα COM.")
            Return
        End If

        Try
            _modbus = New RobustModbusRtuMaster(
                portName,
                Decimal.ToInt32(baudBox.Value),
                Decimal.ToByte(slaveBox.Value))

            _modbus.Open()

            _missionControl =
                New MissionControlClient(_modbus)

            _spectrogramRequested = False
            _lastSpectrumFrame = 0UI
            _spectrumRxCounter = 0UI
            liveSpectrogram.ClearSpectrogram()
            spectrumStatusLabel.Text = "SPECTRUM STOPPED"

            _cancel = New CancellationTokenSource()

            _lastDisplayedSequence = 0UI
            _lastAckedSequence = 0UI
            _lastKnownFifo = CatchUpFifoThreshold

            _rateBytes = 0
            _receiveKbps = 0.0
            _rateTimer.Restart()

            _pollTask = PollLoopAsync(_cancel.Token)

            SetConnected(True)
            AppendConsole("Συνδέθηκε στη " & portName & ".")

        Catch ex As Exception

            If _modbus IsNot Nothing Then
                _modbus.Dispose()
            End If

            _modbus = Nothing
            SetConnected(False)
            AppendConsole("ΣΦΑΛΜΑ ΣΥΝΔΕΣΗΣ: " & ex.Message)

        End Try

    End Sub

    Private Async Function PollLoopAsync(
        token As CancellationToken) As Task

        Dim failedCycles As Integer = 0

        While Not token.IsCancellationRequested

            Dim success As Boolean = False
            Dim errorMessage As String = ""
            Dim processedRecords As Integer = 0

            Try
                Dim active As RobustModbusRtuMaster = _modbus

                If active Is Nothing Then
                    Return
                End If

                Dim automaticAck As Boolean = autoAckBox.Checked

                Dim requestedRecords As Integer

                If automaticAck AndAlso
                   _lastKnownFifo >= CatchUpFifoThreshold Then

                    requestedRecords = CatchUpBatchSize
                Else
                    requestedRecords = 1
                End If

                Dim batch As TelemetryBatchResponse =
                    Await Task.Run(
                        Function()
                            Return active.ReadTelemetryBatch(
                                CByte(requestedRecords),
                                token)
                        End Function,
                        token)

                Dim lastSequence As UInteger = 0UI

                For Each record As TelemetryRecord In batch.Records

                    If record.Sequence <> 0UI AndAlso
                       record.Sequence <> _lastDisplayedSequence Then

                        _lastDisplayedSequence = record.Sequence
                        ShowRecord(record)
                    End If

                    processedRecords += 1
                    lastSequence = record.Sequence

                Next

                If automaticAck AndAlso
                   lastSequence <> 0UI AndAlso
                   lastSequence <> _lastAckedSequence Then

                    Dim high As UShort =
                        CUShort(lastSequence >> 16)

                    Dim low As UShort =
                        CUShort(lastSequence And &HFFFFUI)

                    Await Task.Run(
                        Sub()
                            active.WriteMultipleRegisters(
                                AckSequenceRegister,
                                New UShort() {high, low},
                                token)
                        End Sub,
                        token)

                    _lastAckedSequence = lastSequence

                End If


                ' ============================================================
                ' LIVE SPECTROGRAM
                '
                ' IMPORTANT:
                ' Spectrum uses the SAME polling loop and the SAME
                ' RobustModbusRtuMaster as telemetry. There is no second
                ' background Modbus reader.
                ' ============================================================

                If _spectrogramRequested AndAlso
                   _missionControl IsNot Nothing Then

                    Try

                        Dim spectrum As SpectrumFrame =
                            Await Task.Run(
                                Function() As SpectrumFrame
                                    Return _missionControl.ReadSpectrum(token)
                                End Function,
                                token)

                        If spectrum.FrameNumber <> 0UI AndAlso
                           spectrum.FrameNumber <> _lastSpectrumFrame Then

                            _lastSpectrumFrame =
                                spectrum.FrameNumber

                            _spectrumRxCounter += 1UI

                            liveSpectrogram.AddFrame(
                                spectrum)

                            Dim binCount As Integer =
                                If(
                                    spectrum.Bins Is Nothing,
                                    0,
                                    spectrum.Bins.Length)

                            spectrumStatusLabel.Text =
                                "FRAME " &
                                spectrum.FrameNumber.ToString() &
                                " | BINS " &
                                binCount.ToString() &
                                " | PEAK " &
                                spectrum.PeakFrequencyHz.ToString("F1") &
                                " Hz"

                            ' Log periodically, not every frame, so the
                            ' console is not flooded at ~6 fps.
                            If (_spectrumRxCounter Mod 10UI) = 0UI Then

                                AppendConsole(
                                    "SPECTRUM RX: Frame=" &
                                    spectrum.FrameNumber.ToString() &
                                    " Bins=" &
                                    binCount.ToString() &
                                    " Peak=" &
                                    spectrum.PeakFrequencyHz.ToString("F1") &
                                    " Hz")

                            End If

                            If binCount = 0 Then

                                AppendConsole(
                                    "SPECTRUM WARNING: Frame received but BinCount=0.")

                            End If

                        End If

                    Catch ex As OperationCanceledException

                        Return

                    Catch ex As Exception

                        spectrumStatusLabel.Text =
                            "SPECTRUM ERROR"

                        AppendConsole(
                            "SPECTRUM READ ERROR: " &
                            ex.Message)

                    End Try

                End If


                Dim estimatedRemaining As Integer =
                    Math.Max(
                        0,
                        CInt(batch.FifoCount) -
                        If(automaticAck, processedRecords, 0))

                _lastKnownFifo =
                    CUShort(
                        Math.Min(
                            estimatedRemaining,
                            65535))

                ' Real encoded bytes instead of old 68 bytes/record estimate.
                _rateBytes += CLng(batch.EncodedBytes)

                If _rateTimer.Elapsed.TotalSeconds >= 1.0 Then

                    _receiveKbps =
                        (_rateBytes * 8.0) /
                        _rateTimer.Elapsed.TotalSeconds /
                        1000.0

                    _rateBytes = 0
                    _rateTimer.Restart()

                End If

                statusLabel.Text =
                    "ONLINE | FIFO ~" &
                    _lastKnownFifo.ToString() &
                    "/" &
                    batch.FifoCapacity.ToString() &
                    " | ACK " &
                    _lastAckedSequence.ToString() &
                    " | BATCH " &
                    processedRecords.ToString() &
                    "/" &
                    requestedRecords.ToString() &
                    " | WIRE " &
                    batch.EncodedBytes.ToString() &
                    " B | RX " &
                    _receiveKbps.ToString("F1") &
                    " kbps | " &
                    active.GetLinkStatusText()

                failedCycles = 0
                success = True

            Catch ex As OperationCanceledException

                Return

            Catch ex As Exception

                failedCycles += 1
                errorMessage = ex.Message

            End Try

            If Not success Then

                BeginInvoke(
                    Sub()
                        statusLabel.Text =
                            "LoRa retry " &
                            failedCycles.ToString()

                        AppendConsole(
                            "ΕΠΙΚΟΙΝΩΝΙΑ: " &
                            errorMessage)
                    End Sub)

            End If

            Try
                Dim delayMs As Integer

                If success Then

                    If _lastKnownFifo >= CatchUpFifoThreshold Then
                        delayMs = 5
                    Else
                        delayMs = 150
                    End If

                Else

                    ' Fast recovery, but do not flood the LoRa link.
                    delayMs = Math.Min(1000, 150 + failedCycles * 100)

                End If

                Await Task.Delay(delayMs, token)

            Catch ex As OperationCanceledException

                Return

            End Try

        End While

    End Function

    Private Sub ShowRecord(r As TelemetryRecord)

        If _ermisRecorder IsNot Nothing Then
            Try
                _ermisRecorder.AddRecord(r)

                If _ermisRecorder.RecordCount Mod 20 = 0 Then
                    _ermisRecorder.Save()
                End If

            Catch ex As Exception
                AppendConsole(
                    "ERMIS SAVE ERROR: " &
                    ex.Message)
            End Try
        End If

        UpdateDashboard(r)

        Dim sb As New StringBuilder()

        sb.AppendLine()
        sb.AppendLine(New String("="c, 82))

        sb.AppendLine(
            "SEQ: " &
            r.Sequence.ToString() &
            " | FIFO: " &
            r.FifoCount.ToString() &
            "/" &
            r.FifoCapacity.ToString() &
            " | ACK: " &
            r.LastAck.ToString())

        sb.AppendLine(
            "STATUS: 0x" &
            r.DeviceStatus.ToString("X4") &
            " | FLAGS: " &
            r.ValidFlags.ToString())

        If (r.ValidFlags And ValidFlags.Sen66) = ValidFlags.Sen66 Then

            sb.AppendLine(
                "SEN66 PM1=" &
                r.SenPm1.ToString("F1") &
                " PM2.5=" &
                r.SenPm25.ToString("F1") &
                " PM4=" &
                r.SenPm4.ToString("F1") &
                " PM10=" &
                r.SenPm10.ToString("F1"))

            sb.AppendLine(
                "      Temp=" &
                r.SenTemperature.ToString("F2") &
                " C RH=" &
                r.SenHumidity.ToString("F2") &
                "% VOC=" &
                r.SenVoc.ToString("F1") &
                " NOx=" &
                r.SenNox.ToString("F1") &
                " CO2=" &
                r.SenCo2.ToString())

        End If

        If (r.ValidFlags And ValidFlags.Bmp280) = ValidFlags.Bmp280 Then

            sb.AppendLine(
                "BMP280 Temp=" &
                r.BmpTemperature.ToString("F2") &
                " C Pressure=" &
                (r.BmpPressurePa / 100.0F).ToString("F2") &
                " hPa Altitude=" &
                r.BmpAltitude.ToString("F0") &
                " m")

        End If

        If (r.ValidFlags And ValidFlags.Sht21) = ValidFlags.Sht21 Then

            sb.AppendLine(
                "SHT21 Temp=" &
                r.ShtTemperature.ToString("F2") &
                " C RH=" &
                r.ShtHumidity.ToString("F2") &
                "%")

        End If

        If (r.ValidFlags And ValidFlags.Gps) = ValidFlags.Gps Then
            sb.AppendLine(
                "GPS Lat=" & r.GpsLatitude.ToString("F7") &
                " Lon=" & r.GpsLongitude.ToString("F7") &
                " Alt=" & r.GpsAltitudeM.ToString("F1") &
                " m Sat=" & r.GpsSatellites.ToString())
        End If

        AppendConsole(sb.ToString().TrimEnd())

    End Sub

    Private Sub UpdateDashboard(r As TelemetryRecord)

        Dim hasSen As Boolean =
            (r.ValidFlags And ValidFlags.Sen66) = ValidFlags.Sen66

        Dim hasBmp As Boolean =
            (r.ValidFlags And ValidFlags.Bmp280) = ValidFlags.Bmp280

        Dim hasSht As Boolean =
            (r.ValidFlags And ValidFlags.Sht21) = ValidFlags.Sht21

        Dim hasGps As Boolean =
            (r.ValidFlags And ValidFlags.Gps) = ValidFlags.Gps

        Temp1.Text = If(hasSen, r.SenTemperature.ToString("F2") & " °C", "-- °C")
        Temp2.Text = If(hasBmp, r.BmpTemperature.ToString("F2") & " °C", "-- °C")
        Temp3.Text = If(hasSht, r.ShtTemperature.ToString("F2") & " °C", "-- °C")

        Press1.Text = If(hasBmp, (r.BmpPressurePa / 100.0F).ToString("F2") & " hPa", "-- hPa")
        Press2.Text = If(hasBmp, r.BmpAltitude.ToString("F0") & " m", "-- m")
        Press3.Visible = False

        Pm1.Text = If(hasSen, r.SenPm1.ToString("F1") & " µg/m³", "-- µg/m³")
        Pm25.Text = If(hasSen, r.SenPm25.ToString("F1") & " µg/m³", "-- µg/m³")
        Pm5.Text = If(hasSen, r.SenPm4.ToString("F1") & " µg/m³", "-- µg/m³")
        Pm10.Text = If(hasSen, r.SenPm10.ToString("F1") & " µg/m³", "-- µg/m³")

        Label2.Text = If(hasSen, r.SenCo2.ToString() & " ppm", "-- ppm")
        VOC.Text = If(hasSen, r.SenVoc.ToString("F1"), "--")

        If hasSen Then
            RH.Text = r.SenHumidity.ToString("F2") & " %RH"
        ElseIf hasSht Then
            RH.Text = r.ShtHumidity.ToString("F2") & " %RH"
        Else
            RH.Text = "-- %RH"
        End If

        If hasGps Then
            X.Text = r.GpsLatitude.ToString("F7") & " °"
            Y.Text = r.GpsLongitude.ToString("F7") & " °"
            Z.Text = r.GpsAltitudeM.ToString("F1") & " m  ·  SAT " &
                     r.GpsSatellites.ToString()
            X.ForeColor = Color.FromArgb(0, 142, 90)
            Y.ForeColor = Color.FromArgb(0, 142, 90)
            Z.ForeColor = Color.FromArgb(0, 142, 90)
        Else
            X.Text = "NO FIX"
            Y.Text = "--"
            Z.Text = "--"
            X.ForeColor = Color.FromArgb(185, 62, 62)
            Y.ForeColor = Color.FromArgb(105, 119, 133)
            Z.ForeColor = Color.FromArgb(105, 119, 133)
        End If


        If _statusTool IsNot Nothing Then

            _statusTool.Sen66.State =
                If(hasSen, SensorState.Online, SensorState.Offline)

            _statusTool.Bmp280.State =
                If(hasBmp, SensorState.Online, SensorState.Offline)

            _statusTool.Sht21.State =
                If(hasSht, SensorState.Online, SensorState.Offline)

            If hasGps Then
                _statusTool.Gps.State = SensorState.Online
                _statusTool.Gps.Description =
                    "FIX · SAT " & r.GpsSatellites.ToString()

                _statusTool.Esp32P4.State = SensorState.Online
                _statusTool.Esp32P4.Description =
                    "GPS telemetry received"
            Else
                _statusTool.Gps.State = SensorState.Warning
                _statusTool.Gps.Description =
                    "Waiting for valid fix"

                _statusTool.Esp32P4.State = SensorState.Warning
                _statusTool.Esp32P4.Description =
                    "No GPS telemetry"
            End If

        End If

    End Sub

    Private Sub RefreshPorts()

        Dim ports As String() =
            SerialPort.GetPortNames().
            OrderBy(
                Function(p) p,
                StringComparer.OrdinalIgnoreCase).
            ToArray()

        comList.Items.Clear()
        comList.Items.AddRange(ports)

        If ports.Length > 0 Then
            comList.SelectedIndex = 0
        End If

        AppendConsole(
            If(
                ports.Length = 0,
                "Δεν βρέθηκαν COM.",
                "Βρέθηκαν: " &
                String.Join(", ", ports)))

    End Sub

    Private Async Function DisconnectAsync() As Task

        Dim source As CancellationTokenSource = _cancel
        Dim runningTask As Task = _pollTask

        _cancel = Nothing
        _pollTask = Nothing

        If source IsNot Nothing Then
            source.Cancel()
        End If

        If runningTask IsNot Nothing Then

            Try
                Await runningTask
            Catch
            End Try

        End If

        If source IsNot Nothing Then
            source.Dispose()
        End If

        _spectrogramRequested = False
        _lastSpectrumFrame = 0UI

        If _modbus IsNot Nothing Then
            _modbus.Dispose()
        End If

        _modbus = Nothing
        _missionControl = Nothing

        spectrumStatusLabel.Text =
            "SPECTRUM STOPPED"

        SetConnected(False)
        AppendConsole("Αποσυνδέθηκε.")

    End Function

    Private Async Sub ErmisMonitorForm_FormClosing(
        sender As Object,
        e As FormClosingEventArgs) Handles MyBase.FormClosing

        ' First closing pass: stop communication asynchronously,
        ' then call Close() again.
        If Not _closingAfterDisconnect AndAlso
           _modbus IsNot Nothing Then

            e.Cancel = True

            Await DisconnectAsync()

            _closingAfterDisconnect = True

            Close()

            Return

        End If


        ' Final save of the ERMIS flight file.
        If _ermisRecorder IsNot Nothing Then

            Try

                _ermisRecorder.Save()
                _ermisRecorder.Dispose()

            Catch ex As Exception

                AppendConsole(
                    "ERMIS FINAL SAVE ERROR: " &
                    ex.Message)

            Finally

                _ermisRecorder = Nothing

            End Try

        End If

    End Sub


    Private Sub SetConnected(value As Boolean)

        connectButton.Text =
            If(value, "Αποσύνδεση", "Σύνδεση")

        comList.Enabled = Not value
        refreshButton.Enabled = Not value
        baudBox.Enabled = Not value
        slaveBox.Enabled = Not value

        If value Then
            statusDot.BackColor = Color.FromArgb(0, 176, 117)
            statusLabel.ForeColor = Color.FromArgb(0, 122, 81)
            connectButton.BackColor = Color.FromArgb(204, 67, 67)
        Else
            statusLabel.Text = "OFFLINE"
            statusDot.BackColor = Color.FromArgb(145, 155, 165)
            statusLabel.ForeColor = Color.FromArgb(69, 82, 95)
            connectButton.BackColor = Color.FromArgb(0, 122, 204)
        End If


        If _statusTool IsNot Nothing Then
            If value Then
                _statusTool.LoraLink.State = SensorState.Online
                _statusTool.LoraLink.Description =
                    "Serial Modbus link active"
            Else
                _statusTool.LoraLink.State = SensorState.Offline
                _statusTool.LoraLink.Description =
                    "No ground station link"
            End If
        End If

    End Sub

    Private Sub AppendConsole(message As String)

        If InvokeRequired Then

            BeginInvoke(
                Sub()
                    AppendConsole(message)
                End Sub)

            Return

        End If

        If outputConsole.TextLength >
           MaxConsoleCharacters Then

            outputConsole.Select(
                0,
                MaxConsoleCharacters \ 2)

            outputConsole.SelectedText =
                String.Empty

        End If

        outputConsole.AppendText(
            "[" &
            DateTime.Now.ToString("HH:mm:ss.fff") &
            "] " &
            message &
            Environment.NewLine)

        outputConsole.SelectionStart =
            outputConsole.TextLength

        outputConsole.ScrollToCaret()

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles TempName.Click

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles PressName.Click

    End Sub

    Private Sub Label2_Click_1(sender As Object, e As EventArgs) Handles PartName.Click

    End Sub

    Private Sub Label2_Click_2(sender As Object, e As EventArgs) Handles Pm1.Click

    End Sub

    Private Sub Label2_Click_3(sender As Object, e As EventArgs) Handles CO2Name.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles VOCName.Click

    End Sub

    Private Sub Label3_Click_1(sender As Object, e As EventArgs) Handles XName.Click

    End Sub

    Private Sub Label3_Click_2(sender As Object, e As EventArgs) Handles YName.Click

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub _statusTool_Load(sender As Object, e As EventArgs) Handles _statusTool.Load

    End Sub

    Private Async Sub startMeasurementButton_Click(
        sender As Object,
        e As EventArgs) Handles startMeasurementButton.Click

        Await SendMissionCommandAsync(
            MissionCommand.StartMeasurement)

    End Sub


    Private Async Sub stopMeasurementButton_Click(
        sender As Object,
        e As EventArgs) Handles stopMeasurementButton.Click

        Await SendMissionCommandAsync(
            MissionCommand.StopMeasurement)

    End Sub


    Private Async Sub singleMeasurementButton_Click(
        sender As Object,
        e As EventArgs) Handles singleMeasurementButton.Click

        Await SendMissionCommandAsync(
            MissionCommand.SingleMeasurement)

    End Sub


    Private Async Sub clearBufferButton_Click(
        sender As Object,
        e As EventArgs) Handles clearBufferButton.Click

        Await SendMissionCommandAsync(
            MissionCommand.ClearBuffer)

    End Sub


    Private Async Sub startSpectrumButton_Click(
        sender As Object,
        e As EventArgs) Handles startSpectrumButton.Click

        Dim sent As Boolean =
            Await SendMissionCommandAsync(
                MissionCommand.StartSpectrogram)

        If sent Then

            _spectrogramRequested = True
            _lastSpectrumFrame = 0UI
            _spectrumRxCounter = 0UI

            spectrumStatusLabel.Text =
                "WAITING FOR SPECTRUM..."

            AppendConsole(
                "SPECTROGRAM: polling enabled.")

        End If

    End Sub


    Private Async Sub stopSpectrumButton_Click(
        sender As Object,
        e As EventArgs) Handles stopSpectrumButton.Click

        ' Stop local reads first. The STOP command itself is still
        ' serialized by RobustModbusRtuMaster.SyncLock.
        _spectrogramRequested = False

        Dim sent As Boolean =
            Await SendMissionCommandAsync(
                MissionCommand.StopSpectrogram)

        spectrumStatusLabel.Text =
            "SPECTRUM STOPPED"

        If sent Then

            AppendConsole(
                "SPECTROGRAM: stopped.")

        End If

    End Sub


    Private Async Sub clearSpectrumButton_Click(
        sender As Object,
        e As EventArgs) Handles clearSpectrumButton.Click

        Await SendMissionCommandAsync(
            MissionCommand.ClearSpectrogram)

        liveSpectrogram.ClearSpectrogram()

        _lastSpectrumFrame = 0UI
        _spectrumRxCounter = 0UI

        spectrumStatusLabel.Text =
            If(
                _spectrogramRequested,
                "WAITING FOR SPECTRUM...",
                "SPECTRUM STOPPED")

    End Sub


    Private Async Sub resetAcquisitionButton_Click(
        sender As Object,
        e As EventArgs) Handles resetAcquisitionButton.Click

        _spectrogramRequested = False

        Await SendMissionCommandAsync(
            MissionCommand.ResetAcquisition)

        liveSpectrogram.ClearSpectrogram()

        _lastSpectrumFrame = 0UI
        _spectrumRxCounter = 0UI

        spectrumStatusLabel.Text =
            "SPECTRUM STOPPED"

    End Sub


    Private Async Function SendMissionCommandAsync(
        command As MissionCommand) As Task(Of Boolean)

        If _missionControl Is Nothing Then

            AppendConsole(
                "MISSION: No SCADA connection.")

            Return False

        End If


        Try

            Dim commandId As UShort =
                Await Task.Run(
                    Function() As UShort

                        Return _missionControl.SendCommand(
                            command)

                    End Function)


            AppendConsole(
                "MISSION COMMAND: " &
                command.ToString() &
                " | ID " &
                commandId.ToString())


            Return True


        Catch ex As Exception

            AppendConsole(
                "MISSION COMMAND ERROR: " &
                ex.Message)


            Return False

        End Try

    End Function


End Class
