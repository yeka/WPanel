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
using Yeka.WPanel.AppServer;
using System.Management;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        protected AppRunner runner;
        protected SimpleConfig config;
        protected ProcessManager pm;

        private Growl.Connector.GrowlConnector growl;
        private Growl.Connector.NotificationType notificationType;
        private Growl.Connector.Application growlapp;
        private string sampleNotificationType = "SAMPLE_NOTIFICATION";

        public Form1()
        {
            string config_file = Application.StartupPath + @"\watch.ini";
            try
            {
                config = new SimpleConfig(config_file);
                config.fileChanged += new EventHandler(config_fileChanged);
                config.Sync(this);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error initializing: " + config_file + "\n" + e.Message);
                this.Close();
            }

            InitializeComponent();
            runner = new AppRunner();

            processManagerBootUp();
            growlBootUp();
            autoStartFileWatcher();
            updateProcessInfo();
        }

        private void processManagerBootUp()
        {
            string dir = "";
            string path = "";
            pm = new ProcessManager();
            
            path = Application.StartupPath + Path.DirectorySeparatorChar + "apache.ini";
            dir = File.Exists(path) ? fileGetContents(path).Trim() : "";
            if (dir != "") { MessageBox.Show(dir);  }
            pm.registerApp(new Apache(dir, ""));

            path = Application.StartupPath + Path.DirectorySeparatorChar + "nginx.ini";
            dir = File.Exists(path) ? fileGetContents(path).Trim() : "";
            if (dir != "") { MessageBox.Show(dir); }
            pm.registerApp(new Nginx(dir, ""));

            path = Application.StartupPath + Path.DirectorySeparatorChar + "php.ini";
            dir = File.Exists(path) ? fileGetContents(path).Trim() : "";
            if (dir != "")
            {
                btnPHPStart.Enabled = true;
            }
            pm.registerApp(new PHP(dir, ""));

            path = Application.StartupPath + Path.DirectorySeparatorChar + "mysql.ini";
            dir = File.Exists(path) ? fileGetContents(path).Trim() : "";
            if (dir != "") { MessageBox.Show(dir); }
            pm.registerApp(new MySQL(dir, ""));

            pm.onProcessUpdated += new ProcessManager.ProcessUpdateEventHandler(pm_onProcessUpdated);
            pm.start();
        }

        private void growlBootUp()
        {
            notificationType = new Growl.Connector.NotificationType(sampleNotificationType, "Sample Notification");

            growl = new Growl.Connector.GrowlConnector();
            growl.EncryptionAlgorithm = Growl.Connector.Cryptography.SymmetricAlgorithmType.PlainText;

            growlapp = new Growl.Connector.Application("WPanel");
            growl.Register(growlapp, new Growl.Connector.NotificationType[] { notificationType });
        }

        public void growlNotify(string title, string message, ToolTipIcon bicon)
        {
            string basedir = Application.StartupPath + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar;
            string icon = "";
            if (bicon == ToolTipIcon.Info)
            {
                icon = basedir + "Accept.png";
            }
            else if (bicon == ToolTipIcon.Warning)
            {
                icon = basedir + "Warning.png";
            }
            else if (bicon == ToolTipIcon.Error)
            {
                icon = basedir + "Delete.png";
            }
            Growl.Connector.Notification notification = new Growl.Connector.Notification(
                growlapp.Name, //application name
                notificationType.Name,  // notification name
                DateTime.Now.Ticks.ToString(), // id
                title, // title
                message, // message
                icon, // icon, image location
                false, // stricky
                Growl.Connector.Priority.Normal, // priority
                "" // coalescingID
            );
            growl.Notify(notification);
        }

        public void notify(string title, string message, ToolTipIcon  icon)
        {
            notifier.BalloonTipTitle = title;
            notifier.BalloonTipText = message;
            notifier.BalloonTipIcon = icon;
            notifier.Icon = SystemIcons.Information;
            notifier.ShowBalloonTip(200);
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
                txt_debug.AppendText(value);
                updateProcessInfo();
            });
        }

        public void updateProcessInfo()
        {
            pm.reloadProcess();
            foreach (AppServerInterface app in pm.registeredApp)
            {
                switch (app.name)
                {
                    case "Apache":
                        lbl_apache.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnApacheStop.Enabled = app.count > 0;
                        break;
                    case "Nginx":
                        lbl_nginx.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnNginxStop.Enabled = app.count > 0;
                        break;
                    case "MySQL":
                        lbl_mysql.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnMySQLStop.Enabled = app.count > 0;
                        break;
                    case "PHP":
                        lbl_php.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnPHPStop.Enabled = app.count > 0;
                        break;
                }
            }
        }

        void config_fileChanged(object sender, EventArgs e)
        {
            txt_watcher.Text = DateTime.Now.ToString() + Environment.NewLine;
            txt_watcher.Text += "Configuration's updated" + Environment.NewLine;
            autoStartFileWatcher();
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
            checkWatcheeAction(e.FullPath);
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

                if (workdir.Length > path.Length || path.Substring(0, workdir.Length).ToLower() != workdir.ToLower())
                {
                    continue;
                }

                Match match = Regex.Match(path, watch);
                if (match.Success)
                {
                    timer.Enabled = true;
                    txt_watcher.Text = DateTime.Now.ToString() + Environment.NewLine;
                    txt_watcher.Text += "File " + path + " has been modified" + Environment.NewLine + Environment.NewLine;
                    for (int i = 0; i < match.Groups.Count; i++)
                    {
                        args = args.Replace("{" + i + "}", match.Groups[i].Value);
                    }
                    txt_watcher.Text += "Running: " + cmd + " " + args + Environment.NewLine;
                    Application.DoEvents();
                    string result = runner.Run(cmd, args, workdir);
                    processResult(result);
                    return;
                }
            }
        }

        public void processResult(string result)
        {
            string title;
            string message;
            ToolTipIcon icon;

            txt_watcher.Text += Environment.NewLine + Environment.NewLine + result;
            if (result.IndexOf("OK") > 0)
            {
                message = Regex.Match(result, "OK.*").Value;
                if (message.IndexOf("but") > 0)
                {
                    icon = ToolTipIcon.Warning;
                }
                else
                {
                    icon = ToolTipIcon.Info;
                }
                title = "Success";
            }
            else if (result.IndexOf("FAILURES") > 0)
            {
                title = "FAIL";
                message = Regex.Match(result, "FAILURES.*").Value;
                icon = ToolTipIcon.Error;
            }
            else
            {
                title = "FAIL";
                message = "Error!! Please check the test result!";
                icon = ToolTipIcon.Error;
            }

            growlNotify(title, message, icon);
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

        public string fileGetContents(string fileName)
        {
            string sContents = string.Empty;
            try
            {
                if (fileName.IndexOf("http:") > -1)
                { // URL 
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);
                }
                else
                {
                    // Regular Filename 
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
            }
            return sContents;
        }

        void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            pm.stop();
        }

        private void btnNginxStop_Click(object sender, EventArgs e)
        {
            pm.get("Nginx").kill();
        }

        private void btnPHPStop_Click(object sender, EventArgs e)
        {
            pm.get("PHP").kill();
        }

        private void btnApacheStop_Click(object sender, EventArgs e)
        {
            pm.get("Apache").kill();
        }

        private void btnMySQLStop_Click(object sender, EventArgs e)
        {
            pm.get("MySQL").kill();
        }

        private void btnPHPStart_Click(object sender, EventArgs e)
        {
            pm.get("PHP").start();
        }
    }
}
