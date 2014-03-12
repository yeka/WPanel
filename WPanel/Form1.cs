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
using System.Configuration;

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
            pm = new ProcessManager();
            
            dir = Properties.Settings.Default.apache.Trim();
            btnApacheStart.Tag = dir != "";
            pm.registerApp(new GeneralAppServer("Apache", "apache", dir, @"bin\httpd.exe", "", "", "apache.*|httpd.*"));

            dir = Properties.Settings.Default.nginx.Trim();
            btnNginxStart.Tag = dir != "";
            pm.registerApp(new GeneralAppServer("Nginx", "nginx", dir, "nginx.exe", "nginx.exe -s reload", "nginx.exe -s stop", "nginx.*"));

            dir = Properties.Settings.Default.php.Trim();
            btnPHPStart.Tag = dir != "";
            pm.registerApp(new GeneralAppServer("PHP", "php", dir, "php-cgi.exe -b 127.0.0.1:9541", "", "", "php.*"));

            dir = Properties.Settings.Default.mysql.Trim();
            btnMySQLStart.Tag = dir != "";
            pm.registerApp(new GeneralAppServer("MySQL", "mysql", dir, @"bin\mysqld.exe", "", "", "mysql.*"));

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
                txt_debug.Select(txt_debug.Text.Length, 0);
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
                        btnApacheStart.Enabled = app.count == 0 && (bool)btnApacheStart.Tag;
                        break;
                    case "Nginx":
                        lbl_nginx.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnNginxStop.Enabled = app.count > 0;
                        btnNginxStart.Enabled = app.count == 0 && (bool)btnNginxStart.Tag;
                        break;
                    case "MySQL":
                        lbl_mysql.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnMySQLStop.Enabled = app.count > 0;
                        btnMySQLStart.Enabled = app.count == 0 && (bool)btnMySQLStart.Tag;
                        break;
                    case "PHP":
                        lbl_php.ForeColor = app.count > 0 ? Color.Green : Color.Black;
                        btnPHPStop.Enabled = app.count > 0;
                        btnPHPStart.Enabled = app.count == 0 && (bool)btnPHPStart.Tag;
                        break;
                }
            }
        }

        void config_fileChanged(object sender, EventArgs e)
        {
            txt_watcher.Text = DateTime.Now.ToString() + Environment.NewLine;
            txt_watcher.AppendText("Configuration's updated" + Environment.NewLine);
            txt_watcher.Select(txt_debug.Text.Length, 0);
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
                    txt_watcher.AppendText("File " + path + " has been modified" + Environment.NewLine + Environment.NewLine);
                    for (int i = 0; i < match.Groups.Count; i++)
                    {
                        args = args.Replace("{" + i + "}", match.Groups[i].Value);
                    }
                    txt_watcher.AppendText("Running: " + cmd + " " + args + Environment.NewLine);
                    txt_watcher.Select(txt_debug.Text.Length, 0);
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

            txt_watcher.AppendText(Environment.NewLine + Environment.NewLine + result);
            txt_watcher.Select(txt_debug.Text.Length, 0);
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
            pm.get("Nginx").stop();
        }

        private void btnPHPStop_Click(object sender, EventArgs e)
        {
            pm.get("PHP").stop();
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
            GeneralAppServer php = (GeneralAppServer)pm.get("PHP");
            php.renderConfig(Application.StartupPath + Path.DirectorySeparatorChar);
            php.start();
        }

        private void btnNginxStart_Click(object sender, EventArgs e)
        {
            GeneralAppServer nginx = (GeneralAppServer)pm.get("Nginx");
            nginx.renderConfig(Application.StartupPath + Path.DirectorySeparatorChar);
            nginx.start();
        }

        private void btnApacheStart_Click(object sender, EventArgs e)
        {
            GeneralAppServer apache = (GeneralAppServer)pm.get("Apache");
            apache.renderConfig(Application.StartupPath + Path.DirectorySeparatorChar);
            apache.start();
        }

        private void btnMySQLStart_Click(object sender, EventArgs e)
        {
            GeneralAppServer mysql = (GeneralAppServer)pm.get("MySQL");
            mysql.renderConfig(Application.StartupPath + Path.DirectorySeparatorChar);
            mysql.start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Show form based notification
            // FormNotify a = new FormNotify(2000);
            // a.Show();
             
            // Search files in folders
            string[] files = Directory.GetFiles(@"X:\WS\Server\", "*.ini", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                txt_debug.AppendText(file + Environment.NewLine);
            }
            txt_debug.Select(txt_debug.Text.Length, 0);
        }
    }
}
