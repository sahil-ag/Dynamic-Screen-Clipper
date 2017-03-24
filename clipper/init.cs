using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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

        SortedSet<Int64> oldWindow = new SortedSet<Int64>();
        IntPtr original;
        public init()
        {
            InitializeComponent();
            original = IntPtr.Zero;
            oldWindow.Add(original.ToInt64());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // get handle
            IntPtr handle = GetForegroundWindow();

            if (!oldWindow.Contains(handle.ToInt64()))
            {
                Console.WriteLine(handle);
                oldWindow.Add(handle.ToInt64());
                var temp = new Form1(handle);
                Thread x = new Thread(() => Application.Run(temp));
                x.Start();
                oldWindow.Add(GetForegroundWindow().ToInt64());
                //SetForegroundWindow(original);
                timer1.Stop();
                timer1.Enabled = false;
                label1.Text = "Stop";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
            label1.Text = "Run";
        }
    }
}
