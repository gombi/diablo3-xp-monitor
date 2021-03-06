﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace D3Test
{
    public partial class Form1 : Form
    {
        string Diablo3Monitor;
        string Diablo3SSPath;

        long[] PLVL;

        public Form1()
        {
            InitializeComponent();

            initPLVL();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["Diablo3Monitor"].ToString().Length > 0)
                Diablo3Monitor = ConfigurationManager.AppSettings["Diablo3Monitor"].ToString();
            else
                Diablo3Monitor = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Diablo III\\Monitor";

            if (ConfigurationManager.AppSettings["Diablo3SSPath"].ToString().Length > 0)
                Diablo3SSPath = ConfigurationManager.AppSettings["Diablo3SSPath"].ToString();
            else
                Diablo3SSPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Diablo III\\Screenshots";

            if (!Directory.Exists(Diablo3Monitor))
            {
                Directory.CreateDirectory(Diablo3Monitor);
            }

            if (!Directory.Exists(Diablo3SSPath))
            {
                MessageBox.Show("Cant find your Diablo 3 Screenshot folder. Please edit app.config and update the key Diablo3SSPath with you Diablo 3 screenshot folder location! The restart the program.");
                return;
            }

            openFileDialog1.InitialDirectory = Diablo3Monitor;

            LoadGrid();

            LoadLastRundFlag = true;

            LoadLastRun(0);
        }

        private bool LoadLastRundFlag = false;

        private void LoadGrid()
        {
            if (File.Exists(Diablo3Monitor + "//Stats.txt"))
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("RunNo", typeof(int));
                dt.Columns.Add("Date", typeof(DateTime));
                dt.Columns.Add("Time", typeof(string));
                dt.Columns.Add("Xp", typeof(string));
                dt.Columns.Add("XpHour", typeof(string));

                string[] lines = System.IO.File.ReadAllLines(Diablo3Monitor + "//Stats.txt");

                decimal MaxXPRun = 0;
                int MaxXPRunNo = 0;
                decimal Last5Xp = 0;
                int testCounter = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(';');
                    DateTime date;
                    DateTime EndDate;
                    DateTime.TryParse(data[2], out date);
                    DateTime.TryParse(data[3], out EndDate);
                    int Xp;
                    Decimal XpHour;
                    Int32.TryParse(data[4], out Xp);
                    Decimal.TryParse(data[5], out XpHour);

                    XpHour = XpHour / 1000000;

                    TimeSpan t = EndDate - date;
                    string dur = "";
                    if (t.Hours > 0)
                    {
                        double h = t.TotalHours - (t.TotalHours % 1);
                        dur = h.ToString("n0") + "h " + t.Minutes.ToString() + "min " + t.Seconds + "sec";
                    }
                    else if (t.Minutes > 0)
                    {
                        double m = t.TotalMinutes - (t.TotalMinutes % 1);
                        dur = m.ToString() + "min " + t.Seconds + "sec";
                    }
                    else
                        dur = t.Seconds + "sec";

                    dt.Rows.Add(i + 1, date, dur, Xp.ToString("n0"), XpHour.ToString("n3") + " mil");

                    if (XpHour > MaxXPRun)
                    {
                        MaxXPRun = XpHour;
                        MaxXPRunNo = i;
                    }

                 
                    if (lines.Length > 5)
                    {
                        if (i > (lines.Length - 6))
                        {
                            Last5Xp += XpHour;
                            testCounter++;
                        }
                    }
                    else
                    {
                        lblLast5.Text = "N/A";
                    }
                }

                dataGridView1.DataSource = dt;
                dataGridView1.Sort(dataGridView1.Columns["RunNo"], ListSortDirection.Descending);
                

                lblBestRun.Text = MaxXPRun.ToString("n3") + " mil";
                lblLast5.Text = (Last5Xp / 5).ToString("n3") + " mil";
            }
        }

        private void LoadTimeToLevel(double XpHour, long CurrentXp, long XpLevelEnd)
        {
            long XpLeft = XpLevelEnd - CurrentXp;

            double XpSec = XpHour / 3600;
            double res = XpLeft / XpSec;

            TimeSpan t = TimeSpan.FromSeconds(res);

            if (t.TotalDays >= 1)
            {
                double d = t.TotalDays - (t.TotalDays % 1);
                lblTimeToLevel.Text = d.ToString("n0") + "d " + t.Hours.ToString("n0") + "h " + t.Minutes.ToString("n0") + "m " + t.Seconds.ToString("n0") + "s";
            }
            else if (t.TotalHours >= 1)
            {
                double h = t.TotalHours - (t.TotalHours % 1);
                lblTimeToLevel.Text = h.ToString("n0") + "h " + t.Minutes.ToString("n0") + "m " + t.Seconds.ToString("n0") + "s";
            }
            else
            {
                lblTimeToLevel.Text = t.Minutes.ToString("n0") + "m " + t.Seconds.ToString("n0") + "s";
            }

            if (frmOverlay.Visible)
                frmOverlay.SetTimeToLvl(lblTimeToLevel.Text);
        }

        private void LoadTimeToXXX(double XpHour, long CurrentXp, int CurrentPLvl)
        {
            int EndPara;
            Int32.TryParse(txtTimeTo.Text, out EndPara);

            if (EndPara < CurrentPLvl)
            {
                lblTimeToXXX.Text = "N/A";
                return;
            }

            long TotalXPLeft = GetTotalXpLeftToLvl(CurrentXp, CurrentPLvl, EndPara);

            double XpSec = XpHour / 3600;
            double res = TotalXPLeft / XpSec;

            TimeSpan t = TimeSpan.FromSeconds(res);

            if (t.TotalDays >= 1)
            {
                double d = t.TotalDays - (t.TotalDays % 1);
                lblTimeToXXX.Text = d.ToString("n0") + "d " + t.Hours.ToString("n0") + "h " + t.Minutes.ToString("n0") + "m " + t.Seconds.ToString("n0") + "s";
            }
            else if (t.TotalHours >= 1)
            {
                double h = t.TotalHours - (t.TotalHours % 1);
                lblTimeToXXX.Text = h.ToString("n0") + "h " + t.Minutes.ToString("n0") + "m " + t.Seconds.ToString("n0") + "s";
            }
            else
            {
                lblTimeToXXX.Text = t.Minutes.ToString("n0") + "m " + t.Seconds.ToString("n0") + "s";
            }
        }

        private long GetTotalXpLeftToLvl(long CurrentXp, int CurrentPLvl, int endParaLevel)
        {
            try
            {
                long TotalXp = 0;

                if (CurrentPLvl == (endParaLevel -1))
                    return PLVL[endParaLevel] - CurrentXp;

                int ParaLevelsLeft = endParaLevel - CurrentPLvl;

                // Xp Rest of lvl
                TotalXp += PLVL[CurrentPLvl + 1] - CurrentXp;

                for (int i = 0; i < ParaLevelsLeft - 1; i++)
                {
                    TotalXp += PLVL[(CurrentPLvl + 1) + (i + 1)];
                }

                return TotalXp;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void LoadLastRun(int index)
        {
            if (!LoadLastRundFlag)
                return;

            if (File.Exists(Diablo3Monitor + "//Stats.txt"))
            {
                string[] lines = System.IO.File.ReadAllLines(Diablo3Monitor + "//Stats.txt");
                if (lines.Length > 0)
                {
                    string[] theLine = lines[lines.Length - 1 - index].Split(';');

                    long StartXp;
                    long EndXp;
                    long.TryParse(theLine[0], out StartXp);
                    long.TryParse(theLine[1], out EndXp);

                    int StartPara = 0;
                    int EndPara = 0;
                    long TotalXP = 0;

                    if (theLine.Length == 8)
                    {
                        Int32.TryParse(theLine[6], out StartPara);
                        Int32.TryParse(theLine[7], out EndPara);
                        lblStartXp.Text = StartXp.ToString("n0") + " (" + StartPara.ToString() + ")"; ;
                        lblEndXp.Text = EndXp.ToString("n0") + " (" + EndPara.ToString() + ")"; ;

                        if (StartPara == EndPara)
                            TotalXP = EndXp - StartXp;
                        else if ((EndPara - StartPara) == 1)
                            TotalXP = (PLVL[StartPara + 1] - StartXp) + EndXp;
                        else
                        {
                            int paraLvls = EndPara - StartPara;

                            TotalXP = (PLVL[StartPara + 1] - StartXp) + EndXp;

                            for (int i = 0; i < paraLvls - 1; i++)
                            {
                                TotalXP += PLVL[StartPara + (i + 2)];
                            }
                        }
                    }
                    else
                    {
                        lblStartXp.Text = StartXp.ToString("n0");
                        lblEndXp.Text = EndXp.ToString("n0");
                        TotalXP = EndXp - StartXp;

                        lblTimeToLevel.Text = "N/A";
                        lblTimeToXXX.Text = "N/A";
                    }

                    DateTime StartTid;
                    DateTime EndTid;
                    DateTime.TryParse(theLine[2], out StartTid);
                    DateTime.TryParse(theLine[3], out EndTid);

                    lblStartTime.Text = StartTid.ToLongTimeString();
                    lblEndTime.Text = EndTid.ToLongTimeString();

                    lblRunTotal.Text = TotalXP.ToString("n0");

                    TimeSpan t = EndTid - StartTid;

                    double RunMin = t.TotalSeconds / 60;
                    double XpHour = (TotalXP / RunMin) * 60;

                    lblRunXpHour.Text = XpHour.ToString("n0");

                    if (theLine.Length == 8)
                    {
                        if (XpHour > 0)
                        {
                            LoadTimeToLevel(XpHour, EndXp, PLVL[EndPara + 1]);
                            LoadTimeToXXX(XpHour, EndXp, EndPara);
                        }
                        else
                        {
                            lblTimeToLevel.Text = "N/A";
                            lblTimeToXXX.Text = "N/A";
                        }
                    }
                }
            }
        }

        private string GetText3(string path)
        {
            string OrgFilename = path;

            try
            {
                Ocr ocr = new Ocr();
                using (Bitmap bmp = new Bitmap(OrgFilename))
                {
                    tessnet2.Tesseract tessocr = new tessnet2.Tesseract();
                    tessocr.Init(Environment.CurrentDirectory, "eng", true);
                    tessocr.GetThresholdedImage(bmp, Rectangle.Empty).Save(Diablo3Monitor +"\\" + Guid.NewGuid().ToString() + ".bmp");
                    List<tessnet2.Word> listen = ocr.DoOCRNormal(bmp, "eng");

                    string ress = "";

                    foreach (tessnet2.Word item in listen)
                    {
                        ress += item.Text + " ";
                    }

                    return ress;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }

        private Rectangle FindXpImg(Bitmap bip, bool PoolOfReflection)
        {
            /*
             * 2560 x 1440
             * 1920 x 1080
             * 1680 x 1050
             * 1366 x 768
             * 1360 x 768
             * */

            if (PoolOfReflection)
            {
                string Width = ConfigurationManager.AppSettings["Width"].ToString();
                string Height = ConfigurationManager.AppSettings["Height"].ToString();
                string X = ConfigurationManager.AppSettings["X_Pool"].ToString();
                string Y = ConfigurationManager.AppSettings["Y_Pool"].ToString();

                int intX;
                int intY;
                int intWidth;
                int intHeight;

                Int32.TryParse(X, out intX);
                Int32.TryParse(Y, out intY);
                Int32.TryParse(Width, out intWidth);
                Int32.TryParse(Height, out intHeight);

                if (intX > 0 && intY > 0 && intWidth > 0 && intHeight > 0)
                    return new Rectangle(intX, intY, intWidth, intHeight);
                else if (bip.Height == 1080 && bip.Width == 1920)
                    return new Rectangle(762, 808, 395, 35);
                else if ((bip.Height == 1050 || bip.Height == 1056) && bip.Width == 1680)
                    return new Rectangle(643, 783, 395, 34);
                else if (bip.Height == 768 && (bip.Width == 1368 || bip.Width == 1360))
                    return new Rectangle(530, 570, 303, 22);
                else if (bip.Height == 1440 && bip.Width == 2560)
                    return new Rectangle(1006, 1077, 547, 40);
                else
                {
                    MessageBox.Show("Unknown Screenshot resolution. Please read SetupResolution.pdf !!!");
                    return new Rectangle();
                }
            }
            else
            {
                string X = ConfigurationManager.AppSettings["X"].ToString();
                string Y = ConfigurationManager.AppSettings["Y"].ToString();
                string Width = ConfigurationManager.AppSettings["Width"].ToString();
                string Height = ConfigurationManager.AppSettings["Height"].ToString();
                int intX;
                int intY;
                int intWidth;
                int intHeight;
                Int32.TryParse(X, out intX);
                Int32.TryParse(Y, out intY);
                Int32.TryParse(Width, out intWidth);
                Int32.TryParse(Height, out intHeight);

                if (intX > 0 && intY > 0 && intWidth > 0 && intHeight > 0)
                {
                    return new Rectangle(intX, intY, intWidth, intHeight);
                }

                if (bip.Height == 1080 && bip.Width == 1920)
                    return new Rectangle(762, 908, 395, 35);
                else if ((bip.Height == 1050 || bip.Height == 1056) && bip.Width == 1680)
                    return new Rectangle(643, 881, 395, 34);
                else if (bip.Height == 768 && (bip.Width == 1368 || bip.Width == 1360))
                    return new Rectangle(530, 645, 303, 20);
                else if (bip.Height == 1440 && bip.Width == 2560)
                    return new Rectangle(1006, 1210, 547, 40);
                else
                {
                    MessageBox.Show("Unknown Screenshot resolution. Please read SetupResolution.pdf !!!");
                    return new Rectangle();
                }
            }
        }

        private void InvertBip(Bitmap bipNew)
        {
            Color pixelColor;
            byte A, R, G, B;
            for (int y = 0; y < bipNew.Height; y++)
            {
                for (int x = 0; x < bipNew.Width; x++)
                {
                    pixelColor = bipNew.GetPixel(x, y);

                    A = pixelColor.A;
                    R = (byte)(255 - pixelColor.R);
                    G = (byte)(255 - pixelColor.G);
                    B = (byte)(255 - pixelColor.B);

                    bipNew.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }
        }

        private long GetXpFromSS(string path, ref int ParaLvl, bool PoolOfReflection, ref string tempRefXP)
        {
            try
            {
                string OrgFilename = path;
                string SmallFileName = Diablo3Monitor + "\\" + Path.GetFileName(OrgFilename);

                Bitmap bip = new Bitmap(OrgFilename);

                using (bip)
                {
                    // Find the img of the black XP box and make a small inverted img of that
                    Rectangle rec = FindXpImg(bip, PoolOfReflection);

                    if (rec == new Rectangle())
                        return -100;

                    Bitmap bipNew = bip.Clone(rec, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    bipNew = ResizeBitmap(bipNew, bipNew.Width * 8, bipNew.Height * 8);
                    InvertBip(bipNew);
                    bipNew.Save(SmallFileName);
                }

                string tempFileName = Diablo3Monitor + "\\" + Path.GetFileName(OrgFilename) + "_BW.bmp";
                string currentXP = "";

                Ocr ocr = new Ocr();
                List<tessnet2.Word> list = null;
                using (Bitmap bmp = new Bitmap(SmallFileName))
                {
                    tessnet2.Tesseract tessocr = new tessnet2.Tesseract();

                    using (tessocr)
                    {
                        // Call the OCR and get the XP
                        tessocr.Init(Environment.CurrentDirectory, "eng", true);
                        tessocr.GetThresholdedImage(bmp, Rectangle.Empty).Save(tempFileName);
                        list = ocr.DoOCRNormal(bmp, "eng");
                    }
                }

                if ((list.Count == 1 && !PoolOfReflection) || (list.Count > 3 && !list[3].Text.ToLower().Contains("ex") && !PoolOfReflection))
                {
                    // Pool of reflection
                    File.Delete(SmallFileName);
                    return GetXpFromSS(path, ref ParaLvl, true, ref tempRefXP);
                }
                else
                {
                    if (list.Count >= 4 && list[3].Text.ToLower().Contains("ex"))
                    {
                        currentXP = list[4].Text;

                        if (currentXP[0] == '/')
                        {
                            currentXP = list[3].Text.Replace("Experience:", "");
                        }
                        else if (currentXP.Contains("/"))
                        {
                            currentXP = currentXP.Substring(0, list[4].Text.IndexOf('/'));
                        }

                        // THE OCR if not perfect, but i know that some of the bad chars it makes is like this:
                        currentXP = currentXP.Replace("I", "1");
                        currentXP = currentXP.Replace("i", "1");
                        currentXP = currentXP.Replace("E", "8");
                        currentXP = currentXP.Replace("B", "8");
                        currentXP = currentXP.Replace("G", "6");
                        currentXP = currentXP.Replace("O", "0");
                        currentXP = currentXP.Replace("-", "");
                        currentXP = currentXP.Replace(",", "");
                        currentXP = currentXP.Replace(" ", "");

                        // ParaLvl
                        Int32 intTempPara;
                        Int32.TryParse(list[2].Text.Replace("(", "").Replace(")", ""), out intTempPara);
                        ParaLvl = intTempPara;

                        File.Delete(SmallFileName);

                        tempRefXP = currentXP;

                        long res;
                        long.TryParse(currentXP, out res);
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }

            return 0;
        }

        private string GetDigitOnly(string theString)
        {
            string ress = "";

            for (int i = 0; i < theString.Length; i++)
            {
                if (Char.IsDigit(theString[i]))
                {
                    ress += theString[i].ToString();
                }
            }

            return ress;
        }

        private void LoadScreens()
        {
            try
            {
                if (!Directory.Exists(Diablo3SSPath))
                {
                    MessageBox.Show("Cant find your Diablo 3 Screenshot folder.");
                    return;
                }

                string[] Filer = Directory.GetFiles(Diablo3SSPath, "Screenshot*");

                if (Filer.Length < 2)
                {
                    MessageBox.Show("You need 2 Screenshots for this to work!!!");
                    return;
                }

                int StartParaLvl = 0;
                int EndParaLvl = 0;

                string tempRefXP_Start = "";
                string tempRefXP_End = "";

                long StartXp = GetXpFromSS(Filer[Filer.Length - 2], ref StartParaLvl, false, ref tempRefXP_Start);
                long EndXp = GetXpFromSS(Filer[Filer.Length - 1], ref EndParaLvl, false, ref tempRefXP_End);

                if (StartXp == -100 || EndXp == -100)
                {
                    frmOverlay.ShowBlank(true);
                    return;
                }

                if (cbAllways.Checked || StartParaLvl == 0 || EndParaLvl == 0 || StartXp < 100000 || EndXp < 100000)
                {
                    string SSBWStart = Diablo3Monitor + "\\" + Path.GetFileName(Filer[Filer.Length - 2]) + "_BW.bmp";
                    string SSBwEnd = Diablo3Monitor + "\\" + Path.GetFileName(Filer[Filer.Length - 1]) + "_BW.bmp";

                    FrmOCR ocrForm = new FrmOCR(StartParaLvl, EndParaLvl, StartXp, EndXp, SSBWStart, SSBwEnd, tempRefXP_Start, tempRefXP_End);

                    if (frmOverlay.Visible)
                    {
                        frmOverlay.ShowWarning(true);
                    }

                    DialogResult drRes = ocrForm.ShowDialog();

                    if (drRes == System.Windows.Forms.DialogResult.OK)
                    {
                        StartParaLvl = ocrForm.StartPara;
                        EndParaLvl = ocrForm.EndPara;
                        StartXp = ocrForm.StartXP;
                        EndXp = ocrForm.EndXp;

                        ocrForm.ClosePic();
                        ocrForm.Dispose();

                        frmOverlay.ShowWarning(false);
                    }
                    else
                    {
                        frmOverlay.ShowWarning(false);
                        return;
                    }
                }

                DateTime StartTid = File.GetLastWriteTime(Filer[Filer.Length - 2]);
                DateTime EndTid = File.GetLastWriteTime(Filer[Filer.Length - 1]);

                long TotalXP = 0;
                if (StartParaLvl == EndParaLvl)
                    TotalXP = EndXp - StartXp;
                else if ((StartParaLvl + 1) == EndParaLvl)
                    TotalXP = (PLVL[StartParaLvl + 1] - StartXp) + EndXp;
                else
                {
                    // more then 1 lvl
                    int lvls = EndParaLvl - StartParaLvl;

                    // First add the rest of the first lvl
                    TotalXP = (PLVL[StartParaLvl + 1] - StartXp);

                    // Next add all of the lvls, except the last one
                    for (int i = 0; i < lvls - 1; i++)
                    {
                        int pIndex = (StartParaLvl + 1) + (i + 1);
                        TotalXP += PLVL[pIndex];
                    }

                    // Add last level xp
                    TotalXP += EndXp;
                }

                TimeSpan t = EndTid - StartTid;
                double RunMin = t.TotalSeconds / 60;
                double XpHour = (TotalXP / RunMin) * 60;

                if (TotalXP > 0)
                {
                    if (frmOverlay.Visible)
                        frmOverlay.SetText(t.ToString(), (XpHour / 1000000).ToString("n3") + " mil");

                    System.IO.StreamWriter file = new System.IO.StreamWriter(Diablo3Monitor + "//Stats.txt", true);
                    file.WriteLine(StartXp + ";" + EndXp + ";" + StartTid + ";" + EndTid + ";" + TotalXP + ";" + XpHour + ";" + StartParaLvl.ToString() + ";" + EndParaLvl.ToString());
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fejl: " + ex.Message);
            }

            LoadLastRun(0);
            LoadGrid();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadScreens();
        }

        private void btnOCR_Click(object sender, EventArgs e)
        {
            DialogResult dia = openFileDialog1.ShowDialog();

            if (dia == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(GetText3(openFileDialog1.FileName));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "";

            string business = "df0407@hotmail.com";  // your paypal email
            string description = "Donation";            // '%20' represents a space. remember HTML!
            string country = "US";                  // AU, US, etc.
            string currency = "USD";                 // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            System.Diagnostics.Process.Start(url);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;
                    LoadLastRun(index);
                }
            }
        }

        private void initPLVL()
        {
            // XP TO BECOME THAT LVL   

            PLVL = new long[1501];

            PLVL[0] = 0;
            PLVL[1] = 7200000;
            PLVL[2] = 8640000;
            PLVL[3] = 10080000;
            PLVL[4] = 11520000;
            PLVL[5] = 12960000;
            PLVL[6] = 14400000;
            PLVL[7] = 15840000;
            PLVL[8] = 17280000;
            PLVL[9] = 18720000;
            PLVL[10] = 20160000;
            PLVL[11] = 21600000;
            PLVL[12] = 23040000;
            PLVL[13] = 24480000;
            PLVL[14] = 25920000;
            PLVL[15] = 27360000;
            PLVL[16] = 28800000;
            PLVL[17] = 30240000;
            PLVL[18] = 31680000;
            PLVL[19] = 33120000;
            PLVL[20] = 34560000;
            PLVL[21] = 36000000;
            PLVL[22] = 37440000;
            PLVL[23] = 38880000;
            PLVL[24] = 40320000;
            PLVL[25] = 41760000;
            PLVL[26] = 43200000;
            PLVL[27] = 44640000;
            PLVL[28] = 46080000;
            PLVL[29] = 47520000;
            PLVL[30] = 48960000;
            PLVL[31] = 50400000;
            PLVL[32] = 51840000;
            PLVL[33] = 53280000;
            PLVL[34] = 54720000;
            PLVL[35] = 56160000;
            PLVL[36] = 57600000;
            PLVL[37] = 59040000;
            PLVL[38] = 60480000;
            PLVL[39] = 61920000;
            PLVL[40] = 63360000;
            PLVL[41] = 64800000;
            PLVL[42] = 66240000;
            PLVL[43] = 67680000;
            PLVL[44] = 69120000;
            PLVL[45] = 70560000;
            PLVL[46] = 72000000;
            PLVL[47] = 73440000;
            PLVL[48] = 74880000;
            PLVL[49] = 76320000;
            PLVL[50] = 77760000;
            PLVL[51] = 79200000;
            PLVL[52] = 80640000;
            PLVL[53] = 82080000;
            PLVL[54] = 83520000;
            PLVL[55] = 84960000;
            PLVL[56] = 86400000;
            PLVL[57] = 87840000;
            PLVL[58] = 89280000;
            PLVL[59] = 90720000;
            PLVL[60] = 92160000;
            PLVL[61] = 95040000;
            PLVL[62] = 97920000;
            PLVL[63] = 100800000;
            PLVL[64] = 103680000;
            PLVL[65] = 106560000;
            PLVL[66] = 109440000;
            PLVL[67] = 112320000;
            PLVL[68] = 115200000;
            PLVL[69] = 118080000;
            PLVL[70] = 120960000;
            PLVL[71] = 126000000;
            PLVL[72] = 131040000;
            PLVL[73] = 136080000;
            PLVL[74] = 139740000;
            PLVL[75] = 140760000;
            PLVL[76] = 141780000;
            PLVL[77] = 142800000;
            PLVL[78] = 143820000;
            PLVL[79] = 144840000;
            PLVL[80] = 145860000;
            PLVL[81] = 146880000;
            PLVL[82] = 147900000;
            PLVL[83] = 148920000;
            PLVL[84] = 149940000;
            PLVL[85] = 150960000;
            PLVL[86] = 151980000;
            PLVL[87] = 153000000;
            PLVL[88] = 154020000;
            PLVL[89] = 155040000;
            PLVL[90] = 156060000;
            PLVL[91] = 157080000;
            PLVL[92] = 158100000;
            PLVL[93] = 159120000;
            PLVL[94] = 160140000;
            PLVL[95] = 161160000;
            PLVL[96] = 162180000;
            PLVL[97] = 163200000;
            PLVL[98] = 164220000;
            PLVL[99] = 165240000;
            PLVL[100] = 166260000;
            PLVL[101] = 167280000;
            PLVL[102] = 168300000;
            PLVL[103] = 169320000;
            PLVL[104] = 170340000;
            PLVL[105] = 171360000;
            PLVL[106] = 172380000;
            PLVL[107] = 173400000;
            PLVL[108] = 174420000;
            PLVL[109] = 175440000;
            PLVL[110] = 176460000;
            PLVL[111] = 177480000;
            PLVL[112] = 178500000;
            PLVL[113] = 179520000;
            PLVL[114] = 180540000;
            PLVL[115] = 181560000;
            PLVL[116] = 182580000;
            PLVL[117] = 183600000;
            PLVL[118] = 184620000;
            PLVL[119] = 185640000;
            PLVL[120] = 186660000;
            PLVL[121] = 187680000;
            PLVL[122] = 188700000;
            PLVL[123] = 189720000;
            PLVL[124] = 190740000;
            PLVL[125] = 191760000;
            PLVL[126] = 192780000;
            PLVL[127] = 193800000;
            PLVL[128] = 194820000;
            PLVL[129] = 195840000;
            PLVL[130] = 196860000;
            PLVL[131] = 197880000;
            PLVL[132] = 198900000;
            PLVL[133] = 199920000;
            PLVL[134] = 200940000;
            PLVL[135] = 201960000;
            PLVL[136] = 202980000;
            PLVL[137] = 204000000;
            PLVL[138] = 205020000;
            PLVL[139] = 206040000;
            PLVL[140] = 207060000;
            PLVL[141] = 208080000;
            PLVL[142] = 209100000;
            PLVL[143] = 210120000;
            PLVL[144] = 211140000;
            PLVL[145] = 212160000;
            PLVL[146] = 213180000;
            PLVL[147] = 214200000;
            PLVL[148] = 215220000;
            PLVL[149] = 216240000;
            PLVL[150] = 218280000;
            PLVL[151] = 220320000;
            PLVL[152] = 222360000;
            PLVL[153] = 224400000;
            PLVL[154] = 226440000;
            PLVL[155] = 228480000;
            PLVL[156] = 230520000;
            PLVL[157] = 232560000;
            PLVL[158] = 234600000;
            PLVL[159] = 236640000;
            PLVL[160] = 238680000;
            PLVL[161] = 240720000;
            PLVL[162] = 242760000;
            PLVL[163] = 244800000;
            PLVL[164] = 246840000;
            PLVL[165] = 248880000;
            PLVL[166] = 250920000;
            PLVL[167] = 252960000;
            PLVL[168] = 255000000;
            PLVL[169] = 257040000;
            PLVL[170] = 259080000;
            PLVL[171] = 261120000;
            PLVL[172] = 263160000;
            PLVL[173] = 265200000;
            PLVL[174] = 267240000;
            PLVL[175] = 269280000;
            PLVL[176] = 271320000;
            PLVL[177] = 273360000;
            PLVL[178] = 275400000;
            PLVL[179] = 277440000;
            PLVL[180] = 279480000;
            PLVL[181] = 281520000;
            PLVL[182] = 283560000;
            PLVL[183] = 285600000;
            PLVL[184] = 287640000;
            PLVL[185] = 289680000;
            PLVL[186] = 291720000;
            PLVL[187] = 293760000;
            PLVL[188] = 295800000;
            PLVL[189] = 297840000;
            PLVL[190] = 299880000;
            PLVL[191] = 301920000;
            PLVL[192] = 303960000;
            PLVL[193] = 306000000;
            PLVL[194] = 308040000;
            PLVL[195] = 310080000;
            PLVL[196] = 312120000;
            PLVL[197] = 314160000;
            PLVL[198] = 316200000;
            PLVL[199] = 318240000;
            PLVL[200] = 320280000;
            PLVL[201] = 322320000;
            PLVL[202] = 324360000;
            PLVL[203] = 326400000;
            PLVL[204] = 328440000;
            PLVL[205] = 330480000;
            PLVL[206] = 332520000;
            PLVL[207] = 334560000;
            PLVL[208] = 336600000;
            PLVL[209] = 338640000;
            PLVL[210] = 340680000;
            PLVL[211] = 342720000;
            PLVL[212] = 344760000;
            PLVL[213] = 346800000;
            PLVL[214] = 348840000;
            PLVL[215] = 350880000;
            PLVL[216] = 352920000;
            PLVL[217] = 354960000;
            PLVL[218] = 357000000;
            PLVL[219] = 359040000;
            PLVL[220] = 361080000;
            PLVL[221] = 363120000;
            PLVL[222] = 365160000;
            PLVL[223] = 367200000;
            PLVL[224] = 369240000;
            PLVL[225] = 371280000;
            PLVL[226] = 373320000;
            PLVL[227] = 375360000;
            PLVL[228] = 377400000;
            PLVL[229] = 379440000;
            PLVL[230] = 381480000;
            PLVL[231] = 383520000;
            PLVL[232] = 385560000;
            PLVL[233] = 387600000;
            PLVL[234] = 389640000;
            PLVL[235] = 391680000;
            PLVL[236] = 393720000;
            PLVL[237] = 395760000;
            PLVL[238] = 397800000;
            PLVL[239] = 399840000;
            PLVL[240] = 401880000;
            PLVL[241] = 403920000;
            PLVL[242] = 405960000;
            PLVL[243] = 408000000;
            PLVL[244] = 410040000;
            PLVL[245] = 412080000;
            PLVL[246] = 414120000;
            PLVL[247] = 416160000;
            PLVL[248] = 418200000;
            PLVL[249] = 420240000;
            PLVL[250] = 422280000;
            PLVL[251] = 426360000;
            PLVL[252] = 430440000;
            PLVL[253] = 434520000;
            PLVL[254] = 438600000;
            PLVL[255] = 442680000;
            PLVL[256] = 446760000;
            PLVL[257] = 450840000;
            PLVL[258] = 454920000;
            PLVL[259] = 459000000;
            PLVL[260] = 463080000;
            PLVL[261] = 467160000;
            PLVL[262] = 471240000;
            PLVL[263] = 475320000;
            PLVL[264] = 479400000;
            PLVL[265] = 483480000;
            PLVL[266] = 487560000;
            PLVL[267] = 491640000;
            PLVL[268] = 495720000;
            PLVL[269] = 499800000;
            PLVL[270] = 503880000;
            PLVL[271] = 507960000;
            PLVL[272] = 512040000;
            PLVL[273] = 516120000;
            PLVL[274] = 520200000;
            PLVL[275] = 524280000;
            PLVL[276] = 528360000;
            PLVL[277] = 532440000;
            PLVL[278] = 536520000;
            PLVL[279] = 540600000;
            PLVL[280] = 544680000;
            PLVL[281] = 548760000;
            PLVL[282] = 552840000;
            PLVL[283] = 556920000;
            PLVL[284] = 561000000;
            PLVL[285] = 565080000;
            PLVL[286] = 569160000;
            PLVL[287] = 573240000;
            PLVL[288] = 577320000;
            PLVL[289] = 581400000;
            PLVL[290] = 585480000;
            PLVL[291] = 589560000;
            PLVL[292] = 593640000;
            PLVL[293] = 597720000;
            PLVL[294] = 601800000;
            PLVL[295] = 605880000;
            PLVL[296] = 609960000;
            PLVL[297] = 614040000;
            PLVL[298] = 618120000;
            PLVL[299] = 622200000;
            PLVL[300] = 626280000;
            PLVL[301] = 630360000;
            PLVL[302] = 634440000;
            PLVL[303] = 638520000;
            PLVL[304] = 642600000;
            PLVL[305] = 646680000;
            PLVL[306] = 650760000;
            PLVL[307] = 654840000;
            PLVL[308] = 658920000;
            PLVL[309] = 663000000;
            PLVL[310] = 667080000;
            PLVL[311] = 671160000;
            PLVL[312] = 675240000;
            PLVL[313] = 679320000;
            PLVL[314] = 683400000;
            PLVL[315] = 687480000;
            PLVL[316] = 691560000;
            PLVL[317] = 695640000;
            PLVL[318] = 699720000;
            PLVL[319] = 703800000;
            PLVL[320] = 707880000;
            PLVL[321] = 711960000;
            PLVL[322] = 716040000;
            PLVL[323] = 720120000;
            PLVL[324] = 724200000;
            PLVL[325] = 728280000;
            PLVL[326] = 732360000;
            PLVL[327] = 736440000;
            PLVL[328] = 740520000;
            PLVL[329] = 744600000;
            PLVL[330] = 748680000;
            PLVL[331] = 752760000;
            PLVL[332] = 756840000;
            PLVL[333] = 760920000;
            PLVL[334] = 765000000;
            PLVL[335] = 769080000;
            PLVL[336] = 773160000;
            PLVL[337] = 777240000;
            PLVL[338] = 781320000;
            PLVL[339] = 785400000;
            PLVL[340] = 789480000;
            PLVL[341] = 793560000;
            PLVL[342] = 797640000;
            PLVL[343] = 801720000;
            PLVL[344] = 805800000;
            PLVL[345] = 809880000;
            PLVL[346] = 813960000;
            PLVL[347] = 818040000;
            PLVL[348] = 822120000;
            PLVL[349] = 826200000;
            PLVL[350] = 830280000;
            PLVL[351] = 836400000;
            PLVL[352] = 842520000;
            PLVL[353] = 848640000;
            PLVL[354] = 854760000;
            PLVL[355] = 860880000;
            PLVL[356] = 867000000;
            PLVL[357] = 873120000;
            PLVL[358] = 879240000;
            PLVL[359] = 885360000;
            PLVL[360] = 891480000;
            PLVL[361] = 897600000;
            PLVL[362] = 903720000;
            PLVL[363] = 909840000;
            PLVL[364] = 915960000;
            PLVL[365] = 922080000;
            PLVL[366] = 928200000;
            PLVL[367] = 934320000;
            PLVL[368] = 940440000;
            PLVL[369] = 946560000;
            PLVL[370] = 952680000;
            PLVL[371] = 958800000;
            PLVL[372] = 964920000;
            PLVL[373] = 971040000;
            PLVL[374] = 977160000;
            PLVL[375] = 983280000;
            PLVL[376] = 989400000;
            PLVL[377] = 995520000;
            PLVL[378] = 1001640000;
            PLVL[379] = 1007760000;
            PLVL[380] = 1013880000;
            PLVL[381] = 1020000000;
            PLVL[382] = 1026120000;
            PLVL[383] = 1032240000;
            PLVL[384] = 1038360000;
            PLVL[385] = 1044480000;
            PLVL[386] = 1050600000;
            PLVL[387] = 1056720000;
            PLVL[388] = 1062840000;
            PLVL[389] = 1068960000;
            PLVL[390] = 1075080000;
            PLVL[391] = 1081200000;
            PLVL[392] = 1087320000;
            PLVL[393] = 1093440000;
            PLVL[394] = 1099560000;
            PLVL[395] = 1105680000;
            PLVL[396] = 1111800000;
            PLVL[397] = 1117920000;
            PLVL[398] = 1124040000;
            PLVL[399] = 1130160000;
            PLVL[400] = 1136280000;
            PLVL[401] = 1142400000;
            PLVL[402] = 1148520000;
            PLVL[403] = 1154640000;
            PLVL[404] = 1160760000;
            PLVL[405] = 1166880000;
            PLVL[406] = 1173000000;
            PLVL[407] = 1179120000;
            PLVL[408] = 1185240000;
            PLVL[409] = 1191360000;
            PLVL[410] = 1197480000;
            PLVL[411] = 1203600000;
            PLVL[412] = 1209720000;
            PLVL[413] = 1215840000;
            PLVL[414] = 1221960000;
            PLVL[415] = 1228080000;
            PLVL[416] = 1234200000;
            PLVL[417] = 1240320000;
            PLVL[418] = 1246440000;
            PLVL[419] = 1252560000;
            PLVL[420] = 1258680000;
            PLVL[421] = 1264800000;
            PLVL[422] = 1270920000;
            PLVL[423] = 1277040000;
            PLVL[424] = 1283160000;
            PLVL[425] = 1289280000;
            PLVL[426] = 1295400000;
            PLVL[427] = 1301520000;
            PLVL[428] = 1307640000;
            PLVL[429] = 1313760000;
            PLVL[430] = 1319880000;
            PLVL[431] = 1326000000;
            PLVL[432] = 1332120000;
            PLVL[433] = 1338240000;
            PLVL[434] = 1344360000;
            PLVL[435] = 1350480000;
            PLVL[436] = 1356600000;
            PLVL[437] = 1362720000;
            PLVL[438] = 1368840000;
            PLVL[439] = 1374960000;
            PLVL[440] = 1381080000;
            PLVL[441] = 1387200000;
            PLVL[442] = 1393320000;
            PLVL[443] = 1399440000;
            PLVL[444] = 1405560000;
            PLVL[445] = 1411680000;
            PLVL[446] = 1417800000;
            PLVL[447] = 1423920000;
            PLVL[448] = 1430040000;
            PLVL[449] = 1436160000;
            PLVL[450] = 1444320000;
            PLVL[451] = 1452480000;
            PLVL[452] = 1460640000;
            PLVL[453] = 1468800000;
            PLVL[454] = 1476960000;
            PLVL[455] = 1485120000;
            PLVL[456] = 1493280000;
            PLVL[457] = 1501440000;
            PLVL[458] = 1509600000;
            PLVL[459] = 1517760000;
            PLVL[460] = 1525920000;
            PLVL[461] = 1534080000;
            PLVL[462] = 1542240000;
            PLVL[463] = 1550400000;
            PLVL[464] = 1558560000;
            PLVL[465] = 1566720000;
            PLVL[466] = 1574880000;
            PLVL[467] = 1583040000;
            PLVL[468] = 1591200000;
            PLVL[469] = 1599360000;
            PLVL[470] = 1607520000;
            PLVL[471] = 1615680000;
            PLVL[472] = 1623840000;
            PLVL[473] = 1632000000;
            PLVL[474] = 1640160000;
            PLVL[475] = 1648320000;
            PLVL[476] = 1656480000;
            PLVL[477] = 1664640000;
            PLVL[478] = 1672800000;
            PLVL[479] = 1680960000;
            PLVL[480] = 1689120000;
            PLVL[481] = 1697280000;
            PLVL[482] = 1705440000;
            PLVL[483] = 1713600000;
            PLVL[484] = 1721760000;
            PLVL[485] = 1729920000;
            PLVL[486] = 1738080000;
            PLVL[487] = 1746240000;
            PLVL[488] = 1754400000;
            PLVL[489] = 1762560000;
            PLVL[490] = 1770720000;
            PLVL[491] = 1778880000;
            PLVL[492] = 1787040000;
            PLVL[493] = 1795200000;
            PLVL[494] = 1803360000;
            PLVL[495] = 1811520000;
            PLVL[496] = 1819680000;
            PLVL[497] = 1827840000;
            PLVL[498] = 1836000000;
            PLVL[499] = 1844160000;
            PLVL[500] = 1852320000;
            PLVL[501] = 1872720000;
            PLVL[502] = 1893120000;
            PLVL[503] = 1913520000;
            PLVL[504] = 1933920000;
            PLVL[505] = 1954320000;
            PLVL[506] = 1974720000;
            PLVL[507] = 1995120000;
            PLVL[508] = 2015520000;
            PLVL[509] = 2035920000;
            PLVL[510] = 2056320000;
            PLVL[511] = 2076720000;
            PLVL[512] = 2097120000;
            PLVL[513] = 2117520000;
            PLVL[514] = 2137920000;
            PLVL[515] = 2158320000;
            PLVL[516] = 2178720000;
            PLVL[517] = 2199120000;
            PLVL[518] = 2219520000;
            PLVL[519] = 2239920000;
            PLVL[520] = 2260320000;
            PLVL[521] = 2280720000;
            PLVL[522] = 2301120000;
            PLVL[523] = 2321520000;
            PLVL[524] = 2341920000;
            PLVL[525] = 2362320000;
            PLVL[526] = 2382720000;
            PLVL[527] = 2403120000;
            PLVL[528] = 2423520000;
            PLVL[529] = 2443920000;
            PLVL[530] = 2464320000;
            PLVL[531] = 2484720000;
            PLVL[532] = 2505120000;
            PLVL[533] = 2525520000;
            PLVL[534] = 2545920000;
            PLVL[535] = 2566320000;
            PLVL[536] = 2586720000;
            PLVL[537] = 2607120000;
            PLVL[538] = 2627520000;
            PLVL[539] = 2647920000;
            PLVL[540] = 2668320000;
            PLVL[541] = 2688720000;
            PLVL[542] = 2709120000;
            PLVL[543] = 2729520000;
            PLVL[544] = 2749920000;
            PLVL[545] = 2770320000;
            PLVL[546] = 2790720000;
            PLVL[547] = 2811120000;
            PLVL[548] = 2831520000;
            PLVL[549] = 2851920000;
            PLVL[550] = 2872320000;
            PLVL[551] = 2913120000;
            PLVL[552] = 2953920000;
            PLVL[553] = 2994720000;
            PLVL[554] = 3035520000;
            PLVL[555] = 3076320000;
            PLVL[556] = 3117120000;
            PLVL[557] = 3157920000;
            PLVL[558] = 3198720000;
            PLVL[559] = 3239520000;
            PLVL[560] = 3280320000;
            PLVL[561] = 3321120000;
            PLVL[562] = 3361920000;
            PLVL[563] = 3402720000;
            PLVL[564] = 3443520000;
            PLVL[565] = 3484320000;
            PLVL[566] = 3525120000;
            PLVL[567] = 3565920000;
            PLVL[568] = 3606720000;
            PLVL[569] = 3647520000;
            PLVL[570] = 3688320000;
            PLVL[571] = 3729120000;
            PLVL[572] = 3769920000;
            PLVL[573] = 3810720000;
            PLVL[574] = 3851520000;
            PLVL[575] = 3892320000;
            PLVL[576] = 3933120000;
            PLVL[577] = 3973920000;
            PLVL[578] = 4014720000;
            PLVL[579] = 4055520000;
            PLVL[580] = 4096320000;
            PLVL[581] = 4137120000;
            PLVL[582] = 4177920000;
            PLVL[583] = 4218720000;
            PLVL[584] = 4259520000;
            PLVL[585] = 4300320000;
            PLVL[586] = 4341120000;
            PLVL[587] = 4381920000;
            PLVL[588] = 4422720000;
            PLVL[589] = 4463520000;
            PLVL[590] = 4504320000;
            PLVL[591] = 4545120000;
            PLVL[592] = 4585920000;
            PLVL[593] = 4626720000;
            PLVL[594] = 4667520000;
            PLVL[595] = 4708320000;
            PLVL[596] = 4749120000;
            PLVL[597] = 4789920000;
            PLVL[598] = 4830720000;
            PLVL[599] = 4871520000;
            PLVL[600] = 4912320000;
            PLVL[601] = 4973520000;
            PLVL[602] = 5034720000;
            PLVL[603] = 5095920000;
            PLVL[604] = 5157120000;
            PLVL[605] = 5218320000;
            PLVL[606] = 5279520000;
            PLVL[607] = 5340720000;
            PLVL[608] = 5401920000;
            PLVL[609] = 5463120000;
            PLVL[610] = 5524320000;
            PLVL[611] = 5585520000;
            PLVL[612] = 5646720000;
            PLVL[613] = 5707920000;
            PLVL[614] = 5769120000;
            PLVL[615] = 5830320000;
            PLVL[616] = 5891520000;
            PLVL[617] = 5952720000;
            PLVL[618] = 6013920000;
            PLVL[619] = 6075120000;
            PLVL[620] = 6136320000;
            PLVL[621] = 6197520000;
            PLVL[622] = 6258720000;
            PLVL[623] = 6319920000;
            PLVL[624] = 6381120000;
            PLVL[625] = 6442320000;
            PLVL[626] = 6503520000;
            PLVL[627] = 6564720000;
            PLVL[628] = 6625920000;
            PLVL[629] = 6687120000;
            PLVL[630] = 6748320000;
            PLVL[631] = 6809520000;
            PLVL[632] = 6870720000;
            PLVL[633] = 6931920000;
            PLVL[634] = 6993120000;
            PLVL[635] = 7054320000;
            PLVL[636] = 7115520000;
            PLVL[637] = 7176720000;
            PLVL[638] = 7237920000;
            PLVL[639] = 7299120000;
            PLVL[640] = 7360320000;
            PLVL[641] = 7421520000;
            PLVL[642] = 7482720000;
            PLVL[643] = 7543920000;
            PLVL[644] = 7605120000;
            PLVL[645] = 7666320000;
            PLVL[646] = 7727520000;
            PLVL[647] = 7788720000;
            PLVL[648] = 7849920000;
            PLVL[649] = 7911120000;
            PLVL[650] = 7972320000;
            PLVL[651] = 8053920000;
            PLVL[652] = 8135520000;
            PLVL[653] = 8217120000;
            PLVL[654] = 8298720000;
            PLVL[655] = 8380320000;
            PLVL[656] = 8461920000;
            PLVL[657] = 8543520000;
            PLVL[658] = 8625120000;
            PLVL[659] = 8706720000;
            PLVL[660] = 8788320000;
            PLVL[661] = 8869920000;
            PLVL[662] = 8951520000;
            PLVL[663] = 9033120000;
            PLVL[664] = 9114720000;
            PLVL[665] = 9196320000;
            PLVL[666] = 9277920000;
            PLVL[667] = 9359520000;
            PLVL[668] = 9441120000;
            PLVL[669] = 9522720000;
            PLVL[670] = 9604320000;
            PLVL[671] = 9685920000;
            PLVL[672] = 9767520000;
            PLVL[673] = 9849120000;
            PLVL[674] = 9930720000;
            PLVL[675] = 10012320000;
            PLVL[676] = 10093920000;
            PLVL[677] = 10175520000;
            PLVL[678] = 10257120000;
            PLVL[679] = 10338720000;
            PLVL[680] = 10420320000;
            PLVL[681] = 10501920000;
            PLVL[682] = 10583520000;
            PLVL[683] = 10665120000;
            PLVL[684] = 10746720000;
            PLVL[685] = 10828320000;
            PLVL[686] = 10909920000;
            PLVL[687] = 10991520000;
            PLVL[688] = 11073120000;
            PLVL[689] = 11154720000;
            PLVL[690] = 11236320000;
            PLVL[691] = 11317920000;
            PLVL[692] = 11399520000;
            PLVL[693] = 11481120000;
            PLVL[694] = 11562720000;
            PLVL[695] = 11644320000;
            PLVL[696] = 11725920000;
            PLVL[697] = 11807520000;
            PLVL[698] = 11889120000;
            PLVL[699] = 11970720000;
            PLVL[700] = 12052320000;
            PLVL[701] = 12154320000;
            PLVL[702] = 12256320000;
            PLVL[703] = 12358320000;
            PLVL[704] = 12460320000;
            PLVL[705] = 12562320000;
            PLVL[706] = 12664320000;
            PLVL[707] = 12766320000;
            PLVL[708] = 12868320000;
            PLVL[709] = 12970320000;
            PLVL[710] = 13072320000;
            PLVL[711] = 13174320000;
            PLVL[712] = 13276320000;
            PLVL[713] = 13378320000;
            PLVL[714] = 13480320000;
            PLVL[715] = 13582320000;
            PLVL[716] = 13684320000;
            PLVL[717] = 13786320000;
            PLVL[718] = 13888320000;
            PLVL[719] = 13990320000;
            PLVL[720] = 14092320000;
            PLVL[721] = 14194320000;
            PLVL[722] = 14296320000;
            PLVL[723] = 14398320000;
            PLVL[724] = 14500320000;
            PLVL[725] = 14602320000;
            PLVL[726] = 14704320000;
            PLVL[727] = 14806320000;
            PLVL[728] = 14908320000;
            PLVL[729] = 15010320000;
            PLVL[730] = 15112320000;
            PLVL[731] = 15214320000;
            PLVL[732] = 15316320000;
            PLVL[733] = 15418320000;
            PLVL[734] = 15520320000;
            PLVL[735] = 15622320000;
            PLVL[736] = 15724320000;
            PLVL[737] = 15826320000;
            PLVL[738] = 15928320000;
            PLVL[739] = 16030320000;
            PLVL[740] = 16132320000;
            PLVL[741] = 16234320000;
            PLVL[742] = 16336320000;
            PLVL[743] = 16438320000;
            PLVL[744] = 16540320000;
            PLVL[745] = 16642320000;
            PLVL[746] = 16744320000;
            PLVL[747] = 16846320000;
            PLVL[748] = 16948320000;
            PLVL[749] = 17050320000;
            PLVL[750] = 17152320000;
            PLVL[751] = 17274720000;
            PLVL[752] = 17397120000;
            PLVL[753] = 17519520000;
            PLVL[754] = 17641920000;
            PLVL[755] = 17764320000;
            PLVL[756] = 17886720000;
            PLVL[757] = 18009120000;
            PLVL[758] = 18131520000;
            PLVL[759] = 18253920000;
            PLVL[760] = 18376320000;
            PLVL[761] = 18498720000;
            PLVL[762] = 18621120000;
            PLVL[763] = 18743520000;
            PLVL[764] = 18865920000;
            PLVL[765] = 18988320000;
            PLVL[766] = 19110720000;
            PLVL[767] = 19233120000;
            PLVL[768] = 19355520000;
            PLVL[769] = 19477920000;
            PLVL[770] = 19600320000;
            PLVL[771] = 19722720000;
            PLVL[772] = 19845120000;
            PLVL[773] = 19967520000;
            PLVL[774] = 20089920000;
            PLVL[775] = 20212320000;
            PLVL[776] = 20334720000;
            PLVL[777] = 20457120000;
            PLVL[778] = 20579520000;
            PLVL[779] = 20701920000;
            PLVL[780] = 20824320000;
            PLVL[781] = 20946720000;
            PLVL[782] = 21069120000;
            PLVL[783] = 21191520000;
            PLVL[784] = 21313920000;
            PLVL[785] = 21436320000;
            PLVL[786] = 21558720000;
            PLVL[787] = 21681120000;
            PLVL[788] = 21803520000;
            PLVL[789] = 21925920000;
            PLVL[790] = 22048320000;
            PLVL[791] = 22170720000;
            PLVL[792] = 22293120000;
            PLVL[793] = 22415520000;
            PLVL[794] = 22537920000;
            PLVL[795] = 22660320000;
            PLVL[796] = 22782720000;
            PLVL[797] = 22905120000;
            PLVL[798] = 23027520000;
            PLVL[799] = 23149920000;
            PLVL[800] = 23272320000;
            PLVL[801] = 23394720000;
            PLVL[802] = 23517120000;
            PLVL[803] = 23639520000;
            PLVL[804] = 23761920000;
            PLVL[805] = 23884320000;
            PLVL[806] = 24006720000;
            PLVL[807] = 24129120000;
            PLVL[808] = 24251520000;
            PLVL[809] = 24373920000;
            PLVL[810] = 24496320000;
            PLVL[811] = 24618720000;
            PLVL[812] = 24741120000;
            PLVL[813] = 24863520000;
            PLVL[814] = 24985920000;
            PLVL[815] = 25108320000;
            PLVL[816] = 25230720000;
            PLVL[817] = 25353120000;
            PLVL[818] = 25475520000;
            PLVL[819] = 25597920000;
            PLVL[820] = 25720320000;
            PLVL[821] = 25842720000;
            PLVL[822] = 25965120000;
            PLVL[823] = 26087520000;
            PLVL[824] = 26209920000;
            PLVL[825] = 26332320000;
            PLVL[826] = 26454720000;
            PLVL[827] = 26577120000;
            PLVL[828] = 26699520000;
            PLVL[829] = 26821920000;
            PLVL[830] = 26944320000;
            PLVL[831] = 27066720000;
            PLVL[832] = 27189120000;
            PLVL[833] = 27311520000;
            PLVL[834] = 27433920000;
            PLVL[835] = 27556320000;
            PLVL[836] = 27678720000;
            PLVL[837] = 27801120000;
            PLVL[838] = 27923520000;
            PLVL[839] = 28045920000;
            PLVL[840] = 28168320000;
            PLVL[841] = 28290720000;
            PLVL[842] = 28413120000;
            PLVL[843] = 28535520000;
            PLVL[844] = 28657920000;
            PLVL[845] = 28780320000;
            PLVL[846] = 28902720000;
            PLVL[847] = 29025120000;
            PLVL[848] = 29147520000;
            PLVL[849] = 29269920000;
            PLVL[850] = 29392320000;
            PLVL[851] = 29514720000;
            PLVL[852] = 29637120000;
            PLVL[853] = 29759520000;
            PLVL[854] = 29881920000;
            PLVL[855] = 30004320000;
            PLVL[856] = 30126720000;
            PLVL[857] = 30249120000;
            PLVL[858] = 30371520000;
            PLVL[859] = 30493920000;
            PLVL[860] = 30616320000;
            PLVL[861] = 30738720000;
            PLVL[862] = 30861120000;
            PLVL[863] = 30983520000;
            PLVL[864] = 31105920000;
            PLVL[865] = 31228320000;
            PLVL[866] = 31350720000;
            PLVL[867] = 31473120000;
            PLVL[868] = 31595520000;
            PLVL[869] = 31717920000;
            PLVL[870] = 31840320000;
            PLVL[871] = 31962720000;
            PLVL[872] = 32085120000;
            PLVL[873] = 32207520000;
            PLVL[874] = 32329920000;
            PLVL[875] = 32452320000;
            PLVL[876] = 32574720000;
            PLVL[877] = 32697120000;
            PLVL[878] = 32819520000;
            PLVL[879] = 32941920000;
            PLVL[880] = 33064320000;
            PLVL[881] = 33186720000;
            PLVL[882] = 33309120000;
            PLVL[883] = 33431520000;
            PLVL[884] = 33553920000;
            PLVL[885] = 33676320000;
            PLVL[886] = 33798720000;
            PLVL[887] = 33921120000;
            PLVL[888] = 34043520000;
            PLVL[889] = 34165920000;
            PLVL[890] = 34288320000;
            PLVL[891] = 34410720000;
            PLVL[892] = 34533120000;
            PLVL[893] = 34655520000;
            PLVL[894] = 34777920000;
            PLVL[895] = 34900320000;
            PLVL[896] = 35022720000;
            PLVL[897] = 35145120000;
            PLVL[898] = 35267520000;
            PLVL[899] = 35389920000;
            PLVL[900] = 35512320000;
            PLVL[901] = 35634720000;
            PLVL[902] = 35757120000;
            PLVL[903] = 35879520000;
            PLVL[904] = 36001920000;
            PLVL[905] = 36124320000;
            PLVL[906] = 36246720000;
            PLVL[907] = 36369120000;
            PLVL[908] = 36491520000;
            PLVL[909] = 36613920000;
            PLVL[910] = 36736320000;
            PLVL[911] = 36858720000;
            PLVL[912] = 36981120000;
            PLVL[913] = 37103520000;
            PLVL[914] = 37225920000;
            PLVL[915] = 37348320000;
            PLVL[916] = 37470720000;
            PLVL[917] = 37593120000;
            PLVL[918] = 37715520000;
            PLVL[919] = 37837920000;
            PLVL[920] = 37960320000;
            PLVL[921] = 38082720000;
            PLVL[922] = 38205120000;
            PLVL[923] = 38327520000;
            PLVL[924] = 38449920000;
            PLVL[925] = 38572320000;
            PLVL[926] = 38694720000;
            PLVL[927] = 38817120000;
            PLVL[928] = 38939520000;
            PLVL[929] = 39061920000;
            PLVL[930] = 39184320000;
            PLVL[931] = 39306720000;
            PLVL[932] = 39429120000;
            PLVL[933] = 39551520000;
            PLVL[934] = 39673920000;
            PLVL[935] = 39796320000;
            PLVL[936] = 39918720000;
            PLVL[937] = 40041120000;
            PLVL[938] = 40163520000;
            PLVL[939] = 40285920000;
            PLVL[940] = 40408320000;
            PLVL[941] = 40530720000;
            PLVL[942] = 40653120000;
            PLVL[943] = 40775520000;
            PLVL[944] = 40897920000;
            PLVL[945] = 41020320000;
            PLVL[946] = 41142720000;
            PLVL[947] = 41265120000;
            PLVL[948] = 41387520000;
            PLVL[949] = 41509920000;
            PLVL[950] = 41632320000;
            PLVL[951] = 41754720000;
            PLVL[952] = 41877120000;
            PLVL[953] = 41999520000;
            PLVL[954] = 42121920000;
            PLVL[955] = 42244320000;
            PLVL[956] = 42366720000;
            PLVL[957] = 42489120000;
            PLVL[958] = 42611520000;
            PLVL[959] = 42733920000;
            PLVL[960] = 42856320000;
            PLVL[961] = 42978720000;
            PLVL[962] = 43101120000;
            PLVL[963] = 43223520000;
            PLVL[964] = 43345920000;
            PLVL[965] = 43468320000;
            PLVL[966] = 43590720000;
            PLVL[967] = 43713120000;
            PLVL[968] = 43835520000;
            PLVL[969] = 43957920000;
            PLVL[970] = 44080320000;
            PLVL[971] = 44202720000;
            PLVL[972] = 44325120000;
            PLVL[973] = 44447520000;
            PLVL[974] = 44569920000;
            PLVL[975] = 44692320000;
            PLVL[976] = 44814720000;
            PLVL[977] = 44937120000;
            PLVL[978] = 45059520000;
            PLVL[979] = 45181920000;
            PLVL[980] = 45304320000;
            PLVL[981] = 45426720000;
            PLVL[982] = 45549120000;
            PLVL[983] = 45671520000;
            PLVL[984] = 45793920000;
            PLVL[985] = 45916320000;
            PLVL[986] = 46038720000;
            PLVL[987] = 46161120000;
            PLVL[988] = 46283520000;
            PLVL[989] = 46405920000;
            PLVL[990] = 46528320000;
            PLVL[991] = 46650720000;
            PLVL[992] = 46773120000;
            PLVL[993] = 46895520000;
            PLVL[994] = 47017920000;
            PLVL[995] = 47140320000;
            PLVL[996] = 47262720000;
            PLVL[997] = 47385120000;
            PLVL[998] = 47507520000;
            PLVL[999] = 47629920000;
            PLVL[1000] = 47752320000;
            PLVL[1001] = 47874720000;
            PLVL[1002] = 47997120000;
            PLVL[1003] = 48119520000;
            PLVL[1004] = 48241920000;
            PLVL[1005] = 48364320000;
            PLVL[1006] = 48486720000;
            PLVL[1007] = 48609120000;
            PLVL[1008] = 48731520000;
            PLVL[1009] = 48853920000;
            PLVL[1010] = 48976320000;
            PLVL[1011] = 49098720000;
            PLVL[1012] = 49221120000;
            PLVL[1013] = 49343520000;
            PLVL[1014] = 49465920000;
            PLVL[1015] = 49588320000;
            PLVL[1016] = 49710720000;
            PLVL[1017] = 49833120000;
            PLVL[1018] = 49955520000;
            PLVL[1019] = 50077920000;
            PLVL[1020] = 50200320000;
            PLVL[1021] = 50322720000;
            PLVL[1022] = 50445120000;
            PLVL[1023] = 50567520000;
            PLVL[1024] = 50689920000;
            PLVL[1025] = 50812320000;
            PLVL[1026] = 50934720000;
            PLVL[1027] = 51057120000;
            PLVL[1028] = 51179520000;
            PLVL[1029] = 51301920000;
            PLVL[1030] = 51424320000;
            PLVL[1031] = 51546720000;
            PLVL[1032] = 51669120000;
            PLVL[1033] = 51791520000;
            PLVL[1034] = 51913920000;
            PLVL[1035] = 52036320000;
            PLVL[1036] = 52158720000;
            PLVL[1037] = 52281120000;
            PLVL[1038] = 52403520000;
            PLVL[1039] = 52525920000;
            PLVL[1040] = 52648320000;
            PLVL[1041] = 52770720000;
            PLVL[1042] = 52893120000;
            PLVL[1043] = 53015520000;
            PLVL[1044] = 53137920000;
            PLVL[1045] = 53260320000;
            PLVL[1046] = 53382720000;
            PLVL[1047] = 53505120000;
            PLVL[1048] = 53627520000;
            PLVL[1049] = 53749920000;
            PLVL[1050] = 53872320000;
            PLVL[1051] = 53994720000;
            PLVL[1052] = 54117120000;
            PLVL[1053] = 54239520000;
            PLVL[1054] = 54361920000;
            PLVL[1055] = 54484320000;
            PLVL[1056] = 54606720000;
            PLVL[1057] = 54729120000;
            PLVL[1058] = 54851520000;
            PLVL[1059] = 54973920000;
            PLVL[1060] = 55096320000;
            PLVL[1061] = 55218720000;
            PLVL[1062] = 55341120000;
            PLVL[1063] = 55463520000;
            PLVL[1064] = 55585920000;
            PLVL[1065] = 55708320000;
            PLVL[1066] = 55830720000;
            PLVL[1067] = 55953120000;
            PLVL[1068] = 56075520000;
            PLVL[1069] = 56197920000;
            PLVL[1070] = 56320320000;
            PLVL[1071] = 56442720000;
            PLVL[1072] = 56565120000;
            PLVL[1073] = 56687520000;
            PLVL[1074] = 56809920000;
            PLVL[1075] = 56932320000;
            PLVL[1076] = 57054720000;
            PLVL[1077] = 57177120000;
            PLVL[1078] = 57299520000;
            PLVL[1079] = 57421920000;
            PLVL[1080] = 57544320000;
            PLVL[1081] = 57666720000;
            PLVL[1082] = 57789120000;
            PLVL[1083] = 57911520000;
            PLVL[1084] = 58033920000;
            PLVL[1085] = 58156320000;
            PLVL[1086] = 58278720000;
            PLVL[1087] = 58401120000;
            PLVL[1088] = 58523520000;
            PLVL[1089] = 58645920000;
            PLVL[1090] = 58768320000;
            PLVL[1091] = 58890720000;
            PLVL[1092] = 59013120000;
            PLVL[1093] = 59135520000;
            PLVL[1094] = 59257920000;
            PLVL[1095] = 59380320000;
            PLVL[1096] = 59502720000;
            PLVL[1097] = 59625120000;
            PLVL[1098] = 59747520000;
            PLVL[1099] = 59869920000;
            PLVL[1100] = 59992320000;
            PLVL[1101] = 60114720000;
            PLVL[1102] = 60237120000;
            PLVL[1103] = 60359520000;
            PLVL[1104] = 60481920000;
            PLVL[1105] = 60604320000;
            PLVL[1106] = 60726720000;
            PLVL[1107] = 60849120000;
            PLVL[1108] = 60971520000;
            PLVL[1109] = 61093920000;
            PLVL[1110] = 61216320000;
            PLVL[1111] = 61338720000;
            PLVL[1112] = 61461120000;
            PLVL[1113] = 61583520000;
            PLVL[1114] = 61705920000;
            PLVL[1115] = 61828320000;
            PLVL[1116] = 61950720000;
            PLVL[1117] = 62073120000;
            PLVL[1118] = 62195520000;
            PLVL[1119] = 62317920000;
            PLVL[1120] = 62440320000;
            PLVL[1121] = 62562720000;
            PLVL[1122] = 62685120000;
            PLVL[1123] = 62807520000;
            PLVL[1124] = 62929920000;
            PLVL[1125] = 63052320000;
            PLVL[1126] = 63174720000;
            PLVL[1127] = 63297120000;
            PLVL[1128] = 63419520000;
            PLVL[1129] = 63541920000;
            PLVL[1130] = 63664320000;
            PLVL[1131] = 63786720000;
            PLVL[1132] = 63909120000;
            PLVL[1133] = 64031520000;
            PLVL[1134] = 64153920000;
            PLVL[1135] = 64276320000;
            PLVL[1136] = 64398720000;
            PLVL[1137] = 64521120000;
            PLVL[1138] = 64643520000;
            PLVL[1139] = 64765920000;
            PLVL[1140] = 64888320000;
            PLVL[1141] = 65010720000;
            PLVL[1142] = 65133120000;
            PLVL[1143] = 65255520000;
            PLVL[1144] = 65377920000;
            PLVL[1145] = 65500320000;
            PLVL[1146] = 65622720000;
            PLVL[1147] = 65745120000;
            PLVL[1148] = 65867520000;
            PLVL[1149] = 65989920000;
            PLVL[1150] = 66112320000;
            PLVL[1151] = 66234720000;
            PLVL[1152] = 66357120000;
            PLVL[1153] = 66479520000;
            PLVL[1154] = 66601920000;
            PLVL[1155] = 66724320000;
            PLVL[1156] = 66846720000;
            PLVL[1157] = 66969120000;
            PLVL[1158] = 67091520000;
            PLVL[1159] = 67213920000;
            PLVL[1160] = 67336320000;
            PLVL[1161] = 67458720000;
            PLVL[1162] = 67581120000;
            PLVL[1163] = 67703520000;
            PLVL[1164] = 67825920000;
            PLVL[1165] = 67948320000;
            PLVL[1166] = 68070720000;
            PLVL[1167] = 68193120000;
            PLVL[1168] = 68315520000;
            PLVL[1169] = 68437920000;
            PLVL[1170] = 68560320000;
            PLVL[1171] = 68682720000;
            PLVL[1172] = 68805120000;
            PLVL[1173] = 68927520000;
            PLVL[1174] = 69049920000;
            PLVL[1175] = 69172320000;
            PLVL[1176] = 69294720000;
            PLVL[1177] = 69417120000;
            PLVL[1178] = 69539520000;
            PLVL[1179] = 69661920000;
            PLVL[1180] = 69784320000;
            PLVL[1181] = 69906720000;
            PLVL[1182] = 70029120000;
            PLVL[1183] = 70151520000;
            PLVL[1184] = 70273920000;
            PLVL[1185] = 70396320000;
            PLVL[1186] = 70518720000;
            PLVL[1187] = 70641120000;
            PLVL[1188] = 70763520000;
            PLVL[1189] = 70885920000;
            PLVL[1190] = 71008320000;
            PLVL[1191] = 71130720000;
            PLVL[1192] = 71253120000;
            PLVL[1193] = 71375520000;
            PLVL[1194] = 71497920000;
            PLVL[1195] = 71620320000;
            PLVL[1196] = 71742720000;
            PLVL[1197] = 71865120000;
            PLVL[1198] = 71987520000;
            PLVL[1199] = 72109920000;
            PLVL[1200] = 72232320000;
            PLVL[1201] = 72354720000;
            PLVL[1202] = 72477120000;
            PLVL[1203] = 72599520000;
            PLVL[1204] = 72721920000;
            PLVL[1205] = 72844320000;
            PLVL[1206] = 72966720000;
            PLVL[1207] = 73089120000;
            PLVL[1208] = 73211520000;
            PLVL[1209] = 73333920000;
            PLVL[1210] = 73456320000;
            PLVL[1211] = 73578720000;
            PLVL[1212] = 73701120000;
            PLVL[1213] = 73823520000;
            PLVL[1214] = 73945920000;
            PLVL[1215] = 74068320000;
            PLVL[1216] = 74190720000;
            PLVL[1217] = 74313120000;
            PLVL[1218] = 74435520000;
            PLVL[1219] = 74557920000;
            PLVL[1220] = 74680320000;
            PLVL[1221] = 74802720000;
            PLVL[1222] = 74925120000;
            PLVL[1223] = 75047520000;
            PLVL[1224] = 75169920000;
            PLVL[1225] = 75292320000;
            PLVL[1226] = 75414720000;
            PLVL[1227] = 75537120000;
            PLVL[1228] = 75659520000;
            PLVL[1229] = 75781920000;
            PLVL[1230] = 75904320000;
            PLVL[1231] = 76026720000;
            PLVL[1232] = 76149120000;
            PLVL[1233] = 76271520000;
            PLVL[1234] = 76393920000;
            PLVL[1235] = 76516320000;
            PLVL[1236] = 76638720000;
            PLVL[1237] = 76761120000;
            PLVL[1238] = 76883520000;
            PLVL[1239] = 77005920000;
            PLVL[1240] = 77128320000;
            PLVL[1241] = 77250720000;
            PLVL[1242] = 77373120000;
            PLVL[1243] = 77495520000;
            PLVL[1244] = 77617920000;
            PLVL[1245] = 77740320000;
            PLVL[1246] = 77862720000;
            PLVL[1247] = 77985120000;
            PLVL[1248] = 78107520000;
            PLVL[1249] = 78229920000;
            PLVL[1250] = 78352320000;
            PLVL[1251] = 78474720000;
            PLVL[1252] = 78597120000;
            PLVL[1253] = 78719520000;
            PLVL[1254] = 78841920000;
            PLVL[1255] = 78964320000;
            PLVL[1256] = 79086720000;
            PLVL[1257] = 79209120000;
            PLVL[1258] = 79331520000;
            PLVL[1259] = 79453920000;
            PLVL[1260] = 79576320000;
            PLVL[1261] = 79698720000;
            PLVL[1262] = 79821120000;
            PLVL[1263] = 79943520000;
            PLVL[1264] = 80065920000;
            PLVL[1265] = 80188320000;
            PLVL[1266] = 80310720000;
            PLVL[1267] = 80433120000;
            PLVL[1268] = 80555520000;
            PLVL[1269] = 80677920000;
            PLVL[1270] = 80800320000;
            PLVL[1271] = 80922720000;
            PLVL[1272] = 81045120000;
            PLVL[1273] = 81167520000;
            PLVL[1274] = 81289920000;
            PLVL[1275] = 81412320000;
            PLVL[1276] = 81534720000;
            PLVL[1277] = 81657120000;
            PLVL[1278] = 81779520000;
            PLVL[1279] = 81901920000;
            PLVL[1280] = 82024320000;
            PLVL[1281] = 82146720000;
            PLVL[1282] = 82269120000;
            PLVL[1283] = 82391520000;
            PLVL[1284] = 82513920000;
            PLVL[1285] = 82636320000;
            PLVL[1286] = 82758720000;
            PLVL[1287] = 82881120000;
            PLVL[1288] = 83003520000;
            PLVL[1289] = 83125920000;
            PLVL[1290] = 83248320000;
            PLVL[1291] = 83370720000;
            PLVL[1292] = 83493120000;
            PLVL[1293] = 83615520000;
            PLVL[1294] = 83737920000;
            PLVL[1295] = 83860320000;
            PLVL[1296] = 83982720000;
            PLVL[1297] = 84105120000;
            PLVL[1298] = 84227520000;
            PLVL[1299] = 84349920000;
            PLVL[1300] = 84472320000;
            PLVL[1301] = 84594720000;
            PLVL[1302] = 84717120000;
            PLVL[1303] = 84839520000;
            PLVL[1304] = 84961920000;
            PLVL[1305] = 85084320000;
            PLVL[1306] = 85206720000;
            PLVL[1307] = 85329120000;
            PLVL[1308] = 85451520000;
            PLVL[1309] = 85573920000;
            PLVL[1310] = 85696320000;
            PLVL[1311] = 85818720000;
            PLVL[1312] = 85941120000;
            PLVL[1313] = 86063520000;
            PLVL[1314] = 86185920000;
            PLVL[1315] = 86308320000;
            PLVL[1316] = 86430720000;
            PLVL[1317] = 86553120000;
            PLVL[1318] = 86675520000;
            PLVL[1319] = 86797920000;
            PLVL[1320] = 86920320000;
            PLVL[1321] = 87042720000;
            PLVL[1322] = 87165120000;
            PLVL[1323] = 87287520000;
            PLVL[1324] = 87409920000;
            PLVL[1325] = 87532320000;
            PLVL[1326] = 87654720000;
            PLVL[1327] = 87777120000;
            PLVL[1328] = 87899520000;
            PLVL[1329] = 88021920000;
            PLVL[1330] = 88144320000;
            PLVL[1331] = 88266720000;
            PLVL[1332] = 88389120000;
            PLVL[1333] = 88511520000;
            PLVL[1334] = 88633920000;
            PLVL[1335] = 88756320000;
            PLVL[1336] = 88878720000;
            PLVL[1337] = 89001120000;
            PLVL[1338] = 89123520000;
            PLVL[1339] = 89245920000;
            PLVL[1340] = 89368320000;
            PLVL[1341] = 89490720000;
            PLVL[1342] = 89613120000;
            PLVL[1343] = 89735520000;
            PLVL[1344] = 89857920000;
            PLVL[1345] = 89980320000;
            PLVL[1346] = 90102720000;
            PLVL[1347] = 90225120000;
            PLVL[1348] = 90347520000;
            PLVL[1349] = 90469920000;
            PLVL[1350] = 90592320000;
            PLVL[1351] = 90714720000;
            PLVL[1352] = 90837120000;
            PLVL[1353] = 90959520000;
            PLVL[1354] = 91081920000;
            PLVL[1355] = 91204320000;
            PLVL[1356] = 91326720000;
            PLVL[1357] = 91449120000;
            PLVL[1358] = 91571520000;
            PLVL[1359] = 91693920000;
            PLVL[1360] = 91816320000;
            PLVL[1361] = 91938720000;
            PLVL[1362] = 92061120000;
            PLVL[1363] = 92183520000;
            PLVL[1364] = 92305920000;
            PLVL[1365] = 92428320000;
            PLVL[1366] = 92550720000;
            PLVL[1367] = 92673120000;
            PLVL[1368] = 92795520000;
            PLVL[1369] = 92917920000;
            PLVL[1370] = 93040320000;
            PLVL[1371] = 93162720000;
            PLVL[1372] = 93285120000;
            PLVL[1373] = 93407520000;
            PLVL[1374] = 93529920000;
            PLVL[1375] = 93652320000;
            PLVL[1376] = 93774720000;
            PLVL[1377] = 93897120000;
            PLVL[1378] = 94019520000;
            PLVL[1379] = 94141920000;
            PLVL[1380] = 94264320000;
            PLVL[1381] = 94386720000;
            PLVL[1382] = 94509120000;
            PLVL[1383] = 94631520000;
            PLVL[1384] = 94753920000;
            PLVL[1385] = 94876320000;
            PLVL[1386] = 94998720000;
            PLVL[1387] = 95121120000;
            PLVL[1388] = 95243520000;
            PLVL[1389] = 95365920000;
            PLVL[1390] = 95488320000;
            PLVL[1391] = 95610720000;
            PLVL[1392] = 95733120000;
            PLVL[1393] = 95855520000;
            PLVL[1394] = 95977920000;
            PLVL[1395] = 96100320000;
            PLVL[1396] = 96222720000;
            PLVL[1397] = 96345120000;
            PLVL[1398] = 96467520000;
            PLVL[1399] = 96589920000;
            PLVL[1400] = 96712320000;
            PLVL[1401] = 96834720000;
            PLVL[1402] = 96957120000;
            PLVL[1403] = 97079520000;
            PLVL[1404] = 97201920000;
            PLVL[1405] = 97324320000;
            PLVL[1406] = 97446720000;
            PLVL[1407] = 97569120000;
            PLVL[1408] = 97691520000;
            PLVL[1409] = 97813920000;
            PLVL[1410] = 97936320000;
            PLVL[1411] = 98058720000;
            PLVL[1412] = 98181120000;
            PLVL[1413] = 98303520000;
            PLVL[1414] = 98425920000;
            PLVL[1415] = 98548320000;
            PLVL[1416] = 98670720000;
            PLVL[1417] = 98793120000;
            PLVL[1418] = 98915520000;
            PLVL[1419] = 99037920000;
            PLVL[1420] = 99160320000;
            PLVL[1421] = 99282720000;
            PLVL[1422] = 99405120000;
            PLVL[1423] = 99527520000;
            PLVL[1424] = 99649920000;
            PLVL[1425] = 99772320000;
            PLVL[1426] = 99894720000;
            PLVL[1427] = 100017120000;
            PLVL[1428] = 100139520000;
            PLVL[1429] = 100261920000;
            PLVL[1430] = 100384320000;
            PLVL[1431] = 100506720000;
            PLVL[1432] = 100629120000;
            PLVL[1433] = 100751520000;
            PLVL[1434] = 100873920000;
            PLVL[1435] = 100996320000;
            PLVL[1436] = 101118720000;
            PLVL[1437] = 101241120000;
            PLVL[1438] = 101363520000;
            PLVL[1439] = 101485920000;
            PLVL[1440] = 101608320000;
            PLVL[1441] = 101730720000;
            PLVL[1442] = 101853120000;
            PLVL[1443] = 101975520000;
            PLVL[1444] = 102097920000;
            PLVL[1445] = 102220320000;
            PLVL[1446] = 102342720000;
            PLVL[1447] = 102465120000;
            PLVL[1448] = 102587520000;
            PLVL[1449] = 102709920000;
            PLVL[1450] = 102832320000;
            PLVL[1451] = 102954720000;
            PLVL[1452] = 103077120000;
            PLVL[1453] = 103199520000;
            PLVL[1454] = 103321920000;
            PLVL[1455] = 103444320000;
            PLVL[1456] = 103566720000;
            PLVL[1457] = 103689120000;
            PLVL[1458] = 103811520000;
            PLVL[1459] = 103933920000;
            PLVL[1460] = 104056320000;
            PLVL[1461] = 104178720000;
            PLVL[1462] = 104301120000;
            PLVL[1463] = 104423520000;
            PLVL[1464] = 104545920000;
            PLVL[1465] = 104668320000;
            PLVL[1466] = 104790720000;
            PLVL[1467] = 104913120000;
            PLVL[1468] = 105035520000;
            PLVL[1469] = 105157920000;
            PLVL[1470] = 105280320000;
            PLVL[1471] = 105402720000;
            PLVL[1472] = 105525120000;
            PLVL[1473] = 105647520000;
            PLVL[1474] = 105769920000;
            PLVL[1475] = 105892320000;
            PLVL[1476] = 106014720000;
            PLVL[1477] = 106137120000;
            PLVL[1478] = 106259520000;
            PLVL[1479] = 106381920000;
            PLVL[1480] = 106504320000;
            PLVL[1481] = 106626720000;
            PLVL[1482] = 106749120000;
            PLVL[1483] = 106871520000;
            PLVL[1484] = 106993920000;
            PLVL[1485] = 107116320000;
            PLVL[1486] = 107238720000;
            PLVL[1487] = 107361120000;
            PLVL[1488] = 107483520000;
            PLVL[1489] = 107605920000;
            PLVL[1490] = 107728320000;
            PLVL[1491] = 107850720000;
            PLVL[1492] = 107973120000;
            PLVL[1493] = 108095520000;
            PLVL[1494] = 108217920000;
            PLVL[1495] = 108340320000;
            PLVL[1496] = 108462720000;
            PLVL[1497] = 108585120000;
            PLVL[1498] = 108707520000;
            PLVL[1499] = 108829920000;
            PLVL[1500] = 108952320000;

        }

        private void txtTimeTo_TextChanged(object sender, EventArgs e)
        {
            int ParaLvl = 0;
            Int32.TryParse(txtTimeTo.Text, out ParaLvl);

            if (dataGridView1.Rows.Count > 0)
            {
                int index = dataGridView1.CurrentCell.RowIndex;

                if (ParaLvl > 1500)
                    MessageBox.Show("Supported paragon levels are 1-1500");
                else
                    LoadLastRun(index);
            }
        }

        FileSystemWatcher watcher = new FileSystemWatcher();
        Overlay frmOverlay = new Overlay();

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                string Diablo3SSPath;
                if (ConfigurationManager.AppSettings["Diablo3SSPath"].ToString().Length > 0)
                    Diablo3SSPath = ConfigurationManager.AppSettings["Diablo3SSPath"].ToString();
                else
                    Diablo3SSPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Diablo III\\Screenshots\\";

                watcher.Path = Diablo3SSPath;
                watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;
                watcher.Filter = "*.*";
                watcher.SynchronizingObject = this;
                watcher.Created += watcher_Created;
                watcher.EnableRaisingEvents = true;
            }
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            watcher.EnableRaisingEvents = false;

            System.Threading.Thread.Sleep(200);
            LoadScreens();

            watcher.EnableRaisingEvents = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (btnOverlay.Text == "Show overlay")
            {
                frmOverlay.Show();
                btnOverlay.Text = "Hide overlay";
            }
            else
            {
                frmOverlay.Hide();
                btnOverlay.Text = "Show overlay";
            }
        }
    }

    public class Ocr
    {
        public void DumpResult(List<tessnet2.Word> result)
        {
            foreach (tessnet2.Word word in result)
                Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
        }

        public List<tessnet2.Word> DoOCRNormal(Bitmap image, string lang)
        {
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            ocr.Init(null, lang, false);

            List<tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);
            
            return result;
        }
    }
}
