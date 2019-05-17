﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using mpets.mobi.bot.Libs;

namespace mpets.mobi.bot
{
    public partial class Form1 : Form
    {
        private static readonly INIManager settings = new INIManager(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
        private static HttpClient httpClient;
        private static readonly Random random = new Random();

        private DateTime taskStop;

        private bool isStart;
        private bool isLogin;
        private bool isTimer;
        private bool isHide;
        private readonly bool isDev = false;

        // private int coin = 0;
        // private int heart = 0;
        private int expirience = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void CreateHttpClient()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://mpets.mobi")
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");
        }

        public void Log(string text, bool show_times = true, Color color = new Color())
        {
            if(show_times)
            {
                Invoke(new Action(() => richTextBox1.SelectionColor = color));
                Invoke(new Action(() => richTextBox1.AppendText($" [ {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} ] {text} {Environment.NewLine}")));
                Invoke(new Action(() => richTextBox1.ScrollToCaret()));
            }
            else
            {
                Invoke(new Action(() => richTextBox1.SelectionColor = color));
                Invoke(new Action(() => richTextBox1.AppendText($" {text} {Environment.NewLine}")));
                Invoke(new Action(() => richTextBox1.ScrollToCaret()));
            }
        }

        public void StatusLog(string text, Image image = null)
        {
            Invoke(new Action(() => statusStrip1.Items[0].Text = text));
            Invoke(new Action(() => statusStrip1.Items[0].Image = image));
        }

        public static void AutoRun(bool flag)
        {
            string ExePath = Application.ExecutablePath;
            string name = "";
            FileInfo fi = new FileInfo(ExePath);
            int k = fi.Name.IndexOf('.');
            name = fi.Name.Substring(0, k);

            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (flag)
                    reg.SetValue(name, ExePath);
                else
                    reg.DeleteValue(name);

                reg.Close();
            }
            catch
            {
                MessageBox.Show("Автозапуск не возможен");
            }
        }

        public void HideForm(bool flag)
        {
            if (flag)
            {
                Show();
                ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;

                isHide = false;
            }
            else
            {
                notifyIcon1.BalloonTipText = "Нажмите на иконку приложения 1 раз чтобы скрыть или показать форму.";
                notifyIcon1.ShowBalloonTip(1000);

                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
                Hide();

                isHide = true;
            }
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
            string result = await httpClient.GetAsync("/travel").Result.Content.ReadAsStringAsync();

            if (result.Contains("Гулять дальше"))
            {
                if(isDev)
                {
                    File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}/travel/{DateTime.UtcNow.ToFileTimeUtc()}.txt", result);
                }

                await Task.Delay(random.Next(500, 1000));
                result = await httpClient.GetAsync("/travel").Result.Content.ReadAsStringAsync();
            }

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
                Log("-- Копаю поляну...");

                do
                {
                    result = await httpClient.GetAsync("/glade_dig").Result.Content.ReadAsStringAsync();
                    await Task.Delay(random.Next(500, 1000));
                }
                while (result.Contains("Копать"));

                Log("-- Закончил копать поляну.");
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
                StatusLog("Даю витаминку...", Properties.Resources.heart);

                await Task.Delay(random.Next(500, 1000));

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

                await Task.Delay(random.Next(500, 1000));
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

                await Task.Delay(random.Next(500, 1000));
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

