Imports System.IO
Imports System.Text.Json

Public Class ErmisFileDocument

    Public Property Format As String
    Public Property Version As Integer
    Public Property Mission As String
    Public Property CreatedUtc As String
    Public Property RecordCount As Integer
    Public Property Records As List(Of TelemetryRecord)

    Public Sub New()

        Format = "ERMIS"
        Version = 1
        Mission = "ERMIS II"

        CreatedUtc =
            DateTime.UtcNow.ToString(
                "yyyy-MM-ddTHH:mm:ss.fffZ"
            )

        Records =
            New List(Of TelemetryRecord)()

    End Sub

End Class


Public Class ErmisFileRecorder
    Implements IDisposable

    Private ReadOnly _syncLock As New Object()

    Private ReadOnly _document As ErmisFileDocument

    Private ReadOnly _filePath As String

    Private ReadOnly _jsonOptions As JsonSerializerOptions


    Public Sub New(filePath As String)

        If String.IsNullOrWhiteSpace(filePath) Then

            Throw New ArgumentException(
                "File path cannot be empty."
            )

        End If


        _filePath = filePath

        _document =
            New ErmisFileDocument()

        _jsonOptions =
            New JsonSerializerOptions()

        _jsonOptions.WriteIndented = True

    End Sub


    Public ReadOnly Property FilePath As String

        Get
            Return _filePath
        End Get

    End Property


    Public ReadOnly Property RecordCount As Integer

        Get

            SyncLock _syncLock

                Return _document.Records.Count

            End SyncLock

        End Get

    End Property


    Public Sub AddRecord(
        record As TelemetryRecord
    )

        SyncLock _syncLock

            _document.Records.Add(
                record
            )

        End SyncLock

    End Sub


    Public Sub Save()

        SyncLock _syncLock

            _document.RecordCount =
                _document.Records.Count


            Dim json As String =
                JsonSerializer.Serialize(
                    _document,
                    _jsonOptions
                )


            Dim folderPath As String =
                System.IO.Path.GetDirectoryName(
                    _filePath
                )


            If Not String.IsNullOrWhiteSpace(
                folderPath
            ) Then

                System.IO.Directory.CreateDirectory(
                    folderPath
                )

            End If


            Dim tempFile As String =
                _filePath & ".tmp"


            System.IO.File.WriteAllText(
                tempFile,
                json
            )


            If System.IO.File.Exists(
                _filePath
            ) Then

                System.IO.File.Delete(
                    _filePath
                )

            End If


            System.IO.File.Move(
                tempFile,
                _filePath
            )

        End SyncLock

    End Sub


    Public Sub Clear()

        SyncLock _syncLock

            _document.Records.Clear()

            _document.RecordCount = 0

        End SyncLock

    End Sub


    Public Sub Dispose() _
        Implements IDisposable.Dispose

        Try

            Save()

        Catch

        End Try

    End Sub

End Class