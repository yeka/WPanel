Imports System.IO
Imports System.Diagnostics
Imports System.Text.RegularExpressions

Public Class Form1
    Protected runner As New Yeka.WPanel.Runner

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

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        NotifyIcon1.Icon = SystemIcons.Information
        NotifyIcon1.BalloonTipTitle = "Balloon Tip Title"
        NotifyIcon1.BalloonTipText = "Balloon Tip Text."
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Error
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(200)
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim match = System.Text.RegularExpressions.Regex.Match("Something/That/.php", "That/(.*?)\.php$")
        MsgBox(match.Success)
        MsgBox(match.Groups(1).Value)
    End Sub

    Public Sub CheckForRunner(ByVal path As String)
        Dim cmd = "phpunit.bat"
        Dim basedir = "D:/works/git/sf2-poc/"
        Dim cmd_args = "-c tests {file}"
        Dim search = "vendor/propertyguru/event/src/Guru/EventBundle/(?!Tests/)(.*)\.php"
        Dim test = "vendor/propertyguru/event/src/Guru/EventBundle/Tests/"

        Dim match = Regex.Match(path, search)
        If (match.Success) Then
            Dim args = cmd_args.Replace("{file}", test & match.Groups(1).Value & "Test.php")
            Dim result = runner.Run(cmd, args, basedir.Replace("/", "\"))
            ProcessResult(result)
        End If

        match = Regex.Match(path, test & ".*Test\.php")
        If (match.Success) Then
            Dim args = cmd_args.Replace("{file}", match.Value)
            Dim result = runner.Run(cmd, args, basedir.Replace("/", "\"))
            ProcessResult(result)
        End If

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

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged

    End Sub
End Class
