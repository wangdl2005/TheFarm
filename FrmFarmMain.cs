using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.IO;
using System.Collections;
using Json;

namespace QQWinFarm
{
    public partial class FrmFarmMain : Form
    {
        #region 变量定义
        NewsBog newsbog = new NewsBog();

        private System.Net.CookieContainer cookie = new System.Net.CookieContainer();
        JsonObject _status_filter = new JsonObject("{}");//可操作好友列表
        JsonObject model; //json模板
        JsonObject _status; //用户农场信息
        JsonObject _qzonefriends; //QQ空间 好友列表
        string _qzonefriendsIds = ""; //QQ空间 好友 ID集合        
        JsonObject _shop; //商店
        Thread thread1, thread2; //进程 1  可操作好友查询  2 操作好友
        string _uid = "0"; //user id
        bool _autoweed = true; //自动除草 
        bool _autonorm = true; //自动杀虫 
        bool _autowater = true; //自动浇水 
        bool _autoplant = true; //自动种植
        bool _autosteal = true; //自动收获
        bool _autoscarify = true; //自动翻地
        bool _autoseed = true; //自动购买种子
        bool _autobag = true; //查看背包
        bool _autodog = true;

        int _userInfoTime = 0;//用户信息刷新执行时间
        int _userInfoUpTime = 60 * 5;//用户信息刷新时间

        bool _autoWookBool = true; //是否工作
        int _aotuWookTime = 0;//工作执行时间
        int updateTime = 0;  //更新时间
        string _autourl = "";//默认获取地址
        int setUpTime = 0; //执行的时间
        string runUrl = ""; //操作好友时 运行的url
        int runFriends = 0;
        #endregion

        #region 构造方法
        public FrmFarmMain()
        {
            InitializeComponent();
        }
        public FrmFarmMain(System.Net.CookieContainer cookie)
        {
            InitializeComponent();
            this.cookie = cookie;
        }
        #endregion

        #region 页面打开和关闭
        private void FrmFarmMain_Load(object sender, EventArgs e)
        {
            runFriends = Convert.ToInt32(txtFriends.Text.Trim());
            updateTime = Convert.ToInt32(upTime.Text.Trim());
            setUpTime = updateTime;
            PanelError();
            thread1 = new Thread(new ThreadStart(this.BindUserInfo));
            thread1.Start();
        }

        private void FrmFarmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            System.Environment.Exit(System.Environment.ExitCode);
            #region 说明
            /*
            System.Environment.Exit(System.Environment.ExitCode);  
            this.Dispose();
            this.Close();
            还有一种方法：  System.Threading.Thread.CurrentThread.Abort();
            或者          Process.GetCurrentProcess().Kill() 
            或者        Application.ExitThread();
            或者        Application.ExitThread() 
            Application.Exit(); 方法停止在所有线程上运行的所有消息循环，并关闭应用程序的所有窗口
            Application.Exit 是一种强行退出方式，就像 Win32 的 PostQuitMessage()。它意味着放弃所有消息泵，展开调用堆栈，并将执行返回给系统。
             */

            #endregion
        }
        #endregion


