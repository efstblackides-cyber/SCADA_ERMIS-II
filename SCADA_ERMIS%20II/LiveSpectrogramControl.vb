Option Explicit On
Option Strict On
Option Infer On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Friend Class LiveSpectrogramControl
    Inherits Control

    Private _bitmap As Bitmap = Nothing

    Private _lastFrame As UInteger = 0UI
    Private _peakFrequency As Double = 0.0
    Private _lastBinCount As Integer = 0

    Private Const HeaderHeight As Integer = 32
    Private Const ColumnsPerFrame As Integer = 3

    Public Sub New()

        DoubleBuffered = True
        ResizeRedraw = True

        BackColor = Color.Black

        MinimumSize =
            New Size(
                500,
                220
            )

    End Sub


    Public Sub ClearSpectrogram()

        If _bitmap IsNot Nothing Then

            _bitmap.Dispose()

            _bitmap = Nothing

        End If


        _lastFrame = 0UI
        _peakFrequency = 0.0
        _lastBinCount = 0

        Invalidate()

    End Sub


    Public Sub AddFrame(
        frame As SpectrumFrame
    )

        If frame.Bins Is Nothing OrElse
           frame.Bins.Length = 0 Then

            Return

        End If


        If frame.FrameNumber =
           _lastFrame Then

            Return

        End If


        _lastFrame =
            frame.FrameNumber

        _peakFrequency =
            frame.PeakFrequencyHz

        _lastBinCount =
            frame.Bins.Length


        EnsureBitmap()

        ScrollLeft()

        DrawFrameColumn(
            frame.Bins
        )


        Invalidate()

    End Sub


    Private Sub EnsureBitmap()

        Dim width As Integer =
            Math.Max(
                320,
                ClientSize.Width
            )


        Dim height As Integer =
            Math.Max(
                120,
                ClientSize.Height -
                HeaderHeight
            )


        If _bitmap IsNot Nothing AndAlso
           _bitmap.Width = width AndAlso
           _bitmap.Height = height Then

            Return

        End If


        Dim oldBitmap As Bitmap =
            _bitmap


        _bitmap =
            New Bitmap(
                width,
                height,
                PixelFormat.Format24bppRgb
            )


        Using g As Graphics =
            Graphics.FromImage(
                _bitmap
            )

            g.Clear(
                ViridisColor(
                    0
                )
            )


            If oldBitmap IsNot Nothing Then

                Dim sourceWidth As Integer =
                    Math.Min(
                        oldBitmap.Width,
                        width
                    )


                Dim sourceHeight As Integer =
                    Math.Min(
                        oldBitmap.Height,
                        height
                    )


                g.DrawImage(
                    oldBitmap,
                    New Rectangle(
                        0,
                        0,
                        sourceWidth,
                        sourceHeight
                    ),
                    New Rectangle(
                        0,
                        0,
                        sourceWidth,
                        sourceHeight
                    ),
                    GraphicsUnit.Pixel
                )

            End If

        End Using


        If oldBitmap IsNot Nothing Then

            oldBitmap.Dispose()

        End If

    End Sub


    Private Sub ScrollLeft()

        If _bitmap Is Nothing OrElse
           _bitmap.Width <= ColumnsPerFrame Then

            Return

        End If


        Using copy As Bitmap =
            DirectCast(
                _bitmap.Clone(),
                Bitmap
            )


            Using g As Graphics =
                Graphics.FromImage(
                    _bitmap
                )

                g.DrawImage(
                    copy,
                    New Rectangle(
                        0,
                        0,
                        _bitmap.Width -
                        ColumnsPerFrame,
                        _bitmap.Height
                    ),
                    New Rectangle(
                        ColumnsPerFrame,
                        0,
                        copy.Width -
                        ColumnsPerFrame,
                        copy.Height
                    ),
                    GraphicsUnit.Pixel
                )


                Using background As New SolidBrush(
                    ViridisColor(
                        0
                    )
                )

                    g.FillRectangle(
                        background,
                        _bitmap.Width -
                        ColumnsPerFrame,
                        0,
                        ColumnsPerFrame,
                        _bitmap.Height
                    )

                End Using

            End Using

        End Using

    End Sub


    Private Sub DrawFrameColumn(
        bins As Byte()
    )

        If _bitmap Is Nothing Then
            Return
        End If


        Dim x0 As Integer =
            _bitmap.Width -
            ColumnsPerFrame


        For y As Integer =
            0 To _bitmap.Height - 1


            ' Top = high frequency, bottom = low frequency.
            Dim verticalPosition As Double =
                1.0 -
                (
                    y /
                    CDbl(
                        Math.Max(
                            1,
                            _bitmap.Height - 1
                        )
                    )
                )


            Dim sourcePosition As Double =
                verticalPosition *
                (bins.Length - 1)


            Dim lowerIndex As Integer =
                CInt(
                    Math.Floor(
                        sourcePosition
                    )
                )


            Dim upperIndex As Integer =
                Math.Min(
                    lowerIndex + 1,
                    bins.Length - 1
                )


            Dim fraction As Double =
                sourcePosition -
                lowerIndex


            Dim value As Double =
                bins(lowerIndex) *
                (1.0 - fraction) +
                bins(upperIndex) *
                fraction


            Dim intensity As Integer =
                CInt(
                    Math.Round(
                        value
                    )
                )


            Dim pixelColor As Color =
                ViridisColor(
                    intensity
                )


            For x As Integer =
                x0 To
                _bitmap.Width - 1

                _bitmap.SetPixel(
                    x,
                    y,
                    pixelColor
                )

            Next

        Next

    End Sub


    Private Function ViridisColor(
        value As Integer
    ) As Color

        value =
            Math.Max(
                0,
                Math.Min(
                    255,
                    value
                )
            )


        Dim t As Double =
            value /
            255.0


        ' Compact viridis-like interpolation:
        ' dark purple -> blue -> green -> yellow.

        If t < 0.25 Then

            Dim p As Double =
                t /
                0.25


            Return Color.FromArgb(
                CInt(68 - 8 * p),
                CInt(1 + 70 * p),
                CInt(84 + 75 * p)
            )


        ElseIf t < 0.5 Then

            Dim p As Double =
                (t - 0.25) /
                0.25


            Return Color.FromArgb(
                CInt(60 - 25 * p),
                CInt(71 + 80 * p),
                CInt(159 - 25 * p)
            )


        ElseIf t < 0.75 Then

            Dim p As Double =
                (t - 0.5) /
                0.25


            Return Color.FromArgb(
                CInt(35 + 90 * p),
                CInt(151 + 55 * p),
                CInt(134 - 75 * p)
            )


        Else

            Dim p As Double =
                (t - 0.75) /
                0.25


            Return Color.FromArgb(
                CInt(125 + 128 * p),
                CInt(206 + 25 * p),
                CInt(59 - 22 * p)
            )

        End If

    End Function


    Protected Overrides Sub OnResize(
        e As EventArgs
    )

        MyBase.OnResize(
            e
        )


        If ClientSize.Width > 0 AndAlso
           ClientSize.Height > HeaderHeight Then

            EnsureBitmap()

        End If

    End Sub


    Protected Overrides Sub OnPaint(
        e As PaintEventArgs
    )

        MyBase.OnPaint(
            e
        )


        e.Graphics.Clear(
            Color.Black
        )


        e.Graphics.SmoothingMode =
            SmoothingMode.None


        e.Graphics.InterpolationMode =
            InterpolationMode.NearestNeighbor


        If _bitmap IsNot Nothing Then

            e.Graphics.DrawImage(
                _bitmap,
                New Rectangle(
                    0,
                    HeaderHeight,
                    ClientSize.Width,
                    Math.Max(
                        1,
                        ClientSize.Height -
                        HeaderHeight
                    )
                )
            )

        End If


        Using headerBrush As New SolidBrush(
            Color.FromArgb(
                230,
                5,
                7,
                12
            )
        )

            e.Graphics.FillRectangle(
                headerBrush,
                0,
                0,
                ClientSize.Width,
                HeaderHeight
            )

        End Using


        Using infoFont As New Font(
            "Consolas",
            10.0F,
            FontStyle.Bold
        )

            Using textBrush As New SolidBrush(
                Color.White
            )

                e.Graphics.DrawString(
                    "Frame " &
                    _lastFrame.ToString() &
                    "    Peak " &
                    _peakFrequency.ToString(
                        "F1"
                    ) &
                    " Hz    Bins " &
                    _lastBinCount.ToString(),
                    infoFont,
                    textBrush,
                    New PointF(
                        8.0F,
                        7.0F
                    )
                )

            End Using

        End Using


        If _lastFrame = 0UI Then

            Using textBrush As New SolidBrush(
                Color.LightGray
            )

                e.Graphics.DrawString(
                    "Waiting for spectrum...",
                    Font,
                    textBrush,
                    New PointF(
                        12.0F,
                        48.0F
                    )
                )

            End Using

        End If

    End Sub


    Protected Overrides Sub Dispose(
        disposing As Boolean
    )

        If disposing AndAlso
           _bitmap IsNot Nothing Then

            _bitmap.Dispose()

            _bitmap = Nothing

        End If


        MyBase.Dispose(
            disposing
        )

    End Sub

End Class
