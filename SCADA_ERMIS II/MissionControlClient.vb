Option Explicit On
Option Strict On
Option Infer On

Imports System.Threading

Friend Class MissionControlClient

    Private ReadOnly _modbus As RobustModbusRtuMaster
    Private _commandId As UShort = 0US

    Friend Sub New(
        modbus As RobustModbusRtuMaster)

        _modbus = modbus

    End Sub

    Friend Function SendCommand(
        command As MissionCommand,
        Optional argument1 As UShort = 0US,
        Optional argument2 As UShort = 0US,
        Optional token As CancellationToken = Nothing
    ) As UShort

        _commandId =
            CUShort((_commandId + 1US) And &HFFFFUS)

        If _commandId = 0US Then
            _commandId = 1US
        End If

        Dim values As UShort() = {
            CUShort(command),
            _commandId,
            argument1,
            argument2
        }

        _modbus.WriteMultipleRegisters(
            MissionRegisters.Command,
            values,
            token)

        Return _commandId

    End Function

    Friend Function ReadSpectrum(
        Optional token As CancellationToken = Nothing
    ) As SpectrumFrame

        Dim header As UShort() =
            _modbus.ReadHoldingRegisters(
                MissionRegisters.SpectrumFrameHi,
                4US,
                token)

        Dim frameNumber As UInteger =
            (CUInt(header(0)) << 16) Or
            CUInt(header(1))

        Dim binCount As Integer =
            Math.Min(
                CInt(header(2)),
                MissionRegisters.MaxSpectrumBins)

        Dim result As New SpectrumFrame()

        result.FrameNumber =
            frameNumber

        result.PeakFrequencyHz =
            header(3) / 10.0

        If binCount <= 0 Then
            result.Bins =
                Array.Empty(Of Byte)()
            Return result
        End If

        Dim registerCount As Integer =
            (binCount + 1) \ 2

        Dim packed As UShort() =
            _modbus.ReadHoldingRegisters(
                MissionRegisters.SpectrumData,
                CUShort(registerCount),
                token)

        Dim bins(binCount - 1) As Byte
        Dim outputIndex As Integer = 0

        For Each value As UShort In packed

            If outputIndex < binCount Then
                bins(outputIndex) =
                    CByte((value >> 8) And &HFFUS)
                outputIndex += 1
            End If

            If outputIndex < binCount Then
                bins(outputIndex) =
                    CByte(value And &HFFUS)
                outputIndex += 1
            End If

        Next

        result.Bins = bins

        Return result

    End Function

End Class
