using System;
using System.Collections.Generic;
using System.Text;
namespace Json
{
    [Serializable] 
    public class JsonObject : JsonObjectModel// : IComparable, IConvertible, IComparable<bool>, IEquatable<bool>
    {
        private string rawjson;

        public bool isValue
        {
            get
            {
                if (rawjson.Trim() == "") return false;
                if (isModel || isCollection)
                    return false;
                return true;
            }
        }
        public bool isModel
        {
            get
            {
                if (rawjson.Trim() == "") return false;
                if (rawjson.StartsWith("{"))
                    return true;
                return false;
            }
        }

        public bool isCollection
        {
            get
            {
                if (rawjson.Trim() == "") return false;
                if (rawjson.StartsWith("["))
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 获取Json.JsonObject中实际包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                if (rawjson.Trim() == "") return 0;
                if (isModel == false)
                {
                    return 1;
                }
                return GetCollection().Count;
            }
        }
        /// <summary> 
        /// 当模型是值对象，返回key 
        /// </summary> 
        public string Key
        {
            get
            {
                if (isValue)
                    return base._GetKey(rawjson);

                return null;
            }
        }
        /// <summary> 
        /// 当模型是值对象，返回value 
        /// </summary> 
        public string Value
        {
            get
            {
                if (!isValue)
                    return null;

                return base._GetValue(rawjson);
            }
            set
            {
                rawjson = rawjson.Substring(0, rawjson.IndexOf(':')+1) + value;
            }
        }
        /// <summary>
        /// 获取或设置其键值
        /// </summary>
        /// <param name="key">要获取或设置期键值的键</param>
        /// <returns></returns>
        public virtual object this[object key]
        {
            get
            {
                if (rawjson.Trim() == "") return null;
                if (key.GetType() == typeof(int))
                {
                    if ((int)key < 0)
                        return null;
                    if (isModel == false)
                    {
                        if ((int)key == 0)
                            return Value;
                        else
                            return null;
                    }
                    return GetValue(key.ToString());
                }
                else if (key.GetType() == typeof(string))
                {
                    if (isModel == false)
                        return Value;
                    if (!ContainsKey(key.ToString()))
                        return null;
                    return GetValue(key.ToString());
                }
                return null;
            }
            set
            {
                Insert(key, value, false);
            }
        }

        #region 构造方法
        public JsonObject()
            : this("")
        {

        }

        public JsonObject(object rawjson)
        {
            this.rawjson = rawjson.ToString();
        } 
        #endregion

        #region 拥有的key
        /// <summary> 
        /// 当模型是对象，返回拥有的key 
        /// </summary> 
        /// <returns></returns> 
        public List<string> GetKeys()
        {
            if (isModel==false)
                return null;

            List<string> list = new List<string>();

            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                string key = new JsonObject(subjson).Key;

                if (!string.IsNullOrEmpty(key))
                    list.Add(key);
            }

            return list;
        } 
        #endregion

