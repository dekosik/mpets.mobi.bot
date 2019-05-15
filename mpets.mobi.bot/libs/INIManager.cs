using System.Runtime.InteropServices;
using System.Text;

namespace mpets.bot.Libs
{
    class INIManager
    {
        public INIManager(string aPath)
        {
            path = aPath;
        }

        public INIManager() : this("") { }

        public string get(string aSection, string aKey)
        {
            StringBuilder buffer = new StringBuilder(SIZE);
            GetPrivateString(aSection, aKey, null, buffer, SIZE, path);
            return buffer.ToString();
        }

        public void write(string aSection, string aKey, string aValue)
        {
            WritePrivateString(aSection, aKey, aValue, path);
        }

        public string Path { get { return path; } set { path = value; } }

        private const int SIZE = 1024;
        private string path = null;

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern int WritePrivateString(string section, string key, string str, string path);
    }
}
