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
using System.Web;
using Json;
using System.Threading;
using MyFarm;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using FarmStatus;
using Item;
using Ini;
using System.Collections.Specialized;
using Time;
using System.Text.RegularExpressions;

namespace MyFarm
{
    public partial class FrmFarm : Form
    {
        #region 变量定义
        private CookieContainer cookie = new CookieContainer();     
        //JsonObject _status_filter = new JsonObject("{}");//可操作好友列表
        JsonObject model; //json模板
        JsonObject _status; //用户农场信息
        JsonObject _farmStatus;//当前农场土地信息
        JsonObject dogJson = new JsonObject();//当前农场狗信息
        JsonObject _friends; //好友列表
        JsonObject _friendsFliter;//可操作好友列表
        JsonObject _bagStatus;//用户背包信息
        JsonObject _repertoryStatus;// 用户仓库信息
        List<Mature> _matureList = new List<Mature>();//成熟列表
        string showId = "";//当前显示农场的用户ID
        string uIdx = "";//登录用户ID
        string uinY = "";//登录用户牧场ID？
        string uName = "";//登录用户姓名
        string _friendsIds = ""; //好友 ID集合
        string _friendsUInX = "";//好友 uInX集合;
        Ach theAch = new Ach();//用于保存收获的类
        string cId = "2";//默认种植的植物的ID 2:萝卜
        string runUrl = "http://www.szshbs.com";
        string ncIni = "NC.ini";//读取的农场作物信息文件名
        string configIni = "Config.ini";//配置文件名
        Dictionary<string, string> configDict = new Dictionary<string, string>();//配置字典
        string farmKeyEncodeString = "fisoirjm285240_jnqpmda#$%&irlq";//farmkey加密字符串
        JsonObject _shop; //商店
        Dictionary<string, Dictionary<string, string>> _crop = new Dictionary<string, Dictionary<string, string>>();
        Thread threadGetUserInfo, threadGetFriends, threadGetMatureList, threadPickMatureList, threadShowAch,threadGetFriendsFilter,threadPickFriendsFilterList;
        //进程   获取用户信息       好友列表查询      成熟列表          偷取好友
        bool _autoWeed = true; //自动除草 
        bool _autoWorm = true; //自动杀虫 
        bool _autoWater = true; //自动浇水 
        bool _autoPlant = true; //自动种植
        bool _autoSteal = true; //自动收获
        bool _autoScarify = true; //自动翻地
        bool _autoSeed = true; //自动购买种子
        bool _autoBag = true; //查看背包
        bool _autoDog = true;
        bool _autoCancel = true;//无经验自动取消除草。。

        bool _autoUserInfoBool = false;//用户信息刷新是否自动执行
        int timeUserInfoGo = 0;//用户信息刷新经过时间
        int _userInfoUpTime = 60 * 5;//用户信息刷新时间

        bool _autoWookBool = false; //是否工作
        int _autoWorkTime = 0;//工作执行时间
        int _restTime = 0;
        int timeInterval = 0;  //扫描每个好友的间隔
        int timeToWork = 0; //工作需执行的时间
        int timeToRest = 0; //休息的时间
        int timeRunFriends = 0;//扫描好友时间
        int timeGetMature = 0;//获取成熟列表时间
        int timeGetFriendsFilter = 10;//获取可操作好友列表时间
        Regex regexMature = new Regex(@"第\d+季");//用来匹配第*季的匹配器
        #endregion
        
        #region 构造方法
        public FrmFarm()
        {
            InitializeComponent();
        }
        public FrmFarm(CookieContainer container)
        {
            InitializeComponent();
            cookie = container;
        }
        #endregion

        #region 页面打开和关闭
        private void FrmFarm_Load(object sender, EventArgs e)
        {
            configRead();
            configApply();
            /*
            //读取_shop信息
            try
            {
                readShop();
            }
            catch (Exception)
            {
                _shop = new JsonObject(ScanShop());
                //保存_shop信息
                saveShop();
            }*/
            //读取_crop
            try
            {
                readCrop();
                toLog("读取植物信息成功");
            }
            catch (Exception)
            {
                try
                {
                    readShop();
                }
                catch (Exception)
                {
                    _shop = new JsonObject(ScanShop());
                    //保存_shop信息
                    saveShop();
                }
            }
            foreach(string key in _crop["种子"].Keys)
            {
                comboBoxAutoPlant.Items.Add(key);
            }
            CropItem tempCropItem = new CropItem(GetCropModel(cId));
            comboBoxAutoPlant.Text = tempCropItem.cName;
            threadGetFriends = new Thread(new ThreadStart(ListFriends));
            //ListFriends();
            threadGetUserInfo = new Thread(new ThreadStart(this.GetUserInfo));
            threadGetFriends.Start();
            threadGetUserInfo.Start();
            Thread threadGetBagInfo = new Thread(new ThreadStart(this.getBagInfo));
            threadGetBagInfo.Start();
            timer2.Enabled = false;
            timer1.Enabled = false;
            timer3.Enabled = true;            
            //getUserInfo();
        }
        private void FrmFarm_FormClosed(object sender, FormClosedEventArgs e)
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
                
        #region 包含http请求的方法操作

