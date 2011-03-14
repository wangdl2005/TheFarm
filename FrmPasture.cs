using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using Ini;
using Json;
using System.IO;
using System.Threading;
using TheStatus;
using System.Web;
using Item;
using Time;

namespace MyFarm
{
    public partial class FrmPasture : Form
    {
        #region 变量定义
        private CookieContainer cookie = new CookieContainer();
        //动物信息
        Dictionary<string, Dictionary<string, string>> _animal = new Dictionary<string, Dictionary<string, string>>();
        string mcIni = "MC.ini";//读取的牧场作物信息文件名
        string configIni = "Config.ini";//配置文件名
        string runUrl = "http://www.szshbs.com";
        string uIdx = "";//登录用户牧场ID
        //string uinY = "";//登录用户牧场ID？
        //string uId = ""; //操作目标ID
        string uName = "";//登录用户姓名
        string showIdP = "";//当前显示牧场的用户ID
        string pastureKeyEncodeString = "rwem5EE4=5fsjj{}ie7*0";
        JsonObject _statusP ;//牧场信息
        JsonObject model; //json模板
        JsonObject _pastureStatus;//当前牧场信息
        JsonObject enemyJson = new JsonObject();//当前牧场敌人信息
        JsonObject badInfoJson;//当前农场蚊子大便信息
        JsonObject _friendsP; //好友列表
        Thread threadGetUserInfoP, threadGetFriendsP, threadGetFriendsFilterP,threadTest,threadPickFriendsFilterList;
        JsonObject _friendsFliterP;//可操作好友列表
        JsonObject _bagStatusP;//用户背包信息
        JsonObject _repertoryStatusP;// 用户仓库信息
        string _friendsIdsP = ""; //好友 ID集合
        string _friendsUInXP = "";//好友 uInX集合;
        string cIdP = "1040";//默认动物
        Dictionary<string, string> configDictP = new Dictionary<string, string>();//配置字典
        JsonObject _shopP; //商店
        int pastureExpTimes = 0;
        bool autoExp = true;
        bool _autoPostProduct = true;//自动赶去生产
        bool _autoRaiseBeast = true; //自动放牛 
        bool _autoFightMouse = true; //自动杀鼠 
        bool _autoShit = true; //自动拾大便
        bool _autoKillMosquito = true; //自动杀蚊子
        bool _autoStealAnimal = true; //自动偷取
        bool _autoBuyAnimal = true; //自动买幼仔
        bool _autoBag = true; //查看背包
        bool _autoDog = false;
        bool _autoCancel = true;//无经验自动取消除草。。

        bool _autoUserInfoBool = false;//用户信息刷新是否自动执行
        int timeUserInfoGo = 0;//用户信息刷新经过时间
        int _userInfoUpTime = 60 * 5;//用户信息刷新时间

        bool _autoWookBool = false; //是否工作
        int _autoWorkTime = 0;//工作执行时间
        int _restTime = 0;
        int timeInterval = 0;  //扫描每个好友的间隔
        int timeToWork = 3600; //工作需执行的时间
        int timeToRest = 600; //休息的时间
        int timeRunFriends = 7200;//扫描好友时间
        int timeGetMature = 0;//获取成熟列表时间
        int timeGetFriendsFilter = 120;//获取可操作好友列表时间

        #endregion

        #region 页面初始化
        public FrmPasture()
        {
            InitializeComponent();
        }

        public FrmPasture(CookieContainer container)
        {
            InitializeComponent();
            cookie = container;
        }

        private void FrmPasture_Load(object sender, EventArgs e)
        {
            try
            {
                ReadAnimalItemInfoP();
                toLogP("读取动物信息成功");
            }
            catch (Exception except)
            {
                toLogP(except.Message);
            }
            
            Thread threadGetUserAndFriends = new Thread(new ThreadStart(this.GetUserAndFriends));
            //threadGetFriendsP.Start();
            threadGetUserAndFriends.Start();
            timer2.Enabled = false;
            //timer1.Enabled = false;
            timer3.Enabled = true;  
        }

        private void FrmPasture_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            System.Environment.Exit(System.Environment.ExitCode);
        }
        #endregion

        #region 包含http请求的方法操作
        
