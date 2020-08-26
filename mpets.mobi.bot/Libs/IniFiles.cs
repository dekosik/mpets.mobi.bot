using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace mpets.mobi.bot.Libs
{
    class IniFiles
    {
        private readonly string Path;

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);

        public IniFiles(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString();
        }

        private string Read(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, Path);

            return RetVal.ToString();
        }

        public string ReadString(string Section, string Key)
        {
            return Read(Section, Key);
        }

        public int ReadInt(string Section, string Key)
        {
            return Convert.ToInt32(Read(Section, Key));
        }

        public bool ReadBool(string Section, string Key)
        {
            return Convert.ToBoolean(Read(Section, Key));
        }

        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Section, Key, null);
        }

        public void DeleteSection(string Section = null)
        {
            Write(Section, null, null);
        }

        public bool KeyExists(string Section, string Key)
        {
            return Read(Section, Key).Length > 0;
        }
    }
}
