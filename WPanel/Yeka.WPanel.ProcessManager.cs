using System;
using System.IO;
using System.Diagnostics;
using System.Management;

namespace Yeka.WPanel
{
    public class ProcessManager
    {
        protected ManagementEventWatcher startWatcher;
        protected ManagementEventWatcher stopWatcher;

        public delegate void ProcessUpdateEventHandler(object sender, ManagementBaseObject proc, string status);
        public event ProcessUpdateEventHandler onProcessUpdated;

        public ProcessManager()
        {
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