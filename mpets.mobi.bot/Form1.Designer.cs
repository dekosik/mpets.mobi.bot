namespace mpets.mobi.bot
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.login = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.start = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.OpenCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.HideCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoRunCheckBox = new System.Windows.Forms.CheckBox();
            this.TasksCheckBox = new System.Windows.Forms.CheckBox();
            this.GladeCheckBox = new System.Windows.Forms.CheckBox();
            this.ChestCheckBox = new System.Windows.Forms.CheckBox();
            this.TravelCheckBox = new System.Windows.Forms.CheckBox();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.HeartCurrent = new System.Windows.Forms.ToolStripLabel();
            this.BeautyCurrent = new System.Windows.Forms.ToolStripLabel();
            this.CoinCurrent = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.BeautySession = new System.Windows.Forms.ToolStripLabel();
            this.HeartSession = new System.Windows.Forms.ToolStripLabel();
            this.CoinSession = new System.Windows.Forms.ToolStripLabel();
            this.ExpSession = new System.Windows.Forms.ToolStripLabel();
            this.BotsLogs = new System.Windows.Forms.ToolStripLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя питомца";
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(6, 28);
            this.login.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.login.MaxLength = 20;
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(194, 22);
            this.login.TabIndex = 1;
            this.login.TabStop = false;
            this.login.TextChanged += new System.EventHandler(this.Login_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 52);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Пароль";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(6, 68);
            this.password.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(194, 22);
            this.password.TabIndex = 3;
            this.password.TabStop = false;
            this.password.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.password);
            this.groupBox1.Controls.Add(this.login);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, -2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupBox1.Size = new System.Drawing.Size(206, 177);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.numericUpDown2);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDown1);
            this.groupBox3.Location = new System.Drawing.Point(8, 92);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupBox3.Size = new System.Drawing.Size(190, 78);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Интервал повторов ( мин )";
            this.toolTip1.SetToolTip(this.groupBox3, "Тут можно выбрать интервал повторов.\r\nНапример: Бот выберёт рандомное число от 30" +
        " до 60 минут для следующего старта.");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 52);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "До";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(31, 47);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(154, 22);
            this.numericUpDown2.TabIndex = 8;
            this.numericUpDown2.TabStop = false;
            this.numericUpDown2.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "От";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(31, 21);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(154, 22);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.TabStop = false;
            this.numericUpDown1.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // start
            // 
            this.start.Font = new System.Drawing.Font("Segoe UI", 9.25F);
            this.start.Location = new System.Drawing.Point(6, 179);
            this.start.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(206, 31);
            this.start.TabIndex = 0;
            this.start.TabStop = false;
            this.start.Text = "ЗАПУСТИТЬ БОТА";
            this.toolTip1.SetToolTip(this.start, "Пока бот выполняет действия, остановить его невозможно.");
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.Start_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.OpenCaseCheckBox);
            this.groupBox2.Controls.Add(this.HideCheckBox);
            this.groupBox2.Controls.Add(this.AutoRunCheckBox);
            this.groupBox2.Controls.Add(this.TasksCheckBox);
            this.groupBox2.Controls.Add(this.GladeCheckBox);
            this.groupBox2.Controls.Add(this.ChestCheckBox);
            this.groupBox2.Controls.Add(this.TravelCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(6, 209);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.groupBox2.Size = new System.Drawing.Size(206, 148);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // OpenCaseCheckBox
            // 
            this.OpenCaseCheckBox.Checked = true;
            this.OpenCaseCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OpenCaseCheckBox.Location = new System.Drawing.Point(7, 85);
            this.OpenCaseCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.OpenCaseCheckBox.Name = "OpenCaseCheckBox";
            this.OpenCaseCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.OpenCaseCheckBox.Size = new System.Drawing.Size(192, 22);
            this.OpenCaseCheckBox.TabIndex = 9;
            this.OpenCaseCheckBox.TabStop = false;
            this.OpenCaseCheckBox.Text = "Открывать сундук";
            this.OpenCaseCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.OpenCaseCheckBox, "Бот будет открывать сундук, если на вашем аккаунте не активирован VIP аккаунт и в" +
        " шкафу лежит ключ.");
            this.OpenCaseCheckBox.UseVisualStyleBackColor = true;
            this.OpenCaseCheckBox.CheckedChanged += new System.EventHandler(this.OpenCaseCheckBox_CheckedChanged);
            // 
            // HideCheckBox
            // 
            this.HideCheckBox.Enabled = false;
            this.HideCheckBox.Location = new System.Drawing.Point(7, 121);
            this.HideCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.HideCheckBox.Name = "HideCheckBox";
            this.HideCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.HideCheckBox.Size = new System.Drawing.Size(192, 22);
            this.HideCheckBox.TabIndex = 8;
            this.HideCheckBox.TabStop = false;
            this.HideCheckBox.Text = "Запускать свернутым";
            this.HideCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.HideCheckBox, resources.GetString("HideCheckBox.ToolTip"));
            this.HideCheckBox.UseVisualStyleBackColor = true;
            this.HideCheckBox.CheckedChanged += new System.EventHandler(this.HideCheckBox_CheckedChanged);
            // 
            // AutoRunCheckBox
            // 
            this.AutoRunCheckBox.Location = new System.Drawing.Point(7, 103);
            this.AutoRunCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.AutoRunCheckBox.Name = "AutoRunCheckBox";
            this.AutoRunCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AutoRunCheckBox.Size = new System.Drawing.Size(192, 22);
            this.AutoRunCheckBox.TabIndex = 7;
            this.AutoRunCheckBox.TabStop = false;
            this.AutoRunCheckBox.Text = "Автозапуск и автостарт";
            this.AutoRunCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.AutoRunCheckBox, "Установив данную галочку бот будет загружаться вместе с Windows, а так же сразу с" +
        "тартовать.");
            this.AutoRunCheckBox.UseVisualStyleBackColor = true;
            this.AutoRunCheckBox.CheckedChanged += new System.EventHandler(this.AutoRunCheckBox_CheckedChanged);
            // 
            // TasksCheckBox
            // 
            this.TasksCheckBox.Checked = true;
            this.TasksCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TasksCheckBox.Location = new System.Drawing.Point(7, 67);
            this.TasksCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.TasksCheckBox.Name = "TasksCheckBox";
            this.TasksCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.TasksCheckBox.Size = new System.Drawing.Size(192, 22);
            this.TasksCheckBox.TabIndex = 6;
            this.TasksCheckBox.TabStop = false;
            this.TasksCheckBox.Text = "Забирать задания";
            this.TasksCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.TasksCheckBox, "Бот будет забирать все выполненные задания (включая медали).");
            this.TasksCheckBox.UseVisualStyleBackColor = true;
            this.TasksCheckBox.CheckedChanged += new System.EventHandler(this.TasksCheckBox_CheckedChanged);
            // 
            // GladeCheckBox
            // 
            this.GladeCheckBox.Checked = true;
            this.GladeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GladeCheckBox.Location = new System.Drawing.Point(7, 49);
            this.GladeCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.GladeCheckBox.Name = "GladeCheckBox";
            this.GladeCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.GladeCheckBox.Size = new System.Drawing.Size(192, 22);
            this.GladeCheckBox.TabIndex = 5;
            this.GladeCheckBox.TabStop = false;
            this.GladeCheckBox.Text = "Копать поляну";
            this.GladeCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.GladeCheckBox, "Бот будет копать поляну.");
            this.GladeCheckBox.UseVisualStyleBackColor = true;
            this.GladeCheckBox.CheckedChanged += new System.EventHandler(this.GladeCheckBox_CheckedChanged);
            // 
            // ChestCheckBox
            // 
            this.ChestCheckBox.Checked = true;
            this.ChestCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChestCheckBox.Location = new System.Drawing.Point(7, 31);
            this.ChestCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.ChestCheckBox.Name = "ChestCheckBox";
            this.ChestCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ChestCheckBox.Size = new System.Drawing.Size(192, 22);
            this.ChestCheckBox.TabIndex = 4;
            this.ChestCheckBox.TabStop = false;
            this.ChestCheckBox.Text = "Надевать и продавать вещи";
            this.ChestCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.ChestCheckBox, "Бот будет надевать вещи которые лучше и продавать ненужные.");
            this.ChestCheckBox.UseVisualStyleBackColor = true;
            this.ChestCheckBox.CheckedChanged += new System.EventHandler(this.ChestCheckBox_CheckedChanged);
            // 
            // TravelCheckBox
            // 
            this.TravelCheckBox.Checked = true;
            this.TravelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TravelCheckBox.Location = new System.Drawing.Point(7, 13);
            this.TravelCheckBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.TravelCheckBox.Name = "TravelCheckBox";
            this.TravelCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.TravelCheckBox.Size = new System.Drawing.Size(192, 22);
            this.TravelCheckBox.TabIndex = 3;
            this.TravelCheckBox.TabStop = false;
            this.TravelCheckBox.Text = "Отправлять на прогулку";
            this.TravelCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.TravelCheckBox, "Бот будет отправлять питомца на длительную прогулку.");
            this.TravelCheckBox.UseVisualStyleBackColor = true;
            this.TravelCheckBox.CheckedChanged += new System.EventHandler(this.TravelCheckBox_CheckedChanged);
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.SystemColors.Window;
            this.LogBox.Location = new System.Drawing.Point(216, 34);
            this.LogBox.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.Size = new System.Drawing.Size(382, 283);
            this.LogBox.TabIndex = 8;
            this.LogBox.TabStop = false;
            this.LogBox.Text = "";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "mpets.mobi.bot";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseClick);
            // 
            // button1
            // 
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.ImageIndex = 0;
            this.button1.Location = new System.Drawing.Point(216, 323);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(382, 34);
            this.button1.TabIndex = 8;
            this.button1.TabStop = false;
            this.button1.Text = "НАША ГРУППА ВКОНТАКТЕ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Информация";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HeartCurrent,
            this.BeautyCurrent,
            this.CoinCurrent});
            this.toolStrip1.Location = new System.Drawing.Point(216, 5);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(382, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // HeartCurrent
            // 
            this.HeartCurrent.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.HeartCurrent.Image = global::mpets.mobi.bot.Properties.Resources.heart;
            this.HeartCurrent.Margin = new System.Windows.Forms.Padding(0, 1, 6, 2);
            this.HeartCurrent.Name = "HeartCurrent";
            this.HeartCurrent.Size = new System.Drawing.Size(86, 22);
            this.HeartCurrent.Text = "Сердечек: 0";
            // 
            // BeautyCurrent
            // 
            this.BeautyCurrent.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.BeautyCurrent.Image = global::mpets.mobi.bot.Properties.Resources.beauty;
            this.BeautyCurrent.Name = "BeautyCurrent";
            this.BeautyCurrent.Size = new System.Drawing.Size(77, 22);
            this.BeautyCurrent.Text = "Красота: 0";
            // 
            // CoinCurrent
            // 
            this.CoinCurrent.Image = global::mpets.mobi.bot.Properties.Resources.coin;
            this.CoinCurrent.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.CoinCurrent.Name = "CoinCurrent";
            this.CoinCurrent.Size = new System.Drawing.Size(70, 22);
            this.CoinCurrent.Text = "Монет: 0";
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStrip2.CanOverflow = false;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BeautySession,
            this.HeartSession,
            this.CoinSession,
            this.ExpSession,
            this.BotsLogs});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip2.Location = new System.Drawing.Point(0, 365);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip2.Size = new System.Drawing.Size(603, 25);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // BeautySession
            // 
            this.BeautySession.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.BeautySession.Image = global::mpets.mobi.bot.Properties.Resources.beauty;
            this.BeautySession.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BeautySession.Name = "BeautySession";
            this.BeautySession.Size = new System.Drawing.Size(78, 22);
            this.BeautySession.Text = "0 собрано";
            // 
            // HeartSession
            // 
            this.HeartSession.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.HeartSession.Image = global::mpets.mobi.bot.Properties.Resources.heart;
            this.HeartSession.Name = "HeartSession";
            this.HeartSession.Size = new System.Drawing.Size(78, 22);
            this.HeartSession.Text = "0 собрано";
            // 
            // CoinSession
            // 
            this.CoinSession.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.CoinSession.Image = global::mpets.mobi.bot.Properties.Resources.coin;
            this.CoinSession.Name = "CoinSession";
            this.CoinSession.Size = new System.Drawing.Size(78, 22);
            this.CoinSession.Text = "0 собрано";
            // 
            // ExpSession
            // 
            this.ExpSession.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ExpSession.Image = global::mpets.mobi.bot.Properties.Resources.expirience;
            this.ExpSession.Name = "ExpSession";
            this.ExpSession.Size = new System.Drawing.Size(78, 22);
            this.ExpSession.Text = "0 собрано";
            // 
            // BotsLogs
            // 
            this.BotsLogs.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            this.BotsLogs.Name = "BotsLogs";
            this.BotsLogs.Size = new System.Drawing.Size(87, 22);
            this.BotsLogs.Text = "Запустите бота";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 390);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.start);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Удивительные питомцы";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox login;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox LogBox;
        private System.Windows.Forms.CheckBox AutoRunCheckBox;
        private System.Windows.Forms.CheckBox TasksCheckBox;
        private System.Windows.Forms.CheckBox GladeCheckBox;
        private System.Windows.Forms.CheckBox ChestCheckBox;
        private System.Windows.Forms.CheckBox TravelCheckBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox HideCheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripLabel HeartCurrent;
        private System.Windows.Forms.ToolStripLabel CoinCurrent;
        private System.Windows.Forms.ToolStripLabel BeautyCurrent;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel HeartSession;
        private System.Windows.Forms.ToolStripLabel CoinSession;
        private System.Windows.Forms.ToolStripLabel ExpSession;
        private System.Windows.Forms.ToolStripLabel BotsLogs;
        private System.Windows.Forms.ToolStripLabel BeautySession;
        private System.Windows.Forms.CheckBox OpenCaseCheckBox;
    }
}