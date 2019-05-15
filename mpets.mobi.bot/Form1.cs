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

        public async Task Tasks()
        {
            string result = await httpClient.GetAsync("/task").Result.Content.ReadAsStringAsync();

            MatchCollection reg = new Regex(@"rd\?id=(.*?)\"" class=").Matches(result);

            if (reg.Count > 0)
            {
                Log("-- Найдено выполненных заданий " + reg.Count + " шт.");

                foreach (Match match in reg)
                {
                    string id = match.Groups[1].Value;

                    if (id.Length > 0)
                    {
                        result = await httpClient.GetAsync("/task_reward?id=" + id).Result.Content.ReadAsStringAsync();
                        await Task.Delay(random.Next(500, 1000));
                    }
                }

                Log("-- Награды за задания собраны.");
            }
        }

        public async Task WakeUp()
        {
            string result = await httpClient.GetAsync("/").Result.Content.ReadAsStringAsync();

            if (result.Contains("Дать витаминку за"))
            {
                await httpClient.GetAsync("/wakeup").Result.Content.ReadAsStringAsync();

                Log("-- Питомец получил витаминку.");
            }
        }

        public async Task Food()
        {
            string result = await httpClient.GetAsync("/").Result.Content.ReadAsStringAsync();

            string rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;
            if (rand.Length > 0)
            {
                Log("-- Кормлю питомца...");

                do
                {
                    result = await httpClient.GetAsync("/?action=food&rand=" + rand).Result.Content.ReadAsStringAsync();
                    rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);

                Log("-- Закончил кормить питомца.");
            }
        }

        public async Task Play()
        {
            string result = await httpClient.GetAsync("/").Result.Content.ReadAsStringAsync();

            string rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;
            if (rand.Length > 0)
            {
                Log("-- Играю с питомцем...");

                do
                {
                    result = await httpClient.GetAsync("/?action=play&rand=" + rand).Result.Content.ReadAsStringAsync();
                    rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);

                Log("-- Закончил играть с питомцем.");
            }
        }

        public new async Task Show()
        {
            string result = await httpClient.GetAsync("/").Result.Content.ReadAsStringAsync();

            if (result.Contains("show?start=1"))
            {
                Log("-- Иду на выставку...");

                bool status = false;
                await httpClient.GetAsync("/show?start=1").Result.Content.ReadAsStringAsync();

                do
                {
                    result = await httpClient.GetAsync("/show").Result.Content.ReadAsStringAsync();

                    if (result.Contains("Участвовать за"))
                        status = true;

                    if (result.Contains("Соревноваться"))
                        status = true;

                    if (result.Contains("Забрать награду"))
                        status = false;

                    if (result.Contains("Завершить"))
                        status = false;

                    if (result.Contains("Снежки"))
                        status = false;

                    await Task.Delay(random.Next(500, 1000));
                }
                while (status);

                Log("-- Выставка закончена.");
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
                    bool status = true;
                    do
                    {
                        string result = await httpClient.GetAsync("/").Result.Content.ReadAsStringAsync();
                        bool sleep = false;

                        if (result.Contains("Играть ещё"))
                            status = false;

                        if (result.Contains("Разбудить"))
                            status = false;

                        if (!checkBox1.Checked & !checkBox2.Checked & !checkBox3.Checked)
                            status = false;

                        if (result.Contains("Разбудить бесплатно"))
                        {
                            result = await httpClient.GetAsync("/wakeup_sleep").Result.Content.ReadAsStringAsync();
                            Log("-- Разбудили питомца бесплатно.");
                        }

                        if(new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & !result.Contains("show?start=1"))
                        {
                            sleep = true;
                        }

                        if (status)
                        {
                            if(!sleep)
                            {
                                if (checkBox1.Checked)
                                {
                                    StatusLog("Запуск метода = Food");

                                    await Food();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                if (checkBox2.Checked)
                                {
                                    StatusLog("Запуск метода = Play");

                                    await Play();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                if (checkBox2.Checked)
                                {
                                    StatusLog("Запуск метода = Show");

                                    await Show();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                StatusLog("Запуск метода = WakeUp");

                                await WakeUp();
                                await Task.Delay(random.Next(1000, 2000));
                            }
                        }
                    }
                    while (status);

                    if (checkBox4.Checked)
                    {
                        StatusLog("Запуск метода = Travel");

                        await Travel();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (checkBox6.Checked)
                    {
                        StatusLog("Запуск метода = Glade");

                        await Glade();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (checkBox5.Checked)
                    {
                        StatusLog("Запуск метода = Sell_all");

                        await Sell_all();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (checkBox7.Checked)
                    {
                        StatusLog("Запуск метода = Tasks");

                        await Tasks();
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
                        start.Enabled = true;
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
