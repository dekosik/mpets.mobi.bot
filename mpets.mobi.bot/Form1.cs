using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mpets.bot.Libs;

namespace mpets.mobi.bot
{
    public partial class Form1 : Form
    {
        private static INIManager settings = new INIManager(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
        private static HttpClient client = new HttpClient();
        private static Random random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        public void log(string text, bool show_times, Color color = new Color())
        {

        }
    }
}
