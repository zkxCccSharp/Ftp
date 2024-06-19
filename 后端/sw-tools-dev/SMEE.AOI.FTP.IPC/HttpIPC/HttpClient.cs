using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace SMEE.AOI.FTP.IPC.HttpIPC
{
    public class HttpClient
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(HttpService));

        //FTP传输超时 10分钟
        private const int ftpTimeOut = 10 * 60 * 1000;

        public static string PostUrl(string url, string postData, bool isHeart)
        {
            int timeOut = 3000;
            if (!isHeart)
            {
                timeOut = ftpTimeOut;
            }
            return PostUrl(url, postData, timeOut);
        }
        
        private static string PostUrl(string url, string postData, int timeOut)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeOut;
            request.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;

            using (Stream reqStream=request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.Error($"PostUrl: Request failed, StatusCode:{response.StatusCode}.");
                throw new Exception("Request failed, check http connect.");
            }
            Stream stream = response.GetResponseStream();

            string result = "";
            using (StreamReader reader=new StreamReader(stream,Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
        
    }
}
