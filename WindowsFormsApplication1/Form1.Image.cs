using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
    unsafe partial class Form1
    {
        private bool IsGrayBitmap(Bitmap Bmp)
        {
            if (Bmp.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                return false;
            }
            if (Bmp.Palette.Entries.Length != 256)
            {
                return false;
            }
            for (int Y = 0; Y < Bmp.Palette.Entries.Length; Y++)
            {
                if (Bmp.Palette.Entries[Y] != Color.FromArgb(255, Y, Y, Y))
                {
                    return false;
                }
            }
            return true;
        }
        private Bitmap CreateGrayBitmap(int Width, int Height)
        {
            //创建8位深度的灰度图像
            Bitmap Bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            ColorPalette Pal = Bmp.Palette;
            for (int Y = 0; Y < Pal.Entries.Length; Y++)
            {
                Pal.Entries[Y] = Color.FromArgb(255, Y, Y, Y);//将RGB转化为灰度调色板
            }
            Bmp.Palette = Pal;
            return Bmp;
        }
        private Bitmap CreateGray1Bitmap(int Width, int Height)
        {
            //创建8位深度的灰度图像
            Bitmap Bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            ColorPalette Pal = Bmp.Palette;
            for (int Y = 0; Y < Pal.Entries.Length; Y++)
            {
                if(Y == 120)
                {
                    Pal.Entries[Y] = Color.FromArgb(255, 255, 0, 0);
                }
                else if (Y == 130)
                {
                    Pal.Entries[Y] = Color.FromArgb(255, 0, 0x80, 0);
                }
                else if(Y == 110)
                {
                    Pal.Entries[Y] = Color.FromArgb(255, 0, 0, 255);
                }
                else if(Y == 100)
                {
                    Pal.Entries[Y] = Color.FromArgb(255, 0, 255, 0);
                }
                else if(Y == 140)
                {
                    Pal.Entries[Y] = Color.FromArgb(255, 255, 255, 0);
                }
                else 
                {
                    Pal.Entries[Y] = Color.FromArgb(255, Y, Y, Y);
                }
            }
            Bmp.Palette = Pal;
            return Bmp;
        }
        private Bitmap ConvertToGrayBitmap(Bitmap Src)
        {
            Bitmap Dest = CreateGrayBitmap(Src.Width, Src.Height);
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;         // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。            
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++)
                {
                    *DestP = (byte)((*SrcP + (*(SrcP + 1) << 1) + *(SrcP + 2)) >> 2);//将彩色图像转化为灰度图像的变换公式
                    SrcP += 3;
                    DestP++;
                }
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);//解锁
            return Dest;
        }
        public void DrawHistGram(Bitmap SrcBmp, int[] Histgram)
        {
            BitmapData HistData = SrcBmp.LockBits(new Rectangle(0, 0, SrcBmp.Width, SrcBmp.Height), ImageLockMode.ReadWrite, SrcBmp.PixelFormat);
            int X, Y, Max = 0;
            byte* P;
            /*****************最大灰度值*****************************************/
            for (Y = 0; Y < 256; Y++)
            {
                if (Max < Histgram[Y])
                    Max = Histgram[Y];
            }
            /*****************绘制灰度百分比*****************************************/
            for (X = 0; X < 256; X++)
            {
                P = (byte*)HistData.Scan0 + X;
                for (Y = 0; Y < 100; Y++)
                {
                    if ((100 - Y) > Histgram[X] * 100 / Max)            //确定高度
                        *P = 255;
                    else
                        *P = 0;
                    P += HistData.Stride;
                }
            }
            /*****************二值化阀值点*****************************************/
            P = (byte*)HistData.Scan0 + Convert.ToInt16(textBox1.Text);           
            for (Y = 0; Y < 100; Y++)
            {
                *P = 127;
                P += HistData.Stride;
            }
            SrcBmp.UnlockBits(HistData);
        }
    }
}