        public async Task Showing()
        {
            string result = await httpClient.GetAsync("/").Result.Content.ReadAsStringAsync();

            if (result.Contains("show?start=1"))
            {
                Log("-- Иду на выставку...");

                bool status = false;
                await httpClient.GetAsync("/show?start=1").Result.Content.ReadAsStringAsync();

                await Task.Delay(random.Next(500, 1000));
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

        public void StartBot()
        {
            CreateHttpClient();

            isStart = true;
            isTimer = false;

            Task.Run(async () =>
            {
                StatusLog("Авторизация...", Properties.Resources.document);

                isLogin = await Authorization(login.Text, password.Text);

                if (isLogin)
                {
                    Log("-- Запускаю задачи...");

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

                        if (new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & !result.Contains("show?start=1"))
                        {
                            sleep = true;
                        }

                        await Task.Delay(random.Next(500, 1000));

                        if (status)
                        {
                            if (!sleep)
                            {
                                if (checkBox1.Checked)
                                {
                                    StatusLog("Кормлю питомца...", Properties.Resources.meat);

                                    await Food();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                if (checkBox2.Checked)
                                {
                                    StatusLog("Играю с питомцем...", Properties.Resources.mouse);

                                    await Play();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                if (checkBox2.Checked)
                                {
                                    StatusLog("На выставке...", Properties.Resources.cup);

                                    await Showing();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                await WakeUp();
                                await Task.Delay(random.Next(1000, 2000));
                            }
                        }
                    }
                    while (status);

                    if (checkBox4.Checked)
                    {
                        StatusLog("Проверяю прогулки...", Properties.Resources.travel);

                        await Travel();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (checkBox6.Checked)
                    {
                        StatusLog("Проверяю поляну...", Properties.Resources.garden);

                        await Glade();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (checkBox5.Checked)
                    {
                        StatusLog("Проверяю шкаф...", Properties.Resources.chest);

                        await Sell_all();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (checkBox7.Checked)
                    {
                        StatusLog("Проверяю задания...", Properties.Resources.tasks);

                        await Tasks();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    isTimer = true;
                    taskStop = DateTime.Now.AddMinutes(Convert.ToDouble(random.Next(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value))));

                    Log("-- Все задачи выполнены.");
                    Log("", false);
                }
                else
                {
                    isStart = false;
                    isTimer = true;

                    Log("-- Вы ввели неправильный логин или пароль.");
                }
            });
        }

        private void Start_Click(object sender, EventArgs e)
        {
            StartBot();
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
                        StartBot();
                    }

                    StatusLog($"Жду {taskStop.Subtract(now).ToString("mm")} мин : {taskStop.Subtract(now).ToString("ss")} сек", Properties.Resources.sleep);
                }
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (isStart)
            {
                start.Enabled = false;

                if(isTimer)
                {
                    stop.Enabled = true;
                }
                else
                {
                    stop.Enabled = false;
                }
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
            if(!isDev)
            {
                Size = new Size(625, 452);
            }

            login.Text = settings.Get("Authorization", "Login");
            password.Text = settings.Get("Authorization", "Password");

            if (settings.Get("BotSettings", "TimerIntervalMin").Length > 0) numericUpDown1.Value = Convert.ToInt32(settings.Get("BotSettings", "TimerIntervalMin"));
            if (settings.Get("BotSettings", "TimerIntervalMax").Length > 0) numericUpDown2.Value = Convert.ToInt32(settings.Get("BotSettings", "TimerIntervalMax"));

            if (settings.Get("BotSettings", "Food").Length > 0) checkBox1.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Food"));
            if (settings.Get("BotSettings", "Play").Length > 0) checkBox2.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Play"));
            if (settings.Get("BotSettings", "Showing").Length > 0) checkBox3.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Showing"));
            if (settings.Get("BotSettings", "Travel").Length > 0) checkBox4.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Travel"));
            if (settings.Get("BotSettings", "Sell_All").Length > 0) checkBox5.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Sell_All"));
            if (settings.Get("BotSettings", "Glade").Length > 0) checkBox6.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Glade"));
            if (settings.Get("BotSettings", "Tasks").Length > 0) checkBox7.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Tasks"));

            if (settings.Get("BotSettings", "AutoLoad_and_AutoStart").Length > 0)
            {
                checkBox8.Checked = Convert.ToBoolean(settings.Get("BotSettings", "AutoLoad_and_AutoStart"));

                if(Convert.ToBoolean(settings.Get("BotSettings", "AutoLoad_and_AutoStart")))
                {
                    HideForm(isHide);
                    StartBot();
                }
            }
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            HideForm(isHide);
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = numericUpDown2.Value;
        }

        private void Timer4_Tick(object sender, EventArgs e)
        {
            label7.Text = $"Собрано опыта: {expirience}";
        }

        private void Login_TextChanged(object sender, EventArgs e)
        {
            settings.Write("Authorization", "Login", login.Text);
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            settings.Write("Authorization", "Password", password.Text);
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "TimerIntervalMin", numericUpDown1.Value.ToString());
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "TimerIntervalMax", numericUpDown2.Value.ToString());
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Food", checkBox1.Checked.ToString().ToLower());
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Play", checkBox2.Checked.ToString().ToLower());
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Showing", checkBox3.Checked.ToString().ToLower());
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Travel", checkBox4.Checked.ToString().ToLower());
        }

        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Sell_All", checkBox5.Checked.ToString().ToLower());
        }

        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Glade", checkBox6.Checked.ToString().ToLower());
        }

        private void CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Tasks", checkBox7.Checked.ToString().ToLower());
        }

        private void CheckBox8_CheckedChanged(object sender, EventArgs e)
        {
            AutoRun(checkBox8.Checked);
            settings.Write("BotSettings", "AutoLoad_and_AutoStart", checkBox8.Checked.ToString().ToLower());
        }

        private void LinkLabel1_MouseEnter(object sender, EventArgs e)
        {
            linkLabel1.LinkColor = Color.RoyalBlue;
        }

        private void LinkLabel1_MouseLeave(object sender, EventArgs e)
        {
            linkLabel1.LinkColor = Color.Black;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/mpets_mobi_bot");
        }
    }
}
