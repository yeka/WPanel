Imports System.Text.RegularExpressions

Namespace Yeka.WPanel

    Public Class Runner
        Protected output As String

        Public Function Run(ByVal command As String, ByVal args As String, ByVal working_dir As String) As String
            Dim StartInfo As New System.Diagnostics.ProcessStartInfo
            StartInfo.FileName = command  'starts cmd window
            StartInfo.WorkingDirectory = working_dir
            StartInfo.Arguments = args
            StartInfo.RedirectStandardInput = True
            StartInfo.RedirectStandardOutput = True
            StartInfo.UseShellExecute = False 'required to redirect
            StartInfo.CreateNoWindow = True

            output = ""
            Dim myprocess As New Process
            myprocess.StartInfo = StartInfo
            myprocess.Start()

            Dim SR As System.IO.StreamReader = myprocess.StandardOutput
            'Dim SW As System.IO.StreamWriter = myprocess.StandardInput

            'SW.WriteLine("composer") 'the command you wish to run.....
            'SW.WriteLine("exit") 'exits command prompt window

            output = SR.ReadToEnd 'returns results of the command window

            'SW.Close()
            SR.Close()

            Return cleanOutput(output)
        End Function

        Public Function cleanOutput(ByVal str As String)
            str = Regex.Replace(str, "\e\[.*?m|\e\[.*?K", "")
            str = str.Replace(Chr(13), Chr(10)).Replace(Chr(10) & Chr(10), Chr(10)).Replace(Chr(10), vbCrLf)
            Return str
        End Function

    End Class

End Namespace
