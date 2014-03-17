using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace D3Test
{
    public partial class Overlay : Form
    {
        public Overlay()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }

        public void SetTimeToLvl(string strTimeToLevel)
        {
            lblTimeToLvl.Text = strTimeToLevel;
        }

        public void SetText(string strRun, string strXPHour)
        {
            lblRun.Text = strRun;
            lblXPHour.Text = strXPHour;
        }

        public void ShowWarning(bool show)
        {
            lblWarning.Text = "Bad input";
            lblWarning.Visible = show;
        }

        public void ShowBlank(bool show)
        {
            lblWarning.Text = "Blank";
            lblWarning.Visible = show;
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
    }
}
