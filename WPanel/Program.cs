using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "WPanel - yk");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.AppendPrivatePath(Application.StartupPath + "\\lib");

            Form f = new Form1();
            if (!f.IsDisposed)
            {
                Application.Run(f);
            }
        }
    }
}
