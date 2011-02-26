namespace QQWinFarm
{
    partial class FrmFarmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFarmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbFriendsMessage = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.upTime = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.lbInfo = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.chkDog = new System.Windows.Forms.CheckBox();
            this.chbCancel = new System.Windows.Forms.CheckBox();
            this.chbPlant = new System.Windows.Forms.CheckBox();
            this.chbScarify = new System.Windows.Forms.CheckBox();
            this.chbSpraying = new System.Windows.Forms.CheckBox();
            this.chbWater = new System.Windows.Forms.CheckBox();
            this.chbClearWeed = new System.Windows.Forms.CheckBox();
            this.chbSteal = new System.Windows.Forms.CheckBox();
            this.chbBag = new System.Windows.Forms.CheckBox();
            this.chbSeed = new System.Windows.Forms.CheckBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblExp = new System.Windows.Forms.Label();
            this.picHead = new System.Windows.Forms.PictureBox();
            this.lblMoney = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chbUpdata = new System.Windows.Forms.CheckBox();
            this.lbtnUpdata = new System.Windows.Forms.LinkLabel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuScript = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtFriends = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWorkTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRestTime = new System.Windows.Forms.TextBox();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.txtError = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.操作ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmResize = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNewsBog = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiError = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHead)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuScript.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMsg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 564);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(875, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslMsg
            // 
            this.tsslMsg.Name = "tsslMsg";
            this.tsslMsg.Size = new System.Drawing.Size(50, 17);
            this.tsslMsg.Text = "tssMsg";
            // 
            // lbFriendsMessage
            // 
            this.lbFriendsMessage.FormattingEnabled = true;
            this.lbFriendsMessage.HorizontalScrollbar = true;
            this.lbFriendsMessage.ItemHeight = 12;
            this.lbFriendsMessage.Location = new System.Drawing.Point(7, 179);
            this.lbFriendsMessage.Name = "lbFriendsMessage";
            this.lbFriendsMessage.Size = new System.Drawing.Size(276, 376);
            this.lbFriendsMessage.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "间隔时间(秒)：";
            // 
            // upTime
            // 
            this.upTime.Location = new System.Drawing.Point(101, 24);
            this.upTime.Name = "upTime";
            this.upTime.Size = new System.Drawing.Size(57, 21);
            this.upTime.TabIndex = 7;
            this.upTime.Text = "1";
            this.upTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.upTime_KeyPress);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // lbInfo
            // 
            this.lbInfo.FormattingEnabled = true;
            this.lbInfo.HorizontalScrollbar = true;
            this.lbInfo.ItemHeight = 12;
            this.lbInfo.Location = new System.Drawing.Point(4, 191);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(278, 328);
            this.lbInfo.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.chkDog);
            this.groupBox1.Controls.Add(this.chbCancel);
            this.groupBox1.Controls.Add(this.chbPlant);
            this.groupBox1.Controls.Add(this.chbScarify);
            this.groupBox1.Controls.Add(this.chbSpraying);
            this.groupBox1.Controls.Add(this.chbWater);
            this.groupBox1.Controls.Add(this.chbClearWeed);
            this.groupBox1.Controls.Add(this.chbSteal);
            this.groupBox1.Controls.Add(this.chbBag);
            this.groupBox1.Controls.Add(this.chbSeed);
            this.groupBox1.Location = new System.Drawing.Point(5, 149);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 372);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置自动";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 25;
            this.label12.Text = "摘取部分：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 222);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 23;
            this.label10.Text = "种植部分：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 111);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "经验部分：";
            // 
            // chkDog
            // 
            this.chkDog.AutoSize = true;
            this.chkDog.Checked = true;
            this.chkDog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDog.Location = new System.Drawing.Point(147, 62);
            this.chkDog.Name = "chkDog";
            this.chkDog.Size = new System.Drawing.Size(72, 16);
            this.chkDog.TabIndex = 21;
            this.chkDog.Text = "自动防狗";
            this.chkDog.UseVisualStyleBackColor = true;
            // 
            // chbCancel
            // 
            this.chbCancel.AutoSize = true;
            this.chbCancel.Checked = true;
            this.chbCancel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbCancel.Location = new System.Drawing.Point(30, 182);
            this.chbCancel.Name = "chbCancel";
            this.chbCancel.Size = new System.Drawing.Size(150, 16);
            this.chbCancel.TabIndex = 20;
            this.chbCancel.Text = "无经验自动取消 除草等";
            this.chbCancel.UseVisualStyleBackColor = true;
            // 
            // chbPlant
            // 
            this.chbPlant.AutoSize = true;
            this.chbPlant.Location = new System.Drawing.Point(147, 241);
            this.chbPlant.Name = "chbPlant";
            this.chbPlant.Size = new System.Drawing.Size(72, 16);
            this.chbPlant.TabIndex = 16;
            this.chbPlant.Text = "自动种植";
            this.chbPlant.UseVisualStyleBackColor = true;
            // 
            // chbScarify
            // 
            this.chbScarify.AutoSize = true;
            this.chbScarify.Location = new System.Drawing.Point(30, 241);
            this.chbScarify.Name = "chbScarify";
            this.chbScarify.Size = new System.Drawing.Size(72, 16);
            this.chbScarify.TabIndex = 15;
            this.chbScarify.Text = "自动翻地";
            this.chbScarify.UseVisualStyleBackColor = true;
            // 
            // chbSpraying
            // 
            this.chbSpraying.AutoSize = true;
            this.chbSpraying.Location = new System.Drawing.Point(147, 134);
            this.chbSpraying.Name = "chbSpraying";
            this.chbSpraying.Size = new System.Drawing.Size(72, 16);
            this.chbSpraying.TabIndex = 14;
            this.chbSpraying.Text = "自动杀虫";
            this.chbSpraying.UseVisualStyleBackColor = true;
            // 
            // chbWater
            // 
            this.chbWater.AutoSize = true;
            this.chbWater.Location = new System.Drawing.Point(30, 157);
            this.chbWater.Name = "chbWater";
            this.chbWater.Size = new System.Drawing.Size(72, 16);
            this.chbWater.TabIndex = 13;
            this.chbWater.Text = "自动浇水";
            this.chbWater.UseVisualStyleBackColor = true;
            // 
            // chbClearWeed
            // 
            this.chbClearWeed.AutoSize = true;
            this.chbClearWeed.Location = new System.Drawing.Point(30, 134);
            this.chbClearWeed.Name = "chbClearWeed";
            this.chbClearWeed.Size = new System.Drawing.Size(72, 16);
            this.chbClearWeed.TabIndex = 12;
            this.chbClearWeed.Text = "自动除草";
            this.chbClearWeed.UseVisualStyleBackColor = true;
            // 
            // chbSteal
            // 
            this.chbSteal.AutoSize = true;
            this.chbSteal.Checked = true;
            this.chbSteal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSteal.Location = new System.Drawing.Point(30, 62);
            this.chbSteal.Name = "chbSteal";
            this.chbSteal.Size = new System.Drawing.Size(72, 16);
            this.chbSteal.TabIndex = 11;
            this.chbSteal.Text = "自动摘取";
            this.chbSteal.UseVisualStyleBackColor = true;
            // 
            // chbBag
            // 
            this.chbBag.AutoSize = true;
            this.chbBag.Location = new System.Drawing.Point(147, 267);
            this.chbBag.Name = "chbBag";
            this.chbBag.Size = new System.Drawing.Size(72, 16);
            this.chbBag.TabIndex = 18;
            this.chbBag.Text = "查看背包";
            this.chbBag.UseVisualStyleBackColor = true;
            // 
            // chbSeed
            // 
            this.chbSeed.AutoSize = true;
            this.chbSeed.Location = new System.Drawing.Point(30, 268);
            this.chbSeed.Name = "chbSeed";
            this.chbSeed.Size = new System.Drawing.Size(72, 16);
            this.chbSeed.TabIndex = 17;
            this.chbSeed.Text = "购买种子";
            this.chbSeed.UseVisualStyleBackColor = true;
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(187, 87);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(17, 12);
            this.lblLevel.TabIndex = 14;
            this.lblLevel.Text = "..";
            // 
            // lblExp
            // 
            this.lblExp.AutoSize = true;
            this.lblExp.Location = new System.Drawing.Point(188, 65);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(17, 12);
            this.lblExp.TabIndex = 16;
            this.lblExp.Text = "..";
            // 
            // picHead
            // 
            this.picHead.ErrorImage = null;
            this.picHead.Location = new System.Drawing.Point(30, 20);
            this.picHead.Name = "picHead";
            this.picHead.Size = new System.Drawing.Size(100, 100);
            this.picHead.TabIndex = 12;
            this.picHead.TabStop = false;
            // 
            // lblMoney
            // 
            this.lblMoney.AutoSize = true;
            this.lblMoney.Location = new System.Drawing.Point(188, 46);
            this.lblMoney.Name = "lblMoney";
            this.lblMoney.Size = new System.Drawing.Size(17, 12);
            this.lblMoney.TabIndex = 15;
            this.lblMoney.Text = "..";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(188, 26);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(17, 12);
            this.lblUserName.TabIndex = 13;
            this.lblUserName.Text = "..";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.chbUpdata);
            this.groupBox2.Controls.Add(this.lbtnUpdata);
            this.groupBox2.Controls.Add(this.picHead);
            this.groupBox2.Controls.Add(this.lblLevel);
            this.groupBox2.Controls.Add(this.lblUserName);
            this.groupBox2.Controls.Add(this.lblExp);
            this.groupBox2.Controls.Add(this.lblMoney);
            this.groupBox2.Location = new System.Drawing.Point(7, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 141);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "用户信息";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(148, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 22;
            this.label7.Text = "等级：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(149, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 21;
            this.label6.Text = "经验：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "金币：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(148, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "网名：";
            // 
            // chbUpdata
            // 
            this.chbUpdata.AutoSize = true;
            this.chbUpdata.Location = new System.Drawing.Point(198, 107);
            this.chbUpdata.Name = "chbUpdata";
            this.chbUpdata.Size = new System.Drawing.Size(72, 16);
            this.chbUpdata.TabIndex = 18;
            this.chbUpdata.Text = "自动更新";
            this.chbUpdata.UseVisualStyleBackColor = true;
            // 
            // lbtnUpdata
            // 
            this.lbtnUpdata.AutoSize = true;
            this.lbtnUpdata.Location = new System.Drawing.Point(149, 108);
            this.lbtnUpdata.Name = "lbtnUpdata";
            this.lbtnUpdata.Size = new System.Drawing.Size(29, 12);
            this.lbtnUpdata.TabIndex = 17;
            this.lbtnUpdata.TabStop = true;
            this.lbtnUpdata.Text = "更新";
            this.lbtnUpdata.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbtnUpdata_LinkClicked);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.menuScript;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "QQ农夫";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // menuScript
            // 
            this.menuScript.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMenu,
            this.closeMenu});
            this.menuScript.Name = "contextMenuStrip1";
            this.menuScript.Size = new System.Drawing.Size(119, 48);
            // 
            // showMenu
            // 
            this.showMenu.Name = "showMenu";
            this.showMenu.Size = new System.Drawing.Size(118, 22);
            this.showMenu.Text = "可视界面";
            this.showMenu.Click += new System.EventHandler(this.showMenu_Click);
            // 
            // closeMenu
            // 
            this.closeMenu.Name = "closeMenu";
            this.closeMenu.Size = new System.Drawing.Size(118, 22);
            this.closeMenu.Text = "退出程序";
            this.closeMenu.Click += new System.EventHandler(this.closeMenu_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(184, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "暂停";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(206, 161);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "清空错误";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtFriends);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtWorkTime);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtRestTime);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.upTime);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Location = new System.Drawing.Point(5, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(276, 140);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "时间配置";
            // 
            // txtFriends
            // 
            this.txtFriends.Location = new System.Drawing.Point(101, 52);
            this.txtFriends.Name = "txtFriends";
            this.txtFriends.Size = new System.Drawing.Size(57, 21);
            this.txtFriends.TabIndex = 26;
            this.txtFriends.Text = "120";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "扫描好友时间：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 22;
            this.label3.Text = "工作时间(秒)：";
            // 
            // txtWorkTime
            // 
            this.txtWorkTime.Location = new System.Drawing.Point(101, 83);
            this.txtWorkTime.Name = "txtWorkTime";
            this.txtWorkTime.Size = new System.Drawing.Size(57, 21);
            this.txtWorkTime.TabIndex = 23;
            this.txtWorkTime.Text = "3600";
            this.txtWorkTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.upTime_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "休息时间(秒)：";
            // 
            // txtRestTime
            // 
            this.txtRestTime.Location = new System.Drawing.Point(101, 110);
            this.txtRestTime.Name = "txtRestTime";
            this.txtRestTime.Size = new System.Drawing.Size(57, 21);
            this.txtRestTime.TabIndex = 21;
            this.txtRestTime.Text = "900";
            this.txtRestTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.upTime_KeyPress);
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Interval = 1000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // txtError
            // 
            this.txtError.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtError.Location = new System.Drawing.Point(3, 10);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(278, 131);
            this.txtError.TabIndex = 21;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(291, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(286, 528);
            this.panel1.TabIndex = 22;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.txtError);
            this.panel2.Controls.Add(this.lbInfo);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Location = new System.Drawing.Point(582, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 528);
            this.panel2.TabIndex = 23;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 166);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 22;
            this.label13.Text = "错误信息：";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.操作ToolStripMenuItem,
            this.tsmiNewsBog,
            this.tmiError});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(875, 25);
            this.menuStrip1.TabIndex = 24;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 操作ToolStripMenuItem
            // 
            this.操作ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmResize,
            this.tmiClose});
            this.操作ToolStripMenuItem.Name = "操作ToolStripMenuItem";
            this.操作ToolStripMenuItem.Size = new System.Drawing.Size(72, 21);
            this.操作ToolStripMenuItem.Text = "   操 作   ";
            // 
            // tsmResize
            // 
            this.tsmResize.Name = "tsmResize";
            this.tsmResize.Size = new System.Drawing.Size(112, 22);
            this.tsmResize.Text = "最小化";
            this.tsmResize.Click += new System.EventHandler(this.tsmResize_Click);
            // 
            // tmiClose
            // 
            this.tmiClose.Name = "tmiClose";
            this.tmiClose.Size = new System.Drawing.Size(112, 22);
            this.tmiClose.Text = "退  出";
            this.tmiClose.Click += new System.EventHandler(this.tmiClose_Click);
            // 
            // tsmiNewsBog
            // 
            this.tsmiNewsBog.Name = "tsmiNewsBog";
            this.tsmiNewsBog.Size = new System.Drawing.Size(76, 21);
            this.tsmiNewsBog.Text = " 采摘结果 ";
            this.tsmiNewsBog.Click += new System.EventHandler(this.tsmiNewsBog_Click);
            // 
            // tmiError
            // 
            this.tmiError.Name = "tmiError";
            this.tmiError.Size = new System.Drawing.Size(76, 21);
            this.tmiError.Text = " 错误日志 ";
            this.tmiError.Click += new System.EventHandler(this.tmiError_Click);
            // 
            // FrmFarmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 586);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lbFriendsMessage);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FrmFarmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QQ农夫";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmFarmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmFarmMain_Load);
            this.Resize += new System.EventHandler(this.FrmFarmMain_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHead)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuScript.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslMsg;
        private System.Windows.Forms.ListBox lbFriendsMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox upTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ListBox lbInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chbSeed;
        private System.Windows.Forms.CheckBox chbPlant;
        private System.Windows.Forms.CheckBox chbScarify;
        private System.Windows.Forms.CheckBox chbSpraying;
        private System.Windows.Forms.CheckBox chbWater;
        private System.Windows.Forms.CheckBox chbClearWeed;
        private System.Windows.Forms.CheckBox chbSteal;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.PictureBox picHead;
        private System.Windows.Forms.Label lblMoney;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip menuScript;
        private System.Windows.Forms.ToolStripMenuItem closeMenu;
        private System.Windows.Forms.ToolStripMenuItem showMenu;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chbBag;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWorkTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRestTime;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.LinkLabel lbtnUpdata;
        private System.Windows.Forms.CheckBox chbUpdata;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chbCancel;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtFriends;
        private System.Windows.Forms.CheckBox chkDog;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 操作ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmResize;
        private System.Windows.Forms.ToolStripMenuItem tmiClose;
        private System.Windows.Forms.ToolStripMenuItem tmiError;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewsBog;
        private System.Windows.Forms.Label label13;
    }
}