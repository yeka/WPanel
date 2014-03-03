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
        void setAppDir(string dir);
        void setProcessNamePattern(string name_pattern);
        void getProcess(string process_collections);
        void count();
        void start();
        void restart(); // Soft Reload/Restart, fallback to kill & start if n/a
        void restartHard();
        void stop(); // Soft Stop, fallback to kill if n/a
        void kill(); // Kill process
    }

    public abstract class BaseAppServer
    {
        protected string app_dir;
        protected string proc_name_pattern;

        public BaseAppServer(string dir, string name_pattern)
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

        public void getProcess(string process_collections) {}
        public void count() {}
        public void start() {}
        public void restart() {}
        public void restartHard() {}
        public void stop() {}
        public void kill() {}
    }

    public class Nginx : BaseAppServer
    {
        public Nginx(string dir, string name_pattern) : base(dir, name_pattern) { }
    }

    public class PHP : BaseAppServer
    {
        public PHP(string dir, string name_pattern) : base(dir, name_pattern) { }
    }

    public class MySQL : BaseAppServer
    {
        public MySQL(string dir, string name_pattern) : base(dir, name_pattern) { }
    }

    public class Apache : BaseAppServer
    {
        public Apache(string dir, string name_pattern) : base(dir, name_pattern) { }
    }


}
