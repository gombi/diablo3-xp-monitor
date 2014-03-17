using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace D3Test
{
    public partial class FrmOCR : Form
    {
        public int StartPara;
        public int EndPara;
        public long StartXP;
        public long EndXp;

        public string pathStart;
        public string pathEnd;

        public string tempRefStartXp;
        public string tempRefEndXp;

        public FrmOCR(int StartPara, int EndPara, long StartXP, long EndXp, string pathStart, string pathEnd, string tempRefStartXp, string tempRefEndXp)
        {
            InitializeComponent();

            this.StartPara = StartPara;
            this.EndPara = EndPara;
            this.StartXP = StartXP;
            this.EndXp = EndXp;
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.tempRefStartXp = tempRefStartXp;
            this.tempRefEndXp = tempRefEndXp;

            if (StartXP == 0)
                txtXpStart.Focus();
            else if (StartPara == 0)
                txtParaStart.Focus();
            else if (EndXp == 0)
                txtXpEnd.Focus();
            else if (EndPara == 0)
                txtEndPara.Focus();

            FileStream bitmapFileStart = new FileStream(pathStart, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Image imgStartXp = new Bitmap(bitmapFileStart);
            pictureBox1.Image = imgStartXp;

            FileStream bitmapFileEnd = new FileStream(pathEnd, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Image imgEndXp = new Bitmap(bitmapFileEnd);
            pictureBox2.Image = imgEndXp;

            txtXpStart.Text = StartXP.ToString("n0");
            txtXpEnd.Text = EndXp.ToString("n0");

            if (txtXpStart.Text == "0")
                txtXpStart.Text = tempRefStartXp;

            if (txtXpEnd.Text == "0")
                txtXpEnd.Text = tempRefEndXp;

            txtParaStart.Text = StartPara.ToString();
            txtEndPara.Text = EndPara.ToString();
        }

        private string FixDisplay(string text)
        {
            // TO DO

            string temp = text;

            return temp;
        }

        public void ClosePic()
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long.TryParse(txtXpStart.Text.Replace(".", ""), out StartXP);
            long.TryParse(txtXpEnd.Text.Replace(".", ""), out EndXp);

            Int32.TryParse(txtParaStart.Text, out StartPara);
            Int32.TryParse(txtEndPara.Text, out EndPara);

            if (StartXP > 0 && EndXp > 0 && StartPara > 0 && EndPara > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Input some valid numbers, shithead!");
            }
        }
    }
}
