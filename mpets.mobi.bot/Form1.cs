using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using mpets.bot.Libs;

namespace mpets.mobi.bot
{
    public partial class Form1 : Form
    {
        private static readonly INIManager settings = new INIManager(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly Random random = new Random();

        private DateTime taskStop;

        private bool isStart;
        private bool isLogin;
        private bool isTimer;

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

        public void StatusLog(string text)
        {
            Invoke(new Action(() => statusStrip1.Items[0].Text = text));
        }

        public async Task<bool> Authorization(string name, string password)
        {
            string result = await httpClient.PostAsync("/login", new FormUrlEncodedContent(new[] {

                new KeyValuePair<string, string>("name", name), new KeyValuePair<string, string>("password", password)

            })).Result.Content.ReadAsStringAsync();

            return result.Contains("Чат");
        }

        public async Task Travel()
        {
            string result = await httpClient.GetAsync("/travel?clear=1").Result.Content.ReadAsStringAsync();

            if (!result.Contains("Ваш питомец гуляет"))
            {
                var travel = new Regex(@"go_travel(.*?)\"" class=").Matches(result);

                if(travel.Count > 0)
                {
                    int temp_id = 0, curr_id = 0;

                    foreach (Match match in travel)
                    {
                        int news_id = Convert.ToInt32(match.Groups[1].Value.Replace("?id=", ""));

                        if (news_id > temp_id)
                        {
                            curr_id = news_id;
                        }

                        temp_id = news_id;
                    }

                    result = await httpClient.GetAsync("/go_travel?id=" + curr_id).Result.Content.ReadAsStringAsync();

                    if (result.Contains("Ваш питомец гуляет"))
                    {
                        Log("-- Отправили питомца на прогулку.");
                    }
                }
            }
        }

        public async Task Glade()
        {
            string result = await httpClient.GetAsync("/glade").Result.Content.ReadAsStringAsync();

            if (result.Contains("Копать"))
            {
                Log("Копаю поляну...");

                do
                {
                    result = await httpClient.GetAsync("/glade_dig").Result.Content.ReadAsStringAsync();
                    await Task.Delay(random.Next(500, 1000));
                }
                while (result.Contains("Копать"));

                Log("Закончил копать поляну.");
            }
        }

        public async Task Sell_all()
        {
            string result = await httpClient.GetAsync("/sell_all?confirm=1&backparent=").Result.Content.ReadAsStringAsync();

            if (!result.Contains("У вас нет ненужных вещей на продажу"))
            {
                Log("-- Были проданы ненужные вещи.");
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            isStart = true;
            isTimer = false;

            Task.Run(async () =>
            {
                StatusLog("Запуск метода = Authorization");

                isLogin = await Authorization(login.Text, password.Text);

                if(isLogin)
                {
                    if (checkBox4.Checked)
                    {
                        StatusLog("Запуск метода = Travel");

                        await Travel();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if(checkBox6.Checked)
                    {
                        StatusLog("Запуск метода = Glade");

                        await Glade();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if(checkBox5.Checked)
                    {
                        StatusLog("Запуск метода = Sell_all");

                        await Sell_all();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    isTimer = true;
                    taskStop = DateTime.Now.AddMinutes(Convert.ToDouble(numericUpDown1.Value));
                }
                else
                {
                    Log("Вы ввели неправильный логин или пароль.");
                }
            });
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            isStart = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(isStart)
            {
                if (isTimer)
                {
                    DateTime now = DateTime.Now;

                    if (now.Hour == taskStop.Hour && now.Minute == taskStop.Minute && now.Second == taskStop.Second)
                    {
                        start.PerformClick();
                    }

                    StatusLog($"Жду {taskStop.Subtract(now).Minutes} мин : {taskStop.Subtract(now).Seconds} сек");
                }
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (isStart)
            {
                start.Enabled = false;
                stop.Enabled = true;
            }
            else
            {
                start.Enabled = true;
                stop.Enabled = false;

                StatusLog("Запустите бота");
            }
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
