using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Json;

namespace FarmStatus
{
    //用户信息
    class User
        {
            JsonObject user = new JsonObject();
            public string userId
            {
                get { return user.GetValue("userId"); }
            }
            public string uin
            {
                get { return user.GetValue("uin"); }
            }
            public string userName
            {
                get { return user.GetValue("userName"); }
            }
            public string headPic
            {
                get { return user.GetValue("headPic"); }
            }
            public string yellowLevel
            {
                get { return user.GetValue("yellowLevel"); }
            }
            public string yellowStatus
            {
                get { return user.GetValue("yellowStatus"); }
            }
            public string exp
            {
                get { return user.GetValue("exp"); }
            }
            public string pastureExp
            {
                get { return user.GetValue("pastureExp"); }
            }
            public string money
            {
                get { return user.GetValue("money"); }
            }
            public string pf
            {
                get { return user.GetValue("pf"); }
            }
            public User(JsonObject user)
            {
                this.user = user;
            }
        }
    //成熟信息
    [Serializable]
    class Mature
    {
        public string userId
        { get; set; }
        public string userName
        { get; set; }
        public int place
        { get; set; }
        public string cName
        { get; set; }
        public string cId
        { set; get; }
        public string cStatus
        { get; set; }
        public string cWeed
        { get; set; }
        public string cWorm
        { get; set; }
        public string cDry
        { get; set; }
        public string hasDog
        {
            get;
            set;
        }
        public long cTime
        { get; set; }
        //播种时间q + gorwthCrcle + growthCycleNext*j;
        public Mature()
        {
            this.cWeed = "0";//>0 有草
            this.cWorm = "0";//>0 有虫
            this.cDry = "No"; //=Yes 干旱
        }
    }
    //可操作好友信息
    class FriendFilter
    {            
        public FriendFilter()
        {}
        public string userId
        {
            get;
            set;
        }
        public DoStatus doStatus
        { get; set;}
    }
    //可操作状态
    class DoStatus
    {
        JsonObject doStatusInfo = new JsonObject();
        public DoStatus(JsonObject doStatusInfo)
        { 
            this.doStatusInfo = doStatusInfo;
        }
        public string theDoStatus
        {
            get
            {
                return doStatusInfo.ContainsKey("1") ? "可偷取" :
                       (
                            doStatusInfo.ContainsKey("2") ? "可除草" :
                            (
                                doStatusInfo.ContainsKey("3") ? "可除虫" :
                                (
                                    doStatusInfo.ContainsKey("4") ? "可浇水" : ""
                                )
                            )
                       );
            }
        }
        public string harvestStatus
        { get { return doStatusInfo.ContainsKey("1") ? "可偷取" : ""; ;} }
        public string weedStatus
        { get { return doStatusInfo.ContainsKey("2") ? "可除草" : ""; ;} }
        public string wormStatus
        { get { return doStatusInfo.ContainsKey("3") ? "可除虫" : ""; ;} }
        public string waterStatus
        { get { return doStatusInfo.ContainsKey("4") ? "可浇水" : ""; ;} }
    }
    //土地信息
    class Land
    {
        /*
            *  a:种子的编号
            b:地的状态，1表示有植物在种 7表示枯萎 0表示空地
            c:曾经是否有草
            d:曾经是否有虫子
            e:曾经是否干旱
            f:大于0有草
            g:大于0有虫子
            h:等于0干旱
            i:优秀程度
            j:采摘的次数  j=0 第一季 j>0 第j+1季
            k:成熟的果实数
            l:大于0叶，最小能偷多少？这个不能太确认
            m:大于0时，表示还剩下多少个
            n:偷过我果实的好友uid列表
            0:施肥的次数
            p:动作？
            q:作物播种时间点

            r:更新时间点
            dogId : 0表示无狗
         *  isHungry : 1表示无狗粮，0表示有狗粮
         * 
            */
        JsonObject _farmStatus = new JsonObject();
        public string a
        {
            get { return _farmStatus.GetValue("a"); }
        }//作物id
        public string b
        {
            get { return _farmStatus.GetValue("b"); }
        }
        public string c
        {
            get { return _farmStatus.GetValue("c"); }
        }
        public string d
        {
            get { return _farmStatus.GetValue("d"); }
        }
        public string e
        {
            get { return _farmStatus.GetValue("e"); }
        }
        public string f
        {
            get { return _farmStatus.GetValue("f"); }
        }
        public string g
        {
            get { return _farmStatus.GetValue("g"); }
        }
        public string h
        {
            get { return _farmStatus.GetValue("h"); }
        }
        public string i
        {
            get { return _farmStatus.GetValue("i"); }
        }
        public string j
        {
            get { return _farmStatus.GetValue("j"); }
        }
        public string k
        {
            get { return _farmStatus.GetValue("k"); }
        }
        public string l
        {
            get { return _farmStatus.GetValue("l"); }
        }
        public string m
        {
            get { return _farmStatus.GetValue("m"); }
        }
        public string n
        {
            get { return _farmStatus.GetValue("n"); }
        }
        public string o
        {
            get { return _farmStatus.GetValue("o"); }
        }
        public string p
        {
            get { return _farmStatus.GetValue("p"); }
        }
        public string q
        {
            get { return _farmStatus.GetValue("q"); }
        }
        public string r
        {
            get { return _farmStatus.GetValue("r"); }
        }
        public string bitmap
        {
            get { return _farmStatus.GetValue("bitmap"); }
        }
        public string pid
        {
            get { return _farmStatus.GetValue("pid"); }
        }       
        public Land(JsonObject farmStatus)
        {
            this._farmStatus = farmStatus;
        }
    }
    //收获，除草等的总表
    class Ach
    {
        public int numWorm
        { get; set; }
        public int numWeed
        { get; set; }
        public int numWater
        { get; set; }
        public List<CropGet> cropList
        {
            get;
            set;
        }
        public Ach()
        {
            this.numWater = 0;
            this.numWeed = 0;
            this.numWorm = 0;
            this.cropList = new List<CropGet>();
        }
    }
    //偷取的作物名,作物数
    class CropGet
    {
        public string cropId
        { get; set; }
        public string cropName
        { get; set; }
        public int numCrop
        { get; set; }
        public CropGet()
        {
            this.cropId = "";
            this.cropName = "未知";
            this.numCrop = 0;
        }
    }
    //除草等动作的响应结果
    class DoResult
    {
        JsonObject resultInfo = new JsonObject();
        public string farmlandIndex
        { get { return resultInfo.GetValue("farmlandIndex"); } }
        public string code
        { get { return resultInfo.GetValue("code"); } }
        public string direction
        { get { return resultInfo.GetValue("direction"); } }//新手指南，其他情况下为空
        public string exp
        { get { return resultInfo.GetValue("exp"); } }//本动作获取的经验
        public string levelUp
        { get { return resultInfo.GetValue("levelUp"); } }//玩家是否升级
        public string money
        { get { return resultInfo.GetValue("money"); } }//本动作获取的金钱
        public string poptype
        { get { return resultInfo.GetValue("poptype"); } }
        public string weed
        { get { return resultInfo.GetValue("weed"); } }//草
        public string pest
        { get { return resultInfo.GetValue("pest"); } }//虫
        public string humidity
        { get { return resultInfo.GetValue("humidity"); } }//水
        public string harvest
        { get { return resultInfo.GetValue("harvest"); } }//收获、偷取的个数
        public DoResult(string content)
        {
            this.resultInfo = new JsonObject(content);
        }
    }
}
