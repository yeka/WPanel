Imports System.IO
Imports System.Diagnostics
Imports System.Text.RegularExpressions

Public Class Form1
    Protected runner As New Yeka.WPanel.Runner
    Dim config As Yeka.WPanel.SimpleConfig

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        MsgBox(Application.StartupPath & "watch.ini")
        config = New Yeka.WPanel.SimpleConfig(Application.StartupPath & "\watch.ini")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        FileSystemWatcher1.EnableRaisingEvents = False
        Button1.Enabled = True
        Button2.Enabled = False
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        FileSystemWatcher1.Path = TextBox1.Text().Replace("/", "\")

        FileSystemWatcher1.EnableRaisingEvents = True

        Button1.Enabled = False
        Button2.Enabled = True
    End Sub

    Private Sub FileSystemWatcher1_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Changed, FileSystemWatcher1.Created, FileSystemWatcher1.Deleted
        If Timer1.Enabled Then Exit Sub
        Timer1.Enabled = True

        Dim path As String = e.FullPath.Replace("\", "/")
        TextBox2.Text = TimeOfDay.ToString & vbCrLf
        Application.DoEvents()

        If e.ChangeType = IO.WatcherChangeTypes.Changed Then
            TextBox2.Text &= "File " & path & " has been modified" & vbCrLf
            CheckForRunner(path)
        End If

        If e.ChangeType = IO.WatcherChangeTypes.Created Then
            TextBox2.Text &= "File " & path & " has been created" & vbCrLf
        End If

        If e.ChangeType = IO.WatcherChangeTypes.Deleted Then
            TextBox2.Text &= "File " & path & " has been deleted" & vbCrLf
        End If
    End Sub

    Private Sub FileSystemWatcher1_Renamed(ByVal sender As Object, ByVal e As System.IO.RenamedEventArgs) Handles FileSystemWatcher1.Renamed
        If Timer1.Enabled Then Exit Sub
        Timer1.Enabled = True
        TextBox2.Text &= "File" & e.OldName & " has been renamed to " & e.Name & vbCrLf
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
    End Sub

    Public Sub CheckForRunner(ByVal path As String)
        For Each I In config.Config
            Dim basedir As String = I("basedir")
            Dim watch As String = I("watch")
            Dim cmd As String = I("command")
            Dim args As String = I("arguments")

            Dim match = Regex.Match(path, watch)
            If match.Success Then
                For x = 0 To match.Groups.Count - 1
                    args = args.Replace("{" & x & "}", match.Groups(x).Value)
                Next
                Dim result As String = runner.Run(cmd, args, basedir.Replace("/", "\"))
                ProcessResult(result)
                Exit For
            End If
        Next
    End Sub

    Private Sub ProcessResult(ByVal result As String)
        TextBox2.Text &= vbCrLf & vbCrLf & result
        Dim msg = ""
        If result.IndexOf("OK") > 0 Then
            msg = Regex.Match(result, "OK.*").Value
            If msg.IndexOf("but") > 0 Then
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
            Else
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
            End If
            NotifyIcon1.BalloonTipTitle = "Success"
        ElseIf result.IndexOf("FAILURES") > 0 Then
            msg = Regex.Match(result, "FAILURES.*").Value
            NotifyIcon1.BalloonTipTitle = "FAIL"
            NotifyIcon1.BalloonTipIcon = ToolTipIcon.Error
        Else
            msg = "There are some errors, please check the test result!"
            NotifyIcon1.BalloonTipTitle = "FAIL"
            NotifyIcon1.BalloonTipIcon = ToolTipIcon.Error
        End If

        NotifyIcon1.Icon = SystemIcons.Information
        NotifyIcon1.BalloonTipText = msg
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(200)
    End Sub

End Class
