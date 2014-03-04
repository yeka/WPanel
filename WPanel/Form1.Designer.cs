namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txt_watcher = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.watcher = new System.IO.FileSystemWatcher();
            this.notifier = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txt_debug = new System.Windows.Forms.TextBox();
            this.lbl_apache = new System.Windows.Forms.Label();
            this.lbl_nginx = new System.Windows.Forms.Label();
            this.lbl_mysql = new System.Windows.Forms.Label();
            this.lbl_php = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.watcher)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 75);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(334, 20);
            this.textBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(352, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(352, 72);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txt_watcher
            // 
            this.txt_watcher.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_watcher.Location = new System.Drawing.Point(8, 6);
            this.txt_watcher.Multiline = true;
            this.txt_watcher.Name = "txt_watcher";
            this.txt_watcher.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_watcher.Size = new System.Drawing.Size(658, 399);
            this.txt_watcher.TabIndex = 3;
            this.txt_watcher.WordWrap = false;
            // 
            // timer
            // 
            this.timer.Interval = 200;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // watcher
            // 
            this.watcher.EnableRaisingEvents = true;
            this.watcher.IncludeSubdirectories = true;
            this.watcher.NotifyFilter = ((System.IO.NotifyFilters)(((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)
                        | System.IO.NotifyFilters.LastAccess)));
            this.watcher.SynchronizingObject = this;
            this.watcher.Renamed += new System.IO.RenamedEventHandler(this.watcher_Renamed);
            this.watcher.Created += new System.IO.FileSystemEventHandler(this.watcher_Changed);
            this.watcher.Changed += new System.IO.FileSystemEventHandler(this.watcher_Changed);
            // 
            // notifier
            // 
            this.notifier.Text = "notifyIcon1";
            this.notifier.Visible = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 102);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(415, 277);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txt_watcher);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(407, 251);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Watcher";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txt_debug);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(407, 251);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Debug";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txt_debug
            // 
            this.txt_debug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_debug.Location = new System.Drawing.Point(7, 6);
            this.txt_debug.Multiline = true;
            this.txt_debug.Name = "txt_debug";
            this.txt_debug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_debug.Size = new System.Drawing.Size(393, 239);
            this.txt_debug.TabIndex = 4;
            this.txt_debug.WordWrap = false;
            // 
            // lbl_apache
            // 
            this.lbl_apache.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_apache.Location = new System.Drawing.Point(9, 9);
            this.lbl_apache.Name = "lbl_apache";
            this.lbl_apache.Size = new System.Drawing.Size(80, 16);
            this.lbl_apache.TabIndex = 5;
            this.lbl_apache.Text = "Apache";
            // 
            // lbl_nginx
            // 
            this.lbl_nginx.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_nginx.Location = new System.Drawing.Point(9, 25);
            this.lbl_nginx.Name = "lbl_nginx";
            this.lbl_nginx.Size = new System.Drawing.Size(80, 16);
            this.lbl_nginx.TabIndex = 6;
            this.lbl_nginx.Text = "Nginx";
            // 
            // lbl_mysql
            // 
            this.lbl_mysql.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_mysql.Location = new System.Drawing.Point(95, 9);
            this.lbl_mysql.Name = "lbl_mysql";
            this.lbl_mysql.Size = new System.Drawing.Size(80, 16);
            this.lbl_mysql.TabIndex = 7;
            this.lbl_mysql.Text = "MySQL";
            // 
            // lbl_php
            // 
            this.lbl_php.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_php.Location = new System.Drawing.Point(95, 25);
            this.lbl_php.Name = "lbl_php";
            this.lbl_php.Size = new System.Drawing.Size(80, 16);
            this.lbl_php.TabIndex = 8;
            this.lbl_php.Text = "PHP";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 391);
            this.Controls.Add(this.lbl_php);
            this.Controls.Add(this.lbl_mysql);
            this.Controls.Add(this.lbl_nginx);
            this.Controls.Add(this.lbl_apache);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "WPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.watcher)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txt_watcher;
        private System.Windows.Forms.Timer timer;
        private System.IO.FileSystemWatcher watcher;
        private System.Windows.Forms.NotifyIcon notifier;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txt_debug;
        private System.Windows.Forms.Label lbl_php;
        private System.Windows.Forms.Label lbl_mysql;
        private System.Windows.Forms.Label lbl_nginx;
        private System.Windows.Forms.Label lbl_apache;
    }
}

