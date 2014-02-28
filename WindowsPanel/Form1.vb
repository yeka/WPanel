Imports System.IO
Imports System.Diagnostics

Public Class Form1
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        FileSystemWatcher1.EnableRaisingEvents = False
        Button1.Enabled = True
        Button2.Enabled = False
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        FileSystemWatcher1.Path = TextBox1.Text()

        FileSystemWatcher1.EnableRaisingEvents = True

        Button1.Enabled = False
        Button2.Enabled = True
    End Sub

    Private Sub FileSystemWatcher1_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Changed, FileSystemWatcher1.Created, FileSystemWatcher1.Deleted
        If Timer1.Enabled Then Exit Sub
        Timer1.Enabled = True
        If e.ChangeType = IO.WatcherChangeTypes.Changed Then
            TextBox2.Text &= "File " & e.FullPath & " has been modified" & vbCrLf
        End If
        If e.ChangeType = IO.WatcherChangeTypes.Created Then
            TextBox2.Text &= "File " & e.FullPath & " has been created" & vbCrLf
        End If
        If e.ChangeType = IO.WatcherChangeTypes.Deleted Then
            TextBox2.Text &= "File " & e.FullPath & " has been deleted" & vbCrLf
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
End Class
