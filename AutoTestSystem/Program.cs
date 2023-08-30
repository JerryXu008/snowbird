using AutoTestSystem.Model;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoTestSystem
{
    internal static class Program
    {
        public static Mutex Mutex1 { get; private set; }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Mutex1 = new Mutex(true, Application.ProductName, out var isFirst);
            Global.processInfo = Process.GetProcessesByName(Application.ProductName)[0];
            if (isFirst)
            {
                Application.Run(new MainForm());
            }
            else
            {
#if DEBUG
                Thread.Sleep(20);
#endif
                SwitchToThisWindow(Global.processInfo.MainWindowHandle, true);
            }

        }
    }
}