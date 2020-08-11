using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        // Переменная для своеобразного таймера
        private DateTime taskStop;

        // Системные переменные
        private bool isStart;
        private string isLogin;
        private bool isTimer;
        private bool isHide;

        // Версия бота
        private readonly string version = "v1.5.1";

        // Переменная для разработчика (немного больше логов)
        private bool isDev = false;

        // Переменные для хранения статистики красоты
        private int[] beauty = { 0, 0 };
        private bool beauty_bool = false;

        // Переменные для хранения статистики монет
        private int[] coin = { 0, 0 };
        private bool coin_bool = false;

        // Переменные для хранения статистики сердечек
        private int[] heart = { 0, 0 };
        private bool heart_bool = false;

        // Переменная для хранения очков опыта
        private int exp = 0;

        public Form1()
        {
            InitializeComponent();
        }

        // Метод, который создает новый HttpClient
        private void CreateHttpClient()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("https://mpets.mobi") };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");
        }

        // Метод отправки текста в LogBox
        public void Log(string text, bool show_times = true, Color color = new Color())
        {
            if (show_times)
            {
                Invoke(new Action(() => LogBox.SelectionColor = color));
                Invoke(new Action(() => LogBox.AppendText($" [ {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} ] {text} {Environment.NewLine}")));
                Invoke(new Action(() => LogBox.ScrollToCaret()));
            }
            else
            {
                Invoke(new Action(() => LogBox.SelectionColor = color));
                Invoke(new Action(() => LogBox.AppendText($" {text} {Environment.NewLine}")));
                Invoke(new Action(() => LogBox.ScrollToCaret()));
            }
        }

        // Метод отправки текста в BotsLogs
        public void StatusLog(string text, Image image = null)
        {
            Invoke(new Action(() => BotsLogs.Text = text));
            Invoke(new Action(() => BotsLogs.Image = image));
        }

        // Метод, который убирает или добавляет в автозагрузки Windows
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

        // Метод, который показывает или прячет форму в трей
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
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
                Hide();

                isHide = true;
            }
        }

        // Метод, который отправляет GET запрос
        public async Task<string> HTTP_Get(string url)
        {
            try
            {
                string result = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception)
            {
                return "error Get";
            }
        }

        // Метод, который считает опыт за каждое выполненное действие 
        public void SaveExpirience(string type, string result)
        {
            string expirience;

            switch (type)
            {
                case "Travel":
                    expirience = new Regex(@"expirience.png\"" />(.*?)<br />").Match(result).Groups[1].Value;

                    if (expirience.Contains("heart")) expirience = new Regex(@"(.*?),").Match(expirience).Groups[1].Value;
                    if (expirience.Length > 0) exp += Convert.ToInt32(expirience);
                    if (isDev) Log($"{type} = {expirience}", false);
                    break;

                case "Glade":
                    expirience = new Regex(@"expirience.png\"" />(.*?)</span>").Match(result).Groups[1].Value;

                    if (expirience.Length > 0) exp += Convert.ToInt32(expirience);
                    if (isDev) Log($"{type} = {expirience}", false);
                    break;

                case "Tasks":
                    expirience = new Regex(@"expirience.png\"" />(.*?)</span>").Match(result).Groups[1].Value;

                    if (expirience.Length > 0) exp += Convert.ToInt32(expirience);
                    if (isDev) Log($"{type} = {expirience}", false);
                    break;

                case "Food":
                    expirience = new Regex(@"expirience.png\"" class=\""ml2\"">(.*?)</div>").Match(result).Groups[1].Value.Replace("+", "");

                    if (expirience.Length > 0) exp += Convert.ToInt32(expirience);
                    if (isDev) Log($"{type} = {expirience}", false);
                    break;

                case "Play":
                    expirience = new Regex(@"expirience.png\"" class=\""ml2\"">(.*?)</div>").Match(result).Groups[1].Value.Replace("+", "");

                    if (expirience.Length > 0) exp += Convert.ToInt32(expirience);
                    if (isDev) Log($"{type} = {expirience}", false);
                    break;

                case "Showing":
                    expirience = new Regex(@"expirience.png\"" />(.*?)</td>").Match(result).Groups[1].Value;

                    if (expirience.Length > 0) exp += Convert.ToInt32(expirience);
                    if (isDev & expirience.Length > 0) Log($"{type} = {expirience}", false);
                    break;
            }
        }

        // Метод, который конвертирует число в более читабельное и приятное на глаз
        public string StringFormat(string number)
        {
            return Convert.ToInt32(number).ToString("#,##0", new CultureInfo("en-US"));
        }

        // Метод, который авторизуется в игре
        public async Task<string> Authorization(string name, string password)
        {
            try
            {
                string result = await httpClient.PostAsync("/login", new FormUrlEncodedContent(new[] {

                    new KeyValuePair<string, string>("name", name), new KeyValuePair<string, string>("password", password)

                })).Result.Content.ReadAsStringAsync();

                return result.Contains("Чат").ToString().ToLower();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        // Метод, который выгуливает питомца
        public async Task Travel()
        {
            string result = await HTTP_Get("/travel");

            if (result.Contains("Гулять дальше"))
            {
                await Task.Delay(random.Next(400, 700));

                SaveExpirience("Travel", result);
                result = await HTTP_Get("/travel?clear=1");

                await Task.Delay(random.Next(400, 700));
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

                        if (news_id > temp_id) curr_id = news_id;

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

        // Метод, который копает поляну
        public async Task Glade()
        {
            string result = await HTTP_Get("/glade");

            if (result.Contains("Копать"))
            {
                Log("-- Копаю поляну...");

                do
                {
                    result = await HTTP_Get("/glade_dig");

                    SaveExpirience("Glade", result);

                    await Task.Delay(random.Next(500, 1000));
                }
                while (result.Contains("Копать"));

                Log("-- Закончил копать поляну.");
            }
        }

        // Метод, который одевает вещи и продаёт ненужные
        public async Task Chest()
        {
            string result = await HTTP_Get("/chest");
            string url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;
            string name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;

            if (url.Length > 0 && !url.Contains("open_item"))
            {
                Log("-- В шкафу есть вещи...");

                while (url.Length > 0 && !url.Contains("open_item"))
                {
                    await Task.Delay(random.Next(500, 1000));

                    if (url.Contains("wear_item"))
                        Log($"--- Надел {name}.", true, Color.Green);

                    if (url.Contains("sell_item"))
                        Log($"--- Продал {name}.", true, Color.Red);

                    result = await HTTP_Get(url);

                    url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;
                    name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;
                }

                Log("-- Закончил работу в шкафу...");
            }
        }

        // Метод, который забирает выполненные задания
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

                        SaveExpirience("Tasks", result);

                        await Task.Delay(random.Next(500, 1000));
                    }
                }

                Log("-- Награды за задания собраны.");
            }
        }

        // Метод, который даёт питомцу витаминку
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

        // Метод, который кормит питомца
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

                    SaveExpirience("Food", result);
                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);

                Log("-- Закончил кормить питомца.");
            }
        }

        // Метод, который играет с питомцем
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

                    SaveExpirience("Play", result);

                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);

                Log("-- Закончил играть с питомцем.");
            }
        }

        // Метод, который ходит на выставки
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

                    SaveExpirience("Showing", result);

                    await Task.Delay(random.Next(500, 1000));
                }
                while (status);

                Log("-- Выставка закончена.");
            }
        }

        // Метод, который обновляет статистику красоты, монет и cердечек
        public async Task Statistics()
        {
            StatusLog("Обновляю статистику...", Properties.Resources.about);

            string result = await HTTP_Get("/profile");

            string beauty_string = new Regex(@"Красота: (.*?)</div>").Match(result).Groups[1].Value;
            string coin_string = new Regex(@"Монеты: (.*?)</div>").Match(result).Groups[1].Value;
            string heart_string = new Regex(@"Сердечки: (.*?)</div>").Match(result).Groups[1].Value;

            if (beauty_string.Length > 0)
            {
                BeautyCurrent.Text = $"Красота: {StringFormat(beauty_string)}";

                if (!beauty_bool)
                {
                    beauty[0] = Convert.ToInt32(beauty_string);
                    beauty_bool = true;
                }
                else
                {
                    beauty[1] = Convert.ToInt32(beauty_string) - beauty[0];
                }
            }

            if (coin_string.Length > 0)
            {
                CoinCurrent.Text = $"Монеты: {StringFormat(coin_string)}";

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
                HeartCurrent.Text = $"Сердечки: {StringFormat(heart_string)}";

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

        // Главный метод бота
        public void StartBot()
        {
            CreateHttpClient();

            isStart = true;
            isTimer = false;

            Task.Run(async () =>
            {
                StatusLog("Авторизация...", Properties.Resources.document);

                isLogin = await Authorization(login.Text, password.Text);

                if (isLogin == "true")
                {
                    Log("-- Запускаю задачи...");

                    await Statistics();
                    await Task.Delay(random.Next(500, 1000));

                    bool status = true;
                    do
                    {
                        string result = await HTTP_Get("/");
                        bool sleep = false;

                        if (result.Contains("Разбудить бесплатно"))
                        {
                            result = await HTTP_Get("/wakeup_sleep");
                            Log("-- Разбудили питомца бесплатно.");
                        }

                        if (result.Contains("Играть ещё"))
                            status = false;

                        if (result.Contains("Разбудить"))
                            status = false;

                        if (new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & !result.Contains("show?start=1"))
                        {
                            sleep = true;
                        }

                        await Task.Delay(random.Next(800, 1000));

                        if (status)
                        {
                            if (!sleep)
                            {
                                await Food();
                                await Task.Delay(random.Next(500, 1000));

                                await Play();
                                await Task.Delay(random.Next(500, 1000));

                                await Showing();
                                await Task.Delay(random.Next(500, 1000));

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

                    if (TravelCheckBox.Checked)
                    {
                        StatusLog("Проверяю прогулки...", Properties.Resources.travel);

                        await Travel();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (GladeCheckBox.Checked)
                    {
                        StatusLog("Проверяю поляну...", Properties.Resources.garden);

                        await Glade();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (ChestCheckBox.Checked)
                    {
                        StatusLog("Проверяю шкаф...", Properties.Resources.chest);

                        await Chest();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    if (TasksCheckBox.Checked)
                    {
                        StatusLog("Проверяю задания...", Properties.Resources.tasks);

                        await Tasks();
                        await Task.Delay(random.Next(500, 1000));
                    }

                    await Statistics();
                    await Task.Delay(random.Next(500, 1000));

                    isTimer = true;
                    taskStop = DateTime.Now.AddMinutes(Convert.ToDouble(random.Next(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value))));

                    Invoke(new Action(() => LogBox.Focus()));

                    Log("-- Все задачи выполнены.");
                    Log("", false);
                }
                else if (isLogin == "false")
                {
                    Log("-- Вы ввели неправильное имя или пароль, повтор через 1 минуту.");
                    Log("", false);

                    isTimer = true;
                    taskStop = DateTime.Now.AddMinutes(1);
                }
                else if (isLogin == "error")
                {
                    Log("-- Ошибка сети, повтор через 1 минуту....");
                    Log("", false);

                    isTimer = true;
                    taskStop = DateTime.Now.AddMinutes(1);
                }
            });
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (isStart) isStart = false;
            else StartBot();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (isStart && isTimer)
            {
                DateTime now = DateTime.Now;

                if (now.Hour >= taskStop.Hour && now.Minute >= taskStop.Minute && now.Second >= taskStop.Second)
                {
                    StartBot();
                }

                StatusLog($"Повтор через {taskStop.Subtract(now).ToString("mm")} мин : {taskStop.Subtract(now).ToString("ss")} сек", Properties.Resources.sleep);
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (isStart)
            {
                start.Text = "ОСТАНОВИТЬ БОТА";
                start.Enabled = isTimer ? true : false;

                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
            }
            else
            {
                start.Text = "ЗАПУСТИТЬ БОТА";
                
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                
                start.Enabled = true;

                StatusLog("Запустите бота");
            }

            numericUpDown1.Maximum = numericUpDown2.Value;

            HideCheckBox.Enabled = AutoRunCheckBox.Checked;
            HideCheckBox.Checked = !AutoRunCheckBox.Checked;

            ExpSession.Text = $"{StringFormat(exp.ToString())} собрано";
            CoinSession.Text = $"{StringFormat(coin[1].ToString())} собрано";
            HeartSession.Text = $"{StringFormat(heart[1].ToString())} собрано";
            BeautySession.Text = $"{StringFormat(beauty[1].ToString())} собрано";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            login.Text = settings.Get("Authorization", "Login");
            password.Text = settings.Get("Authorization", "Password");

            if (settings.Get("Timer", "Min").Length > 0) numericUpDown1.Value = Convert.ToInt32(settings.Get("Timer", "Min"));
            if (settings.Get("Timer", "Max").Length > 0) numericUpDown2.Value = Convert.ToInt32(settings.Get("Timer", "Max"));

            if (settings.Get("Settings", "Travel").Length > 0) TravelCheckBox.Checked = Convert.ToBoolean(settings.Get("Settings", "Travel"));
            if (settings.Get("Settings", "Chest").Length > 0) ChestCheckBox.Checked = Convert.ToBoolean(settings.Get("Settings", "Chest"));
            if (settings.Get("Settings", "Glade").Length > 0) GladeCheckBox.Checked = Convert.ToBoolean(settings.Get("Settings", "Glade"));
            if (settings.Get("Settings", "Tasks").Length > 0) TasksCheckBox.Checked = Convert.ToBoolean(settings.Get("Settings", "Tasks"));

            if (settings.Get("Settings", "AutoRun").Length > 0)
            {
                AutoRunCheckBox.Checked = Convert.ToBoolean(settings.Get("Settings", "AutoRun"));
                HideCheckBox.Checked = Convert.ToBoolean(settings.Get("Settings", "Hide"));

                if (Convert.ToBoolean(settings.Get("Settings", "AutoRun")))
                {
                    if (Convert.ToBoolean(settings.Get("Settings", "Hide")))
                    {
                        HideForm(isHide);
                    }

                    StartBot();
                }
            }

            if (isDev) Text = $"Удивительные питомца By DeKoSiK ( {version} ) - Dev"; else Text = $"Удивительные питомца By DeKoSiK ( {version} )";
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            HideForm(isHide);
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
            settings.Write("Timer", "Min", numericUpDown1.Value.ToString());
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            settings.Write("Timer", "Max", numericUpDown2.Value.ToString());
        }

        private void TravelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("Settings", "Travel", TravelCheckBox.Checked.ToString().ToLower());
        }

        private void ChestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("Settings", "Chest", ChestCheckBox.Checked.ToString().ToLower());
        }

        private void GladeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("Settings", "Glade", GladeCheckBox.Checked.ToString().ToLower());
        }

        private void TasksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("Settings", "Tasks", TasksCheckBox.Checked.ToString().ToLower());
        }

        private void AutoRunCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("Settings", "AutoRun", AutoRunCheckBox.Checked.ToString().ToLower());
            AutoRun(AutoRunCheckBox.Checked);
        }

        private void HideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.Write("Settings", "Hide", HideCheckBox.Checked.ToString().ToLower());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.cc/9oWxgt");
        }
    }
}
