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
   
    class Hotkey : IEquatable<Hotkey>
    {
        readonly uint fsModifier, key;
        readonly int id;
        readonly IntPtr Hwnd;

        public Hotkey(IntPtr pHwnd , uint pfsModifier , uint pKey , int pId)
        {
            Hwnd = pHwnd;
            fsModifier = pfsModifier;
            key = pKey;
            id = pId;

        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((Hotkey)obj);
        }

        public bool Equals(Hotkey other)
        {
            return other.fsModifier == fsModifier && other.key == key && Hwnd == other.Hwnd;
        }

        public override int GetHashCode()
        {
            return (int) ( (fsModifier) * 100 + key );
        }

        public IntPtr getWindow()
        {
            return Hwnd;
        }

        public int getId()
        {
            return id;
        }

    }
    class HotKeyManager
    {

        Dictionary<Hotkey, Action> registeredHotkeys;

        public HotKeyManager()
        {
            registeredHotkeys = new Dictionary<Hotkey, Action>();
        }

        public void unRegisterHotkey(IntPtr Hwnd, uint fsModifier, uint key, Action A)
        {
            if (A == null)
                throw new ArgumentNullException("action a", "null action argument");

            Hotkey H = new Hotkey(Hwnd, fsModifier, key, (registeredHotkeys.Count() + 1));
            if (registeredHotkeys.ContainsKey(H) == false)
                throw new InvalidOperationException("hotkey not registered"); 
                        
            registeredHotkeys[H] -= A;

           
        }
        public void registerHotkey(IntPtr Hwnd , uint fsModifier, uint key , Action A)
        {

            if (A == null)
                throw new ArgumentNullException("action a", "null action argument");

            Hotkey H = new Hotkey(Hwnd , fsModifier, key ,  (registeredHotkeys.Count() + 1));

            if (registeredHotkeys.ContainsKey(H))
            {
                registeredHotkeys[H] += A;

            }
            else
            {
                Debug.WriteLine("added new hotkey" + registeredHotkeys.Count() + 1);
                registeredHotkeys.Add(H, A);
                RegisterHotKey(Hwnd, registeredHotkeys.Count(), fsModifier, key);
            }
        }

        void unRegisterHotkeys()
        {
            foreach(KeyValuePair < Hotkey , Action > H in registeredHotkeys)
            {
                UnregisterHotKey(H.Key.getWindow(), H.Key.getId());
            }
        }


        public void handleMessage(IntPtr Hwnd , ref Message m){
            uint key = (uint)(((int)m.LParam >> 16) & 0xFFFF);
            uint modifier = (uint)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
            int id = m.WParam.ToInt32();                         // The id of the hotkey that was pressed.
            Hotkey H = new Hotkey(Hwnd, modifier, key, id);
            Action A = registeredHotkeys[H];
            Debug.WriteLine(key + " " + id);
            A.Invoke();
        }

        ~HotKeyManager()
        {
            unRegisterHotkeys();
        }

        #region WindowsAPI
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

    }
}
