Public Class Form2

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged, TextBox2.TextChanged
        Dim str = ""
        Dim match = System.Text.RegularExpressions.Regex.Match(TextBox1.Text, TextBox2.Text)
        str &= match.Success & vbCrLf
        For I = 0 To match.Groups.Count - 1
            str &= match.Groups(I).Value & vbCrLf
        Next
        TextBox3.Text = str
    End Sub
End Class