using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Yeka.WPanel
{
    public class SimpleConfig
    {
        protected FileSystemWatcher filewatch;
        protected Timer timer;
        protected string file_name;
        public List<Dictionary<string, string>> config;
        public event EventHandler fileChanged;

        public SimpleConfig(string filename)
        {
            if (!File.Exists(filename)) {
                throw new FileNotFoundException();
            }

            file_name = filename;

            config = new List<Dictionary<string, string>>();
            setSelfWatch();
            reload();
        }

        private void setSelfWatch()
        {
            filewatch = new FileSystemWatcher();
            filewatch.Path = Path.GetDirectoryName(file_name);
            filewatch.Filter = "*.*";
            filewatch.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName;
            filewatch.EnableRaisingEvents = true;

            timer = new Timer();
            timer.Interval = 200;
            timer.Enabled = false;

            setEventHandler();
        }

        private void setEventHandler()
        {
            filewatch.Changed += new FileSystemEventHandler(OnFileChanged);
            filewatch.Created += new FileSystemEventHandler(OnFileChanged);
            filewatch.Renamed += new RenamedEventHandler(OnFileChanged);
            timer.Tick += new EventHandler(timerTick);
        }

        private void timerTick(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void OnFileChanged(object sender, RenamedEventArgs e)
        {
            if (Path.GetFileName(e.FullPath) != Path.GetFileName(file_name))
            {
                return;
            }

            reload();
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (Path.GetFileName(e.FullPath) != Path.GetFileName(file_name) || timer.Enabled)
            {
                return;
            }
            timer.Enabled = true;

            if (e.ChangeType == WatcherChangeTypes.Changed 
                || e.ChangeType == WatcherChangeTypes.Created) {

                reload();
            }
        }

        public void reload()
        {
            StreamReader reader;
            System.Threading.Thread.Sleep(500); // Give a chance for editor to close config file before we use it.
            try
            {
                reader = new StreamReader(file_name);
            }
            catch (Exception)
            {
                MessageBox.Show("config in use, can't reload");
                return;
            }

            config.Clear();
            Dictionary<string, string> map = new Dictionary<string, string>();
            string line = "";
            while (reader.Peek() != -1) {
                line = reader.ReadLine();
                if (line.Trim() == "") {
                    if (map.Count  == 4) {
                        config.Add(map);
                        map = new Dictionary<string,string>();
                    }
                    continue;
                }

                string[] piece = line.Split('=');
                string key = piece[0].Trim().ToLower();
                string value = piece[1].Trim();
                map[key] = value;
            }
            if (map.Count == 4) {
                config.Add(map);
            }
            reader.Close();

            EventHandler handler = fileChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public string getCombinedWorkingDir()
        {
            string dir = null;
            foreach (Dictionary<string, string> conf in config)
            {
                if (dir == null)
                {
                    dir = conf["workdir"];
                }
                else if (dir != conf["workdir"])
                {
                    dir = getFirstEqualString(dir, conf["workdir"]);
                }
            }
            return dir;
        }

        public string getFirstEqualString(string first, string second)
        {
            string equals = "";
            int n1 = first.Length;
            int n2 = second.Length;
            int n = (n1 < n2) ? n1 : n2;
            int i = 0;
            for (i = 0; i < n; i++)
            {
                if (first.Substring(i, 1) != second.Substring(i, 1))
                {
                    break;
                }
            }
            equals = first.Substring(0, i);
            return equals;
        }


        public void Sync(System.ComponentModel.ISynchronizeInvoke i)
        {
            filewatch.SynchronizingObject = i;
        }

        ~SimpleConfig()
        {
            filewatch.EnableRaisingEvents = false;
        }
    }
}