        #region 确定Json.JsonObject 是否包含特定键
        /// <summary>
        /// 确定Json.JsonObject 是否包含特定键
        /// </summary>
        /// <param name="key">要在Json.JsonObject定位的键</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (isModel==false)
            {
                if (key == Key)
                {
                    return true;
                }
                return false;
            }
            List<string> keys = GetKeys();
            foreach (string _key in keys)
            {
                if (key == _key)
                {
                    return true;
                }
            }
            return false;
        } 
        #endregion

        #region 获取值
        #region 当模型是对象，key对应是值，则返回key对应的值
        /// <summary> 
        /// 当模型是对象，key对应是值，则返回key对应的值 
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public string GetValue(string key)
        {
            if (isModel==false)
                return null;

            if (string.IsNullOrEmpty(key))
                return null;

            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                JsonObject model = new JsonObject(subjson);

                if (!model.isValue)
                    continue;

                if (model.Key == key)
                    return model.Value;
            }

            return null;
        }
        #endregion

        #region 模型不是对象，返回 索引 对应的值
        /// <summary>
        /// 模型不是对象，返回 索引 对应的值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetValue(int index)
        {
            if (isModel==false)
                return Value;
            string subjson = base._GetCollection(this.rawjson)[index];
            JsonObject model = new JsonObject(subjson);
            if (!model.isValue)
                return null;
            return model.Value;
        }
        #endregion 

        #region 模型不是对象，返回 索引 对应的KEY
        /// <summary>
        /// 模型不是对象，返回 索引 对应的KEY
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetKey(int index)
        {
            if (isModel==false)
                return Key;
            string subjson = base._GetCollection(this.rawjson)[index];
            JsonObject model = new JsonObject(subjson);
            if (!model.isValue)
                return null;
            return model.Key;
        }
        #endregion  
        #endregion

        #region 模型是对象，key对应是对象，返回key对应的对象
        /// <summary> 
        /// 模型是对象，key对应是对象，返回key对应的对象 
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public JsonObject GetJson(string key)
        {
            if (isModel==false)
                return null;

            if (string.IsNullOrEmpty(key))
                return null;

            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                JsonObject model = new JsonObject(subjson);

                if (!model.isValue)
                    continue;

                if (model.Key == key)
                {
                    JsonObject submodel = new JsonObject(model.Value);

                    if (!submodel.isModel)
                        return null;
                    else
                        return submodel;
                }
            }

            return null;
        } 
        #endregion

        #region 模型是对象，索引 对应是对象，返回 索引 对应的对象
        /// <summary>
        /// 模型是对象，索引 对应是对象，返回 索引 对应的对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JsonObject GetJson(int index)
        {
            if (isModel==false)
                return null;
            string subjson = base._GetCollection(this.rawjson)[index];
            if (subjson.Length < 2)
                return null;
            JsonObject model = new JsonObject(subjson);
            return model;
            /*
             * if (!model.isModel)
                return null;
            JsonObject submodel = new JsonObject(model.Value);

            if (!submodel.isModel)
                return null;
            else
                return submodel;
             * */
        }
        #endregion

        #region 模型是对象，key对应是集合，返回集合 
        /// <summary> 
        /// 模型是对象，key对应是集合，返回集合 
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public JsonObject GetCollection(string key)
        {
            if (isModel==false)
                return null;

            if (string.IsNullOrEmpty(key))
                return null;

            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                JsonObject model = new JsonObject(subjson);

                if (!model.isValue)
                    continue;

                if (model.Key == key)
                {
                    JsonObject submodel = new JsonObject(model.Value);

                    if (!submodel.isCollection)
                        return null;
                    else
                        return submodel;
                }
            }

            return null;
        } 
        #endregion

        public bool IsCollection(string key)
        {
            if (isModel==false)
                return false;

            if (string.IsNullOrEmpty(key))
                return false;

            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                JsonObject model = new JsonObject(subjson);

                if (!model.isValue)
                    continue;

                if (model.Key == key)
                {
                    JsonObject submodel = new JsonObject(model.Value);

                    return submodel.isCollection;
                }
            }

            return false;
        }

        #region 模型是集合，返回自身
        /// <summary> 
        /// 模型是集合，返回自身 
        /// </summary> 
        /// <returns></returns> 
        public List<JsonObject> GetCollection()
        {
            List<JsonObject> list = new List<JsonObject>();

            if (isValue)
                return list;

            foreach (string subjson in base._GetCollection(rawjson))
            {
                list.Add(new JsonObject(subjson));
            }

            return list;
        } 
        #endregion

        #region 将带有指定键值和值的元素添加到 Json.JsonObject中
        /// <summary>
        /// 将带有指定键值和值的元素添加到 Json.JsonObject中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            Insert(key, value, true);
        }
        /// <summary>
        /// 将 Json.JsonObject 元素添加到 Json.JsonObject中
        /// </summary>
        /// <param name="json"></param>
        public void Add(JsonObject json)
        {
            Insert(null, json.ToString(), true);
        } 
        #endregion
        /// <summary>
        /// 从 Json.JsonObject 中移除带有指定键的元素
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (!ContainsKey(key))
                return;

            string oldValue = GetValue(key.ToString());
            if (oldValue == null) oldValue = "";
            int statsIndex = rawjson.IndexOf("\"" + key.ToString() + "\"");//key的位置

            string jsonq = rawjson.Substring(0, statsIndex);
            statsIndex = statsIndex + key.ToString().Length + oldValue.Length + 1 + 2;//key的长度  加  :的长度 引号长度
            string jsonh = rawjson.Substring(statsIndex, rawjson.Length - statsIndex);
            bool requ = true;
            if (jsonh.StartsWith(",") && requ)
            {
                jsonh = jsonh.Substring(1);
                requ = false;
            }

            if (jsonq.EndsWith(",") && requ)
                jsonq = jsonq.Substring(0, jsonq.Length - 1);

            rawjson = jsonq + jsonh;
        }
        /// <summary>
        /// 从 Json.JsonObject 中移除带有指定键的元素
        /// </summary>
        /// <param name="JsonObject"></param>
        public void Remove(JsonObject json)
        {
            if (isModel==false)
            {
                return;
            }
            int statsIndex = rawjson.IndexOf(json.ToString());//key的位置
            if (statsIndex < 0) return;
            string jsonq = rawjson.Substring(0, statsIndex);
            statsIndex = statsIndex + json.ToString().Length;//key的长度  加  :的长度 引号长度
            string jsonh = rawjson.Substring(statsIndex, rawjson.Length - statsIndex);
            bool requ = true;
            if (jsonh.StartsWith(",") && requ)
            {
                jsonh = jsonh.Substring(1);
                requ = false;
            }

            if (jsonq.EndsWith(",") && requ)
                jsonq = jsonq.Substring(0, jsonq.Length - 1);

            rawjson = jsonq + jsonh;
        }
        public override string ToString()
        {
            return rawjson;
        }
        private void Insert(object key, object value, bool add)
        {
            object addkey = key;
            if (add==false)
            {
                if (rawjson.Trim() == "")
                {
                    Insert(key, value, true);
                    return;
                }
                if (key.GetType() == typeof(int))
                {
                    if ((int)key < 0)
                        throw new Exception("索引值为负值");
                }
                if (isModel!=true)
                {
                    Value = value.ToString();
                    return;
                }
                if (key.GetType() == typeof(int))
                {
                    key = GetKey((int)key);
                }
                if (!ContainsKey(key.ToString()))
                {
                    Insert(key, value, true);
                    return;
                }
                else
                {
                    string oldValue = GetValue(key.ToString());
                    int statsIndex = rawjson.IndexOf("\"" + key.ToString() + "\"");//key的位置
                    statsIndex += key.ToString().Length + 1 + 2;//key的长度 加  :的长度 引号长度
                    string jsonq = rawjson.Substring(0, statsIndex);
                    statsIndex = statsIndex + oldValue.Length;
                    string jsonh = rawjson.Substring(statsIndex, rawjson.Length - statsIndex);
                    if (value.ToString().IndexOf(',') > -1&&!value.ToString().StartsWith("{") && !value.ToString().StartsWith("["))
                    {
                        value = value = "\"" + value + "\"";
                    }
                    rawjson = jsonq + value + jsonh;
                    return;
                }
            }
            if (add)
            {
                if (value.ToString().IndexOf(',') > -1 && key != null && !value.ToString().StartsWith("{") && !value.ToString().StartsWith("["))
                    value = "\"" + value + "\"";
                string indexValue = "\"" + addkey + "\":" + value;
                if (key==null)
                    indexValue = value.ToString();
                if (rawjson == "")
                {
                    rawjson = indexValue;
                    return;
                }
                if (isModel != true)
                {
                    if (addkey.ToString() == Key)
                    {
                        Value = value.ToString();
                        return;
                    }
                    rawjson = "{" + rawjson + "}";
                }
                if (isModel && addkey!=null&&GetValue(addkey.ToString()) != null)
                {
                    Insert(addkey, value, false);
                    return;
                }
                rawjson = rawjson.Insert(this.rawjson.Length - 1, (this.rawjson.Length > 2 ? "," : "") + indexValue);
            }
        }
    }
}
