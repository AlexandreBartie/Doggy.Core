using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE.Tools.Util
{
    public class myRegistry
    {
        private RegistryKey Key;
        private bool IsKey => Key != null;

        public myRegistry(string prmName)
        {
            if (!Open(prmName))
                Create(prmName);
        }

        public void SetValue(string prmName, Object prmValue) => Key.SetValue(prmName, prmValue);
        public Object GetValue(string prmName) => Key.GetValue(prmName);

        private bool Open(string prmName) { Key = Registry.CurrentUser.OpenSubKey(prmName); return IsKey; }
        private bool Create(string prmName) { Key = Registry.CurrentUser.CreateSubKey(prmName); return IsKey; }
    }
}
