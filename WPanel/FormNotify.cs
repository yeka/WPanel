using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class FormNotify : Form
    {
        protected int interval;

        public FormNotify() : this(2000)
        {
        }

        public FormNotify(int interval)
        {
            InitializeComponent();

            this.interval = interval;
            timer1.Interval = this.interval > 0 ? this.interval : 2000;
            timer1.Enabled = (this.interval > 0);
        }

        private void FormNotify_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
