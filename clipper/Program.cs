using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace clipper
{
    static class progam
    {
        //include getWindowClicked
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IntPtr selectedWindow = GetForegroundWindow();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new init());
        }
    }
}