        #region 扫描好友
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (txtFriends.Text.Trim() == "")
            {
                txtFriends.Text = "0";
            }
            //runFriends = Convert.ToInt32(txtFriends.Text.Trim());
            if (thread2 == null)
            {
                thread2 = new Thread(new ThreadStart(this.AutoRun));
            }
            if (thread1.IsAlive || thread2.IsAlive)
            {
                return;
            }
            if (_status_filter.Count >0)
            {
                return;
            }
            if (runFriends >= Convert.ToInt32(txtFriends.Text.Trim()))
            {
                //ChangeTSSL("");
                runFriends = 0;

                thread1 = new Thread(new ThreadStart(this.statusFilter));
                thread1.Start();
            }
            else
            {
                runFriends++;
                int shengyu = (Convert.ToInt32(txtFriends.Text.Trim()) - runFriends) + 1;
                ChangeTSSL("距离下次执行时间还有：" + shengyu + "秒");
            }
        }


        #endregion

        #region timer2 执行好友
        private void timer2_Tick(object sender, EventArgs e)
        {
           
            if(upTime.Text.Trim()=="")
            {
                upTime.Text="0";
            }
            updateTime = Convert.ToInt32(upTime.Text.Trim());
            if (thread2 == null)
            {
                thread2 = new Thread(new ThreadStart(this.AutoRun));
            }
            if (thread1.IsAlive || thread2.IsAlive)
            {
                return;
            }
            if (_status_filter.Count <1)
            {
                return;
            }
            if (setUpTime >= updateTime)
            {
                //ChangeTSSL("");
                setUpTime = 0;
                thread2 = new Thread(new ThreadStart(this.AutoRun));
                thread2.Start();
            }
            else
            {
                setUpTime++;
                int shengyu = (updateTime - setUpTime) + 1;
                ChangeTSSL("距离下次执行时间还有：" + shengyu + "秒");
            }
        } 
        #endregion

        #region 格式化等级经验显示
        /// <summary> 
        /// 格式化等级经验显示 
        /// </summary> 
        /// <param name="exp"></param> 
        /// <param name="lv"></param> 
        /// <returns></returns> 
        private string FormatExp(int exp, out int lv)
        {
            int level = 0;
            while (exp > (level * 200) + 200)
            {
                exp -= (level * 200) + 200;
                level++;
            }
            lv = level;
            return exp + "/" + ((level * 200) + 200);
        }  
        #endregion

        #region 绑定用户信息
        /// <summary> 
        /// 绑定用户信息 
        /// </summary> 
        private void BindUserInfo()
        {
            ChangeTSSL("正在获取农场信息...");
            if (_uid == "0")
                GetUsrInfo();
            if (_shop == null)
                this._shop = new JsonObject(ScanShop());
            if (_qzonefriends == null || _xiaoyoufriends == null)
                ListFriends();
            ChangeTSSL("成功获取农场信息!");

        }
        #endregion

        #region 运行农场操作
        private void AutoRun()
        {
            _autosteal = chbSteal.Checked;//摘取
            _autoweed = chbClearWeed.Checked;//除草
            _autowater = chbWater.Checked;//浇水
            _autonorm = chbSpraying.Checked;//杀虫 
            _autoscarify = chbScarify.Checked;//铲除
            _autoplant = chbPlant.Checked;//种植
            _autoseed = chbSeed.Checked;//购买
            _autobag = chbBag.Checked;//背包
            _autodog = chkDog.Checked;
            if (_status_filter.Count < 1)
                thread2.Abort();

            JsonObject _statusModel = _status_filter.GetJson(0);
            JsonObject _statusModel_1 = _statusModel.GetJson(0);
            if (_statusModel_1 == null)
                thread2.Abort();
            _status_filter.Remove(_statusModel);
           

            /*
            0：1 干旱
            1 > 0有手
            2：1有草
            3：1有虫
             */
            #region 局部变量
            string frienduid = _statusModel_1.Key;
            JsonObject _friends = _xiaoyoufriends;
            string url = _statusModel.GetValue("url");
            string frienduName = "";
            string urlTitle = "";
            string _autostealstr = "";//摘取
            string _autoweedstr = "";//除草
            string _autowaterstr = "";//浇水
            string _autonormstr = "";//杀虫 
            string _autoscarifystr = "";//铲除
            string _autoplantstr = "";//种植

            switch (url)
            {
                case "xiaoyou":
                    _friends = _xiaoyoufriends;
                    urlTitle = "校友";
                    runUrl = "http://nc.xiaoyou.qq.com";
                    break;
                case "qzone":
                    _friends = _qzonefriends;
                    urlTitle = "QQ空间";
                    runUrl = "http://nc.qzone.qq.com";
                    break;
            }

            #region 查找好友名称

            string _msg = "";
            if (frienduid != _uid)
            {
                for (int i = 0; i < _friends.GetCollection().Count; i++)
                {
                    if (_friends.GetCollection()[i].GetValue("userId") == frienduid)
                    {
                        frienduName = Utils.ConvertUnicodeStringToChinese(_friends.GetCollection()[i].GetValue("userName"));

                        _msg = "查看 " + urlTitle + " 好友【" + frienduName + "】";
                    }
                }
            }
            else
            {
                _msg = "查看 自己 农场";
            }
            ChangeLBFM(_msg);
            ChangeTSSL(_msg);
            #endregion


            #endregion
            _statusModel_1 = new JsonObject(_statusModel_1[frienduid]);
            #region 判断是否需要操作
            if (!frienduid.Contains(_uid))
            {
                bool _autorun = false;
                string _autostr = "";
                if (_autowater)
                {
                    _autostr = _statusModel_1.GetValue("0");
                    if (_autostr != null && Convert.ToInt32(_autostr) > 0)
                        _autorun = true;
                }
                if (_autosteal)
                {
                   
                    _autostr = _statusModel_1.GetValue("1");
                    if (_autostr != null && Convert.ToInt64(_autostr)>0)
                    {
                        _autorun = true;
                        //double times = 0;
                        //double durtime = Convert.ToInt64(times + Convert.ToInt64(_autostr));
                        //if (durtime < now)//成熟 
                        //    _autorun = true;
                    }
                }
                if (_autoweed)
                {
                    _autostr = _statusModel_1.GetValue("2");
                    if (_autostr != null && Convert.ToInt32(_autostr) > 0)
                        _autorun = true;
                }
                if (_autonorm)
                {
                    _autostr = _statusModel_1.GetValue("3");
                    if (_autostr != null && Convert.ToInt32(_autostr) > 0)
                        _autorun = true;
                }
                if (!_autorun)
                {
                    thread2.Abort();
                }
            } 
            #endregion

          
            
            //加载土地信息
            string result = ShowFriend(frienduid, runUrl);
            JsonObject lands = new JsonObject(result);

          
            double nowtime = (DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds;
            double servertime = nowtime;
            if (lands.GetJson("user") != null && lands.GetJson("user").GetJson("healthMode") != null)
            {
                if (lands.GetJson("user").GetJson("healthMode").GetValue("serverTime") != null)
                {
                    servertime = Convert.ToInt32(lands.GetJson("user").GetJson("healthMode").GetValue("serverTime"));
                }
            }
            FarmKey.NetworkDelay = nowtime - servertime;
            if (lands.GetValue("pf") != null && lands.GetValue("pf") != "1")
            {
                if (6 <= DateTime.Now.Hour && DateTime.Now.Hour >= 0)
                {
                    ChangeLBFM("保护模式");
                }
            }
            #region 循环 地
            for (int j = 0; j < lands.GetCollection("farmlandStatus").GetCollection().Count; j++)
            {
                //bool s = false;
                //double times = 0;
                /*//if (lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("j").Equals("0")) 
                //{ 
                for (int x = 0; x < _shop.GetCollection().Count; x++)
                {
                    if (_shop.GetCollection()[x].GetValue("cId").Equals(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("a")))//不是空地 
                    {
                        ChangeLBFM("第" + (j + 1) + "块地种植了【" + Utils.ConvertUnicodeStringToChinese(_shop.GetCollection()[x].GetValue("cName")) + "】");
                        s = true;
                        times = Convert.ToInt64(_shop.GetCollection()[x].GetValue("growthCycle"));
                        break;
                    }
                }
                //} */
                #region 是否有庄稼
                if (Convert.ToInt32(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("a")) > 0)
                {

                    //double durtime = Convert.ToInt64(times + Convert.ToInt64(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("b")));
                    //if (durtime < now)//成熟 
                    string farmlandStatus_b = lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("b");

                    #region 可摘取判断
                    if (farmlandStatus_b == "6")
                    {
                        //ChangeLBFM("第" + (j + 1) + "块地成熟");
                        int farmlandStatus_m = Convert.ToInt32(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("m"));//剩余数
                        int farmlandStatus_l = Convert.ToInt32(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("l"));//可摘取数
                        JsonObject farmlandStatus_n = lands.GetCollection("farmlandStatus").GetCollection()[j].GetJson("n");//摘取记录
                        string ModelRawjson = "";
                        if (farmlandStatus_n != null)
                        {
                            ModelRawjson = farmlandStatus_n.ToString();
                        }
                        if (frienduid.Contains(_uid)||(farmlandStatus_m > farmlandStatus_l && (farmlandStatus_n == null || !ModelRawjson.Contains(_uid))))//没有偷过 
                        {
                            if (_autosteal)
                            {
                                ChangeLBFM("第 " + (j+1) + " 块可以采摘，开始摘取...");
                                bool _rundog = false;
                                #region 查看是否有狗
                                if (lands.GetJson("dog") != null)
                                {
                                    if (lands.GetJson("dog").GetValue("isHungry") == "0")
                                    {
                                        if (_autodog)
                                        {
                                            _rundog = true;
                                            ChangeLBFM("此好友 有狗...");
                                        }
                                        else
                                        {
                                            ChangeLBFM("未开启-不摘有狗");
                                        }
                                    }
                                }
                                #endregion
                                if (frienduid.Contains(_uid))
                                    Harvest(_uid, j.ToString());
                                else
                                {
                                    if (!_rundog)
                                        Steal(frienduid, j.ToString());
                                }

                                //ChangeLBFM("成功摘取...");
                            }
                            if (_autostealstr != "") _autostealstr += ",";
                            _autostealstr += j.ToString();
                        }
                        //else
                        //{
                        //    ChangeLBFM("已经摘取或到最低限额，不可摘取");
                        //}
                    }
                    //else
                    //{
                    //    ChangeLBFM("未成熟，不可摘取");
                    //} 
                    #endregion

                    #region 判断庄稼枯萎
                    //string farmlandStatus_j = lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("j");
                    if (farmlandStatus_b == "7")//lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("j").Equals("1")||
                    {
                        if (_autoscarifystr != "") _autoscarifystr += ",";
                        _autoscarifystr += j.ToString();
                    } 
                    #endregion

                    #region 是否有草判断 //除草 f代表有几棵草 0为没有 k为产量，为0时即尚未成熟 成熟的植物不长草的
                    if (!lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("f").Equals("0") && lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("k").Equals("0"))
                    {
                        ChangeLBFM("第 " + (j + 1) + " 块地有草，开始除草...");
                        for (int x = 0; x < Convert.ToInt32(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("f")); x++)
                        {
                            if (_autoweed || frienduid.Contains(_uid))
                            {
                                //ChangeLBFM("第 " + _autoweedstr + " 块地有草，开始除草...");
                                ClearWeed(frienduid, j.ToString() );
                                // ChangeLBFM("第 " + _autoweedstr + " 块地 除草成功");
                            }
                            if (_autoweedstr != "") _autoweedstr += ",";
                            _autoweedstr += j.ToString();
                            //ClearWeed(frienduid, j.ToString());
                        }
                        //ChangeLBFM("第 " + (j + 1) + " 块地 除草成功");
                    }
                    #endregion

                    #region 是否需浇水
                    if (!lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("h").Equals("1"))//浇水 
                    {
                        ChangeLBFM("第 " + (j + 1) + " 块地干燥，开始浇水...");
                        if (_autowater || frienduid.Contains(_uid))
                        {
                            
                           // ChangeLBFM("第 " + _autowaterstr + " 块地干燥，开始浇水...");
                            Water(frienduid, j.ToString());
                            
                        }
                        if (_autowaterstr != "") _autowaterstr += ",";
                        _autowaterstr += j.ToString();
                        
                        //Water(frienduid, j.ToString());
                        //ChangeLBFM("第 " + (j + 1) + " 块地 浇水成功");
                    }
                    #endregion

                    #region 是否需杀虫
                    if (!lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("g").Equals("0") && lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("k").Equals("0"))//杀虫 g代表有几条虫 0为没有 k为产量，为0时即尚未成熟 成熟的植物不长虫的 
                    {
                        ChangeLBFM("第 " + (j + 1) + " 块地有虫，开始杀虫...");
                        for (int x = 0; x < Convert.ToInt32(lands.GetCollection("farmlandStatus").GetCollection()[j].GetValue("g")); x++)
                        {
                            if (_autonorm || frienduid.Contains(_uid))
                            {

                                //ChangeLBFM("第 " + _autonormstr + " 块地有虫，开始杀虫...");
                                Spraying(frienduid, j.ToString());
                                //ChangeLBFM("第 " + _autonormstr + " 块地 杀虫成功");
                            }
                            if (_autonormstr != "") _autonormstr += ",";
                            _autonormstr += j.ToString();
                            // Spraying(frienduid, j.ToString());
                        }
                        //ChangeLBFM("第 " + (j + 1) + " 块地 杀虫成功");
                    }
                    #endregion


                }
                else
                {
                    if (frienduid.Contains(_uid))
                    {
                        if (_autoplantstr != "") _autoplantstr += ",";
                        _autoplantstr += j.ToString();
                    }
                }
                #endregion
            }
            #endregion

            #region 摘取
            if (_autostealstr != "")
            {
                if (_autosteal || frienduid.Contains(_uid))
                {
                    /*
                    ChangeLBFM("第 " + _autostealstr + " 块可以采摘，开始摘取...");
                    bool _rundog = false;
                    #region 查看是否有狗
                    if (lands.GetJson("dog") != null)
                    {
                        if (lands.GetJson("dog").GetValue("isHungry") == "0")
                        {
                            if (_autodog)
                            {
                                _rundog = true;
                                ChangeLBFM("此好友 有狗...");
                            }
                            else
                            {
                                ChangeLBFM("未开启-不摘有狗");
                            }
                        }
                    }
                    #endregion
                    if (frienduid.Contains(_uid))
                        Harvest(_uid, _autostealstr);
                    else
                    {
                        if (!_rundog)
                            Steal(frienduid, _autostealstr);
                    }
                    */
                    //ChangeLBFM("成功摘取...");
                }
                else
                {
                    ChangeLBFM("未开启自动摘取");
                }
            }
            #endregion

            #region 除草
            if (_autoweedstr != "" )
            {
                if (_autoweed || frienduid.Contains(_uid))
                {
                    //ChangeLBFM("第 " + _autoweedstr + " 块地有草，开始除草...");
                    //ClearWeed(frienduid, _autoweedstr);
                    // ChangeLBFM("第 " + _autoweedstr + " 块地 除草成功");
                }
                else
                {
                    ChangeLBFM("未开启自动除草");
                }
            }
            #endregion

            #region 浇水
            if (_autowaterstr != "" )
            {
                if (_autowater || frienduid.Contains(_uid))
                {
                    /*
                    ChangeLBFM("第 " + _autowaterstr + " 块地干燥，开始浇水...");
                    Water(frienduid, _autowaterstr);
                    */
                }
                else
                {
                    ChangeLBFM("未开启自动浇水");
                }
            }
            #endregion

            #region 杀虫
            if (_autonormstr != "" )
            {
                if (_autonorm || frienduid.Contains(_uid))
                {

                    ChangeLBFM("第 " + _autonormstr + " 块地有虫，开始杀虫...");
                    Spraying(frienduid, _autonormstr);
                    //ChangeLBFM("第 " + _autonormstr + " 块地 杀虫成功");
                }
                else
                {
                    ChangeLBFM("未开启自动杀虫");
                }
            }
            #endregion

            #region 铲除
            if (_autoscarifystr != "")
            {
                if (frienduid.Contains(_uid))
                {
                    if (_autoscarify)
                    {

                        ChangeLBFM("第 " + _autoscarifystr + " 块地枯萎，开始翻地...");
                        Scarify(frienduid, _autoscarifystr);
                        _autoplantstr = _autoscarifystr;
                        //ChangeLBFM("第 " + _autonormstr + " 块地 杀虫成功");
                    }
                    else
                    {
                        ChangeLBFM("未开启自动翻地");
                    }
                }
            }
            #endregion

            #region 种植
            if (_autoplantstr != "")
            {
                if (_autoplant)
                {
                    if (frienduid.Contains(_uid))
                    {
                        ChangeLBFM("第 " + _autoplantstr + " 块地是空地");
                        string[] plantIds = _autoplantstr.Split(',');
                        int plantIdsIndex = 0;
                        int number = 0;
                        string key = "";
                        string value = "";
                        Hashtable plantht = new Hashtable();
                        if (_autobag)
                        {
                            JsonObject bag = new JsonObject(GetBag());
                            ChangeLBFM("开始查看背包...");
                            for (int y = 0; y < bag.GetCollection().Count; y++)
                            {
                                if (plantIdsIndex >= plantIds.Length) break;
                                if (bag.GetCollection()[y].GetValue("type").Equals("1"))
                                {
                                    key = bag.GetCollection()[y].GetValue("cId");
                                    value = "";
                                    if (plantht.ContainsKey(key))
                                    {
                                        value = plantht[key].ToString();
                                        plantht.Remove(key);
                                    }
                                    for (int ount = 0; ount < Convert.ToInt32(bag.GetCollection()[y].GetValue("amount")); ount++)
                                    {
                                        if (plantIdsIndex >= plantIds.Length) break;
                                        if (value != "") value += ",";//amount
                                        value += plantIds[plantIdsIndex];
                                        plantIds[plantIdsIndex] = null;
                                        plantIdsIndex++;
                                    }
                                    plantht.Add(key, value);
                                }
                            }
                        }
                        else
                        {
                            ChangeLBFM("未开启查看背包");
                        }
                        key = "Seed";
                        for (int n = 0; n < plantIds.Length; n++)
                        {
                            if (plantIds[n] != null)
                            {
                                value = "";
                                if (plantht.ContainsKey(key))
                                {
                                    value = plantht[key].ToString();
                                    plantht.Remove(key);
                                }
                                if (value != "") value += ",";
                                value += plantIds[n];
                                plantht.Add(key, value);
                                number++;
                            }
                        }
                        if (plantht.ContainsKey(key))
                        {
                            value = plantht[key].ToString();
                            plantht.Remove(key);
                            if (_autoplant)
                            {
                                key = GetShopLv();
                                if (key != null)
                                {
                                    ChangeLBFM("开始购买 " + Utils.NoHTML(Utils.ConvertUnicodeStringToChinese(GetShopModel(key).GetValue("cName"))) + " " + number + "个");
                                    Seed(_uid, key, number);
                                    plantht.Add(key, value);
                                }
                                else
                                {
                                    ChangeLBFM("没有同一等级的种子");
                                }
                            }
                            else
                            {
                                ChangeLBFM("未开启自动购买");
                            }
                        }

                        foreach (DictionaryEntry plant in plantht)
                        {
                            ChangeLBFM("开始种地...");
                            Plant(plant.Key.ToString(), _uid, plant.Value.ToString());
                        }
                    }
                }
                else
                {
                    ChangeLBFM("未开启自动种植");
                } 
            #endregion
            }
            thread2.Abort();
        }
        #endregion
        #region 查看自己购买的物品 
        /// <summary> 
        /// 查看自己购买的物品 
        /// </summary> 
        /// <param name="uid"></param> 
        /// <param name="cid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string GetBag()
        {
            string farmtime = GetFarmTime();
            string url = "http://happyfarm.xiaoyou.qq.com/api.php?mod=repertory&act=getUserSeed";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&farmTime=" + farmtime;
            string result =GetHtml(url, post, true, cookie);
            return result;
        }
        #endregion

        #region  购买种子
        /// <summary>
        /// 购买种子
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="cid"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private string Seed(string uid, string cid, int number)
        {
            string farmtime = GetFarmTime();
            string url = "http://happyfarm.xiaoyou.qq.com//api.php?mod=repertory&act=buySeed";
            string post = "cId=" + cid + "&uIdx=" + uid + "&number=" + number.ToString() + "&farmKey=" + GetFarmKey(farmtime) + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        } 
        #endregion

        #region 农场操作方法
        /// <summary> 
        /// 铲除
        /// </summary> 
        /// <param name="uid"></param> farmKey=da2c72dcbc442f97827c9f7eb8d89001&place=4&uIdx=361157088&cropStatus=7&farmTime=1260006806&ownerId=361157088
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Scarify(string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_plant?mod=farmlandstatus&act=scarify";
            string post = "uIdx="+uid+"&cropStatus=7&farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + _uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }

         /// <summary> 
        /// 摘取自己菜
        /// </summary> 
        /// <param name="uid"></param> farmKey=9b9f35a6fdc8f0a125051460ec44d29e&uIdx=361157088&farmTime=1259966227&place=6&ownerId=361157088
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Harvest(string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_plant?mod=farmlandstatus&act=harvest";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + _uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }

        /// <summary> 
        /// 偷好友菜 
        /// </summary> 
        /// <param name="uid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Steal(string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_steal?mod=farmlandstatus&act=scrounge";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }
        /// <summary> 
        /// 除草 
        /// </summary> 
        /// <param name="qq"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> /cgi-bin/cgi_farm_steal?mod=farmlandstatus&act=scrounge
        private string ClearWeed(string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_opt?mod=farmlandstatus&act=clearWeed";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }
        /// <summary> 
        /// 种植 
        /// </summary> 
        /// <param name="cid"></param> 
        /// <param name="uid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Plant(string cid, string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_plant?mod=farmlandstatus&act=planting";
            string post = "cId=" + cid + "&farmKey=" + GetFarmKey(farmtime) + "&place=" +
                 HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }
        /// <summary> 
        /// 浇水 
        /// </summary> 
        /// <param name="uid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Water(string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_opt?mod=farmlandstatus&act=water";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
            return "";
        }
        /// <summary> 
        /// 杀虫 
        /// </summary> 
        /// <param name="qq"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Spraying(string uid, string place)
        {
            string farmtime = GetFarmTime();
            string url = runUrl + "/cgi-bin/cgi_farm_opt?mod=farmlandstatus&act=spraying";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }
        #endregion

        #region  获取可操作的用户
        private void addStatus(JsonObject json, string url)
        {
            for (int i = 0; i < _status_filter.Count; i++)
            {
                if (_status_filter.GetJson(i).GetJson(0).Key == json.Key)
                    return;
            }
            json.Add("url", url);
            _status_filter.Add(json);
        }

        
        /// <summary>
        /// 获取可操作的用户
        /// </summary>
        private void statusFilter()
        {
            BindUserInfo();
            ChangeTSSL("正在获取获取可操作的用户列表...");
            ChangeLBFM();
            string farmtime = GetFarmTime();
            string Url = "";
            string urlNum = "";
            string post = "";
            string result = "";

            JsonObject _model_1 = new JsonObject("\"" + _uid + "\":{\"1\":1}");
            if (_model_1.ToString().Contains(_uid))
            {
                addStatus(_model_1, "qzone");
            }
            Url = xiaoyouUrl;
            urlNum = "/cgi-bin/cgi_farm_getstatus_filter?cmd=1";
            post = "friend%5Fuids=" + HttpUtility.UrlEncode(_xiaoyoufriendsIds) + "&farmTime=" + farmtime + "&farmKey=" + GetFarmKey(farmtime) + "&uIdx=" + _uid;
            result = GetHtml(Url + urlNum, post, true, cookie);
            model = new JsonObject(result);
            model = model.GetJson("status");

            for (int i = 0; i < model.GetCollection().Count; i++)
            {
                _model_1 = model.GetCollection()[i];
                addStatus(_model_1, "xiaoyou");
            }
            farmtime = GetFarmTime();
            Url = qzoneUrl;
            post = "friend%5Fuids=" + HttpUtility.UrlEncode(_qzonefriendsIds + "," + _uid) + "&farmTime=" + farmtime + "&farmKey=" + GetFarmKey(farmtime) + "&uIdx=" + _uid;
            result = GetHtml(Url + urlNum, post, true, cookie);
            model = new JsonObject(result);
            model = model.GetJson("status");
            for (int i = 0; i < model.GetCollection().Count; i++)
            {
                _model_1 = model.GetCollection()[i];
                addStatus(_model_1, "qzone");
            }
            ChangeTSSL("获取获取可操作的用户列表完成");
        }
        #endregion

        #region 验证
        private string VerifyHtml(string html)
        {
            //{"code":0,"direction":"转换ID失败","poptype":3}
            //{"direction":"系统错误","errorType":"timeOut","poptype":3}
            if (html == "")
                return "";
            JsonObject model = new JsonObject(html);
            if (model.GetValue("errorType") != null)
            {
                ChangeError(html);
                string errorType = model.GetValue("errorType");
                switch (errorType)
                {
                    case "validateCode":
                        ChangeInfo(Utils.ConvertUnicodeStringToChinese(model.GetValue("errorContent")) + "，时间：" + DateTime.Now.ToString());
                        this.Invoke((MethodInvoker)delegate
                        {
                            //是否弹出验证码
                            if (chbVerify.Checked)
                            {
                                FrmFramVerify framverify = new FrmFramVerify(cookie, _uid);
                                framverify.ShowDialog(this);
                            }
                            else
                            {
                                FrmRest();
                            }
                        });
                        break;
                        /*
                    case "session":

                        break;
                        this.Invoke((MethodInvoker)delegate
                        {
                            Dispose();
                            if (thread1 != null && thread1.IsAlive)
                                thread1.Abort();
                            if (thread2 != null && thread2.IsAlive)
                                thread2.Abort();
                            FrmFarmLogin login = new FrmFarmLogin();
                            login.Show();
                        });
                        
                        break;*/
                    default:
                        if (model.GetValue("errorContent") != null)
                        {
                            ChangeInfo("错误："+Utils.ConvertUnicodeStringToChinese(model.GetValue("errorContent")));
                        }
                        ChangeInfo("错误类型：" + Utils.ConvertUnicodeStringToChinese(model.GetValue("errorType")));
                        break;
                }
                return null;
            }
            string direction = model.GetValue("direction");
            string poptype = model.GetValue("poptype");
            if (poptype != null && poptype=="3"&&direction!=null)
            {
                ChangeError(html);
                ChangeInfo("错误：" + direction);
                return null;
            }
            if (direction != null)
            {
                ChangeMsg(model);
            }
            else
            {
                for (int i = 0; i < model.GetCollection().Count; i++)
                {
                    direction = model.GetCollection()[i].GetValue("direction");
                    if (direction != null)
                    {
                        ChangeMsg(model.GetCollection()[i]);
                    }
                    else break;
                }
            }
            return html;
        }
        #endregion

        #region 消息加载
        /// <summary>
        /// 消息加载
        /// </summary>
        /// <param name="model"></param>
        private void ChangeMsg(JsonObject model)
        {
           
            string direction = model.GetValue("direction");
            if (direction != null)
            {
                direction = Utils.NoHTML(Utils.ConvertUnicodeStringToChinese(direction));
                string exp = model.GetValue("exp");
                string money = model.GetValue("money");
                string harvest = model.GetValue("harvest");
                string cName = model.GetValue("cName");
                JsonObject _cIdmodel = model.GetJson("status");
                string cId = model.GetValue("cId");
                string num = model.GetValue("num");
                if (exp != null || direction != "")
                {
                    ChangeLBFM(direction + (exp != null ? " 经验：" + exp : "") + (money != null ? " 金钱：" + money : "") + (harvest!=null?" 果实："+harvest:""));
                    //自动取消
                    if (direction != "" && exp == "0" && money == "0")
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (chbCancel.Checked)
                            {
                                chbClearWeed.Checked = false;
                                chbWater.Checked = false;
                                chbSpraying.Checked = false;
                            }
                        });
                    }
                }
                if (cId != null&&cName!=null)
                {
                    ChangeLBFM("购买 " + Utils.NoHTML(Utils.ConvertUnicodeStringToChinese(cName)) + " " + num + "个");
                }
                if (_cIdmodel!=null)
                {
                    
                    if (_cIdmodel != null)
                    {
                        cId = _cIdmodel.GetValue("cId");
                        string nNum = null;
                        if (cId != null)
                        {
                            JsonObject _cnamemodel = GetShopModel(cId);
                            if (_cnamemodel == null) cName = "品种未知";
                            else cName = _cnamemodel.GetValue("cName");
                            if (_cIdmodel.GetJson("thief") != null && _cIdmodel.GetJson("thief").ToString().Contains(_uid))
                            {
                                nNum = _cIdmodel.GetJson("thief").GetValue(_uid);
                            }
                            newsbog.AddFruit(cId, cName, (harvest != null ? harvest : nNum).ToString());
                            ChangeLBFM("采得 " + Utils.NoHTML(Utils.ConvertUnicodeStringToChinese(cName)) + "  " + (harvest != null ? harvest : nNum) + "个");
                        }
                    }
                }
            }
        }
        #endregion

        #region 获取HTML
        private Stream GetStream(string url, System.Net.CookieContainer cookieContainer)
        {
            Stream html = HttpHelper.GetStream(url, cookieContainer);
            if (html == null)
            {
                html = GetStream(url, cookieContainer);
            }
            return html;
        }
        /// <summary>
        /// 获取HTML
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">post 提交的字符串</param>
        /// <param name="isPost">是否是post</param>
        /// <param name="cookieContainer">CookieContainer</param>
        /// <returns>html </returns>
        private string GetHtml(string url, string postData, bool isPost, System.Net.CookieContainer cookieContainer)
        {
            string html = VerifyHtml(HttpHelper.GetHtml(url, postData, true, cookieContainer));
            if (html == null)
            {
                html = GetHtml(url, postData, isPost, cookieContainer);
            }
            if (html == "")
            {
                ChangeInfo(url + postData);
                html = GetHtml(url, postData, isPost, cookieContainer);
            }
            return html;
        }
        /// <summary>
        /// 获取HTML
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="cookieContainer">CookieContainer</param>
        /// <returns>HTML</returns>
        private string GetHtml(string url, System.Net.CookieContainer cookieContainer)
        {
            string html = VerifyHtml(HttpHelper.GetHtml(url, cookieContainer));
            if (html == null)
            {
                html = GetHtml(url, cookieContainer);
            }
            if (html == "")
            {
                ChangeInfo(url);
                html = GetHtml(url, cookieContainer);
            }
            return html;
        }
        #endregion

        #region 获取用户农场信息
        /// <summary>
        /// 获取用户农场信息
        /// </summary>
        private void TGetUsrInfo()
        {
            picHead.Image = null;
            lblUserName.Text = "";
            notifyIcon1.Text = "";
            lblMoney.Text = "";
            lblExp.Text = "";
            lblLevel.Text = "";
            Thread thread3 = new Thread(new ThreadStart(GetUsrInfo));
            thread3.IsBackground = true;
            thread3.Start();
        }
        private void GetUsrInfo()
        {
            string farmtime = GetFarmTime();
            string url = "http://nc.xiaoyou.qq.com/cgi-bin/cgi_farm_index?mod=user&act=run";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            _status = new JsonObject(result);
            _uid = _status.GetJson("user").GetValue("uId");

            model = _status;
            string headPic = model.GetJson("user").GetValue("headPic").Replace("\\", "");
            if (headPic != null)
            {
                Stream stream = GetStream(headPic, cookie);
                picHead.Image = Image.FromStream(stream);
            }
            this.Invoke((MethodInvoker)delegate
            {
                if (model.GetJson("user").GetValue("userName") != null)
                {

                    string userName = Utils.ConvertUnicodeStringToChinese(model.GetJson("user").GetValue("userName"));
                    lblUserName.Text = userName;
                    this.Text = userName + "   --QQ农夫";
                    notifyIcon1.Text = "（" + (userName) + "）" + notifyIcon1.Text;

                }
                    lblMoney.Text = model.GetJson("user").GetValue("money");

                int lv = 0;
                lblExp.Text = FormatExp(Convert.ToInt32(model.GetJson("user").GetValue("exp")), out lv);

                lblLevel.Text = lv.ToString();
            });
        }

        #endregion

        #region 获取商店列表
        /// <summary> 
        /// 获取商店列表 
        /// </summary> 
        /// <returns></returns> 
        private string ScanShop()
        {
            string farmtime = GetFarmTime();
            string url = "http://happyfarm.xiaoyou.qq.com/api.php?mod=repertory&act=getSeedInfo";
            string post = "farmKey=" + GetFarmKey(farmtime) + "&farmTime=" + farmtime;
            string result = GetHtml(url, post, true, cookie);
            return result;
        }
        #endregion

        #region 获得制定商品信息
        /// <summary> 
        /// 获得制定商品信息 
        /// </summary> 
        /// <param name="cid"></param> 
        /// <returns></returns> 
        private JsonObject GetShopModel(string cid)
        {
            for (int x = 0; x < _shop.GetCollection().Count; x++)
            {
                if (_shop.GetCollection()[x].GetValue("cId").Equals(cid))//不是空地 
                {
                    return _shop.GetCollection()[x];
                }
            }
            return null;
        }
        #endregion

        #region 获取商店中符合自己等级的cid
        /// <summary>
        /// 获取商店中符合自己等级的cid
        /// </summary>
        /// <returns></returns>
        private string GetShopLv()
        {
            int cLevel = 0;
            string cid = "";
            FormatExp(Convert.ToInt32(_status.GetJson("user").GetValue("exp")), out cLevel);
            for (int x = 0; x < _shop.GetCollection().Count; x++)
            {
                string shopcLevel = _shop.GetCollection()[x].GetValue("cLevel");
                if (Convert.ToInt32(shopcLevel)>cLevel)//判断等级
                {
                    break;
                }
                cid = _shop.GetCollection()[x].GetValue("cId");
            }
            return cid;
        } 
        #endregion

        #region 获取所有好友
        /// <summary>
        /// 获取所有好友 
        /// </summary>
        private void ListFriends()
        {
            string url = "http://happyfarm.xiaoyou.qq.com/api.php?mod=friend";
            string result = GetHtml(url, cookie);
            this._xiaoyoufriends = new JsonObject(result);
            for (int i = 0; i < _xiaoyoufriends.GetCollection().Count; i++)
            {
                if (_xiaoyoufriendsIds != "") _xiaoyoufriendsIds += ",";
                _xiaoyoufriendsIds += _xiaoyoufriends.GetCollection()[i].GetValue("userId");
            }
            url = "http://happyfarm.qzone.qq.com/api.php?mod=friend";
            result = GetHtml(url, cookie);
            this._qzonefriends = new JsonObject(result);
            for (int i = 0; i < _qzonefriends.GetCollection().Count; i++)
            {
                if (_qzonefriendsIds != "") _qzonefriendsIds += ",";
                _qzonefriendsIds += _qzonefriends.GetCollection()[i].GetValue("userId");
            }
        }
        #endregion

        #region 获取某好友信息
        /// <summary> 
        /// 获取某好友信息 
        /// </summary> 
        /// <param name="uid"></param> 
        /// <returns></returns> 
        private string ShowFriend(string uid, string url)
        {
            string farmtime = GetFarmTime();
            string urlnum = url + "/cgi-bin/cgi_farm_index?mod=user&act=run&ownerId=" + uid;
            string post = "farmKey=" + GetFarmKey(farmtime) + "&farmTime=" + farmtime;
            string result = GetHtml(urlnum, post, true, cookie);
            return result;
        }
        #endregion

        #region 修改好友信息
        /// <summary> 
        /// 修改好友信息 
        /// </summary> 
        /// <param name="msg"></param> 
        private void ChangeLBFM(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lbFriendsMessage.Items.Add(msg);
            });
        }

        #endregion

        #region 清空好友信息
        /// <summary> 
        /// 清空好友信息 
        /// </summary> 
        /// <param name="msg"></param> 
        private void ChangeLBFM()
        {
            this.Invoke((MethodInvoker)delegate
            {
                lbFriendsMessage.Items.Clear();
            });
        }

        #endregion

        #region 修改状态栏信息
        /// <summary> 
        /// 修改状态栏信息 
        /// </summary> 
        /// <param name="msg"></param> 
        private void ChangeTSSL(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                tsslMsg.Text = msg;
            });
        }
        #endregion
        #region 修改状态栏信息
        /// <summary> 
        /// 修改状态栏信息 
        /// </summary> 
        /// <param name="msg"></param> 
        private void ChangeError(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtError.Text += msg + "\r\n";
              //   .Text = msg;
            });
        }
        #endregion

        #region 改变用户消息
        /// <summary> 
        /// 改变用户消息 
        /// </summary> 
        /// <param name="msg"></param> 
        private void ChangeInfo(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lbInfo.Items.Add(msg);
            });
        }
        #endregion

        #region 获得FarmTime
        /// <summary> 
        /// 获得FarmTime 
        /// </summary> 
        /// <returns></returns> 
        private string GetFarmTime()
        {
            return FarmKey.GetFarmTime();
        }
        #endregion

        #region 获得FarmKey
        /// <summary> 
        /// 获得FarmKey 
        /// </summary> 
        /// <param name="farmTime"></param> 
        /// <returns></returns> 
        private string GetFarmKey(string farmTime)
        {
            return FarmKey.GetFarmKey(farmTime);
        }
        #endregion

        #region 按钮事件

        private void lbtnUpdata_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TGetUsrInfo();
        }
        private void upTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox keyTxt = (TextBox)sender;
            //阻止从键盘输入键 
            e.Handled = true;
            KeyPressEventArgs _e = new KeyPressEventArgs('\b');
            if (e.KeyChar == _e.KeyChar)
            {
                e.Handled = false;
                if (keyTxt.Text.Length <= 1)
                {
                    keyTxt.Text = "0";
                    e.Handled = true;
                }
            }
            //_e = new KeyPressEventArgs('.');
            //if (e.KeyChar == _e.KeyChar)
            //{
            //    if (keyTxt.Text.Split('.') < 1)
            //    {
            //        e.Handled = false;
            //    }
            //    return;
            //}
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                if (keyTxt.Text == "0")
                {
                    keyTxt.Text = "";
                }
                e.Handled = false;
            }
        }

        #region 任务栏显示
        private void tsmResize_Click(object sender, EventArgs e)
        {
            hideFrom();
        }

        private void tmiClose_Click(object sender, EventArgs e)
        {
            Dispose();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void closeMenu_Click(object sender, EventArgs e)
        {
            Dispose();
            System.Environment.Exit(System.Environment.ExitCode);

        }

        private void FrmFarmMain_Resize(object sender, EventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                hideFrom();
            }
        }
        /// <summary>
        /// 最小化窗口到任务栏
        /// </summary>
        private void hideFrom()
        {
            this.notifyIcon1.Visible = true;
            base.ShowInTaskbar = false;
            base.Hide();
        }
        public void showFrom()
        {
            base.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
            this.Activate();
            this.Show();
            base.WindowState = FormWindowState.Normal;
        }


        private void showMenu_Click(object sender, EventArgs e)
        {
            showFrom();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            showFrom();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "暂停")
            {
                ChangeTSSL("已暂停");
                button1.Text = "开始";
                FrmRest();
                timer3.Enabled = false;
            }
            else
            {
                ChangeTSSL("工作中...");
                button1.Text = "暂停";
                FrmWook();
                timer3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lbInfo.Items.Clear();
        }

        #endregion
        #region 工作 ---休息
        /// <summary>
        /// 工作开始
        /// </summary>
        public void FrmWook()
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (tsslMsg.Text == "")
                {
                    ChangeTSSL("工作中...");
                }
                timer1.Enabled = true;
                timer2.Enabled = true;
                _autoWookBool = true;
                _aotuWookTime = 0;
            });
        }
        /// <summary>
        /// 休息开始
        /// </summary>
        public void FrmRest()
        {
            this.Invoke((MethodInvoker)delegate
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                _autoWookBool = false;
                _aotuWookTime = 0;
                if (thread1 != null && thread1.IsAlive)
                    thread1.Abort();
                if (thread2 != null && thread2.IsAlive)
                    thread2.Abort();
            });
        } 
        #endregion

        #region 工作监视器
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (chbUpdata.Checked)
            {
                if (_userInfoTime >= _userInfoUpTime)
                {
                    _userInfoTime = 0;
                    TGetUsrInfo();
                }
                _userInfoTime++;
            }
            if (_autoWookBool)
            {
                
                if (_aotuWookTime >= Convert.ToInt32(txtWorkTime.Text.Trim()))
                {
                    //休息开始
                    FrmRest();
                    return;
                }
            }
            else
            {
                if (_aotuWookTime >= Convert.ToInt32(txtRestTime.Text.Trim()))
                {
                    ChangeTSSL("");
                    //工作开始
                    FrmWook();
                    return;
                }
                ChangeTSSL("距离下次工作，还有" + (Convert.ToInt32(txtRestTime.Text.Trim()) - _aotuWookTime) + "秒...");
            }
            _aotuWookTime++;
        } 
        #endregion

        private void tmiError_Click(object sender, EventArgs e)
        {
            PanelError();
        }
        /// <summary>
        /// 显示隐藏错误
        /// </summary>
        private void PanelError()
        {
            if (panel2.Visible)
            {
                panel2.Visible = false;
                this.Size = new System.Drawing.Size(this.Size.Width - panel2.Size.Width, this.Size.Height);
            }
            else
            {
                panel2.Visible = true;
                this.Size = new System.Drawing.Size(this.Size.Width + panel2.Size.Width, this.Size.Height);
            }
        }
        #region 摘取显示
        private void tsmiNewsBog_Click(object sender, EventArgs e)
        {
            FrmBog frmbog = new FrmBog(newsbog);
            frmbog.ShowDialog();
        }
        #endregion
    }
}