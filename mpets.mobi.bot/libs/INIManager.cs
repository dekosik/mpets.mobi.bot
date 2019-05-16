using System.Runtime.InteropServices;
using System.Text;

namespace mpets.mobi.bot.Libs
{
    class INIManager
    {
        public INIManager(string aPath)
        {
            Path = aPath;
        }

        public INIManager() : this("") { }

        public string Get(string aSection, string aKey)
        {
            StringBuilder buffer = new StringBuilder(SIZE);
            GetPrivateString(aSection, aKey, null, buffer, SIZE, Path);
            return buffer.ToString();
        }

        public void Write(string aSection, string aKey, string aValue)
        {
            WritePrivateString(aSection, aKey, aValue, Path);
        }

        public string Path { get; set; } = null;

        private const int SIZE = 1024;

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern int WritePrivateString(string section, string key, string str, string path);
    }
}
