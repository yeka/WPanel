Imports System.Text.RegularExpressions

Namespace Yeka.WPanel

    Public Class Runner
        Protected output As String
        Protected myprocess As New Process

        Public Sub New()
            Dim StartInfo As New System.Diagnostics.ProcessStartInfo
            StartInfo.FileName = "phpunit.bat" 'starts cmd window
            StartInfo.WorkingDirectory = "C:\"
            StartInfo.RedirectStandardInput = True
            StartInfo.RedirectStandardOutput = True
            StartInfo.UseShellExecute = False 'required to redirect
            StartInfo.CreateNoWindow = True
            myprocess.StartInfo = StartInfo
        End Sub

        Public Function Run(ByVal command As String) As String
            output = ""
            myprocess.Start()

            Dim SR As System.IO.StreamReader = myprocess.StandardOutput
            Dim SW As System.IO.StreamWriter = myprocess.StandardInput

            SW.WriteLine("composer") 'the command you wish to run.....
            SW.WriteLine("exit") 'exits command prompt window

            output = SR.ReadToEnd 'returns results of the command window

            SW.Close()
            SR.Close()

            Return cleanOutput(output)
        End Function

        Public Function cleanOutput(ByVal str As String)
            Dim rgx As New Regex("\e\[.*?m")
            str = rgx.Replace(str, "")
            rgx = New Regex("\e\[.*?K")
            str = rgx.Replace(str, "")
            'Dim lines As String() = str.Split(new String() {Environment.NewLine}, StringSplitOptions.None)
            'Array.Resize(lines, lines.Length - 1)
            'str = String.Join(Environment.NewLine, lines)
            Return str
        End Function

    End Class

End Namespace
