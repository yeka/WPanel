using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Yeka.WPanel.AppServer
{
    public interface AppServerInterface
    {
        string name { get; }
        int count { get; }
        void setAppDir(string dir);
        void setProcessNamePattern(string name_pattern);
        void getProcess(string process_collections);
        void start();
        void restart(); // Soft Reload/Restart, fallback to kill & start if n/a
        void restartHard();
        void stop(); // Soft Stop, fallback to kill if n/a
        void kill(); // Kill process
        void renderConfig(string wpanel_dir);

        // Process collections
        void clearProcess();
        void addProcess(Process proc);
    }

    public abstract class BaseAppServer: AppServerInterface
    {
        protected string app_dir;
        protected string proc_name_pattern;
        protected List<Process> registeredApp;
        protected Process[] process;
        public abstract string name { get; }

        public BaseAppServer(string dir, string name_pattern)
        {
            registeredApp = new List<Process>();
            setAppDir(dir);
            setProcessNamePattern(name_pattern);
        }

        public void setAppDir(string dir)
        {
            app_dir = dir.Replace("/", Path.DirectorySeparatorChar.ToString());
            if (app_dir.Length>0 && app_dir.Substring(app_dir.Length - 1, 1) != Path.DirectorySeparatorChar.ToString())
            {
                app_dir += Path.DirectorySeparatorChar.ToString();
            }
        }

        public void setProcessNamePattern(string name_pattern)
        {
            proc_name_pattern = name_pattern;
        }

        public void getProcess(string process_collections) {}

        public virtual void renderConfig(string wpanel_dir)
        {
            string[] files = Directory.GetFiles(app_dir, "*.template", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                renderTemplateFile(file, wpanel_dir);
            }
        }

        public virtual void start() { }
        public virtual void restart() { }
        public virtual void stop() { }
        public void restartHard()
        {
            kill();
            start();
        }
        public void kill()
        {
            for (int i = 0; i < registeredApp.Count; i++)
            {
                registeredApp[i].Kill();
            }
        }

        public int count
        {
            get
            {
                return registeredApp.Count;
            }
        }

        public void clearProcess()
        {
            registeredApp.Clear();
        }

        public abstract void addProcess(Process proc);

        protected void run(string command, string arguments, string working_dir)
        {
            ProcessStartInfo start_info = new ProcessStartInfo();
            start_info.WorkingDirectory = working_dir;
            start_info.FileName = working_dir + command;
            start_info.Arguments = arguments;
            start_info.UseShellExecute = false;
            start_info.CreateNoWindow = true;

            Process myprocess = new Process();
            myprocess.StartInfo = start_info;
            myprocess.Start();
        }

        protected void renderTemplateFile(string file, string wpanel_dir)
        {
            file = file.Replace("/", Path.DirectorySeparatorChar.ToString());
            Match match = Regex.Match(file, @"(.*)\.template$");
            if (!match.Success || !File.Exists(file)) {
                return;
            }
            string outfile = match.Groups[1].Value;
            string current_dir = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;

            StreamReader r = new StreamReader(file);
            string s = r.ReadToEnd();
            r.Close();

            s = s.Replace(@"{{current_dir\}}", current_dir.Replace(Path.DirectorySeparatorChar + "", @"\"));
            s = s.Replace(@"{{current_dir\\}}", current_dir.Replace(Path.DirectorySeparatorChar + "", @"\\"));
            s = s.Replace("{{current_dir/}}", current_dir.Replace(Path.DirectorySeparatorChar + "", "/"));
            s = s.Replace("{{current_dir}}", current_dir);

            s = s.Replace(@"{{app_dir\}}", app_dir.Replace(Path.DirectorySeparatorChar + "", @"\"));
            s = s.Replace(@"{{app_dir\\}}", app_dir.Replace(Path.DirectorySeparatorChar + "", @"\\"));
            s = s.Replace("{{app_dir/}}", app_dir.Replace(Path.DirectorySeparatorChar + "", "/"));
            s = s.Replace("{{app_dir}}", app_dir);

            s = s.Replace(@"{{wpanel_dir\}}", wpanel_dir.Replace(Path.DirectorySeparatorChar + "", @"\"));
            s = s.Replace(@"{{wpanel_dir\\}}", wpanel_dir.Replace(Path.DirectorySeparatorChar + "", @"\\"));
            s = s.Replace("{{wpanel_dir/}}", wpanel_dir.Replace(Path.DirectorySeparatorChar + "", "/"));
            s = s.Replace("{{wpanel_dir}}", wpanel_dir);

            StreamWriter w = new StreamWriter(outfile);
            w.Write(s);
            w.Close();
        }
    }

    public class GeneralAppServer : BaseAppServer
    {
        protected string app_name;
        protected string process_name;
        protected string start_command;
        protected string restart_command;
        protected string stop_command;

        public GeneralAppServer(string name, string proc_name, string dir, string start_cmd, string restart_cmd, string stop_cmd, string proc_name_pattern)
        : base(dir, proc_name_pattern)
        {
            app_name = name;
            process_name = proc_name;
            start_command = start_cmd;
            restart_command = restart_cmd;
            stop_command = stop_cmd;
        }

        public override string name
        {
            get
            {
                return app_name;
            }
        }

        public override void addProcess(Process proc)
        {
            if (Regex.Match(proc.ProcessName, proc_name_pattern).Success)
            {
                registeredApp.Add(proc);
            }
        }

        public override void start()
        {
            if (start_command == "")
            {
                return;
            }
            string[] cmd = splitCommand(start_command);
            run(cmd[0], cmd[1], app_dir);
        }

        public override void restart()
        {
            if (restart_command == "")
            {
                restartHard();
                return;
            }
            string[] cmd = splitCommand(start_command);
            run(cmd[0], cmd[1], app_dir);
        }

        public override void stop()
        {
            if (stop_command == "")
            {
                kill();
                return;
            }
            string[] cmd = splitCommand(stop_command);
            run(cmd[0], cmd[1], app_dir);
        }

        protected string[] splitCommand(string command)
        {
            command += " ";
            string[] cmd = command.Split(" ".ToCharArray(), 2);
            cmd[1] = cmd[1].Trim();
            return cmd;
        }
    }
}
