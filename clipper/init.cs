using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace clipper
{

    public partial class init : Form
    {
        // DECLARE GLOBALS //
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;



        //SortedSet<Int64> oldWindow = new SortedSet<Int64>();
        //IntPtr original;
        public init()
        {
            InitializeComponent();
        }

        bool _canDraw = false;
        int startX, startY, xx, yy, xwidth, xheight;
        Rectangle rect;

        private void init_MouseDown(object sender, MouseEventArgs e)
        {
            _canDraw = true;
            startX = e.X;
            startY = e.Y;
        }
        
        private void init_MouseUp(object sender, MouseEventArgs e)
        {
            //The system is no longer allowed to draw rectangles
            _canDraw = false;
            //SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            //this.Hide();
            
            int x = startX;
            int y = startY;
            int xWidth = xwidth;
            int yHeight = xheight;
            rect = new Rectangle(xx, yy, xWidth, yHeight);
            Enabled = false;
            Visible = false;
            Application.Exit();
            Thread app = new Thread(() => Application.Run(new hook(rect)));
            app.Start();


        }
        

        Rectangle _rect;
        
        private void init_MouseMove(object sender, MouseEventArgs e)
        {
            //If we are not allowed to draw, simply return and disregard the rest of the code
            if (!_canDraw) return;

            //Refresh the form and draw the rectangle
            Refresh();
            //The x-value of our rectangle should be the minimum between the start x-value and the current x-position
            int x = Math.Min(startX, e.X);
            //The y-value of our rectangle should also be the minimum between the start y-value and current y-value
            int y = Math.Min(startY, e.Y);
            xx = x; yy = y;
            //The width of our rectangle should be the maximum between the start x-position and current x-position minus
            //the minimum of start x-position and current x-position
            int width = Math.Max(startX, e.X) - Math.Min(startX, e.X);

            //For the hight value, it's basically the same thing as above, but now with the y-values:
            int height = Math.Max(startY, e.Y) - Math.Min(startY, e.Y);
            _rect = new Rectangle(x, y, width, height);
            xwidth = width; xheight = height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, _rect);
            }
        }
    }
}
