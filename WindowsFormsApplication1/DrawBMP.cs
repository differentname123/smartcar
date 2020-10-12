using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication1
{
    public class DrawBMP : Control
    {
        Bitmap bmp;
        int w, h;
        double Proportion;
        private int ImageCount = 0;
        public DrawBMP(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public DrawBMP()
        {
            Width = 400;
            Height = 400;

            bmp = new Bitmap(Height, Width);
        }
        public void DrawBMPInit(int width, int height)
        {
            Proportion = 1.0*height / width;
            bmp = new Bitmap(Height, Width);
            w = width;
            h = height;
        }
        public void PlotBMP(Bitmap b)
        {
            bmp = b;  
        }
        public void SaveImage(string name)
        {
            string Path;
            Path = System.IO.Directory.GetCurrentDirectory();

            bmp.Save(Path + "\\Picture\\" + name + ImageCount.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);//指定图片格式   
            ImageCount++;
        }
        public void SaveImage(string name,string SavePath)
        {
            string Path;
            Path = System.IO.Directory.GetCurrentDirectory();
            bmp.Save(Path + "\\" + SavePath + "\\" + name + ImageCount.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);//指定图片格式   
            ImageCount++;
        }
        public void DrawImage(Bitmap image)
        {
            DrawBMPInit(image.Width, image.Height);
            PlotBMP(image);
            this.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle myRec;
            if (Height > Width * Proportion)
            {
                myRec = new Rectangle(0, 0, (int)(Width * Proportion), Width); //指定显示区域的位置的大小
                g.DrawImage(this.bmp, myRec);
            }
            else if (Height < Width * Proportion)
            {
                myRec = new Rectangle(0, 0, Height, (int)(Height / Proportion)); //指定显示区域的位置的大小
                g.DrawImage(this.bmp, myRec);
            }
            else
            {
                myRec = new Rectangle(0, 0, Height, Width); //指定显示区域的位置的大小
                g.DrawImage(this.bmp, myRec);
            }
        }
    }
}
