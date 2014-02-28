Imports System.IO
Namespace Yeka.WPanel

    Public Class SimpleConfig

        Protected WithEvents filewatch As FileSystemWatcher
        Protected WithEvents timer As Timer
        Public Event FileChanged()
        Public Config As New Collection

        Public Sub New(ByVal file_name As String)
            If Not File.Exists(file_name) Then
                Throw New FileNotFoundException
            End If

            filewatch = New FileSystemWatcher
            filewatch.Filter = Path.GetFileName(file_name)
            filewatch.Path = Path.GetDirectoryName(file_name)
            filewatch.NotifyFilter = NotifyFilters.LastAccess
            filewatch.EnableRaisingEvents = True

            timer = New Timer
            timer.Interval = 250
            timer.Enabled = False

            Reload()

        End Sub

        Private Sub ConfigFileChanged(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles filewatch.Changed, filewatch.Created, filewatch.Deleted
            If timer.Enabled Then Exit Sub
            timer.Enabled = True

            If e.ChangeType = IO.WatcherChangeTypes.Changed Or _
                e.ChangeType = IO.WatcherChangeTypes.Created Or _
                e.ChangeType = IO.WatcherChangeTypes.Deleted Then

                Reload()
                RaiseEvent FileChanged()
            End If
        End Sub

        Private Sub TimeoutCheck() Handles timer.Tick
            timer.Enabled = False
        End Sub

        Public Sub Reload()
            Dim reader As New StreamReader(filewatch.Filter)
            Dim line As String
            Dim C As New Collection

            Config = New Collection

            While (reader.Peek() <> -1)
                line = reader.ReadLine()
                If line.Trim() = "" Then
                    If C.Count = 4 Then
                        Config.Add(C)
                        C = New Collection
                    End If
                    Continue While
                End If

                Dim piece As String() = line.Split("=")
                C.Add(piece(1).Trim, piece(0).Trim().ToLower())
            End While
            If C.Count = 4 Then
                Config.Add(C)
            End If
        End Sub
    End Class

End Namespace