            #region 牧场操作
                #region 放牧草
                private void FeedFoodP(int num)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_feed_food";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime,pastureKeyEncodeString);
                    //foodnum=1&type=0&pastureKey=53cdc48e5e78a4b5981c09e2c120414b41e03e53&farmKey=null&farmTime=1298529994&uIdx=188880
                    string postData = "foodnum=" + num.ToString() + "&type=0&pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        if (content.Contains("added"))
                        {
                            toLogP("放牧草" + num + "根成功");
                        }
                        else
                        {
                            toLogP("放牧草失败");
                        }
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("放牧草失败");
                    }
                }
                private void FeedFoodP(int num, string usrName)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_feed_food";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    //foodnum=1&type=0&pastureKey=53cdc48e5e78a4b5981c09e2c120414b41e03e53&farmKey=null&farmTime=1298529994&uIdx=188880
                    string postData = "foodnum=" + num.ToString() + "&type=0&pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx + "&nick=" + HttpUtility.UrlEncode(usrName);
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        if (content.Contains("added"))
                        {
                            toLogP("放牧草" + num + "根成功");
                        }
                        else 
                        {
                            toLogP("放牧草失败");
                        }
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("放牧草失败");
                    }
                }
                #endregion

                #region 抓去生产??
                private string PostProductP(string serial)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_post_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "serial=" + serial + "&pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        if (content.Contains("addExp"))
                        {
                            toLogP("赶成功");
                        }
                        else
                        {
                            toLogP("赶失败");
                        }
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("赶失败");
                    }
                    return content;
                }
                private string PostProductP(string uid,string serial ,string usrName)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_post_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    //pastureKey=09495be7a3711fc011ba90dd1716e2fe855957ba&farmKey=null&farmTime=1300007525&uId=188985&uIdx=19991&serial=6&nick=%E7%8E%8B%E7%A3%8A
                    string postData = "serial=" + serial + "&pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx + "&uId=" + uid + "&nick=" + HttpUtility.UrlEncode(usrName);
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        if (content.Contains("addExp"))
                        {
                            toLogP("赶成功");
                        }
                        else
                        {
                            toLogP("赶失败");
                        }
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("赶失败");
                    }
                    return content;
                }
                #endregion

                #region 收获农产品??
                private string HarvestAllProductP()
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_harvest_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "harvesttype=1&version=1&farmTime="+farmTime+"&type=%2D1&pastureKey="+pastureKey+"&farmKey=null&uIdx="+uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("收获失败");
                        return "";
                    }
                }

                /*private string HarvestProductP(string typeId)
                {
                    
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_harvest_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "harvesttype=1&version=1&farmTime=" + farmTime + "&type= " + typeId + "&pastureKey=" + pastureKey + "&farmKey=null&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("收获失败");
                        return "";
                    }
                }*/

                private string HarvestAnimalP(string serial,string serialIndex)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_harvest_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "harvesttype=2&version=1&farmTime=" + farmTime + "&serial=" + serial + "&serialIndex=" + serialIndex
                        + "&pastureKey=" + pastureKey + "&farmKey=null&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("收获失败");
                        return "";
                    }
                }
                #endregion

                #region 偷取农产品
                private string StealProductP(string uId,string usrName,string typeId)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_steal_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "pastureKey=" + pastureKey + "&farmKey=null&uId=" + uId + "&nick=" + HttpUtility.UrlEncode(usrName)
                        + "&type=" + typeId + "&version=1&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("偷取失败");
                        return "";
                    }
                }
                #endregion

                #region 一键偷取
                private string StealAllP(string uId,string usrName)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_steal_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "version=1&type=%2D1&uId=" + uId + "&farmTime=" + farmTime + "&nick=" + HttpUtility.UrlEncode(usrName)
                        + "&pastureKey=" + pastureKey + "&farmKey=null&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("偷取失败");
                        return "";
                    }
                }
                #endregion

                #region 购买幼仔
                private string BuyAnimalP(string cId,int num)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_buy_animal";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "cId=" + cId + "&number=" + num.ToString() + "&pastureKey=" + pastureKey
                        + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        toLogP("购买成功");
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("购买失败");
                        return "";
                    }
                }
                #endregion

                #region 拍死蚊子
                private string KillMosquitoP(string uId,string pos,string num)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_help_pasture";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "uId=" + uId + "&farmTime=" + farmTime + "&pos=" + pos + "&num=" + num
                        + "&pastureKey=" + pastureKey + "&farmKey=null&type=1&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("拍死蚊子失败");
                        return "";
                    }
                }
                #endregion

                #region 拾取大便
                private string GetShitsP(string uId,string pos,string num)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_help_pasture";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "uId=" + uId + "&farmTime=" + farmTime + "&pos=" + pos + "&num=" + num
                        + "&pastureKey=" + pastureKey + "&farmKey=null&type=2&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("拾取大便失败");
                        return "";
                    }
                }
                #endregion

                #region 赶走老鼠
                private string FightMouseP(string uId)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_fight";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "farmTime="+farmTime+"&type=1&pastureKey="+pastureKey+"&farmKey=null&uId="
                        + uId + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("赶走老鼠失败");
                        return "";
                    }
                }
                #endregion

                #region 放牛??
                private string RaiseBeastP(string uId)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_farm_raise_beast";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "farmTime=" + farmTime + "&type=1&ownerId=" + uId + "&pastureKey=" + pastureKey + "&farmKey=null&slotid=0&uIdx=" + uIdx + "&isfarm=0";
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("放牛失败");
                        return "";
                    }
                }
                #endregion

                #region 捐赠农产品
                private string DonateAnimal(string serial)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_donate_animal";
                    string farmTime = TheKey.GetFarmTime();
                    //pastureKey=9d0b144f9d3fe81509c6053275248179e57c5dda&farmKey=null&farmTime=1300093000&serial=3&uIdx=188880
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&serial=" + serial + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        toLogP("捐赠成功");
                        return content;
                    }
                    catch (Exception e)
                    {
                        toLogP(e.Message);
                        toLogP("捐赠失败");
                        return "";
                    }
                }
                #endregion
            #endregion

                #region 信息操作

                #region 用户信息
                private void GetUserInfoP()
                {
                    string content = "";
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_enter?";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    //uId=0&farmTime=1298530266&flag=1&newitem=2&pastureKey=e31b52ffba822cae3913846a2a7c7b2af33e7f89&farmKey=null&uIdx=188880
                    //string postData = "uId=0&farmTime=1298530266&flag=1&newitem=2&pastureKey=e31b52ffba822cae3913846a2a7c7b2af33e7f89&farmKey=null&uIdx=188880";
                    try
                    {
                        content = HttpChinese.GetHtml(cookie, url);
                        this._statusP = new JsonObject(content);
                        uName = _statusP.GetJson("user").GetValue("userName");
                        uIdx = _statusP.GetJson("user").GetValue("uId");
                        //uinY = _statusP.GetJson("user").GetValue("uinLogin");
                        model = _statusP;
                        string headPic = model.GetJson("user").GetValue("headPic");
                        int level = 0;
                        string exp = "";
                        exp = _statusP.GetJson("user").GetValue("exp");
                        exp = FormatExpP(Convert.ToInt32(exp), out level);
                        //读取头像可能出现没有头像的结果，导致一直等待
                        if (uIdx.Equals("19991"))
                        {
                            if (headPic != null)
                            {
                                try
                                {
                                    Stream stream = HttpChinese.GetStream(headPic, cookie);
                                    picHead.Image = Image.FromStream(stream);
                                }
                                catch (Exception e)
                                {
                                    toLogP(e.Message);
                                }
                            }
                        }
                        this.Invoke((MethodInvoker)delegate
                        {
                            lblUserName.Text = _statusP.GetJson("user").GetValue("userName");
                            lblMoney.Text = _statusP.GetJson("user").GetValue("money");
                            lblExp.Text = exp;
                            lblLevel.Text = level.ToString();
                        });
                        toLogP("获取用户信息成功");
                        //以下获得农场信息
                        _pastureStatus = new JsonObject(_statusP.GetValue("animal"));
                        enemyJson = new JsonObject(_statusP.GetValue("enemy"));
                        badInfoJson = new JsonObject(_statusP.GetValue("badinfo"));
                        //showPasture(_pastureStatus);
                        showIdP = uIdx;
                        toLogP("获得用户牧场信息成功");
                        toStatusP("当前显示牧场为" + uName + "的牧场");
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("用户信息获取失败");
                    }             
                }
                #endregion

                #region 好友列表
                private void GetFriendsP()
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=friend";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    //pastureKey=71f1d612b74f8ea1032e68ce988a29227b468406&farmKey=null&user=true&farmTime=1298530529&uIdx=188880
                    string postData = "pastureKey="+pastureKey+"&farmKey=null&user=true&farmTime="+farmTime+"&uIdx=" + uIdx;
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(cookie, url);
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("好友信息获取失败");
                    }

                    this._friendsP = new JsonObject(content);
                }
                #endregion

                #region 好友信息
                private JsonObject GetFUserInfo(string id)
                {
                    string url = runUrl +  "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_enter?";                    
                    string ownerId = id;
                    User _friendInfo = new User(GetUserModel(id));
                    string uinX = _friendInfo.uin;
                    string farmTime = TheKey.GetFarmTime();
                    string postData = "flag=1&pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&farmKey=null&farmTime="
                        + farmTime + "&newitem=2&uId=" + id + "&uIdx=" + uIdx;
                    string content = "";
                    JsonObject tempJson = new JsonObject();
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        JsonObject farmJson = new JsonObject(content);
                        tempJson = new JsonObject(farmJson.GetValue("animal"));
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("指定用户信息获取失败");
                    }
                    return new JsonObject(tempJson);
                }

                private JsonObject GetFUserInfo(string id, out JsonObject enemyJson)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_enter?";
                    string ownerId = id;
                    User _friendInfo = new User(GetUserModel(id));
                    string uinX = _friendInfo.uin;
                    string farmTime = TheKey.GetFarmTime();
                    string postData = "flag=1&pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&farmKey=null&farmTime="
                        + farmTime + "&newitem=2&uId=" + id + "&uIdx=" + uIdx; 
                    string content = "";
                    JsonObject tempJson = new JsonObject();
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        JsonObject farmJson = new JsonObject(content);
                        tempJson = new JsonObject(farmJson.GetValue("animal"));
                        enemyJson = new JsonObject(farmJson.GetValue("enemy"));
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("指定用户信息获取失败");
                        enemyJson = new JsonObject();
                    }
                    return tempJson;
                }

                private JsonObject GetFUserInfo(string id, out JsonObject enemyJson,out JsonObject badInfoJson)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_enter?";
                    string ownerId = id;
                    User _friendInfo = new User(GetUserModel(id));
                    string uinX = _friendInfo.uin;
                    string farmTime = TheKey.GetFarmTime();
                    string postData = "flag=1&pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&farmKey=null&farmTime="
                        + farmTime + "&newitem=2&uId=" + id + "&uIdx=" + uIdx;
                    string content = "";
                    JsonObject tempJson = new JsonObject();
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        JsonObject farmJson = new JsonObject(content);
                        tempJson = new JsonObject(farmJson.GetValue("animal"));
                        enemyJson = new JsonObject(farmJson.GetValue("enemy"));
                        badInfoJson = new JsonObject(farmJson.GetValue("badinfo"));
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("指定用户信息获取失败");
                        enemyJson = new JsonObject();
                        badInfoJson = new JsonObject();
                    }
                    return tempJson;
                }

                /*private JsonObject GetFUserInfo(string id, out JsonObject badInfoJson)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_enter?";
                    string ownerId = id;
                    User _friendInfo = new User(GetUserModel(id));
                    string uinX = _friendInfo.uin;
                    string farmTime = TheKey.GetFarmTime();
                    string postData = "flag=1&pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&farmKey=null&farmTime="
                        + farmTime + "&newitem=2&uId=" + id + "&uIdx=" + uIdx;
                    string content = "";
                    JsonObject tempJson = new JsonObject();
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        JsonObject farmJson = new JsonObject(content);
                        tempJson = new JsonObject(farmJson.GetValue("animal"));
                        badInfoJson = new JsonObject(farmJson.GetValue("badinfo"));
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("指定用户信息获取失败");
                        badInfoJson = new JsonObject();
                    }
                    return tempJson;
                }*/
                #endregion

                #region 获取可操作好友信息
                private void getFriendsFliter()
                {
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_get_Exp";
                    string friend_Fuids = _friendsIdsP;
                    friend_Fuids = HttpUtility.UrlEncode(friend_Fuids);
                    string postData = "expflag=0&farmTime=" + farmTime + "&uidlist=" + friend_Fuids + "&optflag=1&pastureKey="
                        + pastureKey + "&farmKey=null&uIdx=" + uIdx;
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                        _friendsFliterP = new JsonObject(content).GetJson("userFlag");
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("可摘取好友信息获取失败");
                    }
                }
                #endregion

                #region 获取背包信息
                private void getBagInfo()
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_get_package";
                    //farmTime=1298649084&pastureKey=34d169d4186861302aae7cbd89730dbc24689c0d&farmKey=null&uIdx=19991
                    string farmTime = TheKey.GetFarmTime();
                    string postData = "farmTime=" + farmTime + "&pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&farmKey=null&uIdx=" + uIdx;
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("背包信息获取失败");
                    }
                    _bagStatusP = new JsonObject(content);
                }
                #endregion

                #region 获取仓库信息
                private void GetRepertoryInfoP()
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_get_repertory?target=animal";
                    //pastureKey=7349b52aa73e6fbb77edbc0af6d057e3c2b5aa96&farmKey=null&farmTime=1298529710&uIdx=188880
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        _repertoryStatusP = new JsonObject(content);
                        toLogP("获取仓库信息成功");
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("获取仓库信息失败");
                    }
                }
                #endregion

                #region 获取商店信息
                private string ScanShop()
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_get_animals";
                    string farmTime = TheKey.GetFarmTime();
                    string postData = "pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("商店信息获取失败");
                    }
                    return content;
                }
                #endregion

                #region 获取VIP礼包
                private void GetVipGiftP()
                {
                    string farmTime = TheKey.GetFarmTime();
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_accept_gift";
                    string postData = "pastureKey=" + TheKey.GetPastureKey(farmTime, pastureKeyEncodeString) + "&uIdx=" + uIdx + "&farmKey=null&farmTime=" + farmTime;
                    string content = "";
                    try
                    {
                        content = HttpChinese.GetHtml(postData, cookie, url);
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("每日VIP礼包信息获取失败");
                    }
                    if (content.Contains("vip"))
                    {
                        toLogP("每日VIP礼包领取成功");
                    }
                    else
                    {
                        toLogP("每日VIP礼包领取失败");
                    }
                }
                #endregion

                #region 出售仓库物品
                //全部！！
                private void SellProducts()
                {
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_sale_product";
                    string postData = "farmTime=" + farmTime + "&pastureKey=" + pastureKey + "&farmKey=null&saleAll=1&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        toLogP("卖掉全部仓库物品成功" + "，获得" + new JsonObject(content).GetValue("money") + "金币");
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("卖掉仓库物品失败");
                    }
                }
                //
                private void SellProduct(string cid,string cName,string num)
                {
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_sale_product";
                    //pastureKey=8320d61eb9a97e492a4d7fee56239e8090207f42&uIdx=19991&farmKey=null&farmTime=1300070055&num=5&cId=1003
                    string postData = "pastureKey=" + pastureKey + "&uIdx=" + uIdx + "&farmKey=null&farmTime=" + farmTime + "&num=" + num + "&cId=" + cid;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        toLogP("卖掉仓库物品" + cName + "成功" + "，获得" + new JsonObject(content).GetValue("money") + "金币");
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("卖掉仓库物品" + cName + "失败");
                    }
                }
                #endregion

                #region 获取牛的信息
                private void GetBeastInfoP()
                {
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_farm_get_userbeast";
                    //farmKey=null&farmTime=1298684832&uIdx=19991&ownerId=19991&pastureKey=2f17c8083c5c7c5645e9e06cfca6cf642ff37f7e
                    string postData = "farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx + "&ownerId=" + uIdx + "&pastureKey=" + pastureKey;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
                        toLogP("卖掉全部仓库物品成功" + "，获得" + new JsonObject(content).GetValue("money") + "金币");
                    }
                    catch (Exception except)
                    {
                        toLogP(except.Message);
                        toLogP("卖掉仓库物品失败");
                    }
                }
                #endregion
            #endregion
        #endregion

        #region 牧场方法调用函数
        private void PastureHarvest(string userId, JsonObject thePastureStatus)
        {
            //string animalStatus = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            string enemyId = "";
            string isHungry = "";
            enemyId = enemyJson.GetValue("type");
            isHungry = enemyJson.GetValue("num");
            if (!userId.Equals(""))
            {
                if (userId.Equals(uIdx))
                {
                    for (int i = 0; i < thePastureStatus.GetCollection().Count; i++)
                    {
                        AnimalStatus newAnimalStatus = new AnimalStatus(thePastureStatus.GetCollection()[i]);
                        bool landDogFlag = true; //true表示可偷，false表示不可偷
                        //自己的地尽管偷
                        //无狗 无狗粮                        
                        if (landDogFlag)
                        {
                           /* GetCropStatus(out animalStatus, newAnimalStatus);
                            if (animalStatus.Equals("饥饿") || animalStatus.Equals("生产中") || animalStatus.Equals("成长中"))
                            {
                            }
                            else*/
                            if(!newAnimalStatus.totalCome.Equals("0"))
                            {
                                result = HarvestAllProductP();
                                if (result.Contains("[["))
                                {
                                    toLogP("收获" + friendInfo.userName + "的牧场的第" + newAnimalStatus.serial + "号"
                                        + new AnimalItem(GetAnimalModel(newAnimalStatus.cId)).cName + "成功");

                                    //saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                                }
                                else
                                {
                                    toLogP("收获" + friendInfo.userName + "的牧场的第" + newAnimalStatus.serial + "号动物失败");
                                }

                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < thePastureStatus.GetCollection().Count; i++)
                    {
                        AnimalStatus newAnimalStatus = new AnimalStatus(thePastureStatus.GetCollection()[i]);
                        bool landDogFlag = true; //true表示可偷，false表示不可偷                           
                        //无狗 无狗粮
                        if (_autoDog)
                        {
                            landDogFlag = (enemyId.Equals("0") || isHungry.Equals("1"));
                            toLogP(friendInfo.userName + "的农场有猎人,不偷");
                        }
                        if (landDogFlag)
                        {
                            //GetCropStatus(out animalStatus, newAnimalStatus);
                            /*if (animalStatus.Equals("饥饿") || animalStatus.Equals("生产中") || animalStatus.Equals("成长中"))
                            {
                            }
                            else
                            {*/
                            if(!newAnimalStatus.totalCome.Equals("0"))
                            {
                                result = StealAllP(userId, friendInfo.userName);
                                if (!result.Contains("[" + newAnimalStatus.cId + ",0]"))
                                {
                                    toLogP("偷取" + friendInfo.userName + "的牧场的第" + newAnimalStatus.serial + "号"
                                        + new AnimalItem(GetAnimalModel(newAnimalStatus.cId)).cName.Replace("\0", "") + "成功");

                                    //saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                                }
                                else
                                {
                                    toLogP("偷取" + friendInfo.userName + "的牧场的第" + newAnimalStatus.serial + "号动物失败");
                                }

                            }
                        }
                    }
                }
            }
        }

        private void PasturePostProduct(string userId, JsonObject thePastureStatus)
        {
            string animalStatus = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            if (!userId.Equals(""))
            {
                for (int i = 0; i < thePastureStatus.GetCollection().Count; i++)
                {
                    AnimalStatus newAnimalStatus = new AnimalStatus(thePastureStatus.GetCollection()[i]);
                    GetCropStatus(out animalStatus, newAnimalStatus);
                   if(animalStatus.Equals("可赶去"))
                   {
                        result = PostProductP(userId,newAnimalStatus.serial,friendInfo.userName);
                        if (result.Contains("{\"addExp\":"))
                        {
                            toLogP("赶" + friendInfo.userName + "的牧场的第" + newAnimalStatus.serial + "号"
                                + new AnimalItem(GetAnimalModel(newAnimalStatus.cId)).cName.Replace("\0", "") + "成功");

                            //saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                        }
                        else
                        {
                            toLogP("赶" + friendInfo.userName + "的牧场的第" + newAnimalStatus.serial + "号动物失败");
                        }

                    }
                }
            }                
        }

        private void PastureKillMosquitoP(string userId, JsonObject thePastureStatus)
        {
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            if (!userId.Equals(""))
            {
                GetFUserInfo(userId, out enemyJson, out badInfoJson);
                BadInfo newBadInfo = new BadInfo(badInfoJson);
                if (!newBadInfo.numOfMosquito.Equals("0"))
                {
                    result = KillMosquitoP(userId, "1", newBadInfo.numOfMosquito);
                    if (result.Contains("{\"addExp\":"))
                    {
                        toLogP("拍掉" + friendInfo.userName + "的牧场的蚊子成功");

                        //saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                    }
                    else
                    {
                        toLogP("拍掉" + friendInfo.userName + "的牧场的蚊子失败");
                    }

                }   
            }   
        }

        private void PastureGetShitsP(string userId, JsonObject thePastureStatus)
        {
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            if (!userId.Equals(""))
            {
                GetFUserInfo(userId, out enemyJson, out badInfoJson);
                BadInfo newBadInfo = new BadInfo(badInfoJson);
                while (!newBadInfo.numOfShit.Equals("0"))
                {
                    result = GetShitsP(userId, "0", newBadInfo.numOfShit);
                    if (result.Contains("{\"num\":"))
                    {
                        toLogP("捡取" + friendInfo.userName + "的牧场的大便成功");

                        //saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                    }
                    else
                    {
                        toLogP("捡取" + friendInfo.userName + "的牧场的大便失败");
                    }
                    GetFUserInfo(userId, out enemyJson, out badInfoJson);
                    newBadInfo = new BadInfo(badInfoJson);
                }
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
        private string FormatExpP(int exp, out int lv)
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

        #region 牧场窝、棚信息操作
        private void GetCropStatus(out string animalStatus, AnimalStatus newAnimalStatus)
        {            
            if (!newAnimalStatus.hungry.Equals("0"))
            {
                animalStatus = "饥饿";
            }
            else if (newAnimalStatus.status.Equals("3"))
            {
                animalStatus = "可赶去";
            }
            else if (newAnimalStatus.status.Equals("5") || newAnimalStatus.status.Equals("1"))
            {
                animalStatus = "成长中";
            }
            else
            {
                animalStatus = "";
            }
        }
        #endregion

        #region 动物信息操作
        private void ReadAnimalItemInfoP()
        {
            try
            {
                string filePath = System.IO.Path.GetFullPath(@mcIni);
                Dictionary<string, Dictionary<string, string>> temp = new Dictionary<string, Dictionary<string, string>>();
                IniFiles readIni = new IniFiles(filePath);
                StringCollection sectionList = new StringCollection();
                StringCollection keyList = new StringCollection();
                string value = "";
                readIni.ReadSections(sectionList);
                foreach (string s in sectionList)
                {
                    Dictionary<string, string> tempCrop = new Dictionary<string, string>();
                    readIni.ReadSection(s, keyList);
                    foreach (string k in keyList)
                    {
                        value = readIni.ReadString(s, k, "");
                        if (!tempCrop.ContainsKey(k))
                        {
                            tempCrop.Add(k, value);
                        }
                    }
                    if (!temp.ContainsKey(s))
                    {
                        temp.Add(s, tempCrop);
                    }
                }
                _animal = temp;
            }
            catch (Exception except)
            {
                throw except;
            }
        }

        #region 获得指定动物的信息
        private Dictionary<string, string> GetAnimalModel(string cid)
        {
            if (_animal.ContainsKey(cid))
            {
                return _animal[cid];
            }
            return null;
        }
        #endregion
        #endregion

        #region 用户信息操作
        #region 获取指定用户信息
        private JsonObject GetUserModel(string cid)
        {
            for (int x = 0; x < _friendsP.GetCollection().Count; x++)
            {
                if (_friendsP.GetCollection()[x].GetValue("uId").Equals(cid))//不是空的
                {
                    return _friendsP.GetCollection()[x];
                }
            }
            return null;
        }
        #endregion   

        #region 显示动物信息
        private void showAnimalInfo(JsonObject _pastureStatus)
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewFarmland.Items.Clear();
            });
            for (int i = 0; i < _pastureStatus.GetCollection().Count; i++)
            {
                ListViewItem lv = new ListViewItem();
                AnimalStatus newAnimal = new AnimalStatus(_pastureStatus.GetCollection()[i]);
                AnimalItem newAnimalItem = new AnimalItem(GetAnimalModel(newAnimal.cId));
                string animalStatus = "";
                GetCropStatus(out animalStatus, newAnimal);
                //lv.SubItems.Add(newLand.b.Equals("0") ? "空地":newShopItem.cName);
                lv.SubItems[0].Text = newAnimal.serial;
                lv.SubItems.Add(newAnimalItem.cName);
                lv.SubItems.Add(newAnimal.status);
                lv.SubItems.Add(TimeFormat.FormatTime(newAnimal.buyTime));               
                lv.SubItems.Add(TimeFormat.FormatTimeToHHMMSS(Convert.ToInt32(newAnimal.growTimeNext)));
                lv.SubItems.Add(animalStatus);
                lv.SubItems.Add(newAnimal.totalCome);
                this.Invoke((MethodInvoker)delegate
                {
                    listViewFarmland.Items.Add(lv);
                });
            }
        }
        #endregion
        #endregion

        #region 好友列表操作
        #region 显示好友列表信息
        private void ListFriends()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewFriends.Items.Clear();
            });
            GetFriendsP();
            string exp = "";
            int level = 0;
            _friendsIdsP = "";
            _friendsUInXP = "";

            //双缓冲实现
            //// create a temp dataTable to store data
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add("id", typeof(String));
            dt.Columns.Add("userId", typeof(String));
            dt.Columns.Add("userName", typeof(String));
            dt.Columns.Add("level", typeof(String));
            dt.Columns.Add("money", typeof(String));
            dt.Columns.Add("canDoStatus", typeof(String));
            //dt.Columns.Add("exp", typeof(String));
            dt.Columns.Add("lastTime", typeof(String));
            for (int i = 0; i < _friendsP.GetCollection().Count; i++)
            {
                if (_friendsIdsP != "") _friendsIdsP += "|";
                if (_friendsUInXP != "") _friendsUInXP += "|";
                _friendsIdsP += _friendsP.GetCollection()[i].GetValue("uId");
                _friendsUInXP += _friendsP.GetCollection()[i].GetValue("uin");
                User _friendsInfo = new User(_friendsP.GetCollection()[i]);
                ;
                exp = _friendsP.GetCollection()[i].GetValue("exp");
                exp = FormatExpP(Convert.ToInt32(exp), out level);
                //string theDoStatus = newFriendsFilter.doStatus.theDoStatus;
                dr = dt.NewRow();
                dr[0] = (i + 1).ToString();
                dr[1] = _friendsP.GetCollection()[i].GetValue("uId");
                dr[2] = _friendsInfo.userName;
                dr[3] = level.ToString();
                dr[4] = _friendsInfo.money;
                dr[5] = exp;
                dr[6] = DateTime.Now.ToString();

                dt.Rows.Add(dr);

            }
            // loop the temp table , and insert to ListView
            int iSize = (dt.Rows.Count > 1000) ? 1000 : dt.Rows.Count;

            ListViewItem lvi;
            ListViewItem[] lvitems = new ListViewItem[iSize];
            for (int i = 0; i < iSize; i++)
            {
                lvi = new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString() });
                lvitems[i] = lvi;
            }
            this.Invoke((MethodInvoker)delegate
            {
                listViewFriends.Items.AddRange(lvitems);
            });
            toLogP("获取好友信息成功");
        }
        #endregion 

        #region 好友信息与用户信息
        private void GetUserAndFriends()
        {
            GetUserInfoP();
            showAnimalInfo(_pastureStatus);
            ListFriends();
        }
        #endregion


        private void listViewFriends_DoubleClick(object sender, EventArgs e)
        {
            //获得当前行 
            int iRowCurr = this.listViewFriends.SelectedItems[0].Index;
            //取得当前行的数据 
            string id = listViewFriends.SelectedItems[0].SubItems[1].Text;
            string userName = listViewFriends.SelectedItems[0].SubItems[2].Text;
            _pastureStatus = GetFUserInfo(id, out enemyJson);
            showAnimalInfo(_pastureStatus);
            showIdP = id;
            toLogP("获取" + userName + "土地信息成功");
            toStatusP("当前显示农场为：" + userName + "的农场");
        }

        private void lbtnUpdata_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            threadGetUserInfoP = new Thread(new ThreadStart(this.GetUserAndFriends));
            //threadGetFriendsP.Start();
            threadGetUserInfoP.Start();
        }

        private void lbtnGetFriends_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (threadGetFriendsP == null || !threadGetFriendsP.IsAlive)
            {
                threadGetFriendsP = new Thread(new ThreadStart(ListFriends));
                threadGetFriendsP.Start();
            }
        }
        #endregion

        #region 可操作好友列表操作
        #region 获取指定好友可操作信息
        private FriendFilterP getFriendsDoStatusModel(string cid)
        {
            FriendFilterP tmp = new FriendFilterP();
            for (int i = 0; i < _friendsFliterP.GetCollection().Count; i++)
            {
                if (_friendsFliterP.GetCollection()[i].Key.Equals(cid))
                {
                    tmp.userId = _friendsFliterP.GetKey(i);
                    tmp.doStatus = new DoStatusP(new JsonObject(_friendsFliterP.GetValue(i)));
                    return tmp;
                }
            }
            return null;
        }
        #endregion

        #region 显示可操作好友列表
        private void showFriendsFilter()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewFriendsFilter.Items.Clear();
            });
            string exp = "";
            int level = 0;
            FriendFilterP newFriendsFilter = new FriendFilterP();
            if (_friendsFliterP != null && _friendsFliterP.GetCollection().Count > 0)
            {
                //双缓冲实现
                //// create a temp dataTable to store data
                DataTable dt = new DataTable();
                DataRow dr;
                dt.Columns.Add("id", typeof(String));
                dt.Columns.Add("userId", typeof(String));
                dt.Columns.Add("userName", typeof(String));
                dt.Columns.Add("level", typeof(String));
                dt.Columns.Add("money", typeof(String));
                dt.Columns.Add("canDoStatus", typeof(String));
                //dt.Columns.Add("exp", typeof(String));
                dt.Columns.Add("lastTime", typeof(String));
                for (int i = 0; i < _friendsFliterP.GetCollection().Count; i++)
                {
                    newFriendsFilter.userId = _friendsFliterP.GetKey(i);
                    newFriendsFilter.doStatus = new DoStatusP(new JsonObject(_friendsFliterP.GetValue(i)));
                    User _friendsInfo = new User(GetUserModel(newFriendsFilter.userId));
                    exp = _friendsInfo.exp;
                    exp = FormatExpP(Convert.ToInt32(exp), out level);
                    string theDoStatus = newFriendsFilter.doStatus.theDoStatus;
                    dr = dt.NewRow();
                    dr[0] = (i + 1).ToString();
                    dr[1] = _friendsFliterP.GetKey(i);
                    dr[2] = _friendsInfo.userName;
                    dr[3] = level.ToString();
                    dr[4] = _friendsInfo.money;
                    dr[5] = theDoStatus;
                    dr[6] = DateTime.Now.ToString();

                    dt.Rows.Add(dr);
                }


                // loop the temp table , and insert to ListView
                int iSize = (dt.Rows.Count > 1000) ? 1000 : dt.Rows.Count;

                ListViewItem lvi;
                ListViewItem[] lvitems = new ListViewItem[iSize];
                for (int i = 0; i < iSize; i++)
                {
                    lvi = new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString() });
                    lvitems[i] = lvi;
                }
                this.Invoke((MethodInvoker)delegate
                {
                    listViewFriendsFilter.Items.AddRange(lvitems);
                });
                
            }
        }
        #endregion

        #region 处理可操作好友列表
        private void dealFriendFilter()
        {
            FriendFilterP newFriendsFilter = new FriendFilterP();
            JsonObject tmpJson = new JsonObject();
            if (_friendsFliterP != null && _friendsFliterP.GetCollection().Count > 0)
            {
                for (int i = 0; i < _friendsFliterP.GetCollection().Count; i++)
                {
                    newFriendsFilter.doStatus = new DoStatusP(new JsonObject(_friendsFliterP.GetValue(i)));
                    if(!newFriendsFilter.doStatus.theDoStatus.Equals(""))
                    {
                        tmpJson.Add(_friendsFliterP.GetKey(i),_friendsFliterP.GetValue(i));
                    }
                }
                _friendsFliterP = tmpJson;
            }
        }
        #endregion

        #region 摘取可操作好友列表
        private void PickFriendsFilterList()
        {
            if (_friendsFliterP != null)
            {
                List<string> idList = _friendsFliterP.GetKeys();
                if(idList!=null){
                    foreach (string id in idList)
                    {
                        FriendFilterP newFriendsFilter = new FriendFilterP();
                        if ((newFriendsFilter = getFriendsDoStatusModel(id)) != null)
                        {
                            if (newFriendsFilter.doStatus.theDoStatus.Equals("可偷取") && _autoStealAnimal)
                            {
                                PastureHarvest(id, GetFUserInfo(id));
                            }
                            if (newFriendsFilter.doStatus.theDoStatus.Equals("可赶去") && _autoPostProduct)
                            {
                                PasturePostProduct(id, GetFUserInfo(id));
                            }
                            if (newFriendsFilter.doStatus.theDoStatus.Equals("有蚊便") && _autoKillMosquito)
                            {
                                PastureKillMosquitoP(id, GetFUserInfo(id));
                                PastureGetShitsP(id, GetFUserInfo(id));
                            }
                            if (newFriendsFilter.doStatus.theDoStatus.Equals("可收获") && _autoStealAnimal)
                            {
                                PastureHarvest(id, GetFUserInfo(id));
                            }
                        }
                    }
                    toLogP("可操作好友列表操作完成");
                    FriendsFliterList();
                }

                
            }
            else
            {
                toLogP("可操作好友列表操作失败");
            }
        }
        #endregion

        #region 可操作好友列表
        private void FriendsFliterList()
        {
            getFriendsFliter();
            toLogP("获取可操作好友列表成功");
            dealFriendFilter();
            toLogP("可操作好友列表处理成功");
            showFriendsFilter();
        }
        #endregion        

        private void lbtnGetFriendsFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (threadGetFriendsFilterP == null || !threadGetFriendsFilterP.IsAlive)
            {
                threadGetFriendsFilterP = new Thread(new ThreadStart(FriendsFliterList));
                threadGetFriendsFilterP.Start();
            }
        }


        private void btnPickFriendFilter_Click(object sender, EventArgs e)
        {
            if (threadPickFriendsFilterList == null || !threadPickFriendsFilterList.IsAlive)
            {
                threadPickFriendsFilterList = new Thread(new ThreadStart(PickFriendsFilterList));
                threadPickFriendsFilterList.Start();
            }
        }
        #endregion

        #region 背包信息操作
        #endregion

        #region 商店信息操作
        #endregion

        #region 仓库信息操作
        private void ShowRepertory()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewRepertory.Items.Clear();
            });
            long allMoney = 0;
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add("id", typeof(String));
            dt.Columns.Add("cId", typeof(String));
            dt.Columns.Add("cName", typeof(String));
            dt.Columns.Add("cMoney", typeof(String));
            dt.Columns.Add("cNum", typeof(String));
            //dt.Columns.Add("canDoStatus",typeof(String));
            dt.Columns.Add("totalMoney", typeof(String));
            dt.Columns.Add("isLock", typeof(String));
            dt.Columns.Add("lastTime", typeof(String));
            for (int i = 0; i < _repertoryStatusP.GetCollection().Count; i++)
            {
                string cid = _repertoryStatusP.GetCollection()[i].GetValue("cId");
                string cNum = _repertoryStatusP.GetCollection()[i].GetValue("amount");
                string lv = _repertoryStatusP.GetCollection()[i].GetValue("lv");
                string cName = _repertoryStatusP.GetCollection()[i].GetValue("cName");
                string price = _repertoryStatusP.GetCollection()[i].GetValue("price");
                long totalMoney = Convert.ToInt32(cNum) * Convert.ToInt32(price);
                dr = dt.NewRow();
                dr[0] = (i + 1).ToString();
                dr[1] = cid;
                dr[2] = cName;
                dr[4] = price;
                dr[5] = cNum;
                dr[6] = totalMoney;
                allMoney += totalMoney;
                dr[3] = lv;
                dr[7] = DateTime.Now.ToString();

                dt.Rows.Add(dr);
            }
            int iSize = (dt.Rows.Count > 1000) ? 1000 : dt.Rows.Count;

            ListViewItem lvi;
            ListViewItem[] lvitems = new ListViewItem[iSize];
            for (int i = 0; i < iSize; i++)
            {
                lvi = new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString(), dt.Rows[i][7].ToString() });
                lvitems[i] = lvi;
            }
            this.Invoke((MethodInvoker)delegate
            {
                listViewRepertory.Items.AddRange(lvitems);
                lblTotalMoney.Text = "总价值为：" + allMoney + "金币";
            });
        }

        #region 仓库信息
        private void GetRepertoryInfoThread()
        {
            GetRepertoryInfoP();
            ShowRepertory();
        }
        #endregion


        private void lbtnGetRepertory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread threadGetRepertory = new Thread(new ThreadStart(GetRepertoryInfoThread));
            threadGetRepertory.Start();
        }


        private void listViewRepertory_DoubleClick(object sender, EventArgs e)
        {
            //获得当前行 
            int iRowCurr = this.listViewRepertory.SelectedItems[0].Index;
            //取得当前行的数据 
            string cid = listViewRepertory.SelectedItems[0].SubItems[1].Text;
            string cName = listViewRepertory.SelectedItems[0].SubItems[2].Text;
            string cNum = listViewRepertory.SelectedItems[0].SubItems[5].Text;
            SellProduct(cid, cName, cNum);
            GetRepertoryInfoP();
            ShowRepertory();
            toLogP("出售" + cName + "成功");
        }      
        #endregion

        #region 动物成长信息操作
        #endregion

        #region 配置文件操作
        private void configReadP()
        {
           
        }

        private void configSaveP()
        {
            
        }

        private void configApplyP()
        {
            
        }

        private void configShowP()
        {
            
        }
        #endregion

        #region 收获结果操作
        #endregion

        #region 状态栏、日志操作
        private void toStatusP(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = msg;
            });
        }

        private void toLogP(string msg)
        {
            string strToLog = DateTime.Now.ToString() + "  " + msg + "\n";
            this.Invoke((MethodInvoker)delegate
            {
                richTextBoxLog.AppendText(strToLog);
                //让文本框获取焦点 
                richTextBoxLog.Focus();
                //设置光标的位置到文本尾 
                richTextBoxLog.Select(richTextBoxLog.TextLength - 1, 0);
                //滚动到控件光标处 
                richTextBoxLog.ScrollToCaret();
            });
        }


        private void lbtnClearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBoxLog.Clear();
        }
        #endregion

        #region 刷经验
            private void lbtnPastureExp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                if (lbtnPastureExp.Text.Equals("刷牧场经验"))
                {
                    pastureExpTimes = Convert.ToInt32(txtPastureExpTimes.Text);
                    autoExp = true;
                    lbtnPastureExp.Text = "停止刷牧场经验";
                }
                else
                {
                    autoExp = false;
                    lbtnPastureExp.Text = "刷牧场经验";
                }
                timer5.Enabled = autoExp;
            }

            private void PastureExp()
            {
                string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_help_pasture";
                //string postData = "num=100&pos=0&type=1";
                string farmTime = TheKey.GetFarmTime();
                string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                string postData = "uId=1&farmTime=" + farmTime + "&pos=1&num=100"
                    + "&pastureKey=" + pastureKey + "&farmKey=null&type=1&uIdx=" + uIdx;
                string content = "";
                try
                {
                    content = HttpChinese.GetHtml(postData, cookie, url);
                }
                catch (Exception e)
                {
                    toLogP(e.Message);
                    toLogP("拍死蚊子失败");
                }
                string getExp = new JsonObject(content).GetValue("addExp");
                autoExp = getExp.Equals("0") ? false : true;
                if (autoExp)
                {
                    toLogP("获取成功" + getExp + "点经验");
                }
                else
                {
                    toLogP("经验已满，停止工作");
                }
            }

            private void PastureExp2()
            {
                string serial = "";
                this.Invoke((MethodInvoker)delegate
                {
                    serial = txtSerialNum.Text.Trim();
                });
                BuyAnimalP(cIdP, 1);
                DonateAnimal(serial);
            }

            private void timer5_Tick(object sender, EventArgs e)
            {
                if (autoExp && pastureExpTimes > 0)
                {
                    if (threadTest == null || !threadTest.IsAlive)
                    {
                        threadTest = new Thread(new ThreadStart(PastureExp2));
                        threadTest.Start();
                        pastureExpTimes--;
                    }
                }
                else if (pastureExpTimes == 0)
                {
                    toLogP("刷经验完成");
                    pastureExpTimes = -1;
                    autoExp = false;
                    this.Invoke((MethodInvoker)delegate
                    {
                        lbtnPastureExp.Text = "刷牧场经验";
                    });
                }
            }
        #endregion


        //工作方式2定时器
        private void timer4_Tick(object sender, EventArgs e)
        {
            timeRunFriends--;
            if (timeRunFriends <= 0)
            {
                threadGetFriendsP = new Thread(new ThreadStart(ListFriends));
                threadGetFriendsP.Start();
                timeRunFriends = Convert.ToInt32(txtTimeFriends.Text.Trim());
            }
            timeGetFriendsFilter--;
            if (timeGetFriendsFilter <= 0)
            {
                if (threadGetFriendsP == null || !threadGetFriendsP.IsAlive)
                {
                    threadGetFriendsFilterP = new Thread(new ThreadStart(FriendsFliterList));
                    threadGetFriendsFilterP.Start();
                    timeGetFriendsFilter = Convert.ToInt32(txtGetFriendsFilter.Text.Trim());
                }
            }
            if (_autoWorkTime < timeToWork)
            {
                //61s进行一次采摘
                if (_autoWorkTime % 31 == 0)
                {
                    if ((threadGetFriendsFilterP == null) || (!threadGetFriendsFilterP.IsAlive))
                    {
                        threadPickFriendsFilterList = new Thread(new ThreadStart(PickFriendsFilterList));
                        threadPickFriendsFilterList.Start();
                    }
                }
                _autoWorkTime++;
            }
            else
            {
                if (_restTime < timeToRest)
                {
                    _restTime++;
                }
                else
                {
                    _autoWorkTime = 0;
                    _restTime = 0;
                }
            }
        }
        //用户信息刷新
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timeUserInfoGo >= _userInfoUpTime)
            {
                threadGetUserInfoP = new Thread(new ThreadStart(GetUserAndFriends));
                threadGetUserInfoP.Start();
                timeUserInfoGo = 0;
            }
            timeUserInfoGo++;
        }
        //状态栏显示定时器
        private void timer3_Tick(object sender, EventArgs e)
        {
            //"                                                                软件已经工作*小时*分钟*秒，休息*小时*分钟*秒;现在时间是2011年1月1日 1:01:01";
            string timeNow = "现在时间是" + DateTime.Now.ToString();
            string timeWork = "软件已经工作" + TimeFormat.FormatTimeToHHMMSS(_autoWorkTime) + ",休息" + TimeFormat.FormatTimeToHHMMSS(_restTime);
            lblTime.Text = "                                                                " + timeWork + ";" + timeNow;
            /*
            if ((TimeFormat.FormatTime(DateTime.Now)) % 30 == 0)
            {
                threadShowAch = new Thread(new ThreadStart(showAch));
                if (!threadShowAch.IsAlive) { threadShowAch.Start(); }
            }
                * */
        }

        private void btnGetGift_Click(object sender, EventArgs e)
        {
            btnGetGift.Enabled = false;
            Thread newThread = new Thread(new ThreadStart(GetVipGiftP));
            newThread.Start();
            btnGetGift.Enabled = true;
        }                

        private void btnHarvest_Click(object sender, EventArgs e)
        {
            btnHarvest.Enabled = false;
            PastureHarvest(showIdP, _pastureStatus);
            _pastureStatus = GetFUserInfo(showIdP);
            showAnimalInfo(_pastureStatus);
            btnHarvest.Enabled = true;
        }

        private void btnPostProduct_Click(object sender, EventArgs e)
        {
            btnPostProduct.Enabled = false;
            PasturePostProduct(showIdP, _pastureStatus);
            _pastureStatus = GetFUserInfo(showIdP);
            showAnimalInfo(_pastureStatus);
            btnPostProduct.Enabled = true;
        }

        private void btnKillMosquitoP_Click(object sender, EventArgs e)
        {
            btnKillMosquitoP.Enabled = false;
            PastureKillMosquitoP(showIdP, _pastureStatus);
            _pastureStatus = GetFUserInfo(showIdP);
            showAnimalInfo(_pastureStatus);
            btnKillMosquitoP.Enabled = true;
        }

        private void btnGetShitsP_Click(object sender, EventArgs e)
        {
            btnGetShitsP.Enabled = false;
            PastureGetShitsP(showIdP, _pastureStatus);
            _pastureStatus = GetFUserInfo(showIdP);
            showAnimalInfo(_pastureStatus);
            btnGetShitsP.Enabled = true;
        }
        
        private void btnAuto2_Click(object sender, EventArgs e)
        {
            if (btnAuto2.Text.Equals("工作方式2"))
            {
                btnAuto2.Text = "停止";
                _autoWookBool = true;
                _autoWorkTime = 0;
                configApplyP();
            }
            else
            {
                btnAuto2.Text = "工作方式2";
                _autoWookBool = false;
            }

            timer4.Enabled = _autoWookBool;
        }

        private void btnBuyAnimalP_Click(object sender, EventArgs e)
        {
            BuyAnimalP(cIdP, 1);
        }       

    }
}
