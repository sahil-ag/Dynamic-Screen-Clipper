using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

namespace clipper
{
    public partial class Form1 : Form
    {
        
        //include SendMessage
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        //this is a constant indicating the window that we want to send a text message
        //const int WM_SETTEXT = 0X000C;
        IntPtr curWindows;
        Bitmap y;
        public Rectangle curRect;
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public static void MakeTopMost(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke((Action)delegate { MakeTopMost(form); });
                return;
            }

            SetWindowPos(form.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public void paint()
        {
            using (y = PrintWindow(curWindows))
            {
                Image temp1 = BackgroundImage;
                try
                {
                    this.BackgroundImage = new Bitmap(y, Size.Width, Size.Height);
                }
                catch
                {
                    try
                    {
                        this.BackgroundImage = new Bitmap(y);
                    }
                    catch
                    {
                        //this.Enabled = false;
                        timer1.Stop();
                        timer1.Enabled = false;
                        //this.Visible = false;
                    }
                    Console.Write("lOL");
                }
                
                try
                {
                    temp1.Dispose();
                }
                catch
                {

                }

            }
        }

        public Form1(IntPtr selectedwin, Rectangle rect)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            curWindows = selectedwin;
            Console.WriteLine(curWindows.ToString());
            curRect = rect;
            Size = curRect.Size;
            paint();
            timer1.Start();
        }

        public Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);

            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppPArgb))
            {
                Graphics gfxBmp = Graphics.FromImage(bmp);
                IntPtr hdcBitmap = gfxBmp.GetHdc();

                PrintWindow(hwnd, hdcBitmap, 0);

                gfxBmp.ReleaseHdc(hdcBitmap);
                gfxBmp.Dispose();
                Bitmap x;
                try
                {
                    x = bmp.Clone(curRect, PixelFormat.Format32bppPArgb);
                }
                catch
                {
                    x = bmp;
                }
                return x;
                
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }


        bool appSelect = false; //false for clipped window and true for real window

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            paint();
            timer1.Start();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern bool SetCursorPos(uint x, uint y);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInputData mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [Flags]
        enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }
        enum SendInputEventType : int
        {
            InputMouse,
            InputKeyboard,
            InputHardware
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !appSelect)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            paint();
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            MakeTopMost(this);
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }


        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                appSelect = !appSelect;
            }
            else if (appSelect && e.Button == MouseButtons.Left)
            {
                //SetCursorPos((uint)(curRect.X + (e.X*(curRect.Width*1.0)/Size.Width)), (uint)(curRect.Y + (e.Y * (curRect.Height*1.0) / Size.Height)));
                SetForegroundWindow(curWindows);

                Point orig = Cursor.Position;
                INPUT mouseDownInput = new INPUT();
                mouseDownInput.type = SendInputEventType.InputMouse;
                
                mouseDownInput.mkhi.mi.dx = (int)(curRect.X + (e.X * (curRect.Width * 1.0) / Size.Width));
                mouseDownInput.mkhi.mi.dy = (int)(curRect.Y + (e.Y * (curRect.Height * 1.0) / Size.Height));
                mouseDownInput.mkhi.mi.mouseData = 0;
                mouseDownInput.mkhi.mi.time = 0;

                //mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
                //SendInput(1, ref mouseDownInput, Marshal.SizeOf(mouseDownInput));
                SetCursorPos((uint)mouseDownInput.mkhi.mi.dx, (uint)mouseDownInput.mkhi.mi.dy);

                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(mouseDownInput));

                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(mouseDownInput));
                //mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)(curRect.X + (e.X * (curRect.Width*1.0) / Size.Width)), (uint)(curRect.Y + (e.Y * (curRect.Height*1.0) / Size.Height)), 0, UIntPtr.Zero);
                SetWindowPos(curWindows, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                SetCursorPos((uint)orig.X, (uint)orig.Y);

            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            const uint WM_KEYDOWN = 0x100;
            PostMessage(curWindows, WM_KEYDOWN, ((IntPtr)e.KeyValue), (IntPtr)0);
        }
    }
}
