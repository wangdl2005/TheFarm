using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyFarm
{
    public partial class FrmLogin : Form
    {
        string userName;//用户名
        string pwd;//密码
        string url;//发送的目标
        string postData;//post数据
        string isLogin = "现在将转入登录前页面。";//登陆成功的字符串；
        string isLogout = "你已退出站点，现在将以游客身份转入退出前页面。";//登陆退出的字符串；
        string isLoginFarm = "农场牧场";//农场登录成功的字符串；
        //新建一个用于保存cookies的容器     
        static CookieContainer container = new CookieContainer();//cookie全部保存在其中
        public static CookieContainer cookie
        {
            get 
            {
                return container;
            }
        }
        public FrmLogin()
        {
            InitializeComponent();
            btnLogin.Enabled = true;
            try { 
                //读取cookie
                IFormatter formatter = new BinaryFormatter(); 
                Stream stream = new FileStream("cookie.bin", FileMode.Open,FileAccess.Read, FileShare.Read); 
                container = (CookieContainer) formatter.Deserialize(stream); 
                stream.Close();
                statusIsLog.Text = "读取cookie成功";
                btnLogout.Enabled = true;
                btnLoginFarm.Enabled = true;
                btnLoginForum.Enabled = true;
            }
            catch(Exception)
            {
                statusIsLog.Text = "未保存cookie";
                btnLogout.Enabled = false;
                btnLoginFarm.Enabled = false;
                btnLoginForum.Enabled = false; ;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //拼接post数据
            userName = txtUserName.Text;
            pwd = txtPwd.Text;
            postData = "fastloginfield=username&username=" + userName 
                + "&cookietime=2592000&password=" + pwd
                + "&quickforward=yes&handlekey=ls&questionid=0&answer=";
            url = "http://www.szshbs.com/bbs/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&inajax=1";
            statusIsLog.Text = "正在登录中……";
            string content = MyFarm.Http.GetHtml(postData,out container, url);
            //将cookie序列化保存
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("cookie.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, container);
            stream.Close(); 

            if (content.Contains(isLogin))
            {
                statusIsLog.Text = userName + "登陆成功";
                btnLogin.Enabled = false;
                btnLogout.Enabled = true;
                btnLoginFarm.Enabled = true;
                btnLoginForum.Enabled = true;
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            url = "http://www.szshbs.com/bbs/member.php?mod=logging&action=logout&formhash=56d59574";
            statusIsLog.Text = "正在退出中……";
            string content = MyFarm.Http.GetHtml(container, url);
            if (content.Contains(isLogout))
            {
                statusIsLog.Text = userName + "退出成功";
                btnLogin.Enabled = true;
                btnLogout.Enabled = false;
                btnLoginFarm.Enabled = false;
                btnLoginForum.Enabled = false;
            }
        }

        private void btnLoginFarm_Click(object sender, EventArgs e)
        {
            url = "http://www.szshbs.com/bbs/plugin.php?id=qqfarm:qqfarm";
            statusIsLog.Text = "正在进入农场中……";
            string content = Http.GetHtml(container, url);
            if (content.Contains(isLoginFarm))
            {
                this.Hide();
                FrmFarm frm = new FrmFarm(container);
                frm.ShowDialog();
                this.Close();
                statusIsLog.Text = "登录农场成功";
            }
            else
            {
                statusIsLog.Text = "您可能今天未签到";
            }
        }

        private void btnLoginForum_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmForum frmForum = new FrmForum();
            frmForum.ShowDialog();
            this.Close();
        }
    }
}
