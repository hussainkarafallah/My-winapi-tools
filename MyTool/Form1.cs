using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MyTool
{
    public partial class Form1 : Form
    {

        IntPtr formWindow;

        HotKeyManager hotkeyManager;

        KeyLogger keyLogger;

        const uint WM_HOTKEY = 0x0312;
        const int WM_SETTEXT = 0X000C;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formWindow = FindWindow(null, "Form1");


            hotkeyManager = new HotKeyManager();

            keyLogger = new KeyLogger();

            registerOurHotkeys();
        }





        void registerOurHotkeys()
        {

            hotkeyManager.registerHotkey(formWindow, (uint)fsModifiers.Control, (uint)Keys.O, performCopy);

            hotkeyManager.registerHotkey(formWindow, (uint)fsModifiers.Control, (uint)Keys.S, keyLogger.start);

            hotkeyManager.registerHotkey(formWindow, (uint)fsModifiers.Control, (uint)Keys.P, keyLogger.pause);

            hotkeyManager.registerHotkey(formWindow, (uint)fsModifiers.Control, (uint)Keys.R, keyLogger.resume);


        }


        void performCopy()
        {
            grabbedText.Text = getSelectedText();
            appName.Text = GetCaptionOfActiveWindow();
            SetForegroundWindow(formWindow);

        }
        String getSelectedText()
        {
            string before;
            if (!Clipboard.ContainsText()) before = "";
                
            else before = Clipboard.GetText();
            
            SendKeys.SendWait("^(c)");
            string ret = Clipboard.GetText();
            Clipboard.SetText(before);
            return ret;


        }
        protected override void WndProc(ref Message m)
        {


            if (m.Msg == WM_HOTKEY)
            {
                hotkeyManager.handleMessage(formWindow, ref m);
            }
            else base.WndProc(ref m);


        }

        private string GetCaptionOfActiveWindow()
        {
            string strTitle = string.Empty;
            IntPtr handle = GetForegroundWindow();
            // Obtain the length of the text   
            int intLength = GetWindowTextLength(handle) + 1;
            StringBuilder stringBuilder = new StringBuilder(intLength);
            if (GetWindowText(handle, stringBuilder, intLength) > 0)
            {
                strTitle = stringBuilder.ToString();
            }
            return strTitle;
        }

        



        private void MyForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        #region WindowsAPI
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        #endregion

        public enum fsModifiers
        {
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Window = 0x0008,
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process pr = new Process();
            ProcessStartInfo prs = new ProcessStartInfo();
            prs.FileName = @"notepad.exe";
            pr.StartInfo = prs;
            pr.Start();

            Thread.Sleep(500);

            IntPtr notepadTextbox = FindWindowEx(pr.MainWindowHandle, IntPtr.Zero, "Edit", null);

            SetForegroundWindow(pr.MainWindowHandle);

            SendMessage(notepadTextbox, WM_SETTEXT, 0, "Text copied from box \n \n " + grabbedText.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageHelper msg = new MessageHelper();
            int result = 0;
            int hWnd = msg.getWindowId(null, "receiverform");
            if(hWnd != 0)
                result = msg.sendWindowsStringMessage(hWnd, 0, grabbedText.Text);
        }

    }       
}
