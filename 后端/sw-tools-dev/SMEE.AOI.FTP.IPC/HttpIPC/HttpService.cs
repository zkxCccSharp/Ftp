using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SMEE.AOI.FTP.Data;

namespace SMEE.AOI.FTP.IPC.HttpIPC
{
    public class HttpService
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(HttpService));
        private static int number = 0;

        public static void Listen(string url, Func<string, Session, string> routeMethod)
        {
            HttpListener listener = new HttpListener();

            try
            {
                logger.Info($"Listen: About to start listen, url:{url}.");
                listener.Prefixes.Add(url);
                listener.Start();
                logger.Info("Listen: Http Service starts listening.");

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    var session = CreateSession();
                    logger.Info($"Listen: Received context, session id:{session.Id}");
                    Task.Factory.StartNew(() =>
                    {
                        SessionManager.Enter(session);
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        bool isSuccess = true;
                        try
                        {
                            ReceivedHandler(context, routeMethod, session);
                        }
                        catch (Exception ex)
                        {
                            isSuccess = false;
                            logger.Error("ReceivedHandler failed.", ex);
                        }
                        finally
                        {
                            sw.Stop();
                            SessionManager.Out(session, sw.Elapsed.TotalSeconds, isSuccess);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Listen: Connect or parse failed.", ex);
                throw new Exception("Listener unexpected stoped.", ex);
            }
            finally
            {
                listener.Stop();
                logger.Info($"Listen: Listener has stopped.");
            }
        }

        private static Session CreateSession()
        {
            Session session = new Session();
            session.Id = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "-" + number++.ToString();
            return session;
        }

        private static void ReceivedHandler(HttpListenerContext context, Func<string, Session, string> routeMethod, Session session)
        {
            HttpListenerRequest request = context.Request;

            string httpMethod = request.HttpMethod;
            NameValueCollection headers = request.Headers;
            long contentLength = request.ContentLength64;
            string contentType = request.ContentType;

            string path = request.Url.LocalPath;
            string requestBody = string.Empty;

            using (Stream body = request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                {
                    requestBody = reader.ReadToEnd();
                }
            }

            string responseString = string.Empty;
            //Route
            try
            {
                session.RequestBody = requestBody;
                responseString = routeMethod(path, session);
            }
            catch (Exception ex)
            {
                logger.Error($"Listen: Deal failed, localPath:{path}, requestBody:{requestBody}. ", ex);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            HttpListenerResponse response = context.Response;

            // Add CORS headers
            response.AddHeader("Access-Control-Allow-Origin", "http://localhost:5173");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
            response.AddHeader("Access-Control-Allow-Credentials", "true");

            // Handle OPTIONS request for CORS preflight
            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
                response.Close();
            }
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }
    }
}
