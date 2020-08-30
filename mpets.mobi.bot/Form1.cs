using Microsoft.Win32;
using mpets.mobi.bot.Libs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mpets.mobi.bot
{
    public partial class Form1 : Form
    {
        private int NumberTabs = 0;

        private readonly bool isDev = false;

        private static readonly string BOT_START_TEXT = "ЗАПУСТИТЬ БОТА";
        private static readonly string BOT_STOP_TEXT = "ОСТАНОВИТЬ БОТА";
        private static readonly string BOT_TABS_TEXT = "Новый питомец";

        private static readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "settings.ini";
        private static readonly string settingsPathTemp = AppDomain.CurrentDomain.BaseDirectory + "settings.temp";

        private static readonly FindControl findControl = new FindControl();
        private static readonly Random random = new Random();
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private static void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            SendMessage(textBox.Handle, 0x1500 + 1, 0, placeholderText);
        }

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
            tabControl1.HandleCreated += (s, e) =>
            {
                _ = SendMessage(tabControl1.Handle, 0x1300 + 49, IntPtr.Zero, (IntPtr)10);
            };
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
                numericUpDown.Maximum = findControl.FindNumericUpDown("numericupdown_interval_do", botID, this).Value;
                // Записываем значения в файл сохранений
                settings.Write($"PETS{botID}", "INTERVAL_FROM", numericUpDown.Value.ToString());
            };

            numericupdown_interval_do.ValueChanged += (s, e) =>
            {
                NumericUpDown numericUpDown = (NumericUpDown)s; int botID = (int)numericUpDown.Tag;

                // Максимальное значение ОТ = ДО
                findControl.FindNumericUpDown("numericupdown_interval_from", botID, this).Maximum = numericUpDown.Value;
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

            SetPlaceholder(textbox_login, "Имя питомца");
            SetPlaceholder(textbox_password, "Пароль");

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

        private static void AutoStart(bool flag)
        {
            // Полный путь к файлу
            string fileFullPath = Application.ExecutablePath;
            // Получаем информацию об файле
            FileInfo fileInfo = new FileInfo(fileFullPath);
            // Получаем имя файла
            string fileName = fileInfo.Name.Replace(".exe", "");
            // Открываем ветку реестра
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            try
            {
                if (flag)
                {
                    registryKey.SetValue(fileName, fileFullPath);
                }
                else
                {
                    registryKey.DeleteValue(fileName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка, автозапуск невозможен.");
            }

            // Закрываем ветку реестра
            registryKey.Close();
        }

        public string StringNumberFormat(string number, bool format_type = true)
        {
            string number_text;

            if (format_type)
            {
                if (Convert.ToDouble(number) < 1000)
                {
                    number_text = number.ToString();
                }
                else if (Convert.ToDouble(number) < 1000000)
                {
                    number_text = (Convert.ToDouble(number) / 1000).ToString("#.##k");
                }
                else if (Convert.ToDouble(number) < 1000000000)
                {
                    number_text = (Convert.ToDouble(number) / 1000000).ToString("#.##m");
                }
                else
                {
                    number_text = (Convert.ToDouble(number) / 1000000000).ToString("#.##b");
                }
            }
            else
            {
                number_text = Convert.ToInt32(number).ToString("#,##0", new CultureInfo("en-US"));
            }

            return number_text;
        }

        public async Task<string> GET(string url, HttpClient httpClient)
        {
            string result;

            try
            {
                result = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                result = "";
            }

            return result;
        }

        public void Log(string text, int botID, Color color = new Color(), bool show_times = true)
        {
            Invoke(new Action(() =>
            {
                RichTextBox log = findControl.FindRichTextBox("richtextbox_bot_log", botID, this);

                log.SelectionColor = color;
                log.AppendText(" " + (show_times ? $"[ {DateTime.Now:dd.MM.yyyy HH:mm:ss} ]" : "") + $" {text} {Environment.NewLine}");
                log.ScrollToCaret();
            }));
        }

        public void StatusLog(string text, int botID, Image image = null)
        {
            Invoke(new Action(() =>
            {
                findControl.FindToolStrip("toolstrip_bottom_log", botID, this).Items[0].Text = text;
                findControl.FindToolStrip("toolstrip_bottom_log", botID, this).Items[0].Image = image;
            }));
        }

        private async Task<string> Authorization(string login, string password, HttpClient httpClient)
        {
            try
            {
                string result = await httpClient.PostAsync("/login", new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("name", login),
                    new KeyValuePair<string, string>("password", password)
                })).Result.Content.ReadAsStringAsync();

                return result.Contains("Чат").ToString().ToLower();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public void SaveExpirience(string type, string result, int botID)
        {
            // Параметры для парсинга
            string param = "";

            // Получаем ссылку на нижний статус лог
            ToolStrip toolstript_session = findControl.FindToolStrip("toolstrip_bottom_log", botID, this);

            // Переменная для хранения общего опыта
            int exp = (int)toolstript_session.Items[4].Tag;

            // Ставим параметры для парсинга по типу
            switch (type)
            {
                case "Travel":
                    param = @"expirience.png\"" />(.*?)<br />";
                    break;

                case "Glade":
                    param = @"expirience.png\"" />(.*?)</span>";
                    break;

                case "Tasks":
                    param = @"expirience.png\"" />(.*?)</span>";
                    break;

                case "Food":
                    param = @"expirience.png\"" class=\""ml2\"">(.*?)</div>";
                    break;

                case "Play":
                    param = @"expirience.png\"" class=\""ml2\"">(.*?)</div>";
                    break;

                case "Showing":
                    param = @"expirience.png\"" />(.*?)</td>";
                    break;
            }

            // Парсим опыт
            string expirience = new Regex(param).Match(result).Groups[1].Value.Replace("+", "");

            // Если тип Travel, выполняем ещё один парсинг
            if (type == "Travel")
            {
                if (expirience.Contains("heart"))
                {
                    expirience = new Regex(@"(.*?),").Match(expirience).Groups[1].Value;
                }
            }

            // Прибавляем опыт
            if (expirience.Length > 0)
            {
                exp += Convert.ToInt32(expirience);
            }

            // Выводим в лог (Для разработчика)
            if (isDev)
            {
                Log($"{type} = {expirience}", botID, show_times: false);
            }

            // Сохраняем общее число и выводим его
            toolstript_session.Items[4].Tag = exp;
            toolstript_session.Items[4].Text = $"{StringNumberFormat(exp.ToString())} собрано";
            toolstript_session.Items[4].ToolTipText = $"{StringNumberFormat(exp.ToString(), false)} собрано";
        }

        public async Task Statistics(int botID, HttpClient httpClient)
        {
            // Информируем в статус лог
            StatusLog("Обновляю статистику...", botID, Properties.Resources.update);

            // Делаем запрос на профиль
            string result = await GET("/profile", httpClient);

            // Парсим основную статистику
            string beauty_string = new Regex(@"Красота: (.*?)</div>").Match(result).Groups[1].Value;
            string coin_string = new Regex(@"Монеты: (.*?)</div>").Match(result).Groups[1].Value;
            string heart_string = new Regex(@"Сердечки: (.*?)</div>").Match(result).Groups[1].Value;
            string level_string = new Regex("height=\"16\" width=\"16\" alt=\"\"/>([0-9].*?)</td>").Match(result).Groups[1].Value;
            string avatar_string = new Regex(@"avatar([0-9][0-9]?).png").Match(result).Groups[1].Value;
            string isVip_string = new Regex(@"category(.*?)effect").Match(result).Groups[1].Value;
            string nickname_string = new Regex("<a class=\"darkgreen_link\" href=\"/avatars\">(.*?)</a>").Match(result).Groups[1].Value;

            // Получаем ссылку на нижний статус лог
            ToolStrip toolstript_session = findControl.FindToolStrip("toolstrip_bottom_log", botID, this);

            // Получаем сохраненный массив значений
            string[] beauty = (string[])toolstript_session.Items[1].Tag;
            string[] heart = (string[])toolstript_session.Items[2].Tag;
            string[] coin = (string[])toolstript_session.Items[3].Tag;

            if (beauty_string.Length > 0)
            {
                // Обновляем текущее количество красоты
                findControl.FindToolStrip("toolstrip_top_log", botID, this).Items[1].Text = $"Красота: {StringNumberFormat(beauty_string, false)}";

                // Высчитываем красоту
                if (!Convert.ToBoolean(beauty[2]))
                {
                    beauty[0] = beauty_string;
                    beauty[2] = true.ToString().ToLower();
                }
                else
                {
                    beauty[1] = (Convert.ToInt32(beauty_string) - Convert.ToInt32(beauty[0])).ToString();
                }
            }

            if (coin_string.Length > 0)
            {
                // Обновляем текущее количество монет
                findControl.FindToolStrip("toolstrip_top_log", botID, this).Items[2].Text = $"Монет: {StringNumberFormat(coin_string, false)}";

                // Высчитываем монеты
                if (!Convert.ToBoolean(coin[2]))
                {
                    coin[0] = coin_string;
                    coin[2] = true.ToString().ToLower();
                }
                else
                {
                    coin[1] = (Convert.ToInt32(coin_string) - Convert.ToInt32(coin[0])).ToString();
                }
            }

            if (heart_string.Length > 0)
            {
                // Обновляем текущее количество сердечек
                ToolStrip heart_current = findControl.FindToolStrip("toolstrip_top_log", botID, this);

                heart_current.Items[0].Text = $"Сердечек: {StringNumberFormat(heart_string)}";
                heart_current.Items[0].ToolTipText = $"Сердечек: {StringNumberFormat(heart_string, false)}";

                // Высчитываем сердечки
                if (!Convert.ToBoolean(heart[2]))
                {
                    heart[0] = heart_string;
                    heart[2] = true.ToString().ToLower();
                }
                else
                {
                    heart[1] = (Convert.ToInt32(heart_string) - Convert.ToInt32(heart[0])).ToString();
                }
            }

            // Проверка на VIP Аккаунт
            findControl.FindLabel("label_isVip", botID, this).Tag = isVip_string.Length > 0;

            // Обновляем в сохранение
            toolstript_session.Items[1].Tag = beauty;
            toolstript_session.Items[2].Tag = heart;
            toolstript_session.Items[3].Tag = coin;

            // Обновляем текст
            toolstript_session.Items[1].Text = $"{StringNumberFormat(beauty[1], false)} собрано";
            toolstript_session.Items[2].Text = $"{StringNumberFormat(heart[1])} собрано";
            toolstript_session.Items[2].ToolTipText = $"{StringNumberFormat(heart[1], false)} собрано";
            toolstript_session.Items[3].Text = $"{StringNumberFormat(coin[1], false)} собрано";


            // Обновляем аватар
            Invoke(new Action(() =>
            {
                TabPage tabPage = findControl.FindTabPage("tabPage", botID, this);

                tabPage.Text = $"{findControl.FindTextBox("textbox_login", botID, this).Text} [ {level_string} ] ";
                tabPage.ImageIndex = imageList1.Images.IndexOfKey($"avatar{avatar_string}");

                findControl.FindLabel("label_nickname", botID, this).Tag = nickname_string;

                settings.Write($"PETS{botID}", "AVATAR", $"avatar{avatar_string}");
                settings.Write($"PETS{botID}", "LEVEL", $"{level_string}");
            }));
        }

        public async Task Charm(int botID, HttpClient httpClient)
        {
            // Переходим на страницу игры в снежки
            string result = await GET("/charm", httpClient);

            // Начниаем играть только если есть активное задание "Проведи 2 игры в снежки"
            // Или если игра уже была начата или уже идёт
            if (result.Contains("Проведи 2 игры в снежки") || result.Contains("Обновить") || result.Contains("Сменить"))
            {
                Log("-- [ Снежки ] Играем...", botID);

                // Проверяем можем ли мы встать в очередь
                if (result.Contains("Встать в очередь"))
                {
                    StatusLog("[ Снежки ] В очереди...", botID, Properties.Resources.charm);

                    // Встаём в очередь
                    result = await GET("/charm?in_queue=1", httpClient);

                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await GET("/charm", httpClient);
                        await Task.Delay(random.Next(800, 1500));
                    }
                }

                // Если нужно подождать начало
                if (result.Contains("Обновить"))
                {
                    StatusLog("[ Снежки ] Ждём начало...", botID, Properties.Resources.charm);

                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await GET("/charm", httpClient);
                        await Task.Delay(random.Next(800, 1500));
                    }
                }

                // Если игра уже началась
                if (result.Contains("Сменить"))
                {
                    StatusLog("[ Снежки ] Начали играть...", botID, Properties.Resources.charm);

                    // Запускаем цикл для игры (Бросаем снежки через смену питомца)
                    while (result.Contains("Сменить"))
                    {
                        result = await GET("/charm?change=1", httpClient);
                        await Task.Delay(random.Next(500, 1000));
                    }
                }

                // На случай если мы выбыли
                if (result.Contains("Обновить"))
                {
                    StatusLog("[ Снежки ] Выбыли, ждём завершения...", botID, Properties.Resources.charm);

                    // Запускаем цикл ожидания конца игры
                    while (result.Contains("Обновить"))
                    {
                        result = await GET("/charm", httpClient);
                        await Task.Delay(random.Next(800, 1500));
                    }
                }

                // Проверяем выиграли или проиграли
                if (result.Contains("Вы победили"))
                {
                    Log("-- [ Снежки ] Мы выиграли.", botID, Color.Green);
                }
                else
                {
                    Log("-- [ Снежки ] Мы проиграли.", botID, Color.Red);
                }
            }
        }

        public async Task Races(int botID, HttpClient httpClient)
        {
            // Переходим на страницу игры в скачки
            string result = await GET("/races", httpClient);

            // Если есть активное задание "Стань призером скачек 2 раза" 
            // Или если игра уже была начата или уже идёт
            if (result.Contains("Стань призером скачек 2 раза") || result.Contains("Обновить") || result.Contains("Бежать"))
            {
                Log("-- [ Скачки ] Начали играть...", botID);

                // Проверяем можем ли мы встать в очередь
                if (result.Contains("Встать в очередь"))
                {
                    StatusLog("[ Скачки ] В очереди...", botID, Properties.Resources.races);

                    // Встаём в очередь
                    result = await GET("/races?in_queue=1", httpClient);

                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await GET("/races", httpClient);
                        await Task.Delay(random.Next(800, 1500));
                    }
                }

                // Если нужно подождать начало
                if (result.Contains("Обновить"))
                {
                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await GET("/races", httpClient);
                        await Task.Delay(random.Next(800, 1500));
                    }
                }

                // Если игра уже началась
                if (result.Contains("Бежать"))
                {
                    StatusLog("[ Скачки ] Начали играть...", botID, Properties.Resources.races);

                    // Запускаем цикл для игры ( Нажимаем кнопку бежать )
                    while (result.Contains("Бежать"))
                    {
                        result = await GET("/races?go=1", httpClient);

                        // Если кнопка толкнуть стала зеленой, жмем
                        if (result.Contains("73px; width: 70px;"))
                        {
                            result = await GET("/races?attack=0", httpClient);
                        }

                        await Task.Delay(random.Next(500, 1000));
                    }
                }

                // На случай если выиграли раньше всех
                if (result.Contains("Обновить"))
                {
                    StatusLog("[ Скачки ] Ждём завершения...", botID, Properties.Resources.races);

                    // Запускаем цикл ожидания конца игры
                    while (result.Contains("Обновить"))
                    {
                        result = await GET("/races", httpClient);
                        await Task.Delay(random.Next(800, 1500));
                    }
                }

                // Нужные переменные для определения мест
                int seats_count = 1, seats = 1;

                // Определяем наше место
                foreach (Match item in new Regex(@"<a href=""/view_profile\?pet_id=[0-9]*.?"">(.*?)</a>").Matches(result))
                {
                    if (item.Groups[1].Value.Contains((string)findControl.FindLabel("label_nickname", botID, this).Tag))
                    {
                        seats = seats_count;
                    }

                    seats_count++;
                }

                Log($"-- [ Скачки ] Завершили, заняли {seats} место.", botID);
            }
        }

        public async Task WakeUp(int botID, HttpClient httpClient)
        {
            // Делаем запрос на главную
            string result = await GET("/", httpClient);

            if (result.Contains("Дать витаминку за"))
            {
                // Информируем в статус лог
                StatusLog("Даю витаминку...", botID, Properties.Resources.heart);

                // Даём вытаминку
                await Task.Delay(random.Next(500, 1000));
                await GET("/wakeup", httpClient);

                // Логируем действие
                Log("-- Питомец получил витаминку.", botID);
            }
        }

        public async Task Food(int botID, HttpClient httpClient)
        {
            // Делаем запрос на главную
            string result = await GET("/", httpClient);

            // Парсим бесполезное число
            string rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

            if (rand.Length > 0)
            {
                // Информируем в статус лог
                StatusLog("Кормлю питомца...", botID, Properties.Resources.meat);

                // Логируем действие
                Log("-- Кормлю питомца...", botID);

                // Задержка
                await Task.Delay(random.Next(500, 1000));

                // Запускаем цикл
                do
                {
                    // Выполняем действие
                    result = await GET("/?action=food&rand=" + rand, httpClient);

                    // Парсим бесполезное число
                    rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    // Записываем опыт
                    SaveExpirience("Food", result, botID);

                    // Задержка
                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);
            }
        }

        public async Task Play(int botID, HttpClient httpClient)
        {
            // Делаем запрос на главную
            string result = await GET("/", httpClient);

            // Парсим бесполезное число
            string rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

            if (rand.Length > 0)
            {
                // Информируем в статус лог
                StatusLog("Играю с питомцем...", botID, Properties.Resources.game);

                // Логируем действие
                Log("-- Играю с питомцем...", botID);

                // Задержка
                await Task.Delay(random.Next(500, 1000));

                // Запускаем цикл
                do
                {
                    // Выполняем действие
                    result = await GET("/?action=play&rand=" + rand, httpClient);

                    // Парсим бесполезное число
                    rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    // Записываем опыт
                    SaveExpirience("Play", result, botID);

                    // Задержка
                    await Task.Delay(random.Next(500, 1000));
                }
                while (rand.Length > 0);
            }
        }

        public async Task Showing(int botID, HttpClient httpClient)
        {
            // Делаем запрос на главную
            string result = await GET("/", httpClient);

            if (result.Contains("show?start=1"))
            {
                // Информируем в статус лог
                StatusLog("На выставке...", botID, Properties.Resources.cup);

                // Логируем действие
                Log("-- Иду на выставку...", botID);

                // Запускаем поход на выставку
                await GET("/show?start=1", httpClient);

                // Задержка
                await Task.Delay(random.Next(500, 1000));

                // Запускаем цикл
                do
                {
                    // Ходим на выставку
                    result = await GET("/show", httpClient);

                    // Записываем опыт
                    SaveExpirience("Showing", result, botID);

                    // Задержка
                    await Task.Delay(random.Next(500, 1000));
                }
                while (result.Contains("Участвовать за") || result.Contains("Соревноваться"));

                // Логируем действие
                Log("-- Выставка закончена.", botID);
            }
        }

        public async Task Travel(int botID, HttpClient httpClient)
        {
            // Делаем запрос на страницу прогулки
            string result = await GET("/travel", httpClient);

            // Если там есть "Гулять дальше"
            if (result.Contains("Гулять дальше"))
            {
                // Задержка
                await Task.Delay(random.Next(400, 700));

                // Записываем опыт
                SaveExpirience("Travel", result, botID);

                // Завершаем прогулку
                result = await GET("/travel?clear=1", httpClient);

                // Задержка
                await Task.Delay(random.Next(400, 700));
            }

            // Если питомец ещё не гуляет
            if (!result.Contains("Ваш питомец гуляет"))
            {
                // Парсим все ID со ссылок
                MatchCollection travel = new Regex(@"go_travel(.*?)\"" class=").Matches(result);

                // Если id больше нуля
                if (travel.Count > 0)
                {
                    // Определяем переменные
                    int temp_id = 0, curr_id = 0;

                    // Проходимся по каждому ID
                    foreach (Match match in travel)
                    {
                        // Получаем ID
                        int news_id = Convert.ToInt32(match.Groups[1].Value.Replace("?id=", ""));

                        // Если текущий ID больше временного
                        if (news_id > temp_id)
                        {
                            // Записываем в текущий
                            curr_id = news_id;
                        }

                        // Записываем во временную переменную текущий ID
                        temp_id = news_id;
                    }

                    // Отправляем на прогулку (Самая длительная)
                    result = await GET("/go_travel?id=" + curr_id, httpClient);

                    // Проверяем успешно ли отправили
                    if (result.Contains("Ваш питомец гуляет"))
                    {
                        // Логируем действие
                        Log("-- Отправили питомца на прогулку.", botID);
                    }
                }
            }
        }

        public async Task Glade(int botID, HttpClient httpClient)
        {
            // Отправляем запрос на страницу поляны
            string result = await GET("/glade", httpClient);

            // Если можно копать
            if (result.Contains("Копать"))
            {
                // Логируем действие
                Log("-- Копаю поляну...", botID);

                // Запускаем цикл
                do
                {
                    // Капаем поляну
                    result = await GET("/glade_dig", httpClient);

                    // Сохраняем опыт
                    SaveExpirience("Glade", result, botID);

                    // Задержка
                    await Task.Delay(random.Next(500, 1000));
                }
                while (result.Contains("Копать"));

                // Логируем действие
                Log("-- Закончил копать поляну.", botID);
            }
        }

        public async Task Chest(int botID, HttpClient httpClient)
        {
            // Отправляем запрос на страницу шкафа
            string result = await GET("/chest", httpClient);

            // Парсим ссылку предмета
            string url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;

            // Парсим имя предмета
            string name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;

            // Если ссылка предмета не ровна нулю и ссылка не ровна открытию сундука
            if (url.Length > 0 && !url.Contains("open_item"))
            {
                // Логируем действие
                Log("-- В шкафу есть вещи...", botID);

                // Запускам цикл
                while (url.Length > 0 && !url.Contains("open_item"))
                {
                    // Задержка
                    await Task.Delay(random.Next(500, 1000));

                    // Надеваем предмет
                    if (url.Contains("wear_item"))
                    {
                        Log($"--- Надел {name}.", botID, Color.Green);
                    }

                    // Продаем предмет
                    if (url.Contains("sell_item"))
                    {
                        Log($"--- Продал {name}.", botID, Color.Red);
                    }

                    // Отправляем запрос
                    result = await GET(url, httpClient);

                    // Парсим ссылку предмета
                    url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;

                    // Парсим имя предмета
                    name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;
                }

                // Логируем действие
                Log("-- Закончил работу в шкафу...", botID);
            }

            // Получаем ссылку на чекбокс "Открывать сундук"
            CheckBox checkBox = findControl.FindCheckBox("checkbox_opencase", botID, this);

            // Если опция включена и ссылка - это сундук
            if (checkBox.Checked & url.Contains("open_item"))
            {
                // Если на аккаунте нет VIP
                if (!(bool)findControl.FindLabel("label_isVip", botID, this).Tag)
                {
                    // Если есть стальной ключ
                    if (result.Contains("Стальной ключ"))
                    {
                        // Открываем сундук
                        await GET(url, httpClient);

                        // Логируем действие
                        Log($"-- Открыл сундук ключем.", botID);
                    }
                }
            }
        }

        public async Task Tasks(int botID, HttpClient httpClient)
        {
            // Отправляем запрос на страницу заданий
            string result = await GET("/task", httpClient);

            // Парсим список id
            MatchCollection reg = new Regex(@"rd\?id=(.*?)\"" class=").Matches(result);

            // Если найдено больше 0
            if (reg.Count > 0)
            {
                // Логируем действие
                Log("-- Найдено выполненных заданий " + reg.Count + " шт.", botID);

                // Запускаем цикл
                foreach (Match match in reg)
                {
                    // Получаем id
                    string id = match.Groups[1].Value;

                    // Если id больше 0
                    if (id.Length > 0)
                    {
                        // Забираем задания
                        result = await GET("/task_reward?id=" + id, httpClient);

                        // Сохраняем опыт
                        SaveExpirience("Tasks", result, botID);

                        // Задержка
                        await Task.Delay(random.Next(500, 1000));
                    }
                }

                // Логируем действие
                Log("-- Награды за задания собраны.", botID);
            }
        }

        private async Task Sleep(int botID, Button btn, int Interval = 1)
        {
            // Инициализируем таймер ожидания
            DateTime taskStop = DateTime.Now.AddMinutes(Interval);

            // Возвращаем доступность кнопки
            Invoke(new Action(() => btn.Enabled = true));

            // Запускам цикл ожидания
            while (true)
            {
                // Получаем текущие время
                DateTime now = DateTime.Now;

                // Если время прошло, выходим из цикла
                if (now.Hour == taskStop.Hour && now.Minute == taskStop.Minute && now.Second == taskStop.Second || btn.Text.Contains(BOT_START_TEXT))
                {
                    break;
                }

                // Обновляем лог
                StatusLog($"Повтор через {taskStop.Subtract(now):mm} мин : {taskStop.Subtract(now):ss} сек", botID, Properties.Resources.sleep);

                // Задержка
                await Task.Delay(100);
            }
        }

        private void CheckEnabledBot(int botID, Button btn, NumericUpDown numericupdown_interval_from, NumericUpDown numericupdown_interval_do)
        {
            if (!btn.Text.Contains(BOT_START_TEXT))
            {
                StartingBot(botID);
            }
            else
            {
                Invoke(new Action(() =>
                {
                    btn.Text = BOT_START_TEXT;
                    numericupdown_interval_from.Enabled = true;
                    numericupdown_interval_do.Enabled = true;
                }));

                StatusLog("", botID);
            }
        }

        private async void StartingBot(int botID)
        {
            // Создаём новый HttpClient
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://mpets.mobi") };

            // Получаем ссылку на кнопку
            Button btn_start_bot = findControl.FindButton("button_start_bot", botID, this);

            // Получаем ссылку на кнопку
            NumericUpDown numericupdown_interval_from = findControl.FindNumericUpDown("numericupdown_interval_from", botID, this);
            NumericUpDown numericupdown_interval_do = findControl.FindNumericUpDown("numericupdown_interval_do", botID, this);

            // Устанавливаем UserAgent для HttpClient
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

            // Меняем текст кнопки, блокируем кнопку и интервалы
            Invoke(new Action(() =>
            {
                btn_start_bot.Text = BOT_STOP_TEXT;
                btn_start_bot.Enabled = false;
                numericupdown_interval_from.Enabled = false;
                numericupdown_interval_do.Enabled = false;
            }));

            // Получаем логин и пароль
            string login = findControl.FindTextBox("textbox_login", botID, this).Text;
            string password = findControl.FindTextBox("textbox_password", botID, this).Text;

            // Проверяем на пустоту логин и пароль
            if (login.Length > 0 & password.Length > 0)
            {
                // Запускам главный поток бота
                await Task.Run(async () =>
                {
                    // Логируем действие
                    StatusLog("Авторизация...", botID, Properties.Resources.auth);

                    // Авторизация
                    string isLogin = await Authorization(login, password, httpClient);

                    // Если авторизация прошла успешно
                    if (isLogin == "true")
                    {
                        // Логируем действие
                        Log("-- Запускаю задачи...", botID);

                        // Обновляем статистику
                        await Statistics(botID, httpClient);
                        await Task.Delay(random.Next(500, 1000));

                        // Статус цикла
                        bool status = true;

                        // Запускаем цикл (Кормим, Играем, Ходим на выставку)
                        do
                        {
                            // Переменная сна
                            bool sleep = false;

                            // Делаем запрос на главную
                            string result = await GET("/", httpClient);

                            // Если можно разбудить бесплатно
                            if (result.Contains("Разбудить бесплатно"))
                            {
                                // Будим бесплатно
                                result = await GET("/wakeup_sleep", httpClient);

                                // Логируем действие
                                Log("-- Разбудили питомца бесплатно.", botID);
                            }

                            // Если не нужно кормить, играть и ходить на выставки
                            if (result.Contains("Играть ещё") || result.Contains("Разбудить"))
                            {
                                status = false;
                            }

                            // Если нет возможности дать витаминку
                            if (new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value.Length == 0 & !result.Contains("show?start=1"))
                            {
                                sleep = true;
                            }

                            // Задержка
                            await Task.Delay(random.Next(800, 1000));

                            // Кормим, Играем, Ходим на выставки
                            if (status)
                            {
                                if (!sleep)
                                {
                                    await Food(botID, httpClient);
                                    await Task.Delay(random.Next(500, 1000));

                                    await Play(botID, httpClient);
                                    await Task.Delay(random.Next(500, 1000));

                                    await Showing(botID, httpClient);
                                    await Task.Delay(random.Next(500, 1000));

                                    await WakeUp(botID, httpClient);
                                    await Task.Delay(random.Next(1000, 2000));
                                }
                                else
                                {
                                    StatusLog("Питомец отдыхает...", botID, Properties.Resources.sleep);
                                    await Task.Delay(random.Next(1000, 5000));
                                }
                            }
                        }
                        while (status);

                        // Если включена опция "Отправлять на прогулку"
                        if (findControl.FindCheckBox("checkbox_travel", botID, this).Checked)
                        {
                            StatusLog("Проверяю прогулки...", botID, Properties.Resources.travel);

                            await Travel(botID, httpClient);
                            await Task.Delay(random.Next(500, 1000));
                        }

                        // Если включена опция "Копать поляну"
                        if (findControl.FindCheckBox("checkbox_glade", botID, this).Checked)
                        {
                            StatusLog("Проверяю поляну...", botID, Properties.Resources.garden);

                            await Glade(botID, httpClient);
                            await Task.Delay(random.Next(500, 1000));
                        }

                        // Если включена опция "Надевать и продавать вещи"
                        if (findControl.FindCheckBox("checkbox_chest", botID, this).Checked)
                        {
                            StatusLog("Проверяю шкаф...", botID, Properties.Resources.chest);

                            await Chest(botID, httpClient);
                            await Task.Delay(random.Next(500, 1000));
                        }

                        // Если включена опция "[ Задание ] Снеговик"
                        if (findControl.FindCheckBox("checkbox_charm", botID, this).Checked)
                        {
                            // Игра в снежки
                            await Charm(botID, httpClient);
                            await Task.Delay(random.Next(500, 1000));
                        }

                        // Если включена опция "[ Задание ] Жокей"
                        if (findControl.FindCheckBox("checkbox_races", botID, this).Checked)
                        {
                            // Скачки
                            await Races(botID, httpClient);
                            await Task.Delay(random.Next(500, 1000));
                        }

                        // Если включена опция "Забирать задания" 
                        if (findControl.FindCheckBox("checkbox_tasks", botID, this).Checked)
                        {
                            StatusLog("Проверяю задания...", botID, Properties.Resources.tasks);

                            await Tasks(botID, httpClient);
                            await Task.Delay(random.Next(500, 1000));
                        }

                        // Обновляем статистику
                        await Statistics(botID, httpClient);
                        await Task.Delay(random.Next(500, 1000));

                        Log("-- Все задачи выполнены.", botID);
                        Log("", botID, show_times: false);

                        // Ожидание
                        await Sleep(botID, btn_start_bot, random.Next(Convert.ToInt32(findControl.FindNumericUpDown("numericupdown_interval_from", botID, this).Value), Convert.ToInt32(findControl.FindNumericUpDown("numericupdown_interval_do", botID, this).Value) + 1));

                        // Проверяем не остановлен ли бот
                        CheckEnabledBot(botID, btn_start_bot, numericupdown_interval_from, numericupdown_interval_do);
                    }
                    else if (isLogin == "false")
                    {
                        Log("-- Вы ввели неправильное имя или пароль, повтор через 1 минуту.", botID);
                        Log("", botID, show_times: false);

                        // Ожидание
                        await Sleep(botID, btn_start_bot, 1);

                        // Проверяем не остановлен ли бот
                        CheckEnabledBot(botID, btn_start_bot, numericupdown_interval_from, numericupdown_interval_do);
                    }
                    else
                    {
                        Log("-- Ошибка сети, повтор через 1 минуту...", botID);
                        Log("", botID, show_times: false);

                        // Ожидание
                        await Sleep(botID, btn_start_bot, 1);

                        // Проверяем не остановлен ли бот
                        CheckEnabledBot(botID, btn_start_bot, numericupdown_interval_from, numericupdown_interval_do);
                    }
                });
            }
            else
            {
                Log("-- Логин или пароль не может быть пустым.", botID);
                Log("", botID, show_times: false);

                Invoke(new Action(() =>
                {
                    btn_start_bot.Enabled = true;
                    btn_start_bot.Text = BOT_START_TEXT;
                    numericupdown_interval_from.Enabled = true;
                    numericupdown_interval_do.Enabled = true;
                }));
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
            findControl.FindTextBox("textbox_login", NumberTabs, this).Text = login;
            findControl.FindTextBox("textbox_password", NumberTabs, this).Text = password;
            findControl.FindNumericUpDown("numericupdown_interval_from", NumberTabs, this).Value = interval_from;
            findControl.FindNumericUpDown("numericupdown_interval_do", NumberTabs, this).Value = interval_do;
            findControl.FindCheckBox("checkbox_travel", NumberTabs, this).Checked = travel;
            findControl.FindCheckBox("checkbox_chest", NumberTabs, this).Checked = chest;
            findControl.FindCheckBox("checkbox_glade", NumberTabs, this).Checked = glade;
            findControl.FindCheckBox("checkbox_tasks", NumberTabs, this).Checked = tasks;
            findControl.FindCheckBox("checkbox_opencase", NumberTabs, this).Checked = open_case;
            findControl.FindCheckBox("checkbox_charm", NumberTabs, this).Checked = charm;
            findControl.FindCheckBox("checkbox_races", NumberTabs, this).Checked = races;
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
                Button button = findControl.FindButton("button_start_bot", i, this);

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
                Button button = findControl.FindButton("button_start_bot", i, this);

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
            AutoStart(toolStripMenuItem5.Checked);
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