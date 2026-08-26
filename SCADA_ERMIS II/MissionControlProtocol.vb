Option Explicit On
Option Strict On
Option Infer On

Friend Enum MissionCommand As UShort
    None = 0
    StartMeasurement = 1
    StopMeasurement = 2
    ClearBuffer = 3
    SingleMeasurement = 4
    StartSpectrogram = 5
    StopSpectrogram = 6
    ClearSpectrogram = 7
    ResetAcquisition = 8
End Enum

Friend Module MissionRegisters
    Public Const Command As UShort = 100US
    Public Const CommandId As UShort = 101US
    Public Const Argument1 As UShort = 102US
    Public Const Argument2 As UShort = 103US

    Public Const SystemState As UShort = 104US
    Public Const MeasurementState As UShort = 105US
    Public Const SpectrogramState As UShort = 106US
    Public Const CommandResult As UShort = 107US
    Public Const LastCommandId As UShort = 108US

    Public Const SpectrumFrameHi As UShort = 120US
    Public Const SpectrumFrameLo As UShort = 121US
    Public Const SpectrumBinCount As UShort = 122US
    Public Const SpectrumPeakX10 As UShort = 123US
    Public Const SpectrumData As UShort = 124US

    Public Const MaxSpectrumBins As Integer = 128
End Module

Friend Structure SpectrumFrame
    Public FrameNumber As UInteger
    Public PeakFrequencyHz As Double
    Public Bins As Byte()
End Structure
