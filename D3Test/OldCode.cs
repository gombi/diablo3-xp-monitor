using System;
using System.Collections.Generic;
using System.Text;

namespace D3Test
{
    class OldCode
    {
        //private int GetXp(string path, bool MakeBig, ref int ParaLvl)
        //{
        //    string OrgFilename = path;
        //    string SmallFileName = Diablo3Monitor + "\\" + Path.GetFileName(OrgFilename);

        //    Bitmap bip = new Bitmap(OrgFilename);

        //    Rectangle rec;
        //    if (bip.Height == 1080 && bip.Width == 1920)
        //     rec = new Rectangle(764, 890, 390, 80);
        //    else if (bip.Height == 1200 && bip.Width == 1920)
        //        rec = new Rectangle(744, 999, 435, 80);
        //    else
        //    {
        //        MessageBox.Show("Forkert format. Send Screenshots til Gombi!!!!");
        //        return 0;
        //    }

        //    Bitmap bipNew = bip.Clone(rec, System.Drawing.Imaging.PixelFormat.DontCare);

        //    if (MakeBig)
        //        bipNew = ResizeBitmap(bipNew, bipNew.Width * 2, bipNew.Height * 2);

        //    bipNew.Save(SmallFileName);

        //    try
        //    {
        //        //OCR Operations ... 
        //        MODI.Document md = new MODI.Document();
        //        md.Create(Convert.ToString(SmallFileName));
        //        md.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, true, true);
        //        MODI.Image image = (MODI.Image)md.Images[0];
        //        string text = image.Layout.Text;
        //        string currentXP = text.Substring(text.IndexOf(':') + 2, text.IndexOf('/') - text.IndexOf(':') - 2).Replace(",", "").Replace(" ", "");
        //        int tempPlvl;
        //        Int32.TryParse(text.Substring(text.IndexOf('(') + 1, text.IndexOf(')') - text.IndexOf('(') - 1), out tempPlvl);
        //        ParaLvl = tempPlvl;

        //        int res;
        //        Int32.TryParse(currentXP, out res);

        //        //Ryd op
        //        bip.Dispose();
        //        bipNew.Dispose();
        //        image = null;
        //        md.Close();

        //        // Delete small
        //        //File.Delete(SmallFileName);

        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return 0;
        //    }
        //}

        //private int GetXp2(string path, ref int ParaLvl)
        //{
        //    string OrgFilename = path;
        //    string SmallFileName = Diablo3Monitor + "\\" + Path.GetFileName(OrgFilename);

        //    Bitmap bip = new Bitmap(OrgFilename);

        //    using (bip)
        //    {
        //        Rectangle rec;
        //        if (bip.Height == 1080 && bip.Width == 1920)
        //            rec = new Rectangle(764, 890, 390, 80);
        //        else if (bip.Height == 1200 && bip.Width == 1920)
        //            rec = new Rectangle(744, 999, 435, 80);
        //        else
        //        {
        //            MessageBox.Show("Forkert format. Send Screenshots til Gombi!!!!");
        //            return 0;
        //        }

        //        Bitmap bipNew = bip.Clone(rec, System.Drawing.Imaging.PixelFormat.DontCare);
        //        using (bipNew)
        //        {
        //            Color pixelColor;
        //            byte A, R, G, B;
        //            for (int y = 0; y < bipNew.Height; y++)
        //            {
        //                for (int x = 0; x < bipNew.Width; x++)
        //                {
        //                    pixelColor = bipNew.GetPixel(x, y);

        //                    A = pixelColor.A;
        //                    R = (byte)(255 - pixelColor.R);
        //                    G = (byte)(255 - pixelColor.G);
        //                    B = (byte)(255 - pixelColor.B);

        //                    bipNew.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
        //                }
        //            }

        //            bipNew.Save(SmallFileName);
        //        }
        //    }

        //    try
        //    {
        //        Puma.Net.PumaPage page = new Puma.Net.PumaPage(SmallFileName);
        //        using (page)
        //        {
        //            page.EnableSpeller = false;
        //            page.Language = Puma.Net.PumaLanguage.English;
        //            page.FileFormat = Puma.Net.PumaFileFormat.TxtAscii;

        //            string text2 = page.RecognizeToString();

        //            string currentXP = text2.Substring(text2.IndexOf("Expenence") + 10, text2.IndexOf('/') - (text2.IndexOf("Expenence") + 10));
        //            currentXP = currentXP.Replace("I", "1");
        //            currentXP = currentXP.Replace("i", "1");
        //            currentXP = currentXP.Replace("O", "0");
        //            currentXP = currentXP.Replace("o", "0");
        //            currentXP = currentXP.Replace(",", "");

        //            string ParaMagicFind = "";
        //            if (text2.IndexOf("%") < text2.IndexOf("\r\n"))
        //            {
        //                int ProIndex = text2.IndexOf('%');
        //                text2 = text2.Remove(ProIndex, 1);
        //                text2 = text2.Insert(ProIndex, "7");
        //                ParaMagicFind = text2.Substring(text2.IndexOf(Environment.NewLine) + 4, text2.IndexOf('%') - text2.IndexOf(Environment.NewLine) - 4);
        //            }
        //            else
        //                ParaMagicFind = text2.Substring(text2.IndexOf(Environment.NewLine) + 4, text2.IndexOf('%') - text2.IndexOf(Environment.NewLine) - 4);

        //            int intMagicFind;
        //            Int32.TryParse(ParaMagicFind, out intMagicFind);
        //            ParaLvl = intMagicFind / 3;

        //            int res;
        //            Int32.TryParse(currentXP, out res);
        //            return res;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return 0;
        //    }
        //}

        //private string GetText(string path)
        //{
        //    string OrgFilename = path;

        //    try
        //    {
        //        //OCR Operations ... 
        //        MODI.Document md = new MODI.Document();
        //        md.Create(Convert.ToString(OrgFilename));
        //        md.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, true, true);
        //        MODI.Image image = (MODI.Image)md.Images[0];
        //        string text = image.Layout.Text;

        //        //Ryd op
        //        image = null;
        //        md.Close();

        //        // Delete small
        //        //File.Delete(SmallFileName);

        //        return text;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return "";
        //    }
        //}

        //private string GetText2(string path)
        //{
        //    string OrgFilename = path;

        //    try
        //    {
        //        Puma.Net.PumaPage page = new Puma.Net.PumaPage(path);
        //        using (page)
        //        {
        //            page.EnableSpeller = false;
        //            page.Language = Puma.Net.PumaLanguage.English;
        //            page.FileFormat = Puma.Net.PumaFileFormat.TxtAscii;

        //            string text2 = page.RecognizeToString();
        //            return text2;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return "";
        //    }
        //    return "";
        //}
    }
}
