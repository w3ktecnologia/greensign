using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greensign
{
    class RegistryWriter
    {
        public static void SetUrlProtocol()
        {
            string exePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            string path = @"SOFTWARE\Classes";
            RegistryKey keyClasses = Registry.CurrentUser.OpenSubKey(path, true);
            RegistryKey keyGreendocs = keyClasses.CreateSubKey("greensign");
            keyGreendocs.SetValue("", "URl:greensign Protocol");
            keyGreendocs.SetValue("Url Protocol", "");
            keyGreendocs.CreateSubKey("DefaultIcon").SetValue("", "\"Greensign.exe,1\"");
            RegistryKey keyShell = keyGreendocs.CreateSubKey("shell");
            keyShell.SetValue("", "open");
            RegistryKey keyCommand = keyShell.CreateSubKey("open").CreateSubKey("command");
            keyCommand.SetValue("", "\"" + exePath + "\" %1");
        }
    }
}
