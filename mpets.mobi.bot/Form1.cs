using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
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

        // Системные переменные
        private bool isStart;
        private bool isLogin;
        private bool isTimer;
        private bool isHide;

        private bool isDev = true;

        // Переменные для хранения статистики опыта
        private int[] expirience = { 0, 0 };
        private bool expirience_bool = false;

        // Переменные для хранения статистики монет
        private int[] coin = { 0, 0 };
        private bool coin_bool = false;

        // Переменные для хранения статистики сердечек
        private int[] heart = { 0, 0 };
        private bool heart_bool = false;

        public Form1()
        {
            InitializeComponent();
        }

        // Метод который создает новый HttpClient
        private void CreateHttpClient()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("https://mpets.mobi") };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");
        }

        // Метод отправки текста в richTextBox
        public void Log(string text, bool show_times = true, Color color = new Color())
        {
            if (show_times)
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

        // Метод отправки в statusStrip1
        public void StatusLog(string text, Image image = null)
        {
            Invoke(new Action(() => statusStrip1.Items[0].Text = text));
            Invoke(new Action(() => statusStrip1.Items[0].Image = image));
        }

        // Метод который убирает или добавляет в автозагрузки Windows
        public static void AutoRun(bool flag)
        {
            string ExePath = Application.ExecutablePath;
            FileInfo fi = new FileInfo(ExePath);
            int k = fi.Name.IndexOf('.');
            string name = fi.Name.Substring(0, k);

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
                MessageBox.Show("Произошла ошибка, автозапуск невозможен.");
            }
        }

        // Метод который показывает или прячет форму в трей
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

        // Метод который отправляет GET запрос
        public async Task<string> HTTP_Get(string url)
        {
            try
            {
                string result = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        // Метод который авторизуется в игре
        public async Task<bool> Authorization(string name, string password)
        {
            try
            {
                string result = await httpClient.PostAsync("/login", new FormUrlEncodedContent(new[] {

                    new KeyValuePair<string, string>("name", name), new KeyValuePair<string, string>("password", password)

                })).Result.Content.ReadAsStringAsync();

                return result.Contains("Чат");
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Метод который выгуливает питомца
        public async Task Travel()
        {
            string result = await HTTP_Get("/travel");

            if (result.Contains("Гулять дальше"))
            {
                await Task.Delay(random.Next(500, 1000));
                result = await HTTP_Get("/travel");
            }

            if (!result.Contains("Ваш питомец гуляет"))
            {
                var travel = new Regex(@"go_travel(.*?)\"" class=").Matches(result);

                if (travel.Count > 0)
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

                    result = await HTTP_Get("/go_travel?id=" + curr_id);

                    if (result.Contains("Ваш питомец гуляет"))
                    {
                        Log("-- Отправили питомца на прогулку.");
                    }
                }
            }
        }

        // Метод который копает поляну
        public async Task Glade()
        {
            string result = await HTTP_Get("/glade");

            if (result.Contains("Копать"))
            {
                Log("-- Копаю поляну...");

                do
                {
                    result = await HTTP_Get("/glade_dig");
                    await Task.Delay(random.Next(500, 1000));
                }
                while (result.Contains("Копать"));

                Log("-- Закончил копать поляну.");
            }
        }

        // Метод который продает ненужные вещи
        public async Task Sell_all()
        {
            string result = await HTTP_Get("/sell_all?confirm=1&backparent=");

            if (!result.Contains("У вас нет ненужных вещей на продажу"))
            {
                Log("-- Были проданы ненужные вещи.");
            }
        }

        // Метод который забирает выполненные задания
        public async Task Tasks()
        {
            string result = await HTTP_Get("/task");

            MatchCollection reg = new Regex(@"rd\?id=(.*?)\"" class=").Matches(result);

            if (reg.Count > 0)
            {
                Log("-- Найдено выполненных заданий " + reg.Count + " шт.");

                foreach (Match match in reg)
                {
                    string id = match.Groups[1].Value;

                    if (id.Length > 0)
                    {
                        result = await HTTP_Get("/task_reward?id=" + id);
                        await Task.Delay(random.Next(500, 1000));
                    }
                }

                Log("-- Награды за задания собраны.");
            }
        }

        // Метод который даёт питомцу витаминку
        public async Task WakeUp()
        {
            string result = await HTTP_Get("/");

            if (result.Contains("Дать витаминку за"))
            {
                StatusLog("Даю витаминку...", Properties.Resources.heart);

                await Task.Delay(random.Next(500, 1000));
                await HTTP_Get("/wakeup");

                Log("-- Питомец получил витаминку.");
            }
        }

        // Метод который кормит питомца
        public async Task Food()
        {
            string result = await HTTP_Get("/");

            string rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;
            if (rand.Length > 0)
            {
                StatusLog("Кормлю питомца...", Properties.Resources.meat);
                Log("-- Кормлю питомца...");

                await Task.Delay(random.Next(500, 1000));
                do
                {
                    result = await HTTP_Get("/?action=food&rand=" + rand);
                    rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);

                Log("-- Закончил кормить питомца.");
            }
        }

        // Метод который играет с питомцем
        public async Task Play()
        {
            string result = await HTTP_Get("/");

            string rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;
            if (rand.Length > 0)
            {
                StatusLog("Играю с питомцем...", Properties.Resources.mouse);
                Log("-- Играю с питомцем...");

                await Task.Delay(random.Next(500, 1000));
                do
                {
                    result = await HTTP_Get("/?action=play&rand=" + rand);
                    rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);

                Log("-- Закончил играть с питомцем.");
            }
        }

        // Метод который ходит на выставки
        public async Task Showing()
        {
            string result = await HTTP_Get("/");

            if (result.Contains("show?start=1"))
            {
                StatusLog("На выставке...", Properties.Resources.cup);
                Log("-- Иду на выставку...");

                bool status = false;
                await HTTP_Get("/show?start=1");

                await Task.Delay(random.Next(500, 1000));
                do
                {
                    result = await HTTP_Get("/show");

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

        // Метод который обновляет статистику сердечек, опыта и монет
        public async Task Statistics()
        {
            StatusLog("Обновляю статистику...", Properties.Resources.about);

            string result = await HTTP_Get("/profile");

            string expirience_string = new Regex(@"Опыт: (.*?) /").Match(result).Groups[1].Value;
            string coin_string = new Regex(@"Монеты: (.*?)</div>").Match(result).Groups[1].Value;
            string heart_string = new Regex(@"Сердечки: (.*?)</div>").Match(result).Groups[1].Value;

            if (expirience_string.Length > 0)
            {
                if (!expirience_bool)
                {
                    expirience[0] = Convert.ToInt32(expirience_string);
                    expirience_bool = true;
                }
                else
                {
                    expirience[1] = Convert.ToInt32(expirience_string) - expirience[0];
                }
            }

            if (coin_string.Length > 0)
            {
                if (!coin_bool)
                {
                    coin[0] = Convert.ToInt32(coin_string);
                    coin_bool = true;
                }
                else
                {
                    coin[1] = Convert.ToInt32(coin_string) - coin[0];
                }
            }

            if (heart_string.Length > 0)
            {
                if (!heart_bool)
                {
                    heart[0] = Convert.ToInt32(heart_string);
                    heart_bool = true;
                }
                else
                {
                    heart[1] = Convert.ToInt32(heart_string) - heart[0];
                }
            }
        }

        // Главный метод старта бота
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

                    await Statistics();
                    await Task.Delay(random.Next(500, 1000));

                    await test();
                    await Task.Delay(random.Next(500, 1000));

                    bool status = true;
                    do
                    {
                        string result = await HTTP_Get("/");
                        bool sleep = false;

                        if (result.Contains("Играть ещё"))
                            status = false;

                        if (result.Contains("Разбудить"))
                            status = false;

                        if (!checkBox1.Checked & !checkBox2.Checked & !checkBox3.Checked)
                            status = false;

                        if (result.Contains("Разбудить бесплатно"))
                        {
                            result = await HTTP_Get("/wakeup_sleep");
                            status = true;
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
                                    await Food();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                if (checkBox2.Checked)
                                {
                                    await Play();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                if (checkBox2.Checked)
                                {
                                    await Showing();
                                    await Task.Delay(random.Next(500, 1000));
                                }

                                await WakeUp();
                                await Task.Delay(random.Next(1000, 2000));
                            }
                            else
                            {
                                StatusLog("Питомец отдыхает...", Properties.Resources.sleep);
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

                    await Statistics();
                    await Task.Delay(random.Next(500, 1000));

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
            if (isStart)
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

                if (isTimer)
                {
                    stop.Enabled = true;
                }
                else
                {
                    stop.Enabled = false;
                }

                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
            }
            else
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;

                start.Enabled = true;
                stop.Enabled = false;

                StatusLog("Запустите бота");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

            if (settings.Get("BotSettings", "AutoRun").Length > 0)
            {
                checkBox8.Checked = Convert.ToBoolean(settings.Get("BotSettings", "AutoRun"));
                checkBox9.Checked = Convert.ToBoolean(settings.Get("BotSettings", "Hide"));

                if (Convert.ToBoolean(settings.Get("BotSettings", "AutoRun")))
                {
                    if (Convert.ToBoolean(settings.Get("BotSettings", "Hide")))
                    {
                        HideForm(isHide);
                    }

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

            checkBox9.Enabled = checkBox8.Checked;
            if (!checkBox8.Checked)
            {
                checkBox9.Checked = false;
            }

            statusStrip1.Items[1].Text = $"{coin[1]} собрано";
            statusStrip1.Items[2].Text = $"{heart[1]} собрано";
            statusStrip1.Items[3].Text = $"{expirience[1]} собрано";
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
            settings.Write("BotSettings", "AutoRun", checkBox8.Checked.ToString().ToLower());
        }

        private void CheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("BotSettings", "Hide", checkBox9.Checked.ToString().ToLower());
        }


        //
        // ВРОДЕ И МЕТОД ГОТОВ, НО РАБОТАЕТ НЕ ТАК КАК НАДО... ОН ОДЕВАЕТ ВЕЩЬ КОТОРАЯ ЛУЧШЕ И СРАЗУ ЖЕ ПРОДАЕТ НЕНУЖНЫЕ.
        // ПОКА ОТЛОЖУ ДАННЫЙ МЕТОД, НУЖНО БОЛЬШЕ ТЕСТИРОВАНИЯ.
        //
        public async Task test()
        {
            string result = await HTTP_Get("/chest");
            string url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;
            string name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;

            if (url.Length > 0 && !url.Contains("open_item"))
            {
                Log("-- Надеваю вещи...");

                while (url.Length > 0 && !url.Contains("open_item"))
                {
                    // Задержка
                    await Task.Delay(random.Next(500, 1000));

                    // если в url есть wear_item, значит мы одеваем вещь
                    if (url.Contains("wear_item"))
                        Log($"--- Надел {name}", true, Color.Green);

                    // если в url есть sell_item, значит мы продаём вещь
                    if (url.Contains("sell_item"))
                        Log($"--- Продал {name}", true, Color.Red);

                    // Выполняем запрос
                    result = await HTTP_Get(url);
                    // Парсим ссылку
                    url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;
                    // Парсим название вещи
                    name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;
                }

                //do
                //{
                    //await Task.Delay(random.Next(500, 1000));

                    //result = await HTTP_Get(url);
                    //url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;

                    //if (url.Contains("open_item"))
                    //{
                        //break;
                    //}
                //}
                //while (url.Length > 0);

                Log("-- Закончил надевать...");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(isDev)
            {
                Task.Run(() => test());
            }
            else
            {
                System.Diagnostics.Process.Start("https://vk.cc/9oWxgt");
            }
        }
    }
}