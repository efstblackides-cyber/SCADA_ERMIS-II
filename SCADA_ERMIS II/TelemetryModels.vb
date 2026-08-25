Option Explicit On
Option Strict On
Option Infer On

Imports System.Collections.Generic

<Flags>
Public Enum ValidFlags As UShort
    None = 0US
    Sen66 = 1US
    Bmp280 = 2US
    Sht21 = 4US
    Gps = 8US
End Enum

Public NotInheritable Class TelemetryRecord

    Public Property Sequence As UInteger
    Public Property TimestampMs As ULong

    Public Property DeviceStatus As UShort
    Public Property FifoCount As UShort
    Public Property FifoCapacity As UShort
    Public Property LastAck As UInteger

    Public Property ValidFlags As ValidFlags

    Public Property SenPm1 As Single
    Public Property SenPm25 As Single
    Public Property SenPm4 As Single
    Public Property SenPm10 As Single

    Public Property SenTemperature As Single
    Public Property SenHumidity As Single
    Public Property SenVoc As Single
    Public Property SenNox As Single
    Public Property SenCo2 As UShort

    Public Property BmpTemperature As Single
    Public Property BmpPressurePa As Single
    Public Property BmpAltitude As Single

    Public Property ShtTemperature As Single
    Public Property ShtHumidity As Single

    ' GPS values carried by compact protocol v2
    Public Property GpsLatitude As Double
    Public Property GpsLongitude As Double
    Public Property GpsAltitudeM As Single
    Public Property GpsSatellites As UShort

End Class

Public NotInheritable Class TelemetryBatchResponse

    Public Sub New()
        Records = New List(Of TelemetryRecord)()
    End Sub

    Public Property DeviceStatus As UShort
    Public Property FifoCount As UShort
    Public Property FifoCapacity As UShort
    Public Property LastAck As UInteger
    Public Property FirstSequence As UInteger
    Public Property BaseTimestampMs As ULong
    Public Property EncodedBytes As Integer

    Public ReadOnly Property Records As List(Of TelemetryRecord)

End Class
