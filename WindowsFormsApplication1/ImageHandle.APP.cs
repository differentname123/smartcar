using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace WindowsFormsApplication1
{
    unsafe partial class ImageHandle
    {
        /**********************************************************************************************************************************************************
         *函数名：HandleImage
         *  
         *作用：此函数用于对图像信息的处理，用户可自行在此添加代码
         * 
         *编写人：WangJianfeng 
         * 
         * 时间：2018/2/10
         * ********************************************************************************************************************************************************/
        public void HandleImage(Bitmap Src, Bitmap Dest, int Threshold,RichTextBox Display)
        {
            byte[,] GrayPicture = new byte[Src.Height, Src.Width];
            byte[,] DestPicture = new byte[Src.Height, Src.Width];
            /*********************************固定阀值二值化处理***********************************************/
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;         // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。            
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++, SrcP++, DestP++)
                {
                    *DestP = *SrcP > Threshold ? byte.MaxValue : byte.MinValue;     // 写成255和0，C#编译器不认。
                }
            }
            /**************************************************************************************************/
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                for (int X = 0; X < Width; X++, SrcP++)
                {
                    GrayPicture[Y, X] = *SrcP;
                }
            }
            for (int Y = 0; Y < Height; Y++)
            {
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++, DestP++)
                {
                    DestPicture[Y, X] = *DestP;
                }
            }
            /*******************************************************APP开始********************************************************************/

            APP(DestPicture, Dest.Height, Dest.Width, Display);




            /*******************************************************APP结束********************************************************************/
            for (int Y = 0; Y < Height; Y++)
            {           
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++,DestP++)
                {
                    *DestP = DestPicture[Y,X];    
                }
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
        }
        /**********************************************************************************************************************************************************
         *函数名：APP
         *  
         *作用：此函数用于对图像信息的处理，用户可自行在此添加代码  byte[,] Image  图像数组   Height    高  Width   宽
         * 
         *编写人：WangJianfeng 
         * 
         * 时间：2018/2/10
         * ********************************************************************************************************************************************************/
        /*用下面的画笔可以在图像上绘制自己的曲线*/
        /*画笔*/
        byte Withe = 255;
        byte Black = 0;
        byte Green = 130;
        byte Yellow = 140;
        byte Red = 120;
        byte Blue = 110;
        byte Greenyellow = 100;
        void APP(byte[,] Image, int Height, int Width, RichTextBox Display)
        {
           int[] Left = new int[Height];
           int[] Right = new int[Height];
           int[] Middle = new int[Height+1];
           Middle[Height] = Width / 2;
            //for (int Y = Height-1; Y > 0; Y--)
            //{
            //    for (int X = Middle[Y+1]; X > 0; X--)
            //    {
            //        if (Image[Y, X + 1] == Withe && Image[Y, X] == Black)
            //        {
            //            Left[Y] = X;
            //            break;
            //        }
            //    }
            //    for (int X = Middle[Y+1]; X < Width; X++)
            //    {
            //        if (Image[Y, X - 1] == Withe && Image[Y, X] == Black)
            //        {
            //            Right[Y] = X;
            //            break;
            //        }
            //    }
            //    Middle[Y] = (Left[Y] + Right[Y]) / 2;
            //}
                
            //    for (int Y = 0; Y < Height; Y++)
            //    {

            //        for (int X = 0; X < Width; X++)
            //        {
            //            Image[Y, Left[Y]] = Red;
            //            Image[Y, Right[Y]] = Red;
            //            Image[Y, Middle[Y]] = Red;
            //        }
            //    }
           Display.Text = "你好";
        }
    }
}
