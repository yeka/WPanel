﻿using System;
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
            // filename = filename.Replace(@"\", "/");

            if (!File.Exists(filename)) {
                throw new FileNotFoundException();
            }

            file_name = filename;

            config = new List<Dictionary<string, string>>();
            filewatch = new FileSystemWatcher();
            filewatch.Filter = Path.GetFileName(file_name);
            filewatch.Path = Path.GetDirectoryName(file_name);
            filewatch.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess;
            filewatch.EnableRaisingEvents = true;

            timer = new Timer();
            timer.Interval = 200;
            timer.Enabled = false;

            setEventHandler();

            reload();
        }

        private void setEventHandler()
        {
            filewatch.Changed += new FileSystemEventHandler(OnFileChanged);
            filewatch.Created += new FileSystemEventHandler(OnFileChanged);
            timer.Tick += new EventHandler(timerTick);
        }

        void timerTick(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (timer.Enabled) {
                return;
            }
            timer.Enabled = true;

            if (e.ChangeType == WatcherChangeTypes.Changed 
                || e.ChangeType == WatcherChangeTypes.Created) {

                reload();

                EventHandler handler = fileChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        public void reload()
        {
            config.Clear();
            StreamReader reader = new StreamReader(filewatch.Filter);
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
        }

        public void Sync(System.ComponentModel.ISynchronizeInvoke i)
        {
            filewatch.SynchronizingObject = i;
        }
    }
}