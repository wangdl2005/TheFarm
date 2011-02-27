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
        JsonObject _friendsP; //好友列表
        Thread threadGetUserInfoP, threadGetFriendsP, threadGetFriendsFilterP;
        JsonObject _friendsFliterP;//可操作好友列表
        JsonObject _bagStatusP;//用户背包信息
        JsonObject _repertoryStatusP;// 用户仓库信息
        string _friendsIdsP = ""; //好友 ID集合
        string _friendsUInXP = "";//好友 uInX集合;
        string cIdP = "1001";//默认动物
        Dictionary<string, string> configDictP = new Dictionary<string, string>();//配置字典
        JsonObject _shopP; //商店
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
                private void PostProductP(string serial)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_post_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "serial=" + serial + "&pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx;
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
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
                }
                private void PostProductP(string serial ,string usrName)
                {
                    string url = runUrl + "/bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_post_product";
                    string farmTime = TheKey.GetFarmTime();
                    string pastureKey = TheKey.GetPastureKey(farmTime, pastureKeyEncodeString);
                    string postData = "serial=" + serial + "&pastureKey=" + pastureKey + "&farmKey=null&farmTime=" + farmTime + "&uIdx=" + uIdx + "&nick=" + HttpUtility.UrlEncode(usrName);
                    try
                    {
                        string content = HttpChinese.GetHtml(postData, cookie, url);
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
                        toLogP("拍死蚊子失败");
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
                    string url = runUrl +  "bbs/source/plugin/qqfarm/core/mymc.php?mod=cgi_enter?";                    
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
                    string url = "http://www.szshbs.com/bbs/source/plugin/qqfarm/core/mync.php?mod=user&act=run";
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
                    return new JsonObject(tempJson);
                }
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
                    string postData = "farmTime=1298529760&pastureKey=5b201abdbaeac64b6827d6e18e1a4c0b60eaba9c&farmKey=null&saleAll=1&uIdx=188880";
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
            string cropStatus = "";
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
                    {/*
                        Land newLand = new Land(thePastureStatus.GetCollection()[i]);
                        bool landDogFlag = true; //true表示可偷，false表示不可偷
                        //自己的地尽管偷
                        //无狗 无狗粮
                        
                        if (landDogFlag)
                        {
                            GetCropStatus(out cropStatus, newLand);
                            if (cropStatus.Equals("无") || cropStatus.Equals("已枯萎") || regexMature.IsMatch(cropStatus) || cropStatus.Equals("已摘取"))
                            {
                            }
                            else
                            {
                                result = Harvest(userId, i.ToString());
                                DoResult doResultItem = new DoResult(result);
                                if (result.Contains("\"farmlandIndex\":" + i.ToString() + ",\"harvest\":"))
                                {
                                    toLog("收获" + friendInfo.userName + "的农场的第" + i.ToString() + "号地的"
                                        + new CropItem(GetCropModel(newLand.a)).cName + "一共" + doResultItem.harvest + "个");

                                    saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                                }
                                else
                                {
                                    toLog("收获" + friendInfo.userName + "的农场的第" + i.ToString() + "号地失败");
                                }

                            }
                        }*/
                    }
                }
                else
                {
                    for (int i = 0; i < thePastureStatus.GetCollection().Count; i++)
                    {/*
                        Land newLand = new Land(thePastureStatus.GetCollection()[i]);
                        bool landDogFlag = true; //true表示可偷，false表示不可偷
                        //无狗 无狗粮
                        if (_autoDog)
                        {
                            landDogFlag = (dogId.Equals("0") || isHungry.Equals("1"));
                            toLog(friendInfo.userName + "的农场有狗,不偷");
                        }
                        if (landDogFlag)
                        {
                            GetCropStatus(out cropStatus, newLand);
                            if (cropStatus.Equals("无") || cropStatus.Equals("已枯萎") || regexMature.IsMatch(cropStatus) || cropStatus.Equals("已摘取"))
                            {
                            }
                            else
                            {
                                result = Steal(userId, i.ToString());
                                DoResult doResultItem = new DoResult(result);
                                if (result.Contains("\"farmlandIndex\":" + i.ToString() + ",\"harvest\":"))
                                {
                                    toLog("摘取" + friendInfo.userName + "的农场的第" + i.ToString() + "号地的"
                                        + new CropItem(GetCropModel(newLand.a)).cName + "一共" + doResultItem.harvest + "个");

                                    saveAch(0, 0, 0, newLand.a, doResultItem.harvest);
                                }
                                else
                                {
                                    toLog("摘取" + friendInfo.userName + "的农场的第" + i.ToString() + "号地失败");
                                }
                            }
                        }*/
                    }
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
            /*
            if (newAnimalStatus.b.Equals("7"))
            {
                cropStatus = "已枯萎";
            }
            else if (newLand.b.Equals("0"))
            {
                cropStatus = "无";
            }
            else if (newLand.m.Equals("0"))
            {

                cropStatus = "第" + (Convert.ToInt32(newLand.j) + 1).ToString() + "季";
            }
            else if (newLand.n.Contains(uIdx))
            {
                cropStatus = "已摘取";
            }
            else
            {
                cropStatus = newLand.m.Equals(newLand.l) ? "已摘完" : (newLand.m + "/" + newLand.k);
            }
             * */
            animalStatus = "";
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
            ListFriends();
        }
        #endregion
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
            getFriendsFliter();
            string exp = "";
            int level = 0;
            FriendFilterP newFriendsFilter = new FriendFilterP();
            if (_friendsFliterP != null)
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
                    exp = _friendsP.GetCollection()[i].GetValue("exp");
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

                /*
                    for (int i = 0; i < _friendsFliter.GetCollection().Count; i++)
                    {
                        newFriendsFilter.userId = _friendsFliter.GetKey(i);
                        newFriendsFilter.doStatus = new DoStatus(new JsonObject(_friendsFliter.GetValue(i)));
                        User _friendsInfo = new User(GetUserModel(newFriendsFilter.userId));
                        ListViewItem lv = new ListViewItem();
                        lv.SubItems[0].Text = (i + 1).ToString();
                        /*
                        lv.SubItems.Add(_friends.GetCollection()[i].GetValue("userId"));
                        lv.SubItems.Add(_friends.GetCollection()[i].GetValue("userName"));
                        exp = _friends.GetCollection()[i].GetValue("exp");
                        exp = FormatExp(Convert.ToInt32(exp),out level);
                        lv.SubItems.Add(level.ToString());
                        lv.SubItems.Add(_friends.GetCollection()[i].GetValue("money"));
                        lv.SubItems.Add(exp);
                         */
                /*
                        string theDoStatus = newFriendsFilter.doStatus.theDoStatus;
                        lv.SubItems.Add(_friendsInfo.userId);
                        lv.SubItems.Add(_friendsInfo.userName);
                        exp = _friendsInfo.exp;
                        exp = FormatExp(Convert.ToInt32(exp), out level);
                        lv.SubItems.Add(level.ToString());
                        lv.SubItems.Add(_friendsInfo.money);
                        lv.SubItems.Add(theDoStatus);
                        lv.SubItems.Add(DateTime.Now.ToString());
                        this.Invoke((MethodInvoker)delegate
                        {
                            listViewFriendsFilter.Items.Add(lv);
                        });
                    }
                    */
            }
        }
        #endregion

        #region 摘取可操作好友列表
        private void PickFriendsFilterList()
        {
            if (_friendsFliterP != null)
            {
                List<string> idList = _friendsFliterP.GetKeys();
                foreach (string id in idList)
                {
                    FriendFilterP newFriendsFilter = new FriendFilterP();
                    newFriendsFilter = getFriendsDoStatusModel(id);
                    /*
                    if (newFriendsFilter.doStatus.theDoStatus.Equals("可偷取") && _autoSteal)
                    {
                        LandHarvest(id, GetFUserInfo(id));
                    }
                    if (newFriendsFilter.doStatus.theDoStatus.Equals("可除草") && _autoWeed)
                    {
                        LandClearWeed(id, GetFUserInfo(id));
                    }
                    if (newFriendsFilter.doStatus.theDoStatus.Equals("可除虫") && _autoWorm)
                    {
                        LandSpraying(id, GetFUserInfo(id));
                    }
                    if (newFriendsFilter.doStatus.theDoStatus.Equals("可浇水") && _autoWater)
                    {
                        LandWater(id, GetFUserInfo(id));
                    }*/
                }

                toLogP("可操作好友列表处理完成");
                FriendsFliterList();
            }
            else
            {
                toLogP("可操作好友列表处理失败");
            }
        }
        #endregion

        #region 可操作好友列表
        private void FriendsFliterList()
        {
            getFriendsFliter();
            toLogP("获取可操作好友列表成功");
            showFriendsFilter();
        }
        #endregion        
        #endregion

        #region 背包信息操作
        #endregion

        #region 商店信息操作
        #endregion

        #region 仓库信息操作
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
        #endregion

        private void btnGetGift_Click(object sender, EventArgs e)
            {
                btnGetGift.Enabled = false;
                Thread newThread = new Thread(new ThreadStart(GetVipGiftP));
                newThread.Start();
                btnGetGift.Enabled = true;
            }

        private void lbtnGetFriendsFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (threadGetFriendsFilterP == null || !threadGetFriendsFilterP.IsAlive)
            {
                threadGetFriendsFilterP = new Thread(new ThreadStart(FriendsFliterList));
                threadGetFriendsFilterP.Start();
            }
        }           

    }
}
