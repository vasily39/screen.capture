using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace screen.capture
{
    static class Program
    {
        static NotifyIcon _ni;
        static ScreenCapture _screenCaptuture;
        static MenuItem _miRecord;


        [STAThread]
        static void Main()
        {
            _screenCaptuture = new ScreenCapture();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;




            _ni = new NotifyIcon();
            _ni.Icon = Properties.Resources.app;
            _ni.Visible = true;
            _ni.ContextMenu = new ContextMenu();

            var miExit = new MenuItem("Exit");
            miExit.Click += MiExit_Click;

            _miRecord = new MenuItem();
            _miRecord.Click += _miRecord_Click;

            _ni.ContextMenu.MenuItems.Add(_miRecord);
            _ni.ContextMenu.MenuItems.Add("-");
            _ni.ContextMenu.MenuItems.Add(miExit);


            SetMiRecordText();
            Application.Run();
        }

        private static void _miRecord_Click(object sender, EventArgs e)
        {
            if (_screenCaptuture != null)
            {
                if (_screenCaptuture.IsStarted())
                {
                    _screenCaptuture.Stop();
                }
                else
                {
                    _screenCaptuture.Start();
                }
            }

            SetMiRecordText();
        }

        private static void MiExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit?", "Confirmation", MessageBoxButtons.OKCancel) == DialogResult.OK)
                Application.Exit();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (_ni != null)
                _ni.Visible = false;

            if (_screenCaptuture != null)
                _screenCaptuture.Dispose();
        }

        private static void SetMiRecordText()
        {
            _miRecord.Text = _screenCaptuture.IsStarted() ? "Stop" : "Start";
        }
    }
}
