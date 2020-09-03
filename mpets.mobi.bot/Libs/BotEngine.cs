using mpets.mobi.bot.Libs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mpets.mobi.bot
{
    internal class BotEngine
    {
        /// <summary>
        /// Запускает задачу, которая авторизуется в игре.
        /// </summary>
        /// <param name="Login">Имя питомца.</param>
        /// <param name="Password">Пароль.</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <returns></returns>
        public static async Task<string> Authorization(string Login, string Password, HttpClient HttpClient)
        {
            try
            {
                // Генерируем POST запрос
                FormUrlEncodedContent parameters = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("name", Login),
                    new KeyValuePair<string, string>("password", Password)
                });

                // Отправляем запрос
                string result = await HttpClient.PostAsync("/login", parameters).Result.Content.ReadAsStringAsync();

                // Возвращаем успешность авторизации
                return result.Contains("Чат").ToString().ToLower();
            }
            catch (Exception)
            {
                return "false";
            }
        }

        /// <summary>
        /// Запускает задачу, которая обновляет основую статистику питомца, а так же за сессию.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <param name="Setting">Ссылка на экземпляр класса <see cref="IniFiles"/>.</param>
        /// <returns></returns>
        public static async Task Statistics(int BotID, HttpClient HttpClient, Form1 Form, IniFiles Setting)
        {
            // Информируем в статус лог
            HelpMethod.StatusLog("Обновление статистики...", BotID, Form, Properties.Resources.update);

            // Делаем запрос на профиль
            string result = await HelpMethod.GET("/profile", HttpClient);

            // Парсим основную статистику
            string beauty_string = new Regex(@"Красота: (.*?)</div>").Match(result).Groups[1].Value;
            string coin_string = new Regex(@"Монеты: (.*?)</div>").Match(result).Groups[1].Value;
            string heart_string = new Regex(@"Сердечки: (.*?)</div>").Match(result).Groups[1].Value;
            string level_string = new Regex("height=\"16\" width=\"16\" alt=\"\"/>([0-9].*?)</td>").Match(result).Groups[1].Value;
            string avatar_string = new Regex(@"avatar([0-9][0-9]?).png").Match(result).Groups[1].Value;
            string isVip_string = new Regex(@"category(.*?)effect").Match(result).Groups[1].Value;
            string nickname_string = new Regex("<a class=\"darkgreen_link\" href=\"/avatars\">(.*?)</a>").Match(result).Groups[1].Value;

            // Получаем ссылку на нижний статус лог
            ToolStrip toolstript_session = HelpMethod.findControl.FindToolStrip("toolstrip_bottom_log", BotID, Form);

            // Получаем сохраненный массив значений
            string[] beauty = (string[])toolstript_session.Items[1].Tag;
            string[] heart = (string[])toolstript_session.Items[2].Tag;
            string[] coin = (string[])toolstript_session.Items[3].Tag;

            if (beauty_string.Length > 0)
            {
                // Обновляем текущее количество красоты
                HelpMethod.findControl.FindToolStrip("toolstrip_top_log", BotID, Form).Items[1].Text = $"Красота: {HelpMethod.StringNumberFormat(beauty_string, false)}";

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
                HelpMethod.findControl.FindToolStrip("toolstrip_top_log", BotID, Form).Items[2].Text = $"Монет: {HelpMethod.StringNumberFormat(coin_string, false)}";

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
                ToolStrip heart_current = HelpMethod.findControl.FindToolStrip("toolstrip_top_log", BotID, Form);

                heart_current.Items[0].Text = $"Сердечек: {HelpMethod.StringNumberFormat(heart_string)}";
                heart_current.Items[0].ToolTipText = $"Сердечек: {HelpMethod.StringNumberFormat(heart_string, false)}";

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
            HelpMethod.findControl.FindLabel("label_isVip", BotID, Form).Tag = isVip_string.Length > 0;

            // Обновляем в сохранение
            toolstript_session.Items[1].Tag = beauty;
            toolstript_session.Items[2].Tag = heart;
            toolstript_session.Items[3].Tag = coin;

            // Обновляем текст
            toolstript_session.Items[1].Text = $"{HelpMethod.StringNumberFormat(beauty[1], false)} собрано";
            toolstript_session.Items[2].Text = $"{HelpMethod.StringNumberFormat(heart[1])} собрано";
            toolstript_session.Items[2].ToolTipText = $"{HelpMethod.StringNumberFormat(heart[1], false)} собрано";
            toolstript_session.Items[3].Text = $"{HelpMethod.StringNumberFormat(coin[1], false)} собрано";


            // Обновляем аватар
            Form.Invoke((MethodInvoker)delegate
            {
                TabPage tabPage = HelpMethod.findControl.FindTabPage("tabPage", BotID, Form);

                tabPage.Text = $"{HelpMethod.findControl.FindTextBox("textbox_login", BotID, Form).Text} [ {level_string} ] ";
                tabPage.ImageIndex = Form.imageList1.Images.IndexOfKey($"avatar{avatar_string}");

                HelpMethod.findControl.FindLabel("label_nickname", BotID, Form).Tag = nickname_string;

                Setting.Write($"PETS{BotID}", "AVATAR", $"avatar{avatar_string}");
                Setting.Write($"PETS{BotID}", "LEVEL", $"{level_string}");
            });
        }

        /// <summary>
        /// Запускает задачу, которая играет в мини-игру "Снежки".
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Charm(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Переходим на страницу игры в снежки
            string result = await HelpMethod.GET("/charm", HttpClient);

            // Начниаем играть только если есть активное задание "Проведи 2 игры в снежки"
            // Или если игра уже была начата или уже идёт
            if (result.Contains("Проведи 2 игры в снежки") || result.Contains("Обновить") || result.Contains("Сменить"))
            {
                // Проверяем можем ли мы встать в очередь
                if (result.Contains("Встать в очередь"))
                {
                    HelpMethod.StatusLog("[ Снежки ] В очереди...", BotID, Form, Properties.Resources.charm);

                    // Встаём в очередь
                    result = await HelpMethod.GET("/charm?in_queue=1", HttpClient);

                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await HelpMethod.GET("/charm", HttpClient);
                    }
                }

                // Если нужно подождать начало
                if (result.Contains("Обновить"))
                {
                    HelpMethod.StatusLog("[ Снежки ] Ждём начало...", BotID, Form, Properties.Resources.charm);

                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await HelpMethod.GET("/charm", HttpClient);
                    }
                }

                // Если игра уже началась
                if (result.Contains("Сменить"))
                {
                    HelpMethod.StatusLog("[ Снежки ] Играем...", BotID, Form, Properties.Resources.charm);

                    // Запускаем цикл для игры (Бросаем снежки через смену питомца)
                    while (result.Contains("Сменить"))
                    {
                        result = await HelpMethod.GET("/charm?change=1", HttpClient);
                    }
                }

                // На случай если мы выбыли
                if (result.Contains("Обновить"))
                {
                    HelpMethod.StatusLog("[ Снежки ] Выбыли, ждём завершения...", BotID, Form, Properties.Resources.charm);

                    // Запускаем цикл ожидания конца игры
                    while (result.Contains("Обновить"))
                    {
                        result = await HelpMethod.GET("/charm", HttpClient);
                    }
                }

                // Проверяем выиграли или проиграли
                if (result.Contains("Вы победили"))
                {
                    HelpMethod.Log("[ Снежки ] Мы выиграли.", BotID, Form, Color.Green);
                }
                else
                {
                    HelpMethod.Log("[ Снежки ] Мы проиграли.", BotID, Form, Color.Red);
                }
            }
        }

        /// <summary>
        /// Запускает задачу, которая играет в мини-игру "Скачки".
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Races(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Переходим на страницу игры в скачки
            string result = await HelpMethod.GET("/races", HttpClient);

            // Фикс фейкового "старта игры"
            if (result.Contains("Обновить"))
            {
                result = await HelpMethod.GET("/races", HttpClient);
            }

            // Если есть активное задание "Стань призером скачек 2 раза" 
            // Или если игра уже была начата или уже идёт
            if (result.Contains("Стань призером скачек 2 раза") || result.Contains("Обновить") || result.Contains("Бежать"))
            {
                // Проверяем можем ли мы встать в очередь
                if (result.Contains("Встать в очередь"))
                {
                    HelpMethod.StatusLog("[ Скачки ] В очереди...", BotID, Form, Properties.Resources.races);

                    // Встаём в очередь
                    result = await HelpMethod.GET("/races?in_queue=1", HttpClient);

                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await HelpMethod.GET("/races", HttpClient);
                    }
                }

                // Если нужно подождать начало
                if (result.Contains("Обновить"))
                {
                    // Ждем пока начнется игра
                    while (result.Contains("Обновить"))
                    {
                        result = await HelpMethod.GET("/races", HttpClient);
                    }
                }

                // Если игра уже началась
                if (result.Contains("Бежать"))
                {
                    HelpMethod.StatusLog("[ Скачки ] Играем...", BotID, Form, Properties.Resources.races);

                    // Запускаем цикл для игры ( Нажимаем кнопку бежать )
                    while (result.Contains("Бежать"))
                    {
                        result = await HelpMethod.GET("/races?go=1", HttpClient);

                        // Если кнопка толкнуть стала зеленой, жмем
                        if (result.Contains("73px; width: 70px;"))
                        {
                            result = await HelpMethod.GET("/races?attack=0", HttpClient);
                        }
                    }
                }

                // На случай если выиграли раньше всех
                if (result.Contains("Обновить"))
                {
                    HelpMethod.StatusLog("[ Скачки ] Ждём завершения...", BotID, Form, Properties.Resources.races);

                    // Запускаем цикл ожидания конца игры
                    while (result.Contains("Обновить"))
                    {
                        result = await HelpMethod.GET("/races", HttpClient);
                    }
                }

                // Нужные переменные для определения мест
                int seats_count = 1, seats = 1;

                // Определяем наше место
                foreach (Match item in new Regex(@"<a href=""/view_profile\?pet_id=[0-9]*.?"">(.*?)</a>").Matches(result))
                {
                    if (item.Groups[1].Value.Contains((string)HelpMethod.findControl.FindLabel("label_nickname", BotID, Form).Tag))
                    {
                        seats = seats_count;
                    }

                    seats_count++;
                }

                HelpMethod.Log($"[ Скачки ] Заняли {seats} место.", BotID, Form, seats <= 3 ? Color.Green : Color.Red);
            }
        }

        /// <summary>
        /// Запускает задачу, которая даёт питомцу витаминку.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task WakeUp(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Делаем запрос на главную
            string result = await HelpMethod.GET("/", HttpClient);

            if (result.Contains("Дать витаминку за"))
            {
                // Информируем в статус лог
                HelpMethod.StatusLog("Даём витаминку...", BotID, Form, Properties.Resources.heart);

                // Даём витаминку
                await HelpMethod.GET("/wakeup", HttpClient);

                // Логируем действие
                HelpMethod.Log("Дали питомцу витаминку.", BotID, Form);
            }
        }

        /// <summary>
        /// Запускает задачу, которая кормит питомца.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Food(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Делаем запрос на главную
            string result = await HelpMethod.GET("/", HttpClient);

            // Парсим бесполезное число
            string rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

            if (rand.Length > 0)
            {
                // Информируем в статус лог
                HelpMethod.StatusLog("Кормим питомца...", BotID, Form, Properties.Resources.meat);

                // Логируем действие
                HelpMethod.Log("Кормим питомца...", BotID, Form);

                // Запускаем цикл
                do
                {
                    // Выполняем действие
                    result = await HelpMethod.GET("/?action=food&rand=" + rand, HttpClient);

                    // Парсим бесполезное число
                    rand = new Regex(@"action=food&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    // Записываем опыт
                    SaveExpirience("Food", result, BotID, Form);
                }
                while (rand.Length > 0);
            }
        }

        /// <summary>
        /// Запускает задачу, которая играет с питомцем.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Play(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Делаем запрос на главную
            string result = await HelpMethod.GET("/", HttpClient);

            // Парсим бесполезное число
            string rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

            if (rand.Length > 0)
            {
                // Информируем в статус лог
                HelpMethod.StatusLog("Играем с питомцем...", BotID, Form, Properties.Resources.game);

                // Логируем действие
                HelpMethod.Log("Играем с питомцем...", BotID, Form);

                // Запускаем цикл
                do
                {
                    // Выполняем действие
                    result = await HelpMethod.GET("/?action=play&rand=" + rand, HttpClient);

                    // Парсим бесполезное число
                    rand = new Regex(@"action=play&rand=(.*?)\"" class=").Match(result).Groups[1].Value;

                    // Записываем опыт
                    SaveExpirience("Play", result, BotID, Form);
                }
                while (rand.Length > 0);
            }
        }

        /// <summary>
        /// Запускает задачу, которая посещает выставки.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Showing(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Делаем запрос на главную
            string result = await HelpMethod.GET("/", HttpClient);

            if (result.Contains("show?start=1"))
            {
                // Информируем в статус лог
                HelpMethod.StatusLog("На выставке...", BotID, Form, Properties.Resources.cup);

                // Логируем действие
                HelpMethod.Log("Идём на выставку...", BotID, Form);

                // Запускаем поход на выставку
                await HelpMethod.GET("/show?start=1", HttpClient);

                // Запускаем цикл
                do
                {
                    // Ходим на выставку
                    result = await HelpMethod.GET("/show", HttpClient);

                    // Если нужно выбрать страну
                    if (result.Contains("Выбрать"))
                    {
                        // Парсим ссылку для выбора страны
                        string show_select = new Regex(@"<a href=""(/.*?\?id=[0-9].?)").Match(result).Groups[1].Value;

                        // Если ссылка не пустая
                        if (show_select.Length > 0)
                        {
                            result = await HelpMethod.GET(show_select, HttpClient);
                            HelpMethod.Log($"Выбрали новую страну на выставке.", BotID, Form, Color.Green, false);
                        }
                    }

                    // Записываем опыт
                    SaveExpirience("Showing", result, BotID, Form);
                }
                while (result.Contains("Участвовать за") || result.Contains("Соревноваться"));

                // Логируем действие
                HelpMethod.Log($"Выставка завершена.", BotID, Form);
            }
        }

        /// <summary>
        /// Запускает задачу, которая отправляет на прогулку питомца.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Travel(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Делаем запрос на страницу прогулки
            string result = await HelpMethod.GET("/travel", HttpClient);

            // Если там есть "Гулять дальше"
            if (result.Contains("Гулять дальше"))
            {
                // Записываем опыт
                SaveExpirience("Travel", result, BotID, Form);

                // Завершаем прогулку
                result = await HelpMethod.GET("/travel?clear=1", HttpClient);
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
                    result = await HelpMethod.GET("/go_travel?id=" + curr_id, HttpClient);

                    // Проверяем успешно ли отправили
                    if (result.Contains("Ваш питомец гуляет"))
                    {
                        // Логируем действие
                        HelpMethod.Log("Отправили питомца на прогулку.", BotID, Form);
                    }
                }
            }
        }

        /// <summary>
        /// Запускает задачу, копает поляну.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Glade(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Отправляем запрос на страницу поляны
            string result = await HelpMethod.GET("/glade", HttpClient);

            // Если можно копать
            if (result.Contains("Копать"))
            {
                // Логируем действие
                HelpMethod.Log("Вскапываем поляну...", BotID, Form);

                // Запускаем цикл
                do
                {
                    // Капаем поляну
                    result = await HelpMethod.GET("/glade_dig", HttpClient);

                    // Сохраняем опыт
                    SaveExpirience("Glade", result, BotID, Form);
                }
                while (result.Contains("Копать"));

                // Логируем действие
                HelpMethod.Log("Поляна вскопана.", BotID, Form);
            }
        }

        /// <summary>
        /// Запускает задачу, которая проверяет шкаф (одевает вещи, продаёт вещи, открывает сундуки).
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Chest(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Отправляем запрос на страницу шкафа
            string result = await HelpMethod.GET("/chest", HttpClient);

            // Парсим ссылку предмета
            string url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;

            // Парсим имя предмета
            string name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;

            // Если ссылка предмета не ровна нулю и ссылка не ровна открытию сундука
            if (url.Length > 0 && !url.Contains("open_item"))
            {
                // Запускам цикл
                while (url.Length > 0 && !url.Contains("open_item"))
                {
                    // Надеваем предмет
                    if (url.Contains("wear_item"))
                    {
                        HelpMethod.Log($"Надел {name}.", BotID, Form, Color.Green, false);
                    }

                    // Продаем предмет
                    if (url.Contains("sell_item"))
                    {
                        HelpMethod.Log($"Продал {name}.", BotID, Form, Color.Red, false);
                    }

                    // Отправляем запрос
                    result = await HelpMethod.GET(url, HttpClient);

                    // Парсим ссылку предмета
                    url = new Regex(@"<a href=\""(.*?)\"" class=\""bbtn mt5 vb\""").Match(result).Groups[1].Value;

                    // Парсим имя предмета
                    name = new Regex(@"<div class=\""mt3\"">(.*?)</div>").Match(result).Groups[1].Value;
                }
            }

            // Получаем ссылку на чекбокс "Открывать сундук"
            CheckBox checkBox = HelpMethod.findControl.FindCheckBox("checkbox_opencase", BotID, Form);

            // Если опция включена и ссылка - это сундук
            if (checkBox.Checked & url.Contains("open_item"))
            {
                // Если на аккаунте нет VIP
                if (!(bool)HelpMethod.findControl.FindLabel("label_isVip", BotID, Form).Tag)
                {
                    // Если есть стальной ключ
                    if (result.Contains("Стальной ключ"))
                    {
                        // Открываем сундук
                        await HelpMethod.GET(url, HttpClient);

                        // Логируем действие
                        HelpMethod.Log("Открыли сундук ключём.", BotID, Form);
                    }
                }
            }
        }

        /// <summary>
        /// Запускает задачу, которая забирает задания (включая медали).
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="HttpClient">Ссылка на экземпляр класса <see cref="HttpClient"/>.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Tasks(int BotID, HttpClient HttpClient, Form1 Form)
        {
            // Отправляем запрос на страницу заданий
            string result = await HelpMethod.GET("/task", HttpClient);

            // Парсим список id
            MatchCollection reg = new Regex(@"rd\?id=(.*?)\"" class=").Matches(result);

            // Если найдено больше 0
            if (reg.Count > 0)
            {
                // Запускаем цикл
                foreach (Match match in reg)
                {
                    // Получаем id
                    string id = match.Groups[1].Value;

                    // Если id больше 0
                    if (id.Length > 0)
                    {
                        // Забираем задания
                        result = await HelpMethod.GET("/task_reward?id=" + id, HttpClient);

                        // Сохраняем опыт
                        SaveExpirience("Tasks", result, BotID, Form);
                    }
                }

                // Логируем действие
                HelpMethod.Log($"Забрали ( {reg.Count} ) ежедневных заданий.", BotID, Form);
            }
        }

        /// <summary>
        /// Запускает задачу ожидания.
        /// </summary>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="Button">Ссылка на экземпляр класса <see cref="Button"/>.</param>
        /// <param name="Interval">Интервал, в минутах.</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        /// <returns></returns>
        public static async Task Sleep(int BotID, Button Button, Form1 Form, int Interval = 1)
        {
            // Инициализируем таймер ожидания
            DateTime taskStop = DateTime.Now.AddMinutes(Interval);

            // Возвращаем доступность кнопки
            Form.Invoke(new Action(() => Button.Enabled = true));

            // Запускам цикл ожидания
            while (true)
            {
                // Получаем текущие время
                DateTime now = DateTime.Now;

                // Если время прошло, выходим из цикла
                if (now.Hour == taskStop.Hour && now.Minute == taskStop.Minute && now.Second == taskStop.Second || Button.Text.Contains(Form1.BOT_START_TEXT))
                {
                    break;
                }

                // Обновляем лог
                HelpMethod.StatusLog($"Повтор через {taskStop.Subtract(now):mm} мин : {taskStop.Subtract(now):ss} сек", BotID, Form, Properties.Resources.sleep);

                // Задержка
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Сохраняет и обновляет общий опыт за сессию.
        /// </summary>
        /// <param name="Type">Тип операции.</param>
        /// <param name="Result">Исходный HTML-код.</param>
        /// <param name="BotID">Индентификатор бота (вкладки).</param>
        /// <param name="Form">Ссылка на экземпляр класса <see cref="Form1"/>.</param>
        private static void SaveExpirience(string Type, string Result, int BotID, Form1 Form)
        {
            // Параметры для парсинга
            string param = "";

            // Получаем ссылку на нижний статус лог
            ToolStrip toolstript_session = HelpMethod.findControl.FindToolStrip("toolstrip_bottom_log", BotID, Form);

            // Переменная для хранения общего опыта
            int exp = (int)toolstript_session.Items[4].Tag;

            // Ставим параметры для парсинга по типу
            switch (Type)
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
            string expirience = new Regex(param).Match(Result).Groups[1].Value.Replace("+", "");

            // Если тип Travel, выполняем ещё один парсинг
            if (Type == "Travel")
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

            // Сохраняем общее число и выводим его
            toolstript_session.Items[4].Tag = exp;
            toolstript_session.Items[4].Text = $"{HelpMethod.StringNumberFormat(exp.ToString())} собрано";
            toolstript_session.Items[4].ToolTipText = $"{HelpMethod.StringNumberFormat(exp.ToString(), false)} собрано";
        }
    }
}
