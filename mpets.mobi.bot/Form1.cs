using mpets.mobi.bot.Libs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mpets.mobi.bot
{
    public partial class Form1 : Form
    {
        private int NumberTabs = 0;

        public static readonly string BOT_START_TEXT = "ЗАПУСТИТЬ БОТА";
        private static readonly string BOT_STOP_TEXT = "ОСТАНОВИТЬ БОТА";
        private static readonly string BOT_TABS_TEXT = "Новый питомец";

        private static readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "settings.ini";
        private static readonly string settingsPathTemp = AppDomain.CurrentDomain.BaseDirectory + "settings.temp";

        private static readonly IniFiles settings = new IniFiles(settingsPath);

        private static readonly Dictionary<string, string> settingKey = new Dictionary<string, string>
        {
            ["LOGIN"] = "",
            ["PASSWORD"] = "",
            ["AVATAR"] = "avatar1",
            ["LEVEL"] = "1",
            ["INTERVAL_FROM"] = "10",
            ["INTERVAL_DO"] = "20",
            ["TRAVEL"] = "true",
            ["CHEST"] = "true",
            ["GLADE"] = "true",
            ["TASKS"] = "true",
            ["OPEN_CASE"] = "true",
            ["CHARM"] = "true",
            ["RACES"] = "true"
        };

        private static readonly Dictionary<string, string> settingSection = new Dictionary<string, string>
        {
            ["AUTO_START"] = "false"
        };

        public Form1()
        {
            InitializeComponent();

            // Загружаем из ресурсов аватары в ImageList
            foreach (DictionaryEntry entry in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                string key = (string)entry.Key;

                if (key.Contains("avatar"))
                {
                    imageList1.Images.Add(key, (Image)entry.Value);
                }
            }

            // Фикс от разворачивания приложения через диспетчер задач
            MaximumSize = new Size(Width, Height);

            // Хак на уменьшения размера последней вкладки
            HelpMethod.TabControlSmallWidth(tabControl1);
        }

        private void CreateTemplateBot(TabPage tabPage)
        {
            Label label_interval_from = new Label
            {
                Location = new Point(8, 26),
                Margin = new Padding(2, 0, 2, 0),
                Name = $"label_interval_from{NumberTabs}",
                Size = new Size(22, 13),
                Text = "От",
                AutoSize = true
            };

            Label label_interval_do = new Label
            {
                Location = new Point(8, 52),
                Margin = new Padding(2, 0, 2, 0),
                Name = $"label_interval_do{NumberTabs}",
                Size = new Size(22, 13),
                Text = "До",
                AutoSize = true
            };

            Label label_isVip = new Label
            {
                Location = new Point(8, 52),
                Margin = new Padding(2, 0, 2, 0),
                Name = $"label_isVip{NumberTabs}",
                Size = new Size(22, 13),
                Text = "",
                AutoSize = true,
                Tag = false
            };

            Label label_nickname = new Label
            {
                Location = new Point(8, 52),
                Margin = new Padding(2, 0, 2, 0),
                Name = $"label_nickname{NumberTabs}",
                Size = new Size(22, 13),
                Text = "",
                AutoSize = true,
                Tag = ""
            };

            TextBox textbox_login = new TextBox
            {
                Location = new Point(6, 15),
                Margin = new Padding(2, 5, 2, 5),
                MaxLength = 20,
                Name = $"textbox_login{NumberTabs}",
                Size = new Size(194, 22),
                TabStop = false,
                Tag = NumberTabs
            };

            TextBox textbox_password = new TextBox
            {
                Location = new Point(6, 45),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"textbox_password{NumberTabs}",
                Size = new Size(194, 22),
                PasswordChar = '*',
                TabStop = false,
                Tag = NumberTabs
            };

            NumericUpDown numericupdown_interval_from = new NumericUpDown
            {
                Location = new Point(31, 21),
                Margin = new Padding(2, 5, 2, 5),
                Maximum = new decimal(new int[] { 1000, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Name = $"numericupdown_interval_from{NumberTabs}",
                Size = new Size(154, 22),
                Value = new decimal(new int[] { 10, 0, 0, 0 }),
                TabStop = false,
                Tag = NumberTabs
            };

            NumericUpDown numericupdown_interval_do = new NumericUpDown
            {
                Location = new Point(31, 47),
                Margin = new Padding(2, 5, 2, 5),
                Maximum = new decimal(new int[] { 1000, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Name = $"numericupdown_interval_do{NumberTabs}",
                Size = new Size(154, 22),
                Value = new decimal(new int[] { 20, 0, 0, 0 }),
                TabStop = false,
                Tag = NumberTabs
            };

            GroupBox groupBox1 = new GroupBox
            {
                Location = new Point(6, -2),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"groupBox1{NumberTabs}",
                Padding = new Padding(2, 5, 2, 5),
                Size = new Size(206, 159),
                TabStop = false
            };

            GroupBox groupBox2 = new GroupBox
            {
                Location = new Point(8, 72),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"groupBox2{NumberTabs}",
                Padding = new Padding(2, 5, 2, 5),
                Size = new Size(190, 78),
                Text = "Интервал повторов ( мин )",
                TabStop = false
            };

            GroupBox groupBox3 = new GroupBox
            {
                Location = new Point(6, 191),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"groupBox3{NumberTabs}",
                Padding = new Padding(2, 5, 2, 5),
                Size = new Size(206, 148),
                TabStop = false
            };

            Button button_start_bot = new Button
            {
                Font = new Font("Segoe UI", 9.25F),
                Location = new Point(6, 161),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"button_start_bot{NumberTabs}",
                Size = new Size(206, 31),
                Text = "ЗАПУСТИТЬ БОТА",
                UseVisualStyleBackColor = true,
                AutoSize = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_travel = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 13),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_travel{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "Отправлять на прогулку",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_chest = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 31),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_chest{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "Надевать и продавать вещи",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_glade = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 49),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_glade{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "Копать поляну",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_tasks = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 67),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_tasks{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "Забирать задания",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_opencase = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 85),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_opencase{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "Открывать сундук",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_charm = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 103),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_charm{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "[ Задание ] Снеговик",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            CheckBox checkbox_races = new CheckBox
            {
                Checked = true,
                CheckState = CheckState.Checked,
                Location = new Point(7, 121),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"checkbox_races{NumberTabs}",
                RightToLeft = RightToLeft.Yes,
                Size = new Size(192, 22),
                Text = "[ Задание ] Жокей",
                TextAlign = ContentAlignment.MiddleRight,
                UseVisualStyleBackColor = true,
                TabStop = false,
                Tag = NumberTabs
            };

            RichTextBox richtextbox_bot_log = new RichTextBox
            {
                BackColor = SystemColors.Window,
                Location = new Point(216, 34),
                Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point),
                Margin = new Padding(2, 5, 2, 5),
                Name = $"richtextbox_bot_log{NumberTabs}",
                ReadOnly = true,
                Size = new Size(382, 305),
                TabStop = false
            };

            ToolStrip toolstrip_top_log = new ToolStrip
            {
                AutoSize = false,
                BackColor = SystemColors.Window,
                CanOverflow = false,
                Dock = DockStyle.None,
                Font = new Font("Segoe UI", 8.25F),
                GripStyle = ToolStripGripStyle.Hidden,
                Location = new Point(216, 5),
                Name = $"toolstrip_top_log{NumberTabs}",
                Padding = new Padding(0, 0, 2, 0),
                Size = new Size(382, 25),
                Stretch = true,
                TabStop = false,
                RenderMode = ToolStripRenderMode.System
            };

            ToolStrip toolstrip_bottom_log = new ToolStrip
            {
                BackColor = SystemColors.ControlLightLight,
                CanOverflow = false,
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 8.25F),
                GripStyle = ToolStripGripStyle.Hidden,
                LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow,
                Location = new Point(0, 329),
                Name = $"toolstrip_bottom_log{NumberTabs}",
                Padding = new Padding(0, 0, 2, 0),
                Size = new Size(603, 25),
                TabStop = false,
                RenderMode = ToolStripRenderMode.System
            };

            ToolStripLabel toolstriplabel_coin_current = new ToolStripLabel
            {
                Image = Properties.Resources.coin,
                Margin = new Padding(0, 1, 6, 2),
                Name = $"toolstriplabel_coin_current{NumberTabs}",
                Size = new Size(70, 22),
                Text = "Монет: 0"
            };

            ToolStripLabel toolstriplabel_beauty_current = new ToolStripLabel
            {
                Alignment = ToolStripItemAlignment.Right,
                Image = Properties.Resources.beauty,
                Margin = new Padding(0, 1, 6, 2),
                Name = $"toolstriplabel_beauty_current{NumberTabs}",
                Size = new Size(77, 22),
                Text = "Красота: 0"
            };

            ToolStripLabel toolstriplabel_heart_current = new ToolStripLabel
            {
                Alignment = ToolStripItemAlignment.Right,
                Image = Properties.Resources.heart,
                Margin = new Padding(0, 1, 6, 2),
                Name = $"toolstriplabel_heart_current{NumberTabs}",
                Size = new Size(86, 22),
                Text = "Сердечек: 0"
            };

            ToolStripLabel toolstriplabel_bots_log = new ToolStripLabel
            {
                Margin = new Padding(4, 1, 0, 2),
                Name = $"toolstriplabel_bots_log{NumberTabs}",
                Size = new Size(103, 22)
            };

            ToolStripLabel toolstriplabel_beauty_session = new ToolStripLabel
            {
                Alignment = ToolStripItemAlignment.Right,
                Image = Properties.Resources.beauty,
                ImageTransparentColor = Color.Magenta,
                Name = $"toolstriplabel_beauty_session{NumberTabs}",
                Size = new Size(78, 22),
                Text = "0 собрано",
                Tag = new string[3] { "0", "0", "false" }
            };

            ToolStripLabel toolstriplabel_heart_session = new ToolStripLabel
            {
                Alignment = ToolStripItemAlignment.Right,
                Image = Properties.Resources.heart,
                Name = $"toolstriplabel_heart_session{NumberTabs}",
                Size = new Size(78, 22),
                Text = "0 собрано",
                Tag = new string[3] { "0", "0", "false" }
            };

            ToolStripLabel toolstriplabel_coin_session = new ToolStripLabel
            {
                Alignment = ToolStripItemAlignment.Right,
                Image = Properties.Resources.coin,
                Name = $"toolstriplabel_coin_session{NumberTabs}",
                Size = new Size(78, 22),
                Text = "0 собрано",
                Tag = new string[3] { "0", "0", "false" }
            };

            ToolStripLabel toolstriplabel_expirience_session = new ToolStripLabel
            {
                Alignment = ToolStripItemAlignment.Right,
                Image = Properties.Resources.expirience,
                Name = $"toolstriplabel_expirience_session{NumberTabs}",
                Size = new Size(78, 22),
                Text = "0 собрано",
                Tag = 0
            };

            groupBox1.Controls.AddRange(new Control[]
            {
                textbox_login,
                textbox_password,
                groupBox2
            });

            groupBox2.Controls.AddRange(new Control[]
            {
                label_interval_from,
                label_interval_do,
                numericupdown_interval_from,
                numericupdown_interval_do
            });

            groupBox3.Controls.AddRange(new Control[]
            {
                checkbox_travel,
                checkbox_chest,
                checkbox_glade,
                checkbox_tasks,
                checkbox_opencase,
                checkbox_charm,
                checkbox_races
            });

            toolstrip_top_log.Items.AddRange(new ToolStripItem[]
            {
                toolstriplabel_heart_current,
                toolstriplabel_beauty_current,
                toolstriplabel_coin_current
            });

            toolstrip_bottom_log.Items.AddRange(new ToolStripItem[]
            {
                toolstriplabel_bots_log,
                toolstriplabel_beauty_session,
                toolstriplabel_heart_session,
                toolstriplabel_coin_session,
                toolstriplabel_expirience_session
            });

            tabPage.Controls.AddRange(new Control[]
            {
                groupBox1,
                groupBox3,
                button_start_bot,
                richtextbox_bot_log,
                toolstrip_top_log,
                toolstrip_bottom_log,
                label_isVip,
                label_nickname
            });

            // Ставим обработчик событий на нажатие кнопки "ЗАПУСТИТЬ БОТА". 
            button_start_bot.Click += (s, e) =>
            {
                Button button = (Button)s;

                // Если текст кнопки равен "ОСТАНОВИТЬ БОТА"
                // То меняет на "ЗАПУСТИТЬ БОТА" и выходим из метода
                if (button.Text == BOT_STOP_TEXT)
                {
                    button.Text = BOT_START_TEXT;
                    return;
                }

                // ЗАПУСКАЕМ БОТА
                StartingBot(Convert.ToInt32(button.Tag));
            };

            // Ставим обработчик событий на изменения TextBox
            textbox_login.TextChanged += (s, e) =>
            {
                TextBox textBox = (TextBox)s; int botID = (int)textBox.Tag, level = settings.ReadInt($"PETS{botID}", "LEVEL");

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "LOGIN", textBox.Text);
                // Изменяем название вкладки
                tabControl1.TabPages[tabControl1.SelectedIndex].Text = textBox.Text.Length > 0 ? $"{textBox.Text} [ {level} ]" : BOT_TABS_TEXT;
            };

            textbox_password.TextChanged += (s, e) =>
            {
                TextBox textBox = (TextBox)s; int botID = (int)textBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "PASSWORD", textBox.Text);
            };

            numericupdown_interval_from.ValueChanged += (s, e) =>
            {
                NumericUpDown numericUpDown = (NumericUpDown)s; int botID = (int)numericUpDown.Tag;

                // Максимальное значения ОТ = ДО
                numericUpDown.Maximum = HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_do", botID, this).Value;
                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "INTERVAL_FROM", numericUpDown.Value.ToString());
            };

            numericupdown_interval_do.ValueChanged += (s, e) =>
            {
                NumericUpDown numericUpDown = (NumericUpDown)s; int botID = (int)numericUpDown.Tag;

                // Максимальное значение ОТ = ДО
                HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_from", botID, this).Maximum = numericUpDown.Value;
                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "INTERVAL_DO", numericUpDown.Value.ToString());
            };

            checkbox_travel.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "TRAVEL", checkBox.Checked.ToString().ToLower());
            };

            checkbox_chest.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "CHEST", checkBox.Checked.ToString().ToLower());
            };

            checkbox_glade.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "GLADE", checkBox.Checked.ToString().ToLower());
            };

            checkbox_tasks.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "TASKS", checkBox.Checked.ToString().ToLower());
            };

            checkbox_opencase.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "OPEN_CASE", checkBox.Checked.ToString().ToLower());
            };

            checkbox_charm.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "CHARM", checkBox.Checked.ToString().ToLower());
            };

            checkbox_races.CheckedChanged += (s, e) =>
            {
                CheckBox checkBox = (CheckBox)s; int botID = (int)checkBox.Tag;

                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "RACES", checkBox.Checked.ToString().ToLower());
            };

            HelpMethod.SetPlaceholder(textbox_login, "Имя питомца");
            HelpMethod.SetPlaceholder(textbox_password, "Пароль");

            // Подсказки
            toolTip1.SetToolTip(button_start_bot, "Пока бот выполняет работу, остановить его невозможно.");
            toolTip1.SetToolTip(numericupdown_interval_from, "Тут можно выбрать интервал повторов.\r\nНапример: Бот выберет рандомное число от 10 до 20 минут до следующего старта.");
            toolTip1.SetToolTip(numericupdown_interval_do, "Тут можно выбрать интервал повторов.\r\nНапример: Бот выберет рандомное число от 10 до 20 минут до следующего старта.");
            toolTip1.SetToolTip(checkbox_travel, "Бот будет отправлять питомца на длительную прогулку.");
            toolTip1.SetToolTip(checkbox_chest, "Бот будет одевать вещи которые лучше одетых и продавать ненужные.");
            toolTip1.SetToolTip(checkbox_glade, "Бот будет копать поляну.");
            toolTip1.SetToolTip(checkbox_tasks, "Бот будет забирать все выполненные задания (включая медали).");
            toolTip1.SetToolTip(checkbox_opencase, "Бот будет открывать сундук, при наличие ключа и отсутствующего VIP аккаунта.");
            toolTip1.SetToolTip(checkbox_charm, "Бот будет играть в мини-игру \"Снежки\", пока не выполнит ежедневное задание.");
            toolTip1.SetToolTip(checkbox_races, "Бот будет пытаться занять призовое место в мини-игре \"Скачки\", чтобы завершить ежедневное задание.");
        }

        private void CheckStopBot(int botID, Button btn, NumericUpDown numericupdown_interval_from, NumericUpDown numericupdown_interval_do)
        {
            if (!btn.Text.Contains(BOT_START_TEXT))
            {
                StartingBot(botID);
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    btn.Text = BOT_START_TEXT;
                    numericupdown_interval_from.Enabled = true;
                    numericupdown_interval_do.Enabled = true;
                });

                HelpMethod.StatusLog("", botID, this);
            }
        }

        private async void StartingBot(int BotID)
        {
            // Создаём новый HttpClient
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://mpets.mobi") };

            // Получаем ссылку на кнопку
            Button btn_start_bot = HelpMethod.findControl.FindButton("button_start_bot", BotID, this);

            // Получаем ссылки на интервал ОТ и ДО
            NumericUpDown numericupdown_interval_from = HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_from", BotID, this);
            NumericUpDown numericupdown_interval_do = HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_do", BotID, this);

            // Устанавливаем UserAgent для HttpClient
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

            // Изменяем текст кнопки (ОСТАНОВИТЬ БОТА), блокируем кнопку и интервалы ОТ и ДО
            Invoke((MethodInvoker)delegate
            {
                btn_start_bot.Text = BOT_STOP_TEXT;
                btn_start_bot.Enabled = false;
                numericupdown_interval_from.Enabled = false;
                numericupdown_interval_do.Enabled = false;
            });

            // Получаем "Имя питомца" и "Пароль"
            string login = HelpMethod.findControl.FindTextBox("textbox_login", BotID, this).Text;
            string password = HelpMethod.findControl.FindTextBox("textbox_password", BotID, this).Text;

            // Проверяем на пустоту "Имя питомца" и "Пароль"
            if (login.Length > 0 & password.Length > 0)
            {
                await Task.Run(async () =>
                {
                    HelpMethod.StatusLog("Авторизация...", BotID, this, Properties.Resources.auth);

                    // Авторизуемся в игре
                    string isLogin = await BotEngine.Authorization(login, password, httpClient);

                    // Если авторизация прошла успешно
                    if (isLogin == "true")
                    {
                        HelpMethod.Log("Запускаем задачи...", BotID, this, Color.DarkSlateGray, false);

                        // Обновляем статистику
                        await BotEngine.Statistics(BotID, httpClient, this, settings);

                        // Статус цикла
                        bool status = true;

                        // Запускаем цикл основных задач (Кормим, Играем и Ходим на выставки)
                        do
                        {
                            // Делаем запрос на главную
                            string result = await HelpMethod.GET("/", httpClient);

                            // Переменная сна
                            bool sleep = false;

                            // Если можно разбудить бесплатно
                            if (result.Contains("Разбудить бесплатно"))
                            {
                                // Будим бесплатно
                                result = await HelpMethod.GET("/wakeup_sleep", httpClient);

                                // Логируем действие
                                HelpMethod.Log("Разбудили питомца бесплатно.", BotID, this);
                            }

                            // Если не нужно кормить, играть и ходить на выставки
                            if (result.Contains("Играть ещё") || result.Contains("Разбудить"))
                            {
                                status = false;
                            }

                            // Если нет возможности дать витаминку, запускаем режим сна
                            if (new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & !result.Contains("show?start=1"))
                            {
                                sleep = true;
                            }

                            // Если нужно, кормим, играем, посещаем выставки и даём витаминки
                            if (status)
                            {
                                if (!sleep)
                                {
                                    // Кормим
                                    await BotEngine.Food(BotID, httpClient, this);

                                    // Играем
                                    await BotEngine.Play(BotID, httpClient, this);

                                    // Посещаем выставку
                                    await BotEngine.Showing(BotID, httpClient, this);

                                    // Даём витаминку
                                    await BotEngine.WakeUp(BotID, httpClient, this);
                                }
                                else
                                {
                                    HelpMethod.StatusLog("Питомец отдыхает...", BotID, this, Properties.Resources.sleep);

                                    // Ждем от 1 до 5 секунд
                                    await HelpMethod.RandomDelay(1000, 5000);
                                }
                            }
                        }
                        while (status);

                        // Если включена опция "Отправлять на прогулку"
                        if (HelpMethod.findControl.FindCheckBox("checkbox_travel", BotID, this).Checked)
                        {
                            HelpMethod.StatusLog("Проверяем прогулки...", BotID, this, Properties.Resources.travel);

                            // Проверяем прогулки
                            await BotEngine.Travel(BotID, httpClient, this);
                        }

                        // Если включена опция "Копать поляну"
                        if (HelpMethod.findControl.FindCheckBox("checkbox_glade", BotID, this).Checked)
                        {
                            HelpMethod.StatusLog("Проверяем поляну...", BotID, this, Properties.Resources.garden);

                            // Проверяем поляну
                            await BotEngine.Glade(BotID, httpClient, this);
                        }

                        // Если включена опция "Надевать и продавать вещи"
                        if (HelpMethod.findControl.FindCheckBox("checkbox_chest", BotID, this).Checked)
                        {
                            HelpMethod.StatusLog("Проверяем шкаф...", BotID, this, Properties.Resources.chest);

                            // Проверяем шкаф
                            await BotEngine.Chest(BotID, httpClient, this);
                        }

                        // Если включена опция "[ Задание ] Снеговик"
                        if (HelpMethod.findControl.FindCheckBox("checkbox_charm", BotID, this).Checked)
                        {
                            // Игра в снежки
                            await BotEngine.Charm(BotID, httpClient, this);
                        }

                        // Если включена опция "[ Задание ] Жокей"
                        if (HelpMethod.findControl.FindCheckBox("checkbox_races", BotID, this).Checked)
                        {
                            // Скачки
                            await BotEngine.Races(BotID, httpClient, this);
                        }

                        // Если включена опция "Забирать задания" 
                        if (HelpMethod.findControl.FindCheckBox("checkbox_tasks", BotID, this).Checked)
                        {
                            HelpMethod.StatusLog("Проверяем ежедневные задания...", BotID, this, Properties.Resources.tasks);

                            // Проверяем ежедневные задания
                            await BotEngine.Tasks(BotID, httpClient, this);
                        }

                        // Обновляем статистику
                        await BotEngine.Statistics(BotID, httpClient, this, settings);

                        HelpMethod.Log($"Все задачи завершены.", BotID, this, Color.DarkSlateGray, false);
                        HelpMethod.Log("", BotID, this, ShowTime: false);

                        // Получаем рандомный интервал ожидания
                        int interval = HelpMethod.getRandomNumber.Next(Convert.ToInt32(HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_from", BotID, this).Value), Convert.ToInt32(HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_do", BotID, this).Value));

                        // Ожидание
                        await BotEngine.Sleep(BotID, btn_start_bot, this, interval);

                        // Проверяем не остановлен ли бот
                        CheckStopBot(BotID, btn_start_bot, numericupdown_interval_from, numericupdown_interval_do);
                    }
                    // Если произошла ошибка (не правильный имя питомца или пароль)
                    else if (isLogin == "false")
                    {
                        HelpMethod.Log("Не правильное имя питомца или пароль, попробую повторить через минуту.", BotID, this, Color.Red, false);
                        HelpMethod.Log("", BotID, this, ShowTime: false);

                        // Запускаем задачу ожидания
                        await BotEngine.Sleep(BotID, btn_start_bot, this);

                        // Проверяем не остановлен ли бот пользователем и запускаем ещё раз
                        CheckStopBot(BotID, btn_start_bot, numericupdown_interval_from, numericupdown_interval_do);
                    }
                    // Если произошла ошибка сети
                    else
                    {
                        HelpMethod.Log("Ошибка сети, повтор через минуту.", BotID, this, Color.Red, false);
                        HelpMethod.Log("", BotID, this, ShowTime: false);

                        // Запускаем задачу ожидания
                        await BotEngine.Sleep(BotID, btn_start_bot, this);

                        // Проверяем не остановлен ли бот пользователем и запускаем ещё раз
                        CheckStopBot(BotID, btn_start_bot, numericupdown_interval_from, numericupdown_interval_do);
                    }
                });
            }
            else
            {
                // Выводим сообщение
                HelpMethod.Log("Имя питомца или пароль, не может быть пустым.", BotID, this, Color.Red, false);
                HelpMethod.Log("", BotID, this, ShowTime: false);

                // Меняем текст кнопки (ЗАПУСТИТЬ БОТА), разблокируем кнопку и интервалы ОТ и ДО
                Invoke((MethodInvoker)delegate
                {
                    btn_start_bot.Enabled = true;
                    btn_start_bot.Text = BOT_START_TEXT;
                    numericupdown_interval_from.Enabled = true;
                    numericupdown_interval_do.Enabled = true;
                });
            }
        }

        private void TabControl1_DoubleClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                DialogResult result = MessageBox.Show("Вы действительно хотите удалить профиль?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    int selectIndex = tabControl1.SelectedIndex - 1;

                    // Удаляем бота из сохранения
                    settings.DeleteSection($"PETS{(int)tabControl1.TabPages[tabControl1.SelectedIndex].Tag}");

                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                    tabControl1.SelectedIndex = selectIndex == -1 ? 0 : selectIndex;
                }
            }
        }

        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            int lastIndex = tabControl1.TabCount - 1;

            if (tabControl1.GetTabRect(lastIndex).Contains(e.Location))
            {
                // Увеличиваем количество вкладкок
                NumberTabs++;

                // Генерируем новую вкладку
                TabPage tabPage = new TabPage
                {
                    Text = BOT_TABS_TEXT,
                    Name = $"tabPage{NumberTabs}",
                    BackColor = Color.White,
                    ToolTipText = "Для удаления профиля, нажмите несколько раз по вкладке.",
                    Tag = NumberTabs,
                    ImageIndex = imageList1.Images.IndexOfKey("avatar1")
                };

                tabPage.DoubleClick += TabControl1_DoubleClick;

                // Добавляем базовый шаблон на вкладку
                CreateTemplateBot(tabPage);

                // Сохраняем стандарный профиль
                SaveDefaultProfile(NumberTabs);

                // Добавляем вкладку
                tabControl1.TabPages.Insert(lastIndex, tabPage);
                tabControl1.SelectedTab = tabPage;
            }
        }

        private void LoadProfile(int petsID = 1)
        {
            // Читаем файл настроек
            string readFiles = File.Exists(settingsPath) ? File.ReadAllText(settingsPath) : "";

            // Инициализируем временный файл настроек
            IniFiles settingTemp = new IniFiles(settingsPathTemp);

            // Проходимся циклом по секциям ini файла
            foreach (Match match in new Regex(@"\[.*\]").Matches(readFiles))
            {
                // Парсим имя секции
                string SectionName = match.Value.Replace("[", "").Replace("]", "");

                // Если в секции есть глобальные настройки
                if (SectionName.Contains("GLOBAL"))
                {
                    // Записываем во временный файл настроек глобальные настройки
                    foreach (KeyValuePair<string, string> item in settingSection)
                    {
                        string str = settings.ReadString(SectionName, item.Key);
                        settingTemp.Write(SectionName, item.Key, str);
                    }
                }

                // Если в секции есть профиль питомца
                if (SectionName.Contains("PETS"))
                {
                    string login, password, avatar;
                    int interval_from, interval_do, level;
                    bool travel, chest, glade, tasks, open_case, charm, races;

                    // Читаем основные настройки профиля
                    login = settings.ReadString(SectionName, "LOGIN");
                    password = settings.ReadString(SectionName, "PASSWORD");
                    avatar = settings.ReadString(SectionName, "AVATAR");
                    level = settings.ReadInt(SectionName, "LEVEL");
                    interval_from = settings.ReadInt(SectionName, "INTERVAL_FROM");
                    interval_do = settings.ReadInt(SectionName, "INTERVAL_DO");
                    travel = settings.ReadBool(SectionName, "TRAVEL");
                    chest = settings.ReadBool(SectionName, "CHEST");
                    glade = settings.ReadBool(SectionName, "GLADE");
                    tasks = settings.ReadBool(SectionName, "TASKS");
                    open_case = settings.ReadBool(SectionName, "OPEN_CASE");
                    charm = settings.ReadBool(SectionName, "CHARM");
                    races = settings.ReadBool(SectionName, "RACES");

                    // Записываем основыные настройки профиля во временный файл
                    settingTemp.Write($"PETS{petsID}", "LOGIN", login);
                    settingTemp.Write($"PETS{petsID}", "PASSWORD", password);
                    settingTemp.Write($"PETS{petsID}", "AVATAR", avatar);
                    settingTemp.Write($"PETS{petsID}", "LEVEL", level.ToString());
                    settingTemp.Write($"PETS{petsID}", "INTERVAL_FROM", interval_from.ToString());
                    settingTemp.Write($"PETS{petsID}", "INTERVAL_DO", interval_do.ToString());
                    settingTemp.Write($"PETS{petsID}", "TRAVEL", travel.ToString().ToLower());
                    settingTemp.Write($"PETS{petsID}", "CHEST", chest.ToString().ToLower());
                    settingTemp.Write($"PETS{petsID}", "GLADE", glade.ToString().ToLower());
                    settingTemp.Write($"PETS{petsID}", "TASKS", tasks.ToString().ToLower());
                    settingTemp.Write($"PETS{petsID}", "OPEN_CASE", open_case.ToString().ToLower());
                    settingTemp.Write($"PETS{petsID}", "CHARM", charm.ToString().ToLower());
                    settingTemp.Write($"PETS{petsID}", "RACES", races.ToString().ToLower());

                    // Добавляем новый профиль (вкладку)
                    AddProfile(login, password, avatar, level, interval_from, interval_do, travel, chest, glade, tasks, open_case, charm, races);

                    petsID++;
                }
            }

            // Удаляем основной файл настроек
            File.Delete(settingsPath);

            // Переименновываем временный файл настроек в основной
            if (File.Exists(settingsPathTemp))
            {
                File.Move(settingsPathTemp, settingsPath);
            }

            // Автоматический старт
            if (settings.KeyExists("GLOBAL", "AUTO_START"))
            {
                // Получаем значение
                bool auto_start = settings.ReadBool("GLOBAL", "AUTO_START");

                // Если включено
                if (auto_start)
                {
                    // Меняем настройку
                    toolStripMenuItem5.Checked = auto_start;

                    // Запускаем всех ботов
                    toolStripMenuItem3.PerformClick();

                    // Скрываем приложее в трей
                    WindowState = FormWindowState.Minimized;
                }
            }
        }

        private void SaveDefaultProfile(int botID)
        {
            // Записываем основные настройки профиля
            foreach (KeyValuePair<string, string> item in settingKey)
            {
                settings.Write($"PETS{botID}", item.Key, item.Value);
            }
        }

        private void AddProfile(string login, string password, string avatar, int level, int interval_from, int interval_do, bool travel, bool chest, bool glade, bool tasks, bool open_case, bool charm, bool races)
        {
            int lastIndex = tabControl1.TabCount - 1;

            // Увеличиваем количество вкладкок
            NumberTabs++;

            // Генерируем новую вкладку
            TabPage tabPage = new TabPage
            {
                Text = login.Length > 0 ? $"{login} [{level}]" : BOT_TABS_TEXT,
                Name = $"tabPage{NumberTabs}",
                BackColor = Color.White,
                ToolTipText = "Для удаления профиля, нажмите несколько раз по вкладке.",
                Tag = NumberTabs,
                ImageIndex = imageList1.Images.IndexOfKey(avatar)
            };

            // Ставим на двойной клик обработчик
            tabPage.DoubleClick += TabControl1_DoubleClick;

            // Добавляем базовый шаблон на вкладку
            CreateTemplateBot(tabPage);

            // Добавляем вкладку
            tabControl1.TabPages.Insert(lastIndex, tabPage);
            tabControl1.SelectedTab = tabPage;

            // Загружаем настройки
            HelpMethod.findControl.FindTextBox("textbox_login", NumberTabs, this).Text = login;
            HelpMethod.findControl.FindTextBox("textbox_password", NumberTabs, this).Text = password;
            HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_from", NumberTabs, this).Value = interval_from;
            HelpMethod.findControl.FindNumericUpDown("numericupdown_interval_do", NumberTabs, this).Value = interval_do;
            HelpMethod.findControl.FindCheckBox("checkbox_travel", NumberTabs, this).Checked = travel;
            HelpMethod.findControl.FindCheckBox("checkbox_chest", NumberTabs, this).Checked = chest;
            HelpMethod.findControl.FindCheckBox("checkbox_glade", NumberTabs, this).Checked = glade;
            HelpMethod.findControl.FindCheckBox("checkbox_tasks", NumberTabs, this).Checked = tasks;
            HelpMethod.findControl.FindCheckBox("checkbox_opencase", NumberTabs, this).Checked = open_case;
            HelpMethod.findControl.FindCheckBox("checkbox_charm", NumberTabs, this).Checked = charm;
            HelpMethod.findControl.FindCheckBox("checkbox_races", NumberTabs, this).Checked = races;
        }

        private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == tabControl1.TabCount - 1)
            {
                e.Cancel = true;
            }
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Запускаем всех ботов
            for (int i = 1; i <= NumberTabs; i++)
            {
                Button button = HelpMethod.findControl.FindButton("button_start_bot", i, this);

                if (button.Text == BOT_START_TEXT & button.Enabled)
                {
                    StartingBot(i);
                }
            }
        }

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            // Останавливаем всех ботов
            for (int i = 1; i <= NumberTabs; i++)
            {
                Button button = HelpMethod.findControl.FindButton("button_start_bot", i, this);

                if (button.Text.Contains(BOT_STOP_TEXT))
                {
                    button.Text = BOT_START_TEXT;
                }
            }
        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            // Меняем Checked
            toolStripMenuItem5.Checked = !toolStripMenuItem5.Checked;

            // Сохраняем
            settings.Write("GLOBAL", "AUTO_START", toolStripMenuItem5.Checked.ToString().ToLower());

            // Добавляем в автозагрузку
            HelpMethod.AutoRun(toolStripMenuItem5.Checked);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Если окно было свернуто
            if (WindowState == FormWindowState.Minimized)
            {
                // Прячем из панели задач
                ShowInTaskbar = false;

                // Прячем форму
                Hide();

                // Показываем иконку в трее
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_Click(object sender, EventArgs e)
        {
            // Если была нажата правая кнопка мыши
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                // Показываем форму
                Show();

                // Разворачиваем приложение
                WindowState = FormWindowState.Normal;

                // Показываем значек в панели задач
                ShowInTaskbar = true;

                // Скрываем иконку из трее
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProfile();
        }

        private void ToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            _ = Process.Start("https://vk.cc/9oWxgt");
        }

        private void ToolStripMenuItem9_Click(object sender, EventArgs e)
        {
            _ = Process.Start("https://github.com/dekosik/mpets.mobi.bot");
        }
    }
}