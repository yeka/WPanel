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
            app_dir = dir;
        }

        public void setProcessNamePattern(string name_pattern)
        {
            proc_name_pattern = name_pattern;
        }

        public void getProcess(string process_collections) {}

        public virtual void start() { }
        public void restart() { }
        public void restartHard() { }
        public void stop() { }
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
        
    }

    public class Nginx : BaseAppServer
    {
        public Nginx(string dir, string name_pattern) : base(dir, name_pattern) { }

        public override string name
        {
            get
            {
                return "Nginx";
            }
        }

        public override void addProcess(Process proc)
        {
            if (Regex.Match(proc.ProcessName, "nginx.*").Success)
            {
                registeredApp.Add(proc);
            }
        }

        public override void start()
        {
            run("nginx.exe", "", app_dir);
        }
    }

    public class PHP : BaseAppServer
    {
        public PHP(string dir, string name_pattern) : base(dir, name_pattern) { }

        public override string name
        {
            get
            {
                return "PHP";
            }
        }

        public override void addProcess(Process proc)
        {
            if (Regex.Match(proc.ProcessName, "php.*").Success)
            {
                registeredApp.Add(proc);
            }
        }

        public override void start()
        {
            run("php-cgi.exe", "-b 127.0.0.1:9001", app_dir);
        }
    }

    public class MySQL : BaseAppServer
    {
        public MySQL(string dir, string name_pattern) : base(dir, name_pattern) { }

        public override string name
        {
            get
            {
                return "MySQL";
            }
        }

        public override void addProcess(Process proc)
        {
            if (Regex.Match(proc.ProcessName, "mysql.*").Success)
            {
                registeredApp.Add(proc);
            }

        }

        public override void start()
        {
            run(@"bin\mysqld.exe", "", app_dir);
        }
    }

    public class Apache : BaseAppServer
    {
        public Apache(string dir, string name_pattern) : base(dir, name_pattern) { }

        public override string name
        {
            get
            {
                return "Apache";
            }
        }

        public override void addProcess(Process proc)
        {
            if (Regex.Match(proc.ProcessName, "httpd.*").Success || Regex.Match(proc.ProcessName, "apache.*").Success)
            {
                registeredApp.Add(proc);
            }
        }

        public override void start()
        {
            run(@"bin\httpd.exe", "", app_dir);
        }
    }
}
