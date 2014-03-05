using System;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Yeka.WPanel.AppServer;

namespace Yeka.WPanel
{
    public class ProcessManager
    {
        protected ManagementEventWatcher startWatcher;
        protected ManagementEventWatcher stopWatcher;
        public List<AppServerInterface> registeredApp;

        public delegate void ProcessUpdateEventHandler(object sender, ManagementBaseObject proc, string status);
        public event ProcessUpdateEventHandler onProcessUpdated;

        public ProcessManager()
        {
            registeredApp = new List<AppServerInterface>();
            startWatcher = createWatcher(ProcessStatus.Start);
            stopWatcher = createWatcher(ProcessStatus.Stop);
        }

        public void start()
        {
            startWatcher.Start();
            stopWatcher.Start();
        }

        public void stop()
        {
            startWatcher.Stop();
            stopWatcher.Stop();
        }

        public void registerApp(AppServerInterface app)
        {
            registeredApp.Add(app);
        }

        public void reloadProcess()
        {
            Process[] process_list = Process.GetProcesses();
            foreach (AppServerInterface app in registeredApp)
            {
                app.clearProcess();
                foreach(Process proc in process_list)
                {
                    app.addProcess(proc);
                }
            }
        }

        public List<string> getProcess()
        {
            Process[] process_list = Process.GetProcesses();
            List<string> found = new List<string>();

            foreach(Process proc in process_list)
            {
                if (Regex.Match(proc.ProcessName, "php.*").Success)
                {
                    found.Add(proc.ProcessName);
                }
                else if (Regex.Match(proc.ProcessName, "nginx.*").Success )
                {
                    found.Add(proc.ProcessName);
                }
                else if (Regex.Match(proc.ProcessName, "apache.*").Success)
                {
                    found.Add(proc.ProcessName);
                }
                else if (Regex.Match(proc.ProcessName, "mysql.*").Success)
                {
                    found.Add(proc.ProcessName);
                }
                else if (Regex.Match(proc.ProcessName, "httpd.*").Success)
                {
                    found.Add(proc.ProcessName);
                }
            }
            return found;
        }

        private ManagementEventWatcher createWatcher(string status)
        {
            WqlEventQuery startQuery = new WqlEventQuery(
                status,
                new TimeSpan(0, 0, 1),
                "TargetInstance isa \"Win32_Process\""
                );
            ManagementEventWatcher watcher = new ManagementEventWatcher(startQuery);
            watcher.EventArrived += new EventArrivedEventHandler(onEventArrived);
            return watcher;
        }

        private void onEventArrived(object sender, EventArrivedEventArgs e)
        {
            string state = sender.Equals(startWatcher) ? ProcessStatus.Start : ProcessStatus.Stop;
            ManagementBaseObject proc = ((ManagementBaseObject)e.NewEvent["TargetInstance"]);
            onProcessUpdated(this, proc, state);
        }

        ~ProcessManager()
        {
            stop();
        }
    }

    public static class ProcessStatus
    {
        public static string Start = "__InstanceCreationEvent";
        public static string Stop = "__InstanceDeletionEvent";
    }
}