        #region 农场操作方法
        /// <summary> 
        /// 铲除
        /// </summary> 
        /// <param name="uid"></param> 
        /// cropStatus=7&fName=kk12345&farmTime=1298276706&tName=kk12345&uIdx=188880&uinY=198880&farmKey=bdb8e9dca0446f88654b8355f6d61386&ownerId=188880&place=0
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Scarify(string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=scarify";
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string postData = "cropStatus=7&fName=" + HttpUtility.UrlEncode(uName) + "&farmTime=" + farmtime
                + "&tName=" + HttpUtility.UrlEncode(tName) + "&uIdx=" + uIdx + "&uinY=" + uinY + "farmKey="
                + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString) + "&ownerId=" + uid + "&place=" + place;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            }
            return result;
        }

        /// <summary> 
        /// 摘取自己菜
        /// </summary> 
        /// <param name="uid"></param> 
        /// uIdx=188880&fName=kk12345&farmTime=1298276615&tName=kk12345&uinY=198880&farmKey=8b39a015ab3dfe4419075181a0514a75&ownerId=188880&place=0
        /// <param name="place"></param> 
        /// <returns></returns> 
        private string Harvest(string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=harvest";
            //string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + _uid + "&farmTime=" + farmtime;
            string postData = "uIdx=" + uIdx + "&fName=" + HttpUtility.UrlEncode(uName) + "&farmTime="
                + farmtime + "&tName=" + HttpUtility.UrlEncode(tName) + "&uinY=" + uinY + "&farmKey="
                + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString) + "&ownerId=" + uid + "&place=" + place;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            } 
            return result;
        }

        /// <summary> 
        /// 偷好友菜 
        /// </summary> 
        /// <param name="uid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        /// uinX=53197&ownerId=43197&fName=%E7%8E%8B%E7%A3%8A&uinY=29991&place=3&uIdx=19991&tName=%E5%91%A8%E5%B2%B3%E7%BF%B0&farmKey=ce1f1ff65fae4437940dd617489421c8&farmTime=1298283128        
        private string Steal(string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string uinX = _friendInfo.uin;
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=scrounge";
            //string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string postData = "uinX=" + uinX + "&ownerId=" + uid + "&fName=" + HttpUtility.UrlEncode(uName) + "&uinY=" + uinY
                + "&place=" + place + "&uIdx=" + uIdx + "&tName=" + HttpUtility.UrlEncode(tName) + "&farmKey="
                + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString) + "&farmTime=" + farmtime;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            } 
            return result;
        }

        /// <summary> 
        /// 除草 
        /// </summary> 
        /// <param name="qq"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        /// uIdx=188880&fName=kk12345&farmTime=1298277215&tName=kk12345&uinY=198880&farmKey=f81276853b2638a9d2a86138b814cc93&ownerId=188880&place=1
        private string ClearWeed(string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=clearWeed";
            //string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string postData = "uIdx=" + uIdx + "&fName=" + HttpUtility.UrlEncode(uName) + "&farmTime="
                + farmtime + "&tName=" + HttpUtility.UrlEncode(tName) + "&uinY=" + uinY + "&farmKey="
                + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString) + "&ownerId=" + uid + "&place=" + place;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            } 
            return result;
        }

        /// <summary> 
        /// 种植 
        /// </summary> 
        /// <param name="cid"></param> 
        /// <param name="uid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        /// uIdx=188880&fName=kk12345&farmTime=1298276973&cId=7&tName=kk12345&uinY=198880&farmKey=c09ae1907a79af16fe369b39d2a655e1&ownerId=188880&place=0
        private string Plant(string cid, string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=planting";
            //string post = "cId=" + cid + "&farmKey=" + GetFarmKey(farmtime) + "&place=" +
            //     HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string postData = "uIdx=" + uIdx + "&fName=" + HttpUtility.UrlEncode(uName) + "&farmTime=" + farmtime
                + "&cId=" + cid + "&tName=" + HttpUtility.UrlEncode(tName) + "&uinY=" + uinY
                + "&farmKey=" + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString) + "&ownerId=" + uid + "&place=" + place;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            } 
            return result;
        }

        /// <summary> 
        /// 浇水 
        /// </summary> 
        /// <param name="uid"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        /// uIdx=188880&fName=kk12345&farmTime=1298277122&tName=kk12345&uinY=198880&farmKey=7363ba61b9bafbe0d00ffe3e9772620c&ownerId=188880&place=2
        private string Water(string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=water";
            //string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string postData = "uIdx=" + uIdx + "&fName=" + HttpUtility.UrlEncode(uName) + "&farmTime=" + farmtime
                + "&tName=" + HttpUtility.UrlEncode(tName) + "&uinY=" + uinY + "&farmKey=" + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString)
                + "&ownerId=" + uid + "&place=" + place;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            } 
            return result;
        }

        /// <summary> 
        /// 杀虫 
        /// </summary> 
        /// <param name="qq"></param> 
        /// <param name="place"></param> 
        /// <returns></returns> 
        /// uIdx=188880&fName=kk12345&farmTime=1298277275&tName=kk12345&uinY=198880&farmKey=b892ada8e8d94e6f35d2a441081cf7cf&ownerId=188880&place=3
        private string Spraying(string uid, string place)
        {
            string farmtime = FarmKey.GetFarmTime();
            User _friendInfo = new User(GetUserModel(uid));
            string tName = _friendInfo.userName;
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=farmlandstatus&act=spraying";
            //string post = "farmKey=" + GetFarmKey(farmtime) + "&place=" + HttpUtility.UrlEncode(place) + "&ownerId=" + uid + "&farmTime=" + farmtime;
            string postData = "uIdx=" + uIdx + "&fName=" + HttpUtility.UrlEncode(uName) + "&farmTime=" + farmtime
                + "&tName=" + HttpUtility.UrlEncode(tName) + "&uinY=" + uinY + "&farmKey=" + FarmKey.GetFarmKey(farmtime, farmKeyEncodeString)
                + "&ownerId=" + uid + "&place=" + place;
            string result = "";
            try
            {
                result = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
            } 
            return result;
        }
        #endregion

        #region 信息操作方法
        #region 获取好友信息
        private void GetFriends()
        {
            string url = "http://www.szshbs.com/bbs/source/plugin/qqfarm/core/mync.php?mod=friend";
            //string postData = "uinY=29991&farmKey=37a509272e0937f4075453bf9c30f9a4&uIdx=19991&farmTime=1298201167";
            string content = "";
            try
            {
                content = HttpChinese.GetHtml(cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("好友信息获取失败");
            }
             
            this._friends = new JsonObject(content);
        }
        #endregion

        #region 获取商店列表
        /// <summary> 
        /// 获取商店列表 
        /// </summary> 
        /// <returns></returns> 
        private string ScanShop()
        {
            string url = "http://www.szshbs.com/bbs/source/plugin/qqfarm/core/mync.php?mod=usertool&act=getSeedInfo";
            string farmTime = FarmKey.GetFarmTime();
            //string postData = "farmKey=651d78aa55f705453876ee4d797e0561&uinY=29991&farmTime=1298264268&uIdx=19991";
            //string postData = "uinY=29991&uIdx=19991&farmTime=1298266368&farmKey=8592329421dc69647af2674acded7574";
            string postData = "uinY=" + uinY + "&uIdx=" + uIdx + "&farmTime=" + farmTime + "&farmKey=" + FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string content = "";
            try
            {
                content = HttpChinese.GetHtml(postData,cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("商店信息获取失败");
            }
            return content;
        }
        #endregion

        #region 购买种子
        private void buySeed(string cid, string number)
        {
            string farmTime = FarmKey.GetFarmTime();
            string farmKey = FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=repertory&act=buySeed";
            //uIdx=19991&cId=2&uinY=29991&number=2&farmTime=1298473054&farmKey=bc8ab86352bf312e86f204366eaf097b
            string postData = "uIdx=" + uIdx + "&cId=" + cid + "uinY=" + uinY + "&number=" + number + "&farmTime=" + farmTime + "&farmKey=" + farmKey;
            string content = "";
            try
            {
                content = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("购买种子失败");
            }
            CropItem newCropItem = new CropItem(GetCropModel(cid));
            if (content.Contains("\"cId\":" + cid))
            {
                toLog("购买" + newCropItem.cName + "种子" + number + "个成功");
            }
            else
            {
                toLog("购买" + newCropItem.cName + "种子" + number + "个失败");
            }
        }
        #endregion

        #region 获取背包信息
        private void getBagInfo()
        {
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=repertory&act=getUserSeed";
            //uIdx=19991&uinY=29991&farmTime=1298471523&farmKey=c69a093d8cd3e289c59b603b1a9d5db8
            string farmTime = FarmKey.GetFarmTime();
            string postData = "uIdx=" + uIdx + "&uinY=" + uinY + "&farmTime=" + farmTime + "&farmKey=" + FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string content = "";
            try
            {
                content = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("背包信息获取失败");
            }
            _bagStatus = new JsonObject(content);
        }
        #endregion

        #region 获取指定用户土地信息
        private JsonObject GetFUserInfo(string id)
        {
            string url = "http://www.szshbs.com/bbs/source/plugin/qqfarm/core/mync.php?mod=user&act=run";
            //string postData = "ownerId=161380&uinY=29991&uIdx=19991&uinX=171380&farmTime=1298272127&farmKey=b52a85425816b04fbe2cd96830ef0e89";
            //ownerId  : 好友ID
            //uinX 	：猜测是牧场ID，可从user的uinLogin获得
            //uIdx	：自己的ID
            //uinY	：可从user的uinLogin获得
            string ownerId = id;
            User _friendInfo = new User(GetUserModel(id));
            string uinX = _friendInfo.uin;
            string farmTime = FarmKey.GetFarmTime();
            string postData = "ownerId=" + ownerId + "&uinY=" + uinY + "&uIdx=" + uIdx
                + "&uinX=" + uinX + "&farmTime=" + farmTime + "&farmKey=" + FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string content = "";
            JsonObject tempJson = new JsonObject();
            try
            {
                content = HttpChinese.GetHtml(postData, cookie, url);
                JsonObject farmJson = new JsonObject(content);
                tempJson = new JsonObject(farmJson.GetValue("farmlandStatus"));
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("指定用户信息获取失败");
            }
            return new JsonObject(tempJson);
        }

        private JsonObject GetFUserInfo(string id, out JsonObject dogJson)
        {
            string url = "http://www.szshbs.com/bbs/source/plugin/qqfarm/core/mync.php?mod=user&act=run";
            //string postData = "ownerId=161380&uinY=29991&uIdx=19991&uinX=171380&farmTime=1298272127&farmKey=b52a85425816b04fbe2cd96830ef0e89";
            //ownerId  : 好友ID
            //uinX 	：猜测是牧场ID，可从user的uinLogin获得
            //uIdx	：自己的ID
            //uinY	：可从user的uinLogin获得
            string ownerId = id;
            User _friendInfo = new User(GetUserModel(id));
            string uinX = _friendInfo.uin;
            string farmTime = FarmKey.GetFarmTime();
            string postData = "ownerId=" + ownerId + "&uinY=" + uinY + "&uIdx=" + uIdx
                + "&uinX=" + uinX + "&farmTime=" + farmTime + "&farmKey=" + FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string content = "";
            JsonObject tempJson = new JsonObject();
            try
            {
                content = HttpChinese.GetHtml(postData, cookie, url);
                JsonObject farmJson = new JsonObject(content);
                tempJson = new JsonObject(farmJson.GetValue("farmlandStatus"));
                dogJson = new JsonObject(farmJson.GetValue("dog"));
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("指定用户信息获取失败");
                dogJson = new JsonObject();
            }
            return new JsonObject(tempJson);
        }
        #endregion

        #region 获取每日礼包
        private void getGift()
        {
            string farmTime = FarmKey.GetFarmTime();
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=Feast&act=getPackage";
            //uinY=29991&actid=10&ownerId=19991&farmKey=bba495ffafc391c5dd2f06318fa2458f&farmTime=1298308687&uIdx=19991
            string postData = "uinY=" + uinY + "&farmKey=" + FarmKey.GetFarmKey(farmTime, farmKeyEncodeString)
                + "&farmTime=" + farmTime + "&uIdx=" + uIdx;
            string content = "";
            try
            {
                content = HttpChinese.GetHtml(postData, cookie, url);
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("每日VIP礼包信息获取失败");
            }
            if (content.Contains("vip"))
            {
                toLog("每日VIP礼包领取成功");
            }
            else
            {
                toLog("每日VIP礼包领取失败");
            }
        }
        #endregion

        #region 获取用户信息
        private void GetUserInfo()
        {
            string url = "http://www.szshbs.com/bbs/source/plugin/qqfarm/core/mync.php?mod=user&act=run";
            string content = "";
            try
            {
                content = HttpChinese.GetHtml(cookie, url);
                this._status = new JsonObject(content);
                uName = _status.GetJson("user").GetValue("userName");
                uIdx = _status.GetJson("user").GetValue("uId");
                uinY = _status.GetJson("user").GetValue("uinLogin");
                model = _status;
                string headPic = model.GetJson("user").GetValue("headPic");
                int level = 0;
                string exp = "";
                exp = _status.GetJson("user").GetValue("exp");
                exp = FormatExp(Convert.ToInt32(exp), out level);
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
                            toLog(e.Message);
                        }
                    }
                }
                this.Invoke((MethodInvoker)delegate
                {
                    lblUserName.Text = _status.GetJson("user").GetValue("userName");
                    lblMoney.Text = _status.GetJson("user").GetValue("money");
                    lblExp.Text = exp;
                    lblLevel.Text = level.ToString();
                });
                toLog("获取用户信息成功");
                //以下获得农场信息
                _farmStatus = new JsonObject(_status.GetValue("farmlandStatus"));
                dogJson = new JsonObject(_status.GetValue("dog"));
                showFarmland(_farmStatus);
                showId = uIdx;
                toLog("获得用户土地信息成功");
                toStatus("当前显示农场为" + uName + "的农场");
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("用户信息获取失败");
            }             
        }
        #endregion

        #region 获取可操作好友列表
        private void getFriendsFliter()
        {
            string farmTime = FarmKey.GetFarmTime();
            string farmKey = FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=friend_1-3";
            //friend%5Fuins=177152%2C171380%2C170910%2C41131%2C51529%2C21212%2C178400%2C23587%2C173687%2C29991%2C97861%2C69758%2C176582%2C21895%2C47487%2C186460%2C53197%2C22445%2C193473%2C100125%2C44755%2C49155%2C94339%2C10728%2C79011%2C196379%2C42576%2C65975%2C25668%2C58280%2C198880%2C197733%2C110460%2C186528%2C22507%2C188238%2C68166
            //&farmKey=1fbdac99ce71af50c778e3987169f5b2&uIdx=19991&cmd=1
            //friend%5Fuids==167152%2C161380%2C160910%2C31131%2C41529%2C11212%2C168400%2C13587%2C163687%2C19991%2C87861%2C59758%2C166582%2C11895%2C37487%2C176460%2C43197%2C12445%2C183473%2C90125%2C34755%2C39155%2C84339%2C728%2C69011%2C186379%2C32576%2C55975%2C15668%2C48280%2C188880%2C187733%2C100460%2C176528%2C12507%2C178238%2C58166
            //&farmTime=1298479254&uinY=29991
            //
            //
            //string friend_Fuins = "friend_uins=177152,171380,170910,41131,51529,21212,178400,23587,173687,29991,97861,69758,176582,21895,47487,186460,53197,22445,193473,100125,44755,49155,94339,10728,79011,196379,42576,65975,25668,58280,198880,197733,110460,186528,22507,188238,68166";
            string friend_Fuins = _friendsUInX;
            friend_Fuins = "friend%5Fuins=" + HttpUtility.UrlEncode(friend_Fuins);
            //string friend_Fuids = "friend_uids=161380,167152,160910,31131,41529,11212,168400,13587,163687,87861,59758,11895,166582,37487,176460,19991,43197,183473,12445,34755,39155,90125,84339,728,69011,186379,55975,32576,15668,48280,100460,187733,178238,12507,58166";
            string friend_Fuids = _friendsIds;
            friend_Fuids = "friend%5Fuids=" + HttpUtility.UrlEncode(friend_Fuids);
            string postData = "uIdx=" + uIdx + "&uinY=" + uinY + "&farmTime=" + farmTime + "&farmKey=" + farmKey + "&cmd=1" + "&" + friend_Fuins + "&" + friend_Fuids;
            //
            //
            string content = "";
            try{
                content = HttpChinese.GetHtml(postData, cookie, url);
                _friendsFliter = new JsonObject(content).GetJson("status");
            }
            catch(Exception except)
            {
                toLog(except.Message);
                toLog("可摘取好友信息获取失败");
            }
        }
        #endregion

        #region 获取仓库信息
        private void GetRepertoryInfo()
        {
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=repertory&act=getUserCrop";
           //uIdx=188880&uinY=198880&farmTime=1298276786&farmKey=262c0f6d7598f7f0a7c655767d3021bb
            string farmTime = FarmKey.GetFarmTime();
            string farmKey = FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string postData = "uIdx=" + uIdx + "&uinY=" + uinY + "&farmTime=" + farmTime + "&farmKey=" + farmKey;
            try
            {
                string content = HttpChinese.GetHtml(postData, cookie, url);
                _repertoryStatus = new JsonObject(content); 
                toLog("获取仓库信息成功");
            }
            catch (Exception except)
            {
                toLog(except.Message);
                toLog("获取仓库信息失败");
            }
        }
        #endregion

        #region 卖掉仓库物品
        //cids为欲卖掉的物品id集合，如：2,3
        private void SellProducts(string cids)
        {
            string farmTime = FarmKey.GetFarmTime();
            string farmKey = FarmKey.GetFarmKey(farmTime, farmKeyEncodeString);
            string url = runUrl + "/bbs/source/plugin/qqfarm/core/mync.php?mod=repertory&act=saleAll";
            string postData = "farmTime=" + farmTime + "&cIds=" + HttpUtility.UrlEncode(cids)
                + "&onlineTime=0&farmKey=" + farmKey + "&uId=" + uIdx + "&uIdx=" + uIdx + "&uinY=" + uinY;
            try
            {
                string content = HttpChinese.GetHtml(postData, cookie, url);
                toLog("卖掉仓库物品成功" + "，获得" + new JsonObject(content).GetValue("money") + "金币");
            }
            catch(Exception except)
            {
                toLog(except.Message);
                toLog("卖掉仓库物品失败");
            }
        }
        #endregion
        #endregion
        #endregion

        #region 农场操作方法调用函数
        private void LandHarvest(string userId, JsonObject theFarmStatus)
        {
            string cropStatus = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            string dogId = "";
            string isHungry = "";
            dogId = dogJson.GetValue("dogId");
            isHungry = dogJson.GetValue("isHungry");
            if (!userId.Equals(""))
            {
                if (userId.Equals(uIdx))
                {
                    for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                    {
                        Land newLand = new Land(theFarmStatus.GetCollection()[i]);
                        bool landDogFlag = true; //true表示可偷，false表示不可偷
                        //自己的地尽管偷
                        //无狗 无狗粮
                       /*
                        if (_autoDog)
                        {
                            landDogFlag = (dogId.Equals("0") || isHungry.Equals("1"));
                            toLog(friendInfo.userName + "的农场有狗,不偷");
                        }*/
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
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                    {
                        Land newLand = new Land(theFarmStatus.GetCollection()[i]);
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
                        }
                    }
                }
            }
        }

        private void LandClearWeed(string userId, JsonObject theFarmStatus)
        {
            string weed = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            if (!userId.Equals(""))
            {
                for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                {
                    Land newLand = new Land(theFarmStatus.GetCollection()[i]);
                    weed = newLand.f;
                    if (!weed.Equals("0"))//有草
                    {
                        for (int j = 0; j < Convert.ToInt32(weed); j++)
                        {
                            result = ClearWeed(userId, i.ToString());
                            if (result.Contains("\"farmlandIndex\":" + i.ToString()))
                            {
                                toLog("除掉" + friendInfo.userName + "的农场的第" + i.ToString() + "号地的草成功");

                                saveAch(0, 1, 0, "", "0");
                            }
                            else
                            {
                                toLog("除掉" + friendInfo.userName + "的农场的第" + i.ToString() + "号地的草失败");
                            }
                        }
                    }
                }
            }
        }

        private void LandSpraying(string userId, JsonObject theFarmStatus)
        {
            string worm = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            if (!userId.Equals(""))
            {
                for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                {
                    Land newLand = new Land(theFarmStatus.GetCollection()[i]);
                    worm = newLand.g;
                    if (!worm.Equals("0"))//有虫
                    {
                        for (int j = 0; j < Convert.ToInt32(worm); j++)
                        {
                            result = Spraying(userId, i.ToString());
                            if (result.Contains("\"farmlandIndex\":" + i.ToString()))
                            {
                                toLog("除掉" + friendInfo.userName + "的农场的第" + i.ToString() + "号地的虫子成功");

                                saveAch(0, 0, 1, "", "0");
                            }
                            else
                            {
                                toLog("除掉" + friendInfo.userName + "的农场的第" + i.ToString() + "号地的虫子失败");
                            }
                        }
                    }
                }
            }
        }

        private void LandWater(string userId, JsonObject theFarmStatus)
        {
            string dry = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            if (!userId.Equals(""))
            {
                for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                {
                    Land newLand = new Land(theFarmStatus.GetCollection()[i]);
                    dry = newLand.h;
                    if (dry.Equals("0"))//干旱
                    {
                        result = Water(userId, i.ToString());
                        if (result.Contains("\"farmlandIndex\":" + i.ToString()))
                        {
                            toLog("对" + friendInfo.userName + "的农场的第" + i.ToString() + "号地浇水成功");

                            saveAch(1, 0, 0, "", "0");
                        }
                        else
                        {
                            toLog("对" + friendInfo.userName + "的农场的第" + i.ToString() + "号地浇水失败");
                        }
                    }
                }
            }
        }

        private void LandScaify(string userId, JsonObject theFarmStatus)
        {
            string kuwei = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            //需要判断枯萎时的变量
            if (!userId.Equals(""))
            {
                for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                {
                    Land newLand = new Land(theFarmStatus.GetCollection()[i]);
                    kuwei = newLand.b;//？
                    if (kuwei.Equals("7"))//无植物?
                    {
                        result = Scarify(userId, i.ToString());
                        if (result.Contains("\"farmlandIndex\":" + i.ToString()))
                        {
                            toLog("锄掉" + friendInfo.userName + "的农场的第" + i.ToString() + "号地成功");
                        }
                        else
                        {
                            toLog("锄掉" + friendInfo.userName + "的农场的第" + i.ToString() + "号地失败");
                        }
                        System.Threading.Thread.Sleep(200);
                    }

                }
            }
        }

        private void LandPlant(string userId, JsonObject theFarmStatus)
        {
            string have = "";
            string result = "";
            User friendInfo = new User(GetUserModel(userId));
            CropItem cropInfo = new CropItem(GetCropModel(cId));

            if (!userId.Equals(""))
            {
                for (int i = 0; i < theFarmStatus.GetCollection().Count; i++)
                {
                    Land newLand = new Land(theFarmStatus.GetCollection()[i]);
                    have = newLand.b;
                    if (have.Equals("0"))//无植物
                    {
                        BagItem newBagItem = new BagItem(GetBagItemModel(cId));
                        if (_autoSeed && (Convert.ToInt32(newBagItem.amount) == 0))
                        {
                            buySeed(cId, "1");
                        }
                        else
                        { }
                        result = Plant(cId, userId, i.ToString());
                        if (result.Contains("\"farmlandIndex\":" + i.ToString()) && result.Contains("\"cId\":" + cId.ToString()))
                        {
                            toLog("往" + friendInfo.userName + "的农场的第" + i.ToString() + "号地种植" + cropInfo.cName + "成功");
                        }
                        else
                        {
                            toLog("往" + friendInfo.userName + "的农场的第" + i.ToString() + "号地种植" + cropInfo.cName + "失败");
                        }
                    }
                }
            }
        }

        private void MatureListPickThread()
        {
            MatureListPick();
            MatureListRefresh();
            MatureListSort(_matureList);
            MatureListDeal();
            MatureListShow();
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

        #region 商品、crop信息操作
        #region 读取保存商品信息
        private void readShop()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream readStream = new FileStream("shop.bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _shop = (JsonObject)formatter.Deserialize(readStream);
                readStream.Close();
            }
            catch (Exception except)
            {
                throw except;
            }
        }

        private void saveShop()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream writeStream = new FileStream("shop.bin", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                formatter.Serialize(writeStream, _shop);
                writeStream.Close();
            }
            catch (Exception except)
            {
                throw except;
            }
        }
        #endregion

        #region 读取保存的crop信息
        private void readCrop()
        {
            try
            {
                string filePath = System.IO.Path.GetFullPath(@ncIni);
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
                _crop = temp;
            }
            catch (Exception except)
            {
                throw except;
            }
        }
        #endregion

        #region 增加商品信息
        private void addShopInfo(ShopItem shopItem)
        {
            _shop.GetCollection().Add(shopItem.shopInfo);
            saveShop();
        }
        #endregion

        #region 获得指定植物的信息
        private Dictionary<string, string> GetCropModel(string cid)
        {
            if (_crop.ContainsKey(cid))
            {
                return _crop[cid];
            }
            return null;
        }
        #endregion

        #region 获得指定商品信息
        /// <summary> 
        /// 获得指定商品信息 
        /// </summary> 
        /// <param name="cid"></param> 
        /// <returns></returns> 
        private JsonObject GetShopModel(string cid)
        {
            for (int x = 0; x < _shop.GetCollection().Count; x++)
            {
                if (_shop.GetCollection()[x].GetValue("cId").Equals(cid))//不是空的
                {
                    return _shop.GetCollection()[x];
                }
            }
            return null;
        }
        #endregion     
        #endregion

        #region 好友操作
        #region 显示好友列表信息
        private void ListFriends()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewFriends.Items.Clear();
            });
            GetFriends();
            string exp = "";
            int level = 0;
            _friendsIds = "";
            _friendsUInX = "";

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
            for (int i = 0; i < _friends.GetCollection().Count; i++)
            {
                if (_friendsIds != "") _friendsIds += ",";
                if (_friendsUInX != "") _friendsUInX += ",";
                _friendsIds += _friends.GetCollection()[i].GetValue("userId");
                _friendsUInX += _friends.GetCollection()[i].GetValue("uin");
                User _friendsInfo = new User(_friends.GetCollection()[i]);
;
                exp = _friends.GetCollection()[i].GetValue("exp");
                exp = FormatExp(Convert.ToInt32(exp), out level);
                //string theDoStatus = newFriendsFilter.doStatus.theDoStatus;
                dr = dt.NewRow();
                dr[0] = (i + 1).ToString();
                dr[1] = _friendsInfo.userId;
                dr[2] = _friendsInfo.userName;
                dr[3] = level.ToString();
                dr[4] = _friendsInfo.money;
                dr[5] = exp;
                dr[6] = DateTime.Now.ToString();

                dt.Rows.Add(dr);                

                //ListViewItem lv = new ListViewItem();
                //lv.SubItems[0].Text = (i + 1).ToString();
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
                lv.SubItems.Add(_friendsInfo.userId);
                lv.SubItems.Add(_friendsInfo.userName);
                exp = _friendsInfo.exp;
                exp = FormatExp(Convert.ToInt32(exp), out level);
                lv.SubItems.Add(level.ToString());
                lv.SubItems.Add(_friendsInfo.money);
                lv.SubItems.Add(exp);
                lv.SubItems.Add(DateTime.Now.ToString());
                this.Invoke((MethodInvoker)delegate
                {
                    listViewFriends.Items.Add(lv);
                });*/
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
            toLog("获取好友信息成功");
        }
        #endregion 
                
        #region 获取指定好友可操作信息
        private FriendFilter getFriendsDoStatusModel(string cid)
        {
            FriendFilter tmp = new FriendFilter();
            for (int i = 0; i < _friendsFliter.GetCollection().Count; i++)
            {
                if (_friendsFliter.GetCollection()[i].Key.Equals(cid))
                {
                    tmp.userId = _friendsFliter.GetKey(i);
                    tmp.doStatus = new DoStatus(new JsonObject(_friendsFliter.GetValue(i)));
                    return tmp;
                }
            }
            return null;
        }
        #endregion
        #endregion

        #region 可操作好友列表操作
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
            FriendFilter newFriendsFilter = new FriendFilter();
            if (_friendsFliter != null)
            {
                //双缓冲实现
                //// create a temp dataTable to store data
                DataTable dt = new DataTable();
                DataRow dr;
                dt.Columns.Add("id",typeof(String));
                dt.Columns.Add("userId", typeof(String));
                dt.Columns.Add("userName", typeof(String));
                dt.Columns.Add("level", typeof(String));
                dt.Columns.Add("money", typeof(String));
                dt.Columns.Add("canDoStatus",typeof(String));
                //dt.Columns.Add("exp", typeof(String));
                dt.Columns.Add("lastTime",typeof(String));
	            for(int i = 0 ;i< _friendsFliter.GetCollection().Count; i++)
                {
                    newFriendsFilter.userId = _friendsFliter.GetKey(i);
                    newFriendsFilter.doStatus = new DoStatus(new JsonObject(_friendsFliter.GetValue(i)));
                    User _friendsInfo = new User(GetUserModel(newFriendsFilter.userId));
                    exp = _friends.GetCollection()[i].GetValue("exp");
                    exp = FormatExp(Convert.ToInt32(exp),out level);
                    string theDoStatus = newFriendsFilter.doStatus.theDoStatus;
                    dr = dt.NewRow();
                    dr[0] = (i+1).ToString();
                    dr[1] = _friendsInfo.userId;
                    dr[2] = _friendsInfo.userName;
                    dr[3] = level.ToString();
                    dr[4] = _friendsInfo.money;
                    dr[5] = theDoStatus;
                    dr[6] = DateTime.Now.ToString();

                    dt.Rows.Add(dr);
                }


                // loop the temp table , and insert to ListView
                int iSize = (dt.Rows.Count>1000)?1000:dt.Rows.Count;
                   
                ListViewItem lvi;
                ListViewItem[] lvitems = new ListViewItem[iSize];
                for (int i = 0; i < iSize; i++)                    
                {
                    lvi = new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(),dt.Rows[i][4].ToString(),dt.Rows[i][5].ToString(),dt.Rows[i][6].ToString()});
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
            if (_friendsFliter != null)
            {
                List<string> idList = _friendsFliter.GetKeys();
                foreach (string id in idList)
                {
                    FriendFilter newFriendsFilter = new FriendFilter();
                    newFriendsFilter = getFriendsDoStatusModel(id);
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
                    }
                }

                toLog("可操作好友列表处理完成");
                FriendsFliterList();
            }
            else
            {
                toLog("可操作好友列表处理失败");
            }
        }
        #endregion

        #region 可操作好友列表
        private void FriendsFliterList()
        {
            getFriendsFliter();
            toLog("获取可操作好友列表成功");
            showFriendsFilter();
        }
        #endregion        
        #endregion
        
        #region 用户信息操作
        #region 获取指定用户信息
        private JsonObject GetUserModel(string cid)
        {
            for (int x = 0; x < _friends.GetCollection().Count; x++)
            {
                if (_friends.GetCollection()[x].GetValue("userId").Equals(cid))//不是空的
                {
                    return _friends.GetCollection()[x];
                }
            }
            return null;
        }
        #endregion        

        #region 获取用户信息，并自动进行操作
        private void GetUserAndDo()
        {
            GetUserInfo();
            if (_autoBag)
            {
                getBagInfo();
            }
            UserLandAuto();
        }
        #endregion
        #endregion

        #region 土地信息操作
        #region 显示指定的农场土地信息
        private void showFarmland(JsonObject farmStatus)
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewFarmland.Items.Clear();
            });
            string landStatus = "";
            string cropStatus = "";
            for (int i = 0; i < farmStatus.GetCollection().Count; i++)
            {
                ListViewItem lv = new ListViewItem();
                lv.SubItems[0].Text = i.ToString();
                Land newLand = new Land(farmStatus.GetCollection()[i]);
                CropItem newCropItem = new CropItem(GetCropModel(newLand.a));
                //lv.SubItems.Add(newLand.b.Equals("0") ? "空地":newShopItem.cName);
                landStatus = GetLandStatus(newLand, newCropItem);
                lv.SubItems.Add(landStatus);
                lv.SubItems.Add(newLand.f.Equals("0") ? "无" : newLand.f);
                lv.SubItems.Add(newLand.g.Equals("0") ? "无" : newLand.g);
                lv.SubItems.Add(newLand.h.Equals("0") ? "是" : "否");
                GetCropStatus(out cropStatus, newLand);
                lv.SubItems.Add(cropStatus);
                lv.SubItems.Add(TimeFormat.FormatTime((Convert.ToInt64(newLand.q) + Convert.ToInt64(newCropItem.growthCycle)).ToString()));
                this.Invoke((MethodInvoker)delegate
                {
                    listViewFarmland.Items.Add(lv);
                });
            }
        }
        #endregion

        #region 通过land信息判断农作物信息
        private string GetLandStatus(Land newLand, CropItem newCropItem)
        {
            string landStatus;
            if (newLand.b.Equals("0"))
            {
                landStatus = "空地";
            }
            else if (newLand.b.Equals("7"))
            {
                landStatus = "枯萎";
            }
            else
            {
                landStatus = newCropItem.cName;
            }
            return landStatus;
        }
        #endregion

        #region 通过land信息判断土地成熟信息
        private void GetCropStatus(out string cropStatus, Land newLand)
        {
            if (newLand.b.Equals("7"))
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
        }
        #endregion

        #region 当前农场自动收获，除草，除虫，浇水，锄地，播种
        private void UserLandAuto()
        {
            if (_autoSteal)
            {
                LandHarvest(showId, _farmStatus);
            }
            if (_autoWeed)
            {
                LandClearWeed(showId, _farmStatus);
            }
            if (_autoWorm)
            {
                LandSpraying(showId, _farmStatus);
            }
            if (_autoWater)
            {
                LandWater(showId, _farmStatus);
            }
            System.Threading.Thread.Sleep(5000);
            if (_autoScarify)
            {
                LandScaify(showId, _farmStatus);
            }
            System.Threading.Thread.Sleep(5000);
            if (_autoPlant)
            {
                LandPlant(showId, _farmStatus);
            }
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
        }
        #endregion
        #endregion

        #region 背包信息操作
        #region 获取背包特定id信息
        private JsonObject GetBagItemModel(string cid)
        {
            for (int x = 0; x < _bagStatus.GetCollection().Count; x++)
            {
                if (_bagStatus.GetCollection()[x].GetValue("cId") != null&&_bagStatus.GetCollection()[x].GetValue("cId").Equals(cid))//不是空的
                {
                    return _bagStatus.GetCollection()[x];
                }
            }
            return null;
        }
        #endregion        
        #endregion

        #region 成熟信息操作
        #region 获取成熟列表
        private void GetMatureList()
        {
            _matureList.Clear();
            for (int i = 0; i < _friends.GetCollection().Count; i++)
            {
                User _friendsInfo = new User(_friends.GetCollection()[i]);
                toLog("正在侦查" + _friendsInfo.userName + "的农场");
                JsonObject farmStatus = GetFUserInfo(_friendsInfo.userId);
                for (int j = 0; j < farmStatus.GetCollection().Count; j++)
                {
                    Mature matureStatus = new Mature();
                    /*
                    //JsonObject matureStatus = new JsonObject();
                    Land newLand = new Land(farmStatus.GetCollection()[j]);
                    ShopItem newShopItem = new ShopItem(GetShopModel(newLand.a));
                    //matureStatus.Add("userId", _friendsInfo.userId);
                    //matureStatus.Add("userName", _friendsInfo.userName);
                    //matureStatus.Add("place", j.ToString());
                    //matureStatus.Add("cName", newShopItem.cName);
                    //matureStatus.Add("cStatus", newLand.m.Equals("0") ? "未成熟" : (newLand.m + "/" + newLand.k));
                    //matureStatus.Add("cTime", (Convert.ToInt64(newLand.q) + Convert.ToInt64(newShopItem.growthCycle)).ToString());
                    matureStatus.userId = _friendsInfo.userId;
                    matureStatus.userName = _friendsInfo.userName;
                    matureStatus.place = j;
                    landStatus = GetLandStatus(newLand, newShopItem);
                    matureStatus.cName = landStatus;
                    GetCropStatus(out cropStatus, newLand);
                    matureStatus.cStatus = cropStatus;
                    matureStatus.cTime = Convert.ToInt64(newLand.q) + Convert.ToInt64(newShopItem.growthCycle);
                     * */
                    matureStatus = GetMature(_friendsInfo, j);
                    MatureListDoWork(matureStatus);
                    _matureList.Add(matureStatus);
                }
                toLog("完成侦查" + _friendsInfo.userName + "的农场");
                System.Threading.Thread.Sleep(timeInterval * 1000);
            }
        }
        #endregion

        #region 读取、保存成熟列表
        private void MatureListRead()
        {
            try
            {
                //读取_matureList
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(uName + "_matureList.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                _matureList = (List<Mature>)formatter.Deserialize(stream);
                stream.Close();
                toLog("读取成熟列表成功");
                MatureListShow();
            }
            catch(Exception except)
            {
                toLog(except.Message);
                toLog("读取成熟列表失败");
            }
        }

        private void MatureListSave()
        {
            if (_matureList != null)
            {
                try
                {
                    //将matureList序列化保存
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(uName + "_matureList.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, _matureList);
                    stream.Close();
                    toLog("保存成熟列表成功");
                }
                catch (Exception except)
                {
                    toLog(except.Message);
                    toLog("保存成熟列表失败");
                }
            }
            else
            {
                MessageBox.Show("请先获取成熟列表");
            }
        }
        #endregion

        #region 通过用户User，place 获取成熟信息
        private Mature GetMature(User _friendsInfo, int place)
        {
            JsonObject farmStatus = GetFUserInfo(_friendsInfo.userId, out dogJson);
            Land newLand = new Land(farmStatus.GetCollection()[place]);
            CropItem newCropItem = new CropItem(GetCropModel(newLand.a));
            string landStatus = "";
            string cropStatus = "";
            string dogId = "";
            string isHungry = "";
            dogId = dogJson.GetValue("dogId");
            isHungry = dogJson.GetValue("isHungry");
            Mature m = new Mature();
            m.userId = _friendsInfo.userId;
            m.userName = _friendsInfo.userName;
            m.place = place;
            landStatus = GetLandStatus(newLand, newCropItem);
            m.cName = landStatus;
            GetCropStatus(out cropStatus, newLand);
            m.cStatus = cropStatus;
            m.cId = newLand.a;
            m.cWeed = newLand.f;//f > 0有草
            m.cWorm = newLand.g;//g > 0有虫
            m.cDry = newLand.h.Equals("0") ? "Yes" : "No";// h = 0干旱
            m.hasDog = (dogId.Equals("0") || isHungry.Equals("1")) ? "No" : "Yes";
            //无狗或者无狗粮时等于没有狗
            m.cTime = Convert.ToInt64(newLand.q) + Convert.ToInt64(newCropItem.growthCycle) + Convert.ToInt64(newCropItem.growthCycleNext) * Convert.ToInt64(newLand.j);
            return m;
        }
        #endregion

        #region 检查Mature进行除草，除虫，浇水动作
        private void MatureListDoWork(Mature m)
        {
            int numWeed = Convert.ToInt32(m.cWeed);
            int numWorm = Convert.ToInt32(m.cWorm);
            string isDry = m.cDry;
            string result = "";
            if (_autoWeed && (numWeed > 0))
            {
                for (int i = 0; i < numWeed; i++)
                {
                    result = ClearWeed(m.userId, m.place.ToString());
                    DoResult doResultItem = new DoResult(result);
                    if (_autoCancel && doResultItem.exp.Equals("0"))
                    {
                        _autoWorm = false;
                        _autoWeed = false;
                        _autoWater = false;
                        toLog("无经验自动取消除草等操作");
                    }
                    else if (result.Contains("\"farmlandIndex\":" + m.place.ToString()))
                    {
                        toLog("除掉" + m.userName + "的农场的第" + m.place.ToString() + "号地的草成功");
                        saveAch(0, 1, 0, m.cId, "0");
                    }
                    else
                    {
                        toLog("除掉" + m.userName + "的农场的第" + m.place.ToString() + "号地的草失败");
                    }
                }
            }
            if (_autoWorm && (numWorm > 0))
            {
                for (int i = 0; i < numWorm; i++)
                {
                    result = Spraying(m.userId, m.place.ToString());
                    DoResult doResultItem = new DoResult(result);
                    if (_autoCancel && doResultItem.exp.Equals("0"))
                    {
                        _autoWorm = false;
                        _autoWeed = false;
                        _autoWater = false;
                        toLog("无经验自动取消除草等操作");
                    }
                    else if (result.Contains("\"farmlandIndex\":" + m.place.ToString()))
                    {
                        toLog("除掉" + m.userName + "的农场的第" + m.place.ToString() + "号地的虫子成功");
                        saveAch(0, 0, 1, m.cId, "0");
                    }
                    else
                    {
                        toLog("除掉" + m.userName + "的农场的第" + m.place.ToString() + "号地的虫子失败");
                    }
                }
            }
            if (_autoWater && isDry.Equals("Yes"))
            {
                result = Water(m.userId, m.place.ToString());
                DoResult doResultItem = new DoResult(result);
                if (_autoCancel && doResultItem.exp.Equals("0"))
                {
                    _autoWorm = false;
                    _autoWeed = false;
                    _autoWater = false;
                    toLog("无经验自动取消除草等操作");
                }
                else if (result.Contains("\"farmlandIndex\":" + m.place.ToString()))
                {
                    toLog("对" + m.userName + "的农场的第" + m.place.ToString() + "号地浇水成功");
                    saveAch(1, 0, 0, m.cId, "0");
                }
                else
                {
                    toLog("对" + m.userName + "的农场的第" + m.place.ToString() + "号地浇水失败");
                }
            }
        }
        #endregion

        #region 成熟列表排序
        private static int Compare(Mature v1, Mature v2)
        {
            if (v1 != null && v2 != null)
            {
                return v1.cTime.CompareTo(v2.cTime);
            }
            return 0;
        }
        private void MatureListSort(List<Mature> matureList)
        {
            matureList.Sort(Compare);
        }
        #endregion

        #region 成熟列表处理
        //处理成熟列表中已经摘完或者土地为空的信息根据配置处理掉有狗
        private void MatureListDeal()
        {
            long sec = TimeFormat.FormatTime(DateTime.Now);
            List<Mature> _matureListTemp = new List<Mature>();
            Mature temp = new Mature();
            for (int i = 0; i < _matureList.Count; i++)
            {
                temp = _matureList[i];
                if (temp.userId.Equals(uIdx))
                {
                    continue;
                }
                else if (temp.cTime <= sec)
                {
                    //判断是否已经摘完
                    //判断是否为空
                    
                    if (temp.cStatus.Equals("无") || temp.cStatus.Equals("已枯萎") || temp.cStatus.Equals("已摘完") || regexMature.IsMatch(temp.cStatus) || temp.cStatus.Equals("已摘取"))
                    {
                        continue;
                    }
                    else if (_autoDog && temp.hasDog.Equals("Yes"))//设置自动防狗的话
                    {
                        continue;
                    }
                    else
                    {
                        _matureListTemp.Add(temp);
                    }
                }
                else
                {
                    _matureListTemp.Add(temp);
                }
            }
            _matureList = _matureListTemp;
        }
        #endregion

        #region 显示成熟列表
        private void MatureListShow()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewMatureList.Items.Clear();
            });
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add("id",typeof(String));
            dt.Columns.Add("userId", typeof(String));
            dt.Columns.Add("userName", typeof(String));
            dt.Columns.Add("place", typeof(String));
            dt.Columns.Add("cName", typeof(String));
            //dt.Columns.Add("canDoStatus",typeof(String));
            dt.Columns.Add("cStatus", typeof(String));
            dt.Columns.Add("lastTime",typeof(String));
            int i = 0;
            foreach (Mature m in _matureList)
            {
                dr = dt.NewRow();
                dr[0] = (i + 1).ToString();
                dr[1] = m.userId;
                dr[2] = m.userName;
                dr[3] = m.place.ToString();
                dr[4] = m.cName;
                dr[5] = m.cStatus;
                dr[6] = TimeFormat.FormatTime(m.cTime.ToString());

                dt.Rows.Add(dr);
                i++;
            }
            int iSize = (dt.Rows.Count > 1000) ? 1000 : dt.Rows.Count;

            ListViewItem lvi;
            ListViewItem[] lvitems = new ListViewItem[iSize];
            for (i = 0; i < iSize; i++)
            {
                lvi = new ListViewItem(new string[] { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString() });
                lvitems[i] = lvi;
            }
            this.Invoke((MethodInvoker)delegate
            {
                listViewMatureList.Items.AddRange(lvitems);
            });
            /*
            int i = 1;
            foreach (Mature m in _matureList)
            {
                ListViewItem lv = new ListViewItem();
                lv.SubItems[0].Text = i.ToString();
                lv.SubItems.Add(m.userId);
                lv.SubItems.Add(m.userName);
                lv.SubItems.Add(m.place.ToString());
                lv.SubItems.Add(m.cName);
                lv.SubItems.Add(m.cStatus);
                lv.SubItems.Add(TimeFormat.FormatTime(m.cTime.ToString()));
                this.Invoke((MethodInvoker)delegate
                {
                    listViewMatureList.Items.Add(lv);
                });
                i++;
            }
             * */
        }
        #endregion

        #region 摘取成熟列表
        private void MatureListPick()
        {
            long sec = TimeFormat.FormatTime(DateTime.Now);
            if (_matureList.Count == 0)
            {
                MessageBox.Show("请先获取成熟列表");
            }
            else
            {
                Mature temp = new Mature();
                string result = "";
                for (int i = 0; i < _matureList.Count; i++)
                {
                    temp = _matureList[i];
                    if (temp.cTime <= sec)
                    {
                        //判断是否已经摘完
                        //判断是否为空
                        //判断是否已经摘取过
                        if (!_autoSteal || temp.cStatus.Equals("无") || temp.cStatus.Equals("已枯萎") || temp.cStatus.Equals("已摘完") || regexMature.IsMatch(temp.cStatus) || temp.cStatus.Equals("已摘取"))
                        {
                            continue;
                        }//_autoSteal 自动偷 
                        else
                        {
                            result = Steal(temp.userId, temp.place.ToString());
                            DoResult doResultItem = new DoResult(result);
                            if (result.Contains("\"farmlandIndex\":" + temp.place.ToString() + ",\"harvest\":"))
                            {
                                toLog("摘取" + temp.userName + "的农场的第" + temp.place.ToString() + "号地" + temp.cName + "共" + doResultItem.harvest + "个");
                                saveAch(0, 0, 0, temp.cId, doResultItem.harvest);
                            }
                            else
                            {
                                toLog("摘取" + temp.userName + "的农场的第" + temp.place.ToString() + "号地" + temp.cName + "失败");
                            }
                            _matureList.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region 成熟列表更新
        private void MatureListRefresh()
        {
            long seconds = TimeFormat.FormatTime(DateTime.Now);
            Mature m = new Mature();
            for (int i = 0; i < _matureList.Count; i++)
            {
                m = _matureList[i];
                if (m.cTime <= seconds)
                {
                    //获取指定用户指定土地信息
                    m = GetMature(new User(GetUserModel(m.userId)), Convert.ToInt32(m.place));
                }
                else
                {
                    break;
                }
                _matureList[i] = m;
            }
        }
        #endregion

        #region 成熟列表刷新
        private void MatureListRefreshThread()
        {
            MatureListRefresh();
            MatureListSort(_matureList);
            MatureListDeal();
            MatureListShow();
        }
        #endregion

        #region 成熟列表
        private void MatureList()
        {
            GetMatureList();
            toLog("获取成熟列表成功");
            this.Invoke((MethodInvoker)delegate {
                lbtnGetMatureList.Text = "获取成熟列表";
            });
            MatureListSort(_matureList);
            toLog("成熟列表排序成功");
            MatureListDeal();
            toLog("成熟列表处理成功");
            MatureListShow();
        }
        #endregion
        #endregion

        #region 收获信息操作
        #region 保存收获
        private void saveAch(int numWater, int numWeed, int numWorm, string cId, string numCrop)
        {
            saveAch(numWater, numWeed, numWorm, getCropGet(cId, numCrop));
        }

        private CropGet getCropGet(string cId, string numCrop)
        {
            CropGet temp = new CropGet();
            temp.cropId = cId;
            CropItem tmpCropItem = new CropItem(GetCropModel(cId));
            temp.cropName = tmpCropItem.cName;
            if (cId.Equals(""))
            {
                temp.numCrop = 0;
            }
            else
            {
                temp.numCrop = Convert.ToInt32(numCrop);
            }
            return temp;
        }

        private void saveAch(int numWater, int numWeed, int numWorm, CropGet cropThatGet)
        {
            theAch.numWater += numWater;
            theAch.numWeed += numWeed;
            theAch.numWorm += numWorm;
            bool foundTheSame = false;
            if (theAch.cropList != null && theAch.cropList.Count > 0)
            {
                for (int i = 0; i < theAch.cropList.Count; i++)
                {
                    if (theAch.cropList[i].cropId.Equals(cropThatGet.cropId))
                    {
                        theAch.cropList[i].numCrop += cropThatGet.numCrop;
                        foundTheSame = true;
                        break;
                    }
                }
                if (!foundTheSame)
                {
                    theAch.cropList.Add(cropThatGet);
                }

            }
            else
            {
                theAch.cropList.Add(cropThatGet);
            }
        }
        #endregion

        #region 显示收获
        private void showAch()
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtAch.Clear();
                txtAch.Text = "目前：\n";
                txtAch.AppendText("  已除草" + theAch.numWeed + "根\n");
                txtAch.AppendText("  已除虫" + theAch.numWorm + "只\n");
                txtAch.AppendText("  已浇水" + theAch.numWater + "次\n");
                if (theAch.cropList != null)
                {
                    foreach (CropGet cG in theAch.cropList)
                    {
                        if (!cG.cropId.Equals(""))
                        {
                            txtAch.AppendText("  已偷取" + cG.cropName.Replace("\0", "") + "共" + cG.numCrop + "个\n");
                        }
                    }
                }
            });
        }
        #endregion
        #endregion

        #region 仓库信息操作
        #region 显示仓库物品
        private void ShowRepertory()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewRepertory.Items.Clear();
            });
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
            //这里只显示作物   
            JsonObject crop = new JsonObject(_repertoryStatus.GetValue("crop"));
            for (int i = 0; i < crop.GetCollection().Count; i++)
            {
                string cid = crop.GetCollection()[i].GetValue("cId");
                string cNum = crop.GetCollection()[i].GetValue("amount");
                string isLocked = crop.GetCollection()[i].GetValue("isLock").Equals("0") ? "否" : "是";//0:默认无锁，1加锁，难以判断
                CropItem newCropItem = new CropItem(GetCropModel(cid));
                string cName = newCropItem.cName;
                string cMoney = newCropItem.priceOnSale;
                long totalMoney = Convert.ToInt32(cNum) * Convert.ToInt32(cMoney);
                dr = dt.NewRow();
                dr[0] = (i + 1).ToString();
                dr[1] = cid;
                dr[2] = cName;
                dr[3] = cMoney;
                dr[4] = cNum;
                dr[5] = totalMoney;
                dr[6] = isLocked;
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
            });
        }
        #endregion

        #region 出售仓库未锁定物品
        #endregion

        #region 仓库信息
        private void GetRepertoryInfoThread()
        {
            GetRepertoryInfo();
            ShowRepertory();
        }
        #endregion 
        #endregion

        #region 日志、状态栏信息操作
        #region 处理日志
        private void toLog(string msg)
        {
            string strToLog = DateTime.Now.ToString() + "  " + msg + "\n";
            this.Invoke((MethodInvoker)delegate
            {
                richTextBoxLog.AppendText(strToLog);
                //让文本框获取焦点 
                richTextBoxLog.Focus();
                //设置光标的位置到文本尾 
                richTextBoxLog.Select(richTextBoxLog.TextLength, 0);
                //滚动到控件光标处 
                richTextBoxLog.ScrollToCaret();
            });
        }
        #endregion

        #region 处理状态栏
        private void toStatus(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = msg;
            });
        }
        #endregion
        #endregion

        #region 配置文件Config.ini操作
        private void configRead()
        {
            try
            {
                string filePath = System.IO.Path.GetFullPath(@configIni);
                IniFiles readIni = new IniFiles(filePath);
                string value = "";
                StringCollection keyList = new StringCollection();
                readIni.ReadSection("农场配置", keyList);
                configDict.Clear();
                foreach (string k in keyList)
                {
                    value = readIni.ReadString("农场配置", k, "");
                    configDict.Add(k, value);
                }
                cId = configDict["自动种植植物ID"];
                timeInterval = Convert.ToInt32(configDict["间隔时间"]);
                _autoUserInfoBool = Convert.ToBoolean(configDict["用户自动更新"]);
                _userInfoUpTime = Convert.ToInt32(configDict["用户信息刷新间隔"]);
                timeToWork = Convert.ToInt32(configDict["工作需执行的时间"]);
                timeToRest = Convert.ToInt32(configDict["休息的时间"]);
                timeRunFriends = Convert.ToInt32(configDict["扫描好友时间"]);
                timeGetMature = Convert.ToInt32(configDict["获取成熟列表时间"]);
                timeGetFriendsFilter = Convert.ToInt32(configDict["获取可操作好友时间间隔"]);
                _autoWeed = Convert.ToBoolean(configDict["自动除草"]);
                _autoWorm = Convert.ToBoolean(configDict["自动杀虫"]);
                _autoWater = Convert.ToBoolean(configDict["自动浇水"]);
                _autoPlant = Convert.ToBoolean(configDict["自动种植"]);
                _autoSteal = Convert.ToBoolean(configDict["自动收获"]);
                _autoScarify = Convert.ToBoolean(configDict["自动翻地"]);
                _autoSeed = Convert.ToBoolean(configDict["自动购买种子"]);
                _autoBag = Convert.ToBoolean(configDict["查看背包"]);
                _autoDog = Convert.ToBoolean(configDict["自动防狗"]);
                _autoCancel = Convert.ToBoolean(configDict["无经验自动取消除草等操作"]);
                farmKeyEncodeString = configDict["farmKeyEncodeString"];
            }
            catch (Exception except)
            {
                toLog(except.Message);
            }
        }
        private void configSave()
        {
            try
            {
                string filePath = System.IO.Path.GetFullPath(@configIni);
                IniFiles saveIni = new IniFiles(filePath);
                /*
                 *      timeInterval = Convert.ToInt32(txtTimeInterval.Text.Trim());//间隔时间
                        _autoUserInfoBool = chbUpdata.Checked;//是否用户自动更新
                        _userInfoUpTime = Convert.ToInt32(txtTimeGetUserInfo.Text.Trim());//用户信息刷新间隔
                        timeToWork = Convert.ToInt32(txtTimeWork.Text.Trim()); //工作需执行的时间
                        timeToRest = Convert.ToInt32(txtTimeRest.Text.Trim()); //休息的时间
                        timeRunFriends = Convert.ToInt32(txtTimeFriends.Text.Trim()) * 60;//扫描好友时间
                        timeGetMature = Convert.ToInt32(txtTimeGetMature.Text.Trim());//获取成熟列表时间
                        _autoWeed = chbClearWeed.Checked; //自动除草 
                        _autoWorm = chbSpraying.Checked; //自动杀虫 
                        _autoWater = chbWater.Checked; //自动浇水 
                        _autoPlant = chbPlant.Checked; //自动种植
                        _autoSteal = chbSteal.Checked; //自动收获
                        _autoScarify = chbScarify.Checked; //自动翻地
                        _autoSeed = chbSeed.Checked; //自动购买种子
                        _autoBag = chbBag.Checked; //查看背包
                        _autoDog = chbDog.Checked;//自动防狗
                 * 
                 * 
                 * 
                 * */
                configDict.Clear();
                configDict.Add("自动种植植物ID", cId);
                configDict.Add("自动种植植物名称", new CropItem(GetCropModel(cId)).cName);
                configDict.Add("间隔时间", timeInterval.ToString());
                configDict.Add("用户自动更新", _autoUserInfoBool.ToString());
                configDict.Add("用户信息刷新间隔", _userInfoUpTime.ToString());
                configDict.Add("工作需执行的时间", timeToWork.ToString());
                configDict.Add("休息的时间",timeToRest.ToString());
                configDict.Add("扫描好友时间",timeRunFriends.ToString());
                configDict.Add("获取成熟列表时间", timeGetMature.ToString());
                configDict.Add("自动除草", _autoWeed.ToString());
                configDict.Add("自动杀虫", _autoWorm.ToString());
                configDict.Add("自动浇水", _autoWater.ToString());
                configDict.Add("自动种植", _autoPlant.ToString());
                configDict.Add("自动收获", _autoSteal.ToString());
                configDict.Add("自动翻地", _autoScarify.ToString());
                configDict.Add("自动购买种子", _autoSeed.ToString());
                configDict.Add("查看背包", _autoBag.ToString());
                configDict.Add("自动防狗", _autoDog.ToString());
                configDict.Add("farmKeyEncodeString", farmKeyEncodeString);
                configDict.Add("获取可操作好友时间间隔",timeGetFriendsFilter.ToString());
                configDict.Add("无经验自动取消除草等操作", _autoCancel.ToString());
                string value = "";
                foreach (string key in configDict.Keys)
                { 
                    value = configDict[key];
                    saveIni.WriteString("农场配置", key, value);
                }
            }
            catch (Exception)
            { }
        }
        private void configApply()
        {
            timeGetFriendsFilter = Convert.ToInt32(txtGetFriendsFilter.Text.Trim());//获取可操作好友列表间隔
            timeInterval = Convert.ToInt32(txtTimeInterval.Text.Trim());//间隔时间
            _autoUserInfoBool = chbUpdata.Checked;//是否用户自动更新
            _userInfoUpTime = Convert.ToInt32(txtTimeGetUserInfo.Text.Trim());//用户信息刷新间隔
            timeToWork = Convert.ToInt32(txtTimeWork.Text.Trim()); //工作需执行的时间
            timeToRest = Convert.ToInt32(txtTimeRest.Text.Trim()); //休息的时间
            timeRunFriends = Convert.ToInt32(txtTimeFriends.Text.Trim());//扫描好友时间
            timeGetMature = Convert.ToInt32(txtTimeGetMature.Text.Trim());//获取成熟列表时间
            _autoWeed = chbClearWeed.Checked; //自动除草 
            _autoWorm = chbSpraying.Checked; //自动杀虫 
            _autoWater = chbWater.Checked; //自动浇水 
            _autoPlant = chbPlant.Checked; //自动种植
            _autoSteal = chbSteal.Checked; //自动收获
            _autoScarify = chbScarify.Checked; //自动翻地
            _autoSeed = chbSeed.Checked; //自动购买种子
            _autoBag = chbBag.Checked; //查看背包
            _autoDog = chbDog.Checked;//自动防狗
            _autoCancel = chbCancel.Checked;
        }
        private void configShow()
        {
            /*
                 *      timeInterval = Convert.ToInt32(txtTimeInterval.Text.Trim());//间隔时间
                        _autoUserInfoBool = chbUpdata.Checked;//是否用户自动更新
                        _userInfoUpTime = Convert.ToInt32(txtTimeGetUserInfo.Text.Trim());//用户信息刷新间隔
                        timeToWork = Convert.ToInt32(txtTimeWork.Text.Trim()); //工作需执行的时间
                        timeToRest = Convert.ToInt32(txtTimeRest.Text.Trim()); //休息的时间
                        timeRunFriends = Convert.ToInt32(txtTimeFriends.Text.Trim()) * 60;//扫描好友时间
                        timeGetMature = Convert.ToInt32(txtTimeGetMature.Text.Trim());//获取成熟列表时间
                        _autoWeed = chbClearWeed.Checked; //自动除草 
                        _autoWorm = chbSpraying.Checked; //自动杀虫 
                        _autoWater = chbWater.Checked; //自动浇水 
                        _autoPlant = chbPlant.Checked; //自动种植
                        _autoSteal = chbSteal.Checked; //自动收获
                        _autoScarify = chbScarify.Checked; //自动翻地
                        _autoSeed = chbSeed.Checked; //自动购买种子
                        _autoBag = chbBag.Checked; //查看背包
                        _autoDog = chbDog.Checked;//自动防狗
                 * 
                 * 
                 * 
                 * */
            txtGetFriendsFilter.Text = timeGetFriendsFilter.ToString();
            txtTimeInterval.Text = timeInterval.ToString();
            chbUpdata.Checked = _autoUserInfoBool;
            txtTimeGetUserInfo.Text = _userInfoUpTime.ToString();
            txtTimeWork.Text = timeToWork.ToString();
            txtTimeRest.Text = timeToRest.ToString();
            txtTimeFriends.Text = timeRunFriends.ToString();
            txtTimeGetMature.Text = timeGetMature.ToString();
            chbClearWeed.Checked = _autoWeed;
            chbSpraying.Checked = _autoWorm;
            chbWater.Checked = _autoWater;
            chbPlant.Checked = _autoPlant;
            chbSteal.Checked = _autoSteal;
            chbScarify.Checked = _autoScarify;
            chbSeed.Checked = _autoSeed;
            chbBag.Checked = _autoBag;
            chbDog.Checked = _autoDog;
            chbCancel.Checked = _autoCancel;
            toLog("当前farmkey加密字符串为" + farmKeyEncodeString);
        }
        #endregion

        #region 控件事件
        private void lbtnUpdata_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //getUserInfo();
            threadGetUserInfo = new Thread(new ThreadStart(this.GetUserAndDo));
            threadGetUserInfo.Start();
        }

        private void lbtnClearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBoxLog.Clear();
        }

        private void listViewFriends_DoubleClick(object sender, EventArgs e)
        {
            //获得当前行 
            int iRowCurr = this.listViewFriends.SelectedItems[0].Index;
            //取得当前行的数据 
            string id = listViewFriends.SelectedItems[0].SubItems[1].Text;
            string userName = listViewFriends.SelectedItems[0].SubItems[2].Text;
            _farmStatus = GetFUserInfo(id,out dogJson);
            showFarmland(_farmStatus);
            showId = id;
            toLog("获取" + userName + "土地信息成功" );
            toStatus("当前显示农场为：" + userName + "的农场");
        }


        private void listViewRepertory_DoubleClick(object sender, EventArgs e)
        {
            //获得当前行 
            int iRowCurr = this.listViewRepertory.SelectedItems[0].Index;
            //取得当前行的数据 
            string cid = listViewRepertory.SelectedItems[0].SubItems[1].Text;
            string cName = listViewRepertory.SelectedItems[0].SubItems[2].Text;
            SellProducts(cid);
            GetRepertoryInfo();
            ShowRepertory();
            toLog("出售" + cName + "成功");
        }

        private void lbtnGetMatureList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lbtnGetMatureList.Text.Trim().Equals("获取成熟列表"))
            {
                if (threadGetMatureList == null || !threadGetMatureList.IsAlive)
                {
                    threadGetMatureList = new Thread(new ThreadStart(MatureList));
                    threadGetMatureList.Start();
                    lbtnGetMatureList.Text = "停止获取成熟列表";
                }
            }
            else if (lbtnGetMatureList.Text.Trim().Equals("停止获取成熟列表"))
            {
                if (threadGetMatureList != null && threadGetMatureList.IsAlive)
                {
                    threadGetMatureList.Abort();
                    lbtnGetMatureList.Text = "获取成熟列表";
                    toLog("停止获取成熟列表");
                }
            }
        }

        private void lbtnRefreshMatureList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread threadRefreshMatureList = new Thread(new ThreadStart(MatureListRefreshThread));
            threadRefreshMatureList.Start();
        }

        private void btnPickMature_Click(object sender, EventArgs e)
        {
            if (threadPickMatureList == null || !threadPickMatureList.IsAlive)
            {
                threadPickMatureList = new Thread(new ThreadStart(MatureListPickThread));
                threadPickMatureList.Start();
            }
        }
        
        private void btnHarvest_Click(object sender, EventArgs e)
        {
            btnHarvest.Enabled = false;
            LandHarvest(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnHarvest.Enabled = true;
        }
        
        private void btnClearWeed_Click(object sender, EventArgs e)
        {
            btnClearWeed.Enabled = false;
            LandClearWeed(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnClearWeed.Enabled = true;
        }
        
        private void btnSpraying_Click(object sender, EventArgs e)
        {
            btnSpraying.Enabled = false;
            LandSpraying(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnSpraying.Enabled = true;
        }

        private void btnWater_Click(object sender, EventArgs e)
        {
            btnWater.Enabled = false;
            LandWater(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnWater.Enabled = true;
        }

        private void btnScarify_Click(object sender, EventArgs e)
        {
            btnScarify.Enabled = false;
            LandScaify(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnScarify.Enabled = true;
        }

        private void btnPlant_Click(object sender, EventArgs e)
        {
            btnPlant.Enabled = false;
            LandPlant(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnPlant.Enabled = true;
        }

        private void btnDoAll_Click(object sender, EventArgs e)
        {
            btnDoAll.Enabled = false;
            LandHarvest(showId, _farmStatus);
            LandClearWeed(showId, _farmStatus);
            LandSpraying(showId, _farmStatus);
            LandWater(showId, _farmStatus);
            System.Threading.Thread.Sleep(2000);
            LandScaify(showId, _farmStatus);
            System.Threading.Thread.Sleep(2000);
            LandPlant(showId, _farmStatus);
            _farmStatus = GetFUserInfo(showId);
            showFarmland(_farmStatus);
            btnDoAll.Enabled = true;
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (btnAuto.Text.Equals("工作方式1"))
            {
                btnAuto.Text = "停止";
                _autoWookBool = true;
                _autoWorkTime = 0;
                configApply();                
            }
            else
            {
                btnAuto.Text = "工作方式1";
                _autoWookBool = false;
            }

            timer1.Enabled = _autoWookBool;
        }

        private void btnAuto2_Click(object sender, EventArgs e)
        {
            if (btnAuto2.Text.Equals("工作方式2"))
            {
                btnAuto2.Text = "停止";
                _autoWookBool = true;
                _autoWorkTime = 0;
                configApply();
            }
            else
            {
                btnAuto2.Text = "工作方式2";
                _autoWookBool = false;
            }

            timer4.Enabled = _autoWookBool;
        }

        private void chbUpdata_CheckedChanged(object sender, EventArgs e)
        {
            timer2.Enabled = chbUpdata.Checked;
        }

        private void btnApplyConfig_Click(object sender, EventArgs e)
        {
            configApply();
            toLog("配置文件应用成功");
        }

        private void btnGetGift_Click(object sender, EventArgs e)
        {
            btnGetGift.Enabled = false;
            Thread newThread = new Thread(new ThreadStart(getGift));
            newThread.Start();
            btnGetGift.Enabled = true;
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            configSave();
            toLog("配置文件保存成功");
        }

        private void btnReadConfig_Click(object sender, EventArgs e)
        {
            configRead();
            configShow();
            toLog("配置文件读取成功");
        }
        
        private void comboBoxAutoPlant_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cropName = comboBoxAutoPlant.SelectedItem.ToString();
            string cropId = _crop["种子"][cropName];
            cId = cropId;
        }

        private void lbtnGetFriendsFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (threadGetFriendsFilter==null||!threadGetFriendsFilter.IsAlive)
            {
                threadGetFriendsFilter = new Thread(new ThreadStart(FriendsFliterList));
                threadGetFriendsFilter.Start();
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

        private void lbtnGetFriends_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (threadGetFriends == null || !threadGetFriends.IsAlive)
            {
                threadGetFriends = new Thread(new ThreadStart(ListFriends));
                threadGetFriends.Start();
            }
        }

        private void ltbnReadMatureList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread threadReadMatureList = new Thread(new ThreadStart(MatureListRead));
            threadReadMatureList.Start();
        }

        private void lbtnSaveMatureList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread threadSaveMatureList = new Thread(new ThreadStart(MatureListSave));
            threadSaveMatureList.Start();
        }


        private void lbtnGetRepertory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread threadGetRepertory = new Thread(new ThreadStart(GetRepertoryInfoThread));
            threadGetRepertory.Start();
        }
        #endregion

        #region timer事件
        //工作方式1定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            timeRunFriends--;
            if (timeRunFriends <= 0)
            {
                threadGetFriends = new Thread(new ThreadStart(ListFriends));
                threadGetFriends.Start();
                timeRunFriends = Convert.ToInt32(txtTimeFriends.Text.Trim()) * 60;
            }
            timeGetMature--;
            if (timeGetMature <= 0)
            {
                if (threadGetFriends ==null||!threadGetFriends.IsAlive)
                {
                    threadGetMatureList = new Thread(new ThreadStart(MatureList));
                    threadGetMatureList.Start();
                    timeGetMature = Convert.ToInt32(txtTimeGetMature.Text.Trim());
                }
            }
            if (_autoWorkTime < timeToWork)
            {
                //60s处理一次成熟列表
                if (_autoWorkTime % 60 == 0)
                {
                    if ((threadGetMatureList == null)||(!threadGetMatureList.IsAlive))
                    {
                        threadPickMatureList = new Thread(new ThreadStart(MatureListPickThread));
                        threadPickMatureList.Start();
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
        //用户信息定时器
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timeUserInfoGo >= _userInfoUpTime)
            {
                threadGetUserInfo = new Thread(new ThreadStart(GetUserInfo));
                threadGetUserInfo.Start();
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
            if((TimeFormat.FormatTime(DateTime.Now))%30 == 0)
            {
                threadShowAch = new Thread(new ThreadStart(showAch));
                if (!threadShowAch.IsAlive) { threadShowAch.Start(); }
            }
        }
        //工作方式2定时器
        private void timer4_Tick(object sender, EventArgs e)
        {
            timeRunFriends--;
            if (timeRunFriends <= 0)
            {
                threadGetFriends = new Thread(new ThreadStart(ListFriends));
                threadGetFriends.Start();
                timeRunFriends = Convert.ToInt32(txtTimeFriends.Text.Trim()) * 60;
            }
            timeGetFriendsFilter--;
            if (timeGetFriendsFilter <= 0)
            {
                if (threadGetFriends == null|| !threadGetFriends.IsAlive)
                {
                    threadGetFriendsFilter = new Thread(new ThreadStart(FriendsFliterList));
                    threadGetFriendsFilter.Start();
                    timeGetFriendsFilter = Convert.ToInt32(txtGetFriendsFilter.Text.Trim());
                }
            }
            if (_autoWorkTime < timeToWork)
            {
                //61s进行一次采摘
                if (_autoWorkTime % 61 == 0)
                {
                    if ((threadGetFriendsFilter == null)||(!threadGetFriendsFilter.IsAlive))
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
        #endregion

        
    }    
}
