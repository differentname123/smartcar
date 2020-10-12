using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public unsafe partial  class Form1 : Form
    {
        public SerialPort Port;
        private object ThisLockSerial = new object();
        public List<Byte> listRecvRecordData = new List<byte>();
        int LastReceiveCount = 0;
        int ReceiveDataCount = 0;
        int ImageReceiveCount = 0;
        int ImageHeight = 0;
        int ImageWidth = 0;
        bool LastStatue = false;
        private int ImageCount = 0;
        ImageHandle ImageHandle1 = new ImageHandle();
        private Bitmap SrcBmp;
        private Bitmap DestBmp;
        private Bitmap HandleBmp;
        private Bitmap HistBmp;
        private Bitmap SmoothHistBmp;

        private int[] HistGram = new int[256];
        private int[] HistGramS = new int[256];


        public Form1()
        {
            InitializeComponent();
        }
        private void SearchAllComPort()
        {
            string[] COMNames = SerialPort.GetPortNames();
            ComboBox_COM.Items.Clear();
            for (int i = 0; i < COMNames.Length; i++)
            {
                ComboBox_COM.Items.Add(COMNames[i]);
            }
            if (ComboBox_COM.Items.Count > 0)
            {
                ComboBox_COM.SelectedIndex = 0;
            }
        }
        private void SeriaPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int recvCount, ret, i;
            string Str;
            byte[] buff;
            Str = string.Empty;
            try
            {
                recvCount = Port.BytesToRead;
                buff = new byte[recvCount];
                ret = Port.Read(buff, 0, recvCount);
                if (ret > 0)
                {
                    lock (ThisLockSerial)
                    {
                        if (listRecvRecordData == null)
                            listRecvRecordData = new List<byte>();
                        for (i = 0; i < recvCount; i++)
                        {
                            listRecvRecordData.Add(buff[i]);
                            ReceiveDataCount++;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Button_Gain_Click(object sender, EventArgs e)
        {
            SearchAllComPort();
        }
        private void Button_Open_Click(object sender, EventArgs e)
        {
            if (Button_Open.Text == "打开串口")
            {
                Port.PortName = ComboBox_COM.Text.Trim();
                Port.BaudRate = Convert.ToInt32(ComboBox_Band.Text.Trim());
                Port.DataBits = Convert.ToInt32(ComboBox_Data.Text.Trim());
                Port.Parity = Parity.None;
                if (ComboBox_Verify.Text.Trim() == "无校验")
                {
                    Port.Parity = Parity.None;
                }
                else if (ComboBox_Verify.Text.Trim() == "偶校验")
                {
                    Port.Parity = Parity.Even;
                }
                else
                {
                    Port.Parity = Parity.Odd;
                }
                Port.StopBits = StopBits.One;
                if (ComboBox_Stop.Text.Trim() == "1")
                {
                    Port.StopBits = StopBits.One;
                }
                else if (ComboBox_Stop.Text.Trim() == "1.5")
                {
                    Port.StopBits = StopBits.OnePointFive;
                }
                else if (ComboBox_Stop.Text.Trim() == "2")
                {
                    Port.StopBits = StopBits.Two;
                }

                if (Port.IsOpen)
                {
                    Port.Close();
                }
                try
                {
                    Port.Open();
                }
                catch
                {
                    MessageBox.Show("端口打开失败");
                }
                if (Port.IsOpen)
                {
                    label5.Text = "打开成功";
                    label5.ForeColor = Color.Red;
                    Button_Open.Text = "关闭串口";
                    PortEnable(false);
                }
                else
                {
                    label5.Text = "打开失败";
                    label5.ForeColor = Color.Blue;
                }
            }
            else 
            {
                Port.Close();
                Button_Open.Text = "打开串口";
                label5.Text = "关闭成功";
                PortEnable(true);
                listRecvRecordData.Clear();
                Timer_Image.Stop();
            }
        }
        private void PortEnable(bool E)
        {
            ComboBox_Stop.Enabled = E;
            ComboBox_Data.Enabled = E;
            ComboBox_COM.Enabled = E;
            ComboBox_Band.Enabled = E;
            ComboBox_Verify.Enabled = E;
        }
        private void SerialInit()
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            listRecvRecordData.Clear();
            SearchAllComPort();
            if (Port == null)
            {
                Port = new SerialPort();
            }
            Port.DataReceived += new SerialDataReceivedEventHandler(SeriaPort_DataReceived);
        }
        void InitParameter()
        {
            string str;

            LastReceiveCount = 0;
            ReceiveDataCount = 0;
            ImageReceiveCount = 0;
            int abc = comboBox_ImageSize.Text.LastIndexOf("x");
            str = comboBox_ImageSize.Text.Substring(0, abc);
            ImageWidth = Convert.ToInt32(str);
            str = comboBox_ImageSize.Text.Substring(abc + 1, abc);
            ImageHeight = Convert.ToInt32(str);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitForm();
            InitImage();
            SerialInit();
            InitParameter();
        }
        void InitImage()
        {
            Bitmap Temp = (Bitmap)Bitmap.FromFile(System.IO.Directory.GetCurrentDirectory() + "\\Picture\\Image" + "0" + ".bmp");
            if (IsGrayBitmap(Temp) == true)
            {
                SrcBmp = Temp;
            }
            else
            {
                SrcBmp = ConvertToGrayBitmap(Temp);
                Temp.Dispose();                                             //释放图像所占用的资源
            }
            DestBmp = CreateGrayBitmap(SrcBmp.Width, SrcBmp.Height);
            comboBox_ImageSize.Text = SrcBmp.Height.ToString() + 'x' + SrcBmp.Width.ToString();
            SrcPicture.Image = SrcBmp;
            HistBmp = CreateGrayBitmap(256, 100);
            SmoothHistBmp = CreateGrayBitmap(256, 100);
            SrcHist.Image = HistBmp;
            ImageHandle1.GetHistGram(SrcBmp, HistGram);

            DrawHistGram(HistBmp, HistGram);
            SrcHist.Invalidate();
        }
        public void SaveImage(string name)
        {
            string Path;
            Path = System.IO.Directory.GetCurrentDirectory();

            SrcPicture.Image.Save(Path + "\\Picture\\" + name + ImageCount.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);//指定图片格式   
            ImageCount++;
        }
        void SaveImage(Image Src, string name)
        {
            string Path;
            Path = System.IO.Directory.GetCurrentDirectory();

            Src.Save(Path + "\\Picture\\" + name + ImageCount.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);//指定图片格式   
            ImageCount++;
        }
        public void SaveImage(string name, string SavePath)
        {
            string Path;
            Path = System.IO.Directory.GetCurrentDirectory();
            SrcPicture.Image.Save(Path + "\\" + SavePath + "\\" + name + ImageCount.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);//指定图片格式   
            ImageCount++;
        }
        private void 打开图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;                              //假设用户在搜索文件的过程中更改了目录，那么，如果对话框会将当前目录还原为初始值，则值为 true；反之，值为 false。 默认值为 false。            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SrcBmp.Dispose();
                Bitmap Temp = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                if (IsGrayBitmap(Temp) == true)
                {
                    SrcBmp = Temp;
                }
                else
                {
                    SrcBmp = ConvertToGrayBitmap(Temp);
                    Temp.Dispose();                                             //释放图像所占用的资源
                }
                SrcPicture.Image = SrcBmp;
                DestBmp.Dispose();
                DestBmp = CreateGrayBitmap(SrcBmp.Width, SrcBmp.Height);
                ImageHandle1.GetHistGram(SrcBmp, HistGram);
                DrawHistGram(HistBmp, HistGram);
                SrcHist.Invalidate();
                comboBox_ImageSize.Text = SrcBmp.Height.ToString() + 'x' + SrcBmp.Width.ToString();
                richTextBox1.Text = openFileDialog.ToString();
            }
        }
        private void 保存图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage("Imaging");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "接收图像")
            {
                Timer_Image.Start();
                LastReceiveCount = ReceiveDataCount;
                button1.Text = "不接收图像";
            }
            else if (button1.Text == "不接收图像")
            {
                button1.Text = "接收图像";
                Timer_Image.Stop();
            }
        }
        void ReceiveImage()
        {
            int i = 0;
            i = LastReceiveCount;
            while (i < ReceiveDataCount - ImageHeight * ImageWidth - 10)
            {
                if (listRecvRecordData[i] == 0xFE && listRecvRecordData[i + 1] == 0xEF)
                {
                    if(listRecvRecordData[i + 2 + ImageHeight * ImageWidth] == 0xEF && listRecvRecordData[i + 3 + ImageHeight * ImageWidth] == 0xFE)
                    {

                        i++;
                        i++;
                        ImageReceiveCount++;
                        TextBox_ReceiveCount.Text = ImageReceiveCount.ToString();
                        SrcBmp.Dispose();
                        SrcBmp = CreateGrayBitmap(ImageHeight, ImageWidth);
                        BitmapData SrcData = SrcBmp.LockBits(new Rectangle(0, 0, ImageHeight, ImageWidth), ImageLockMode.ReadWrite, SrcBmp.PixelFormat);
                        int SrcStride = SrcData.Stride;
                        byte* SrcP;
                        for (int Y = 0; Y < ImageWidth; Y++)
                        {
                            SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;                // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。 
                            for (int X = 0; X < ImageHeight; SrcP++, X++)
                            {
                                *SrcP = listRecvRecordData[i];
                                i++;
                            }
                        }
                        SrcBmp.UnlockBits(SrcData);
                        ImageHandle1.GetHistGram(SrcBmp, HistGram);
                        DrawHistGram(HistBmp, HistGram);
                        SrcHist.Invalidate();

                        SrcPicture.Image = SrcBmp;
                        if (radioButton1.Checked)
                        {
                            SaveImage("imaging","AutoPicture");
                        }
                        i++;
                        i++;
                        LastReceiveCount = i;
                    }
                }
                i++;
            }
            
        }
        private void Timer_Image_Tick(object sender, EventArgs e)
        {
            ReceiveImage();
        }
        private void comboBox_ImageSize_TextChanged(object sender, EventArgs e)
        {
            int abc = comboBox_ImageSize.Text.LastIndexOf("x");
            ImageWidth = Convert.ToInt32(comboBox_ImageSize.Text.Substring(0 , abc));
            ImageHeight = Convert.ToInt32(comboBox_ImageSize.Text.Substring(abc + 1, abc));
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SaveImage(DestPicture.Image,"Imaged");
        }
        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
            if (LastStatue)
            {
                radioButton1.Checked = false;
                LastStatue = false;
            }
            else
            {
                radioButton1.Checked = true;
                LastStatue = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ImageHandle1.DoBinaryzation(SrcBmp, DestBmp,110);
            DestPicture.Image = DestBmp;
            textBox1.Text = 110.ToString();
            DrawHistGram(HistBmp, HistGram);
            SrcHist.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HandleBmp = CreateGray1Bitmap(SrcBmp.Width, SrcBmp.Height);
            ImageHandle1.HandleImage(SrcBmp, HandleBmp, 110, richTextBox1);
            DestPicture.Image = HandleBmp;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ImageHandle1.DoBinaryzation(SrcBmp, DestBmp, Convert.ToInt16(textBox1.Text));
            DestPicture.Image = DestBmp;
            DrawHistGram(HistBmp, HistGram);
            SrcHist.Invalidate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ImageHandle1.DoBinaryzation(SrcBmp, DestBmp, Threshold.GetOSTUThreshold(HistGram));
            DestPicture.Image = DestBmp;
            textBox1.Text = Threshold.GetOSTUThreshold(HistGram).ToString();
            DrawHistGram(HistBmp, HistGram);
            SrcHist.Invalidate();
        }

        private void comboBox_ImageSize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
