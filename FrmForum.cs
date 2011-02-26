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
using MyFarm;
using System.Text.RegularExpressions;

namespace MyFarm
{
    public partial class FrmForum : Form
    {
        static int leftSec = 0;//任务可再申请剩余时间 
        string url;
        string formHash = "56d59574";
        bool isAutoApply = false;
        CookieContainer container = MyFarm.FrmLogin.cookie;
        public FrmForum()
        {
            InitializeComponent();
        }

        private void FrmForum_Load(object sender, EventArgs e)
        {
            refreshInfo();
            comboBox1.SelectedIndex = 6;//默认VIP级别
            timerEveryday.Enabled = false;//默认不计时
            timerEveryday.Interval = 500;//500ms
            //getFormHash();
        }

        private void getFormHash()
        { 
            string url = "http://www.szshbs.com/bbs/forum.php";
            string content = Http.GetHtml(container, url);
            string toMatch = "input type=\"hidden\" name=\"formhash\" value=\"[\\w\\d]{8}\" ";
            string toMatch2 = @"[\w\d]{8}";
            Regex regex = new Regex(toMatch);
            Regex regex2 = new Regex(toMatch2);
            if(regex.IsMatch(content))
            {
                string thatMatch = regex.Match(content).ToString();
                formHash = regex2.Matches(thatMatch)[1].ToString();
                statusLabel.Text = "formHash=" + formHash;
            }
        }

        private void refreshInfo()
        {
            string url = "http://www.szshbs.com/bbs/home.php?mod=task&item=done";
            string content = Http.GetHtml(container, url);
            if (content.Contains("添加好友任务"))
            {
                int hour, min, sec;
                hour = 0;
                min = 0;
                sec = 0;
                string toMatch = @"(\d+小时)?(\d+分钟)?(\d+秒)? 后可以再次申请";
                string toMatch1 = @"(\d+小时)(\d+分钟) 后可以再次申请";//2小时2分钟
                string toMatch2 = @"(\d+分钟)(\d+秒) 后可以再次申请";//2分钟12秒
                string toMatch3 = @"(\d+秒) 后可以再次申请";//12秒
                string toMatchNum = @"(\d+)";//用于获取字符串中的数字
                Regex regex = new Regex(toMatch);
                Regex regex1 = new Regex(toMatch1);
                Regex regex2 = new Regex(toMatch2);
                Regex regex3 = new Regex(toMatch3);
                Regex regex4 = new Regex(toMatchNum);
                if (regex.IsMatch(content))
                {
                    String thatMatch = regex.Match(content).ToString();
                    if (regex1.IsMatch(content))
                    {
                        hour = Convert.ToInt32(regex4.Matches(thatMatch)[0].ToString());
                        min = Convert.ToInt32(regex4.Matches(thatMatch)[1].ToString());
                        sec = 0;
                    }
                    else if (regex2.IsMatch(content))
                    {
                        min = Convert.ToInt32(regex4.Matches(thatMatch)[0].ToString());
                        sec = Convert.ToInt32(regex4.Matches(thatMatch)[1].ToString());
                        hour = 0;
                    }
                    else if (regex3.IsMatch(content))
                    {
                        hour = 0;
                        min = 0;
                        sec = Convert.ToInt32(regex4.Matches(thatMatch)[0].ToString());
                    }
                    leftSec = hour * 3600 + min * 60 + sec;
                    timerTaskFriend.Enabled = true;
                    labelTimeLeft.Text = "剩余" + leftSec.ToString() + "s后可以再次申请";
                    btnApply.Text = "请等待申请";
                }
            }
            else
            {
                leftSec = 0;
                timerTaskFriend.Enabled = false;
                btnApply.Text = "立即申请";
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            url = "http://www.szshbs.com/bbs/home.php?mod=task&do=apply&id=21";
            string content = Http.GetHtml(container, url);
            if (content.Contains("任务申请成功"))
            {
                statusLabel.Text = "任务申请成功";
            }
            else
            {
                statusLabel.Text = "任务申请失败";
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            url = "http://www.szshbs.com/bbs/home.php?mod=task&do=draw&id=21";
            string content = Http.GetHtml(container, url);
            if (content.Contains("任务已成功完成"))
            {
                statusLabel.Text = "任务已成功完成";
            }
            else 
            {
                statusLabel.Text = "任务未能完成";
            }
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            url = "http://www.szshbs.com/bbs/plugin.php?id=dsu_paulsign:sign&operation=qiandao&infloat=1&inajax=1";
            //formHash = "29a87a55";
            string postData = "formhash=" + formHash + "&qdxq=wl&qdmode=2&todaysay=&fastreply=1";
            //56d59574   wangdl2005账号Hash
            //29a87a55   kk12345   账号Hash
            string content = Http.GetHtml(postData, container, url);
            if (content.Contains("签到成功"))
            {
                statusLabel.Text = "签到成功";
            }
            else
            {
                statusLabel.Text = "签到失败";
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            int packetid = 9;
            if (comboBox1.SelectedIndex == 5)
            { }
            else
            {
                packetid = comboBox1.SelectedIndex + 3;
            }
            string postData = "packetid=" + packetid.ToString();//新人红包：3;vip:9;尉级：4;中校级：5;大校级:6；将军级:7
            url = "http://www.szshbs.com/bbs/plugin.php?id=luckypacket:luckypacket&module=get&action=get&getsubmit=yes";
            string content = Http.GetHtml(postData, container, url);
            if (content.Contains("红包领取成功"))
            {
                statusLabel.Text = "红包领取成功";
            }
            else
            {
                statusLabel.Text = "红包领取失败";
            }
        }

        private void timerEveryday_Tick(object sender, EventArgs e)
        {
            btnSign_Click(sender, e);
            btnGet_Click(sender, e);
        }

        private void btnAutoEveryday_Click(object sender, EventArgs e)
        {
            if (btnAutoEveryday.Text.Equals("自动每日"))
            {
                timerEveryday.Enabled = true;
                btnAutoEveryday.Text = "停止自动";
            }
            else if (btnAutoEveryday.Text.Equals("停止自动"))
            {
                timerEveryday.Enabled = false;
                btnAutoEveryday.Text = "自动每日";
            }
        }

        private void timerTaskFriend_Tick(object sender, EventArgs e)
        {
            if (leftSec > 0)
            {
                leftSec--;
                labelTimeLeft.Text = "剩余" + leftSec.ToString() + "s后可以再次申请";
                btnApply.Text = "请等待申请";
            }
            else
            {
                labelTimeLeft.Text = "现在可以再次申请";
                btnApply.Text = "立即申请";
                if (isAutoApply&&leftSec==0)
                {
                    btnApply_Click(sender, e);
                    System.Threading.Thread.Sleep(2000);
                    btnComplete_Click(sender, e);
                    System.Threading.Thread.Sleep(2000);
                    refreshInfo();
                }
                
            }
        }

        private void btnAutoApply_Click(object sender, EventArgs e)
        {
            if (isAutoApply = !isAutoApply)
            {
                btnAutoApply.Text = "停止自动";
            }
            else
            {
                btnAutoApply.Text = "自动申请";
            }
        }

    }
}
