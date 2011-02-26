using System.Collections.Generic;
using System.Text;
using System;
namespace Json
{
    [Serializable] 
    public class JsonObjectModel
    {
        protected string _GetKey(string rawjson)
        {
            if (string.IsNullOrEmpty(rawjson))
                return rawjson;

            rawjson = rawjson.Trim();

            string[] jsons = rawjson.Split(new char[] { ':' });

            if (jsons.Length < 2)
                return rawjson;

            return jsons[0].Replace("\"", "").Trim();
        }

        protected string _GetValue(string rawjson)
        {
            if (string.IsNullOrEmpty(rawjson))
                return rawjson;

            rawjson = rawjson.Trim();

            string[] jsons = rawjson.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (jsons.Length < 2)
                return rawjson;

            StringBuilder builder = new StringBuilder();

            for (int i = 1; i < jsons.Length; i++)
            {
                builder.Append(jsons[i]);

                builder.Append(":");
            }

            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);

            string value = builder.ToString();

            if (value.StartsWith("\""))
                value = value.Substring(1);

            if (value.EndsWith("\""))
                value = value.Substring(0, value.Length - 1);

            return value;
        }

        protected List<string> _GetCollection(string rawjson)
        {
            //[{},{}] 

            List<string> list = new List<string>();

            if (string.IsNullOrEmpty(rawjson))
                return list;

            rawjson = rawjson.Trim();

            StringBuilder builder = new StringBuilder();

            int nestlevel = -1;

            int mnestlevel = -1;

            bool strlevel =false;
            for (int i = 0; i < rawjson.Length; i++)
            {
                char jsonchar = rawjson[i];
                if (i == 0)
                    continue;
                else if (i == rawjson.Length - 1)
                {
                    if (jsonchar == '}' || jsonchar == ']')
                        continue;
                }



                if (jsonchar == '{')
                {
                    nestlevel++;
                }

                if (jsonchar == '}')
                {
                    nestlevel--;
                }

                if (jsonchar == '[')
                {
                    mnestlevel++;
                }

                if (jsonchar == ']')
                {
                    mnestlevel--;
                }
                if (jsonchar == '"') strlevel = (!strlevel);
                if (jsonchar == ',' && nestlevel == -1 && mnestlevel == -1 && !strlevel)
                {
                    list.Add(builder.ToString());

                    builder = new StringBuilder();
                }
                else
                {
                    builder.Append(jsonchar);
                }
            }

            if (builder.Length > 0)
                list.Add(builder.ToString());

            return list;
        }
    }
}
