using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
     unsafe partial  class ImageHandle
    {
        /**********************************************************************************************************************************************************
         * 函数名：BinaryImage
         *  
         * 作用：固定阀值得二值化处理
         * 
         *编写人：WangJianfeng 
         * 
         * 时间：2018/2/10
         * ********************************************************************************************************************************************************/
        public void DoBinaryzation(Bitmap Src, Bitmap Dest, int Threshold)
        {
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;         // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。            
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++, SrcP++, DestP++)
                    *DestP = *SrcP > Threshold ? byte.MaxValue : byte.MinValue;     // 写成255和0，C#编译器不认。
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
        }
        public void GetHistGram(Bitmap Src, int[] HistGram)
        {
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height, SrcStride = SrcData.Stride;
            byte* SrcP;
            for (int Y = 0; Y < 256; Y++) 
                HistGram[Y] = 0;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                for (int X = 0; X < Width; X++, SrcP++) 
                    HistGram[*SrcP]++;
            }
            Src.UnlockBits(SrcData);
        }
    }
}
