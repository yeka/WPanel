using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void btnPHPStop_Click(object sender, EventArgs e)
        {

        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get { return check.Text; }
            set { check.Text = value; }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool Started
        {
            get { return btnStop.Enabled; }
            set
            {
                btnStop.Enabled = value;
                btnStart.Text = value ? "Restart" : "Start";
            }
        }
    }
}
