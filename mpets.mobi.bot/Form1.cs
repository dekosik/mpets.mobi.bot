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
        private static HttpClient httpClient = new HttpClient();
        private static Random random = new Random();

        public Form1()
        {
            httpClient.BaseAddress = new Uri("https://mpets.mobi");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

            InitializeComponent();
        }

        public void Log(string text, bool show_times = true, Color color = new Color())
        {
            if(show_times)
            {
                Invoke(new Action(() => richTextBox1.SelectionColor = color));
                Invoke(new Action(() => richTextBox1.AppendText($"[ {DateTime.Now} ] {text} {Environment.NewLine}")));
                Invoke(new Action(() => richTextBox1.ScrollToCaret()));
            }
            else
            {
                Invoke(new Action(() => richTextBox1.SelectionColor = color));
                Invoke(new Action(() => richTextBox1.AppendText($"{text} {Environment.NewLine}")));
                Invoke(new Action(() => richTextBox1.ScrollToCaret()));
            }
        }

        static async Task<bool> Authorization(string name, string password)
        {
            string result = await httpClient.PostAsync("/login", new FormUrlEncodedContent(new[] {

                new KeyValuePair<string, string>("name", name), new KeyValuePair<string, string>("password", password)

            })).Result.Content.ReadAsStringAsync();

            return result.Contains("Чат");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (settings.get("Authorization", "Login").Length > 0) login.Text = settings.get("Authorization", "Login");
            if (settings.get("Authorization", "Password").Length > 0) password.Text = settings.get("Authorization", "Password");
        }

        private void Login_TextChanged(object sender, EventArgs e)
        {
            settings.write("Authorization", "Login", login.Text);
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            settings.write("Authorization", "Password", password.Text);
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            settings.write("BotSettings", "IntervalTimer", numericUpDown1.Value.ToString());
        }

        private void ToolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/mpets_mobi_bot");
        }
    }
}
