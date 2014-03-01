using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Yeka.WPanel
{
    public class AppRunner
    {
        protected string output;

        public String Run(string command, string arguments, string working_dir)
        {
            ProcessStartInfo start_info = new ProcessStartInfo();
            start_info.WorkingDirectory = working_dir;
            start_info.FileName = command;
            start_info.Arguments = arguments;
            start_info.RedirectStandardOutput = true;
            start_info.UseShellExecute = false;
            start_info.CreateNoWindow = true;

            this.output = "";
            Process myprocess = new Process();
            myprocess.StartInfo = start_info;
            myprocess.Start();

            StreamReader SR = myprocess.StandardOutput ;
            output = SR.ReadToEnd();                
            SR.Close();

            return this.cleanOutput(this.output);
        }

        public string cleanOutput(string str)
        {
            str = Regex.Replace(str, @"\e\[.*?m|\e\[.*?K", "");
            str = str.Replace("\r\n", "\n").Replace("\n" , Environment.NewLine);
            return str;
        }
    }
}
