using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyTool
{
    
    

    public class KeyboardHook
    {
        #region winapi
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int code, int wParam, ref keyBoardHookStruct lParam);
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardEventHandler callback, IntPtr hInstance, uint theardID);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        #endregion

        public List < Keys > sensedKeys  = new List<Keys>();

        IntPtr Hook = IntPtr.Zero;

        public delegate int keyboardEventHandler(int Code, int wParam, ref keyBoardHookStruct lParam);

        public event KeyEventHandler KeyDown , KeyUp;

        

        
        public KeyboardHook(){ }
        
        public void hook()
        {
          
            Hook = SetWindowsHookEx(WH_KEYBOARD_LL, catcher , IntPtr.Zero , 0);
        }


        public int catcher(int Code, int wParam, ref keyBoardHookStruct lParam)
        {
            if (Code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                if (sensedKeys.Contains(key))
                {
                    KeyEventArgs kArg = new KeyEventArgs(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                        KeyDown(this, kArg);
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                        KeyUp(this, kArg);
                    if (kArg.Handled)
                        return 1;
                }
            }
            return CallNextHookEx(Hook, Code, wParam, ref lParam);
        }

        public void unhook()
        {
            UnhookWindowsHookEx(Hook);
        }

        ~KeyboardHook()
        {
            unhook();
        }

        public struct keyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        #region messageId
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYDOWN = 0x0104;
        const int WM_SYSKEYUP = 0x0105;
        #endregion

    }


}
