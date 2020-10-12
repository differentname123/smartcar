using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WindowsFormsApplication1
{
    partial class Form1
    {
        AutoSizeFormClass asc = new AutoSizeFormClass();
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && this.WindowState != FormWindowState.Maximized)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 274, 61440 + 9, 0);
            }
        }
        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case (int)WindowsMessage.WM_NCHITTEST:
                    this.WM_NCHITTEST(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        public static int LOWORD(int value)
        {
            return value & 0xFFFF;
        }
        public static int HIWORD(int value)
        {
            return value >> 16;
        }
        private void WM_NCHITTEST(ref Message m)
        {
            int wparam = m.LParam.ToInt32();
            Point point = new Point(LOWORD(wparam), HIWORD(wparam));
            point = this.PointToClient(point);

            if (point.X <= 5)
            {
                if (point.Y <= 5)
                    m.Result = (IntPtr)WinAPIConst.HTTOPLEFT;
                else if (point.Y > this.Height - 5)
                    m.Result = (IntPtr)WinAPIConst.HTBOTTOMLEFT;
                else
                    m.Result = (IntPtr)WinAPIConst.HTLEFT;
            }
            else if (point.X >= this.Width - 5)
            {
                if (point.Y <= 5)
                    m.Result = (IntPtr)WinAPIConst.HTTOPRIGHT;
                else if (point.Y >= this.Height - 5)
                    m.Result = (IntPtr)WinAPIConst.HTBOTTOMRIGHT;
                else
                    m.Result = (IntPtr)WinAPIConst.HTRIGHT;
            }
            else if (point.Y <= 5)
            {
                m.Result = (IntPtr)WinAPIConst.HTTOP;
            }
            else if (point.Y >= this.Height - 5)
            {
                m.Result = (IntPtr)WinAPIConst.HTBOTTOM;
            }
            else
                base.WndProc(ref m);
        }
        public enum WindowsMessage
        {
            /// <summary>
            /// 移动鼠标，桉树或释放鼠标时发生
            /// </summary>
            WM_NCHITTEST = 0x84,
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
        void InitForm()
        {
            this.TipMain.SetToolTip(this.pictureBox1, "关闭");
            this.TipMain.SetToolTip(this.pictureBox2, "最大化");
            this.TipMain.SetToolTip(this.pictureBox3, "最小化");
            asc.controllInitializeSize(this);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!this.ShowInTaskbar)
            {
                this.Hide();
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                this.TipMain.SetToolTip(this.pictureBox2, "还原");
            }
            else
            {
                this.TipMain.SetToolTip(this.pictureBox2, "最大化");
                this.WindowState = FormWindowState.Normal;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
