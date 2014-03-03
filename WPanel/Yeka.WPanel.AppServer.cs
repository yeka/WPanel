using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Yeka.WPanel.AppServer
{
    interface AppServerInterface
    {
        public void setAppDir(string dir);
        public void setProcessNamePattern(string name_pattern);
        public void getProcess(process_collections);
        public void count();
        public void start();
        public void restart(); // Soft Reload/Restart, fallback to kill & start if n/a
        public void restartHard();
        public void stop(); // Soft Stop, fallback to kill if n/a
        public void kill(); // Kill process
    }

    public abstract class BaseAppServer
    {
        protected string app_dir;
        protected string proc_name_pattern;

        public void BaseAppServer(string dir, string name_pattern)
        {
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

        public void getProcess(process_collections) {}
        public void count() {}
        public void start() {}
        public void restart() {}
        public void restartHard() {}
        public void stop() {}
        public void kill() {}
    }

    public class Nginx : BaseAppServer
    {
    }

    public class PHP : BaseAppServer
    {
    }

    public class MySQL : BaseAppServer
    {
    }

    public class Apache : BaseAppServer
    {
    }


}
