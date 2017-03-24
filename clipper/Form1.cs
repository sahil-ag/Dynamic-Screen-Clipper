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
        //include FindWindowEx
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        //include SendMessage
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        //this is a constant indicating the window that we want to send a text message
        const int WM_SETTEXT = 0X000C;

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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //getting notepad's process | at least one instance of notepad must be running
            Process notepadProccess = Process.GetProcessesByName("notepad")[0];

            //getting notepad's textbox handle from the main window's handle
            //the textbox is called 'Edit'
            //IntPtr notepadTextbox = FindWindowEx(notepadProccess.MainWindowHandle, IntPtr.Zero, "Edit", null);
            //sending the message to the textbox
            //SendMessage(notepadTextbox, WM_SETTEXT, 0, "This is the new Text!!!");
            Bitmap y = PrintWindow(notepadProccess.MainWindowHandle);
            //float scale = Math.Min(Size.Width / y.Width, Size.Height / y.Height);
            Bitmap x = new Bitmap(y, new Size(Size.Width, Size.Height));
            BackgroundImage = x;


        }
    }
}
