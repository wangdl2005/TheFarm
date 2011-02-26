using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MyFarm
{
    class Http
    {
        //获取cookie的方法
        public static string GetHtml(string postData, out CookieContainer container, string url)
        {
            container = new CookieContainer();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.KeepAlive = true;
            request.CookieContainer = container;  //返回的cookie会附加在这个容器里面
            //发送数据
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Cookies = container.GetCookies(request.RequestUri);

            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string content = reader.ReadToEnd();
            return content ;
        }
        //利用cookie Post数据
        public static string GetHtml(string postData, CookieContainer container, string url)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Post";
            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = data.Length;
            request.KeepAlive = true;
            request.CookieContainer = container;  //返回的cookie会附加在这个容器里面
            //发送数据
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Cookies = container.GetCookies(request.RequestUri);

            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string content = reader.ReadToEnd();
            return content;
        }
        //利用Cookie Get数据
        public static string GetHtml(CookieContainer container,string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            request.CookieContainer = container;  //返回的cookie会附加在这个容器里面
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Cookies = container.GetCookies(request.RequestUri);
                        
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string content = reader.ReadToEnd();
            return content;
        }
        //直接Get数据
        public static string GetHtml(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
           

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string content = reader.ReadToEnd();
            return content;
        }
    }
}
