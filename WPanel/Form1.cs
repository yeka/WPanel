using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Yeka.WPanel;
using System.Management;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        protected AppRunner runner;
        protected SimpleConfig config;
        protected ProcessManager pm;

        public Form1()
        {
            string config_file = Application.StartupPath + @"\watch.ini";
            try
            {
                config = new SimpleConfig(config_file);
                config.fileChanged+=new EventHandler(config_fileChanged);
                config.Sync(this);
            }
            catch (Exception)
            {
                MessageBox.Show("File not found: " + config_file);
                this.Close();
            }

            InitializeComponent();
            runner = new AppRunner();
            pm = new ProcessManager();
            pm.onProcessUpdated += new ProcessManager.ProcessUpdateEventHandler(pm_onProcessUpdated);
            pm.start();

            autoStartFileWatcher();
        }

        private void autoStartFileWatcher()
        {
            string workdir = config.getCombinedWorkingDir();
            if (workdir.Length > 0)
            {
                textBox1.Text = workdir;
                button1_Click();
            }
        }

        public void pm_onProcessUpdated(object sender, ManagementBaseObject proc, string status)
        {
            Invoke((MethodInvoker)delegate
            {
                string msg = ProcessStatus.Start.Equals(status) ? "Start: " : "End: ";
                string value = msg + proc["Name"] + " @ " + proc["ExecutablePath"] + "\r\n";

                List<string> a = pm.getProcess();
                foreach (string i in a)
                {
                    txt_debug.Text += i + "\r\n";
                }

                txt_debug.Text += value;
            });
        }

        void config_fileChanged(object sender, EventArgs e)
        {
            txt_watcher.Text = DateTime.Now.ToString() + Environment.NewLine;
            txt_watcher.Text += "Configuration's updated" + Environment.NewLine;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            checkWatcheeAction(e.FullPath);
        }

        void watcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            checkWatcheeAction(e.Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1_Click();
        }

        private void button1_Click()
        {
            button1.Visible = false;
            button2.Visible = true;
            watcher.Path = textBox1.Text.Replace("/", "\\");
            watcher.EnableRaisingEvents = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            button1.Visible = true;
            button2.Visible = false;
        }

        public void checkWatcheeAction(string path)
        {
            if (timer.Enabled)
            {
                return;
            }
            
            path = path.Replace("\\", "/");
            foreach (Dictionary<string, string> conf in config.config)
            {
                string workdir = conf["workdir"];
                string watch = conf["watch"] + "$";
                string cmd = conf["command"];
                string args = conf["arguments"];
                Match match = Regex.Match(path, watch);
                if (match.Success)
                {
                    timer.Enabled = true;
                    txt_watcher.Text = DateTime.Now.ToString() + Environment.NewLine;
                    txt_watcher.Text += "File " + path + " has been modified" + Environment.NewLine + Environment.NewLine;
                    Application.DoEvents();
                    for (int i = 0; i < match.Groups.Count; i++)
                    {
                        args = args.Replace("{" + i + "}", match.Groups[i].Value);
                    }
                    txt_watcher.Text += "Running: " + cmd + " " + args + Environment.NewLine;
                    string result = runner.Run(cmd, args, workdir);
                    processResult(result);
                    return;
                }
            }
        }

        public void processResult(string result)
        {
            txt_watcher.Text += Environment.NewLine + Environment.NewLine + result;
            if (result.IndexOf("OK") > 0)
            {
                notifier.BalloonTipText = Regex.Match(result, "OK.*").Value;
                if (notifier.BalloonTipText.IndexOf("but") > 0)
                {
                    notifier.BalloonTipIcon = ToolTipIcon.Warning;
                }
                else
                {
                    notifier.BalloonTipIcon = ToolTipIcon.Info;
                }
                notifier.BalloonTipTitle = "Success";
            }
            else if (result.IndexOf("FAILURES") > 0)
            {
                notifier.BalloonTipText = Regex.Match(result, "FAILURES.*").Value;
                notifier.BalloonTipTitle = "FAIL";
                notifier.BalloonTipIcon = ToolTipIcon.Error;
            }
            else
            {
                notifier.BalloonTipText = "Error!! Please check the test result!";
                notifier.BalloonTipTitle = "FAIL";
                notifier.BalloonTipIcon = ToolTipIcon.Error;
            }

            notifier.Icon = SystemIcons.Information;
            notifier.ShowBalloonTip(200);
        }

        public void renderTemplateFile()
        {
            string file = @"V:\temp\abc.txt.template";
            string outfile = @"V:\temp\abc.txt";
            string current_dir = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;
            string wpanel_dir = Application.StartupPath + Path.DirectorySeparatorChar;

            StreamReader r = new StreamReader(file);
            string s = r.ReadToEnd();
            r.Close();

            s = s.Replace(@"{{current_dir\}}", current_dir.Replace(Path.DirectorySeparatorChar + "", @"\"));
            s = s.Replace(@"{{current_dir\\}}", current_dir.Replace(Path.DirectorySeparatorChar + "", @"\\"));
            s = s.Replace("{{current_dir/}}", current_dir.Replace(Path.DirectorySeparatorChar + "", "/"));
            s = s.Replace("{{current_dir}}", current_dir);
            s = s.Replace(@"{{wpanel_dir\}}", wpanel_dir.Replace(Path.DirectorySeparatorChar + "", @"\"));
            s = s.Replace(@"{{wpanel_dir\\}}", wpanel_dir.Replace(Path.DirectorySeparatorChar + "", @"\\"));
            s = s.Replace("{{wpanel_dir/}}", wpanel_dir.Replace(Path.DirectorySeparatorChar + "", "/"));
            s = s.Replace("{{wpanel_dir}}", wpanel_dir);

            StreamWriter w = new StreamWriter(outfile);
            w.Write(s);
            w.Close();
        }

        void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            pm.stop();
        }


    }
}
