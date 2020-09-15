using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mpets.mobi.bot.Libs
{
    internal class HelpMethod
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// Инициализированная переменная класса <see cref="Random"/>.
        /// </summary>
        public static readonly Random getRandomNumber = new Random();

        /// <summary>
        /// Ищет компоненты на форме.
        /// </summary>
        public static FindControl findControl = new FindControl();

        /// <summary>
        /// Создаёт задачу которая, будет выполнена после случайной задержки.
        /// </summary>
        /// <param name="Minimum">Минимальное число задержки.</param>
        /// <param name="Maximum">Максимальное число задержки.</param>
        /// <returns>Задача, представляющая случайную временную задержку.</returns>
        public static async Task<string> RandomDelay(int Minimum, int Maximum)
        {
            int delay = getRandomNumber.Next(Minimum, Maximum + 1);
            await Task.Delay(delay);

            return delay.ToString();
        }

        /// <summary>
        /// Устанавливает placeholder для текстовых полей.
        /// </summary>
        /// <param name="TextBox">Ссылка на экземпляр класса <see cref="TextBox"/>.</param>
        /// <param name="PlaceholderText">Текст placeholder.</param>
        public static void SetPlaceholder(TextBox TextBox, string PlaceholderText)
        {
            SendMessage(TextBox.Handle, 0x1500 + 1, 0, PlaceholderText);
        }

        /// <summary>
        /// Устанавливает размер последней вкладки <see cref="TabControl"/> минимального размера.
        /// </summary>
        /// <param name="TabControl">Ссылка на экземпляр <see cref="TabControl"/>.</param>
        public static void TabControlSmallWidth(TabControl TabControl)
        {
            TabControl.HandleCreated += (s, e) =>
            {
                _ = SendMessage(TabControl.Handle, 0x1300 + 49, IntPtr.Zero, (IntPtr)10);
            };
        }

        /// <summary>
        /// Добавляет приложение в автозагрузку Windows.
        /// </summary>
        /// <param name="Flag">True - Добавляет, False - Убирает</param>
        public static void AutoRun(bool Flag)
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
                if (Flag)
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

        /// <summary>
        /// Отправляет GET запрос на указанный URL-адрес.
        /// </summary>
        /// <param name="Url">Ссылка на страницу.</param>
        /// <param name="HttpClient">Ссылка на класс <see cref="HttpClient"/>.</param>
        /// <returns>Вернёт исходный код страницы.</returns>
        public static async Task<string> GET(string Url, HttpClient HttpClient)
        {
            try
            {
                // Задержка
                string delay = await RandomDelay(400, 1000);

                // Для дебага
                Debug.WriteLine($"https://mpets.mobi{Url}, {delay}ms");

                // Отправляем запрос
                return await HttpClient.GetAsync(Url).Result.Content.ReadAsStringAsync();
            }
            catch
            {
                // Если произошла ошибка, возвращаем пустую строку
                return "";
            }
        }

        /// <summary>
        /// Форматирует цифровую строку в красивый вид.
        /// </summary>
        /// <param name="Number">Число строкой.</param>
        /// <param name="Format_type">Тип форматирования, true - 1.11k, false - 100,000</param>
        /// <returns></returns>
        public static string StringNumberFormat(string Number, bool Format_type = true)
        {
            string number_text;

            if (Format_type)
            {
                if (Convert.ToDouble(Number) < 1000)
                {
                    number_text = Number.ToString();
                }
                else if (Convert.ToDouble(Number) < 1000000)
                {
                    number_text = (Convert.ToDouble(Number) / 1000).ToString("#.##k");
                }
                else if (Convert.ToDouble(Number) < 1000000000)
                {
                    number_text = (Convert.ToDouble(Number) / 1000000).ToString("#.##m");
                }
                else
                {
                    number_text = (Convert.ToDouble(Number) / 1000000000).ToString("#.##b");
                }
            }
            else
            {
                number_text = Convert.ToInt32(Number).ToString("#,##0", new CultureInfo("en-US"));
            }

            return number_text;
        }

        /// <summary>
        /// Отправляет строку в <see cref="RichTextBox"/> с поддержкой цвета и скрытие времени.
        /// </summary>
        /// <param name="Text">Строка.</param>
        /// <param name="BotID">Идентификатор бота (вкладки).</param>
        /// <param name="Form1">Ссылка на <see cref="Form1"/>.</param>
        /// <param name="Color">Цвет текста.</param>
        /// <param name="ShowTime">True - Показывать время, False - Не показывать время.</param>
        public static void Log(string Text, int BotID, Form1 Form1, Color Color = new Color(), bool ShowTime = true)
        {
            Form1.Invoke((MethodInvoker)delegate
            {
                RichTextBox log = findControl.FindRichTextBox("richtextbox_bot_log", BotID, Form1);

                log.SelectionColor = SystemColors.ControlDarkDark;
                log.AppendText($" {(ShowTime ? $"[ { DateTime.Now:dd.MM.yyyy HH:mm:ss} ]" : "")}");

                log.SelectionColor = Color;
                log.AppendText($" {(Text.Length > 0 ? "--" : "")} {Text} {Environment.NewLine}");

                log.ScrollToCaret();
            });
        }

        /// <summary>
        /// Отправляет строку в <see cref="ToolStrip"/> с поддержкой <see cref="Image"/>.
        /// </summary>
        /// <param name="Text">Строка.</param>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="Form1">Ссылка на <see cref="Form1"/>.</param>
        /// <param name="Image">Картинка.</param>
        public static void StatusLog(string Text, int BotID, Form1 Form1, Image Image = null)
        {
            Form1.Invoke((MethodInvoker)delegate
            {
                findControl.FindToolStrip("toolstrip_bottom_log", BotID, Form1).Items[0].Text = Text;
                findControl.FindToolStrip("toolstrip_bottom_log", BotID, Form1).Items[0].Image = Image;
            });
        }
    }
}
