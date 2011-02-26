using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Json;

namespace Item
{
    class ShopItem
    {
        public JsonObject shopInfo = new JsonObject();
        public string cId
        {
            get { return shopInfo.GetValue("cId"); }
        }
        public string cLevel
        {
            get { return shopInfo.GetValue("cLevel"); }
        }
        public string cName
        {
            get { return shopInfo.GetValue("cName"); }
        }
        public string cType
        {
            get { return shopInfo.GetValue("cType"); }
        }
        public string cropExp
        {
            get { return shopInfo.GetValue("cropExp"); }
        }
        public string expect
        {
            get { return shopInfo.GetValue("expect"); }
        }
        public string growthCycle
        {
            get { return shopInfo.GetValue("growthCycle"); }
        }
        public string maturingTime
        {
            get { return shopInfo.GetValue("maturingTime"); }
        }
        public string output
        {
            get { return shopInfo.GetValue("output"); }
        }
        public string price
        {
            get { return shopInfo.GetValue("price"); }
        }
        public string sale
        {
            get { return shopInfo.GetValue("sale"); }
        }
        public ShopItem(JsonObject shopInfo)
        {
            if (shopInfo != null)
            {
                this.shopInfo = shopInfo;
            }
            else
            {
                //商店信息中无此对象
                JsonObject nullShopInfo = new JsonObject();
                nullShopInfo.Add("cId", "0");
                nullShopInfo.Add("cLevel", "0");
                nullShopInfo.Add("cName", "未识别");
                nullShopInfo.Add("cType", "0");
                nullShopInfo.Add("cropExp", "0");
                nullShopInfo.Add("expect", "0");
                nullShopInfo.Add("growthCycle", "0");
                nullShopInfo.Add("maturingTime", "0");
                nullShopInfo.Add("output", "0");
                nullShopInfo.Add("price", "0");
                nullShopInfo.Add("sale", "0");
                this.shopInfo = nullShopInfo;
            }
        }
    }
    class CropItem
    {
        public Dictionary<string,string> cropInfo = new Dictionary<string,string>();
        public CropItem(Dictionary<string, string> cropInfo)
        {
            if (cropInfo != null)
            {
                this.cropInfo = cropInfo;
            }
            else
            {
                //商店信息中无此对象
                Dictionary<string, string> nullCropInfo = new Dictionary<string, string>();
                nullCropInfo.Add("cId", "0");
                nullCropInfo.Add("cLevel", "0");
                nullCropInfo.Add("cName", "未识别");
                nullCropInfo.Add("cType", "0");
                nullCropInfo.Add("cropExp", "0");
                nullCropInfo.Add("expect", "0");
                nullCropInfo.Add("growthCycle", "0");
                //nullCropInfo.Add("maturingTime", "0");
                nullCropInfo.Add("growthCycleNext", "0");
                nullCropInfo.Add("price", "0");
                nullCropInfo.Add("numGetExpect", "0");
                this.cropInfo = nullCropInfo;
            }
        }
        public string cId
        {
            get { return cropInfo.ContainsKey("编号") ? cropInfo["编号"] : "0"; }
        }
        public string cName
        {
            get { return cropInfo.ContainsKey("名称") ? cropInfo["名称"].Replace("\0","") : "未识别"; } 
        }
        public string growthCycle
        {
            get { return cropInfo.ContainsKey("成熟时间") ? cropInfo["成熟时间"] : "0"; }
        }
        public string growthCycleNext
        {
            get { return cropInfo.ContainsKey("再熟时间") ? cropInfo["再熟时间"] : "0"; }
        }
        public string cType
        {
            get { return cropInfo.ContainsKey("类型") ? cropInfo["类型"] : "0"; }
        }
        public string price
        {
            get { return cropInfo.ContainsKey("价格") ? cropInfo["价格"] : "0"; }
        }
        public string expect
        {
            get { return cropInfo.ContainsKey("预计收入") ? cropInfo["预计收入"] : "0"; }
        }
        public string numGetExpect
        {
            get { return cropInfo.ContainsKey("预计产量") ? cropInfo["预计产量"] : "0"; }
        }
        public string cLevel
        {
            get { return cropInfo.ContainsKey("种植等级") ? cropInfo["种植等级"] : "0"; }
        }
        public string cropExp
        {
            get { return cropInfo.ContainsKey("收获经验") ? cropInfo["收获经验"] : "0"; }
        }
        public string priceOnSale
        {
            get { return cropInfo.ContainsKey("单个果实单价") ? cropInfo["单个果实单价"] : "0"; }
        }
    }
    class BagItem
    {
        JsonObject bagInfo = new JsonObject();
        public BagItem(JsonObject bagInfo)
        {
            if (bagInfo != null)
            {
                this.bagInfo = bagInfo;
            }
            else
            {
                JsonObject tmpBagInfo = new JsonObject();
                tmpBagInfo.Add("type","1"); ;
                tmpBagInfo.Add("cId","0");
                tmpBagInfo.Add("cName","未识别");
                tmpBagInfo.Add("amount","0");
                tmpBagInfo.Add("lifecycle","0");
                tmpBagInfo.Add("level","0");
                tmpBagInfo.Add("tId","0");
                tmpBagInfo.Add("tName", "未识别");
                tmpBagInfo.Add("depict","0");
                this.bagInfo = tmpBagInfo;
            }
        }
        public string type
        { get { return bagInfo.GetValue("type"); } }//1：植物;3：化肥
        public string cId
        { get { return bagInfo.GetValue("type").Equals("1") ? bagInfo.GetValue("cId") : ""; } }
        public string cName
        { get{ return bagInfo.GetValue("type").Equals("1") ? bagInfo.GetValue("cName") : ""; } }//种植名
        public string amount
        { get { return bagInfo.GetValue("amount"); } }//数目
        public string lifecycle
        { get{ return bagInfo.GetValue("type").Equals("1") ? bagInfo.GetValue("lifecycle") : ""; } }//成熟时间（小时）
        public string level
        { get{ return bagInfo.GetValue("type").Equals("1") ? bagInfo.GetValue("level") : ""; } }//等级
        public string tId
        { get{ return bagInfo.GetValue("type").Equals("3") ? bagInfo.GetValue("tId") : ""; }  }//化肥Id
        public string tName
        { get{ return bagInfo.GetValue("type").Equals("3") ? bagInfo.GetValue("tName") : ""; }  }//化肥名
        public string depict
        { get{ return bagInfo.GetValue("type").Equals("3") ? bagInfo.GetValue("depict") : ""; }  }//化肥作用说明
    }
}
