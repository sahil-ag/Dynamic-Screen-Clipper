using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace clipper
{
    public partial class Form1 : Form
    {
        
        //include SendMessage
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        //this is a constant indicating the window that we want to send a text message
        //const int WM_SETTEXT = 0X000C;
        IntPtr curWindows;
        Bitmap y;

        public Form1(IntPtr selectedwin)
        {
            curWindows = selectedwin;
            y = PrintWindow(curWindows);
            BackgroundImage = y;
            Size = y.Size;
            InitializeComponent();
        }

        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
        

        
            
            //sending the message to the textbox
            //SendMessage(notepadTextbox, WM_SETTEXT, 0, "This is the new Text!!!");
            
            //BackgroundImage = new Bitmap(y, new Size(Size.Width, Size.Height));
        

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            BackgroundImage = new Bitmap(y, new Size(Size.Width, Size.Height));
        }
    }
}
