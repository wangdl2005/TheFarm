namespace MyFarm
{
    partial class FrmLogin
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusIsLog = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnLoginFarm = new System.Windows.Forms.Button();
            this.btnLoginForum = new System.Windows.Forms.Button();
            this.btnLoginPasture = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(52, 110);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码：";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(139, 33);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(100, 21);
            this.txtUserName.TabIndex = 3;
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(139, 66);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(100, 21);
            this.txtPwd.TabIndex = 4;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(164, 110);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "退出";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusIsLog});
            this.statusStrip1.Location = new System.Drawing.Point(0, 184);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(302, 22);
            this.statusStrip1.TabIndex = 7;
            // 
            // statusIsLog
            // 
            this.statusIsLog.Name = "statusIsLog";
            this.statusIsLog.Size = new System.Drawing.Size(0, 17);
            // 
            // btnLoginFarm
            // 
            this.btnLoginFarm.Location = new System.Drawing.Point(16, 148);
            this.btnLoginFarm.Name = "btnLoginFarm";
            this.btnLoginFarm.Size = new System.Drawing.Size(75, 23);
            this.btnLoginFarm.TabIndex = 8;
            this.btnLoginFarm.Text = "进入农场";
            this.btnLoginFarm.UseVisualStyleBackColor = true;
            this.btnLoginFarm.Click += new System.EventHandler(this.btnLoginFarm_Click);
            // 
            // btnLoginForum
            // 
            this.btnLoginForum.Location = new System.Drawing.Point(202, 148);
            this.btnLoginForum.Name = "btnLoginForum";
            this.btnLoginForum.Size = new System.Drawing.Size(75, 23);
            this.btnLoginForum.TabIndex = 9;
            this.btnLoginForum.Text = "进入论坛";
            this.btnLoginForum.UseVisualStyleBackColor = true;
            this.btnLoginForum.Click += new System.EventHandler(this.btnLoginForum_Click);
            // 
            // btnLoginPasture
            // 
            this.btnLoginPasture.Location = new System.Drawing.Point(110, 148);
            this.btnLoginPasture.Name = "btnLoginPasture";
            this.btnLoginPasture.Size = new System.Drawing.Size(75, 23);
            this.btnLoginPasture.TabIndex = 10;
            this.btnLoginPasture.Text = "进入牧场";
            this.btnLoginPasture.UseVisualStyleBackColor = true;
            this.btnLoginPasture.Click += new System.EventHandler(this.btnLoginPasture_Click);
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 206);
            this.Controls.Add(this.btnLoginPasture);
            this.Controls.Add(this.btnLoginForum);
            this.Controls.Add(this.btnLoginFarm);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLogin);
            this.MaximizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "论坛登录";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusIsLog;
        private System.Windows.Forms.Button btnLoginFarm;
        private System.Windows.Forms.Button btnLoginForum;
        private System.Windows.Forms.Button btnLoginPasture;
    }
}

