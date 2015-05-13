using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Timers;

using Mario.WebUI.WeiXin.Entity;
using System.Net;
using Newtonsoft.Json;


namespace Mario.WebUI.WeiXin
{
    public class WeiXinHelper
    {
        /// <summary>
        /// Token请求URL(对应Mario公众号)
        /// </summary>
        public const string TOKEN_REQUEST_URL = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wxc744af8cca600ad4&secret=d76a70cb74638d57a00a9c707c9413ee";

        
        public static readonly WeiXinHelper Instance = new WeiXinHelper();

        private WeiXinHelper()
        {
            getRemoteToken();
            tokenTimer.Elapsed += tokenTimer_Elapsed;
            tokenTimer.Enabled = true;
        }


        #region 微信Token

        public string AccessToken { get; set; }

        /// <summary>
        /// 微信Token超时时间(当前为一个小时)
        /// </summary>
        private Timer tokenTimer = new Timer(3600000); 
        private void tokenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            getRemoteToken();
        }

        private string getRemoteToken()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(TOKEN_REQUEST_URL);
            request.Method = "GET";
            using (WebResponse wr = request.GetResponse())
            {
                string reqstr = wr.ResponseUri.ToString();
                WebClient mywebclient = new WebClient();
                byte[] data = mywebclient.DownloadData(reqstr);
                string result = Encoding.UTF8.GetString(data);
                WeiXinTokenMessage tokenMessage = JsonConvert.DeserializeObject<WeiXinTokenMessage>(result);
                this.AccessToken = tokenMessage.access_token;
            }
            return AccessToken;
        }

        #endregion

        #region 微信消息

        /// <summary>
        /// 获取一个微信消息对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public WeiXinBaseMessage GetWeiXinInputMessage(HttpContext context)
        {
            StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            string msgType = document.SelectSingleNode("xml").SelectSingleNode("MsgType").InnerText;
            string toUserName = document.SelectSingleNode("xml").SelectSingleNode("ToUserName").InnerText;
            string fromUserName = document.SelectSingleNode("xml").SelectSingleNode("FromUserName").InnerText;

            if (msgType.Trim() == "text")
            {
                WeiXinTextMessage message = new WeiXinTextMessage();
                message.MsgType = msgType;
                message.ToUserName = toUserName;
                message.FromUserName = fromUserName;
                message.Content = document.SelectSingleNode("xml").SelectSingleNode("Content").InnerText;
                return message;
            }
            else if (msgType.Trim() == "event")
            {
                WeiXinBaseEvent message = new WeiXinBaseEvent();
                message.MsgType = msgType;
                message.ToUserName = toUserName;
                message.FromUserName = fromUserName;
                message.EventName = document.SelectSingleNode("Event").InnerText;
                message.EventKey = document.SelectSingleNode("EventKey").InnerText;
                return message;
            }
            else
            {
                return null;
            }
        }

        public string BuildTextMessage(string toUserName, string fromUserName, string content)
        {
            string formatString = "<xml><ToUserName><![CDATA[{0}]]></ToUserName><FromUserName><![CDATA[{1}]]></FromUserName><CreateTime>{2}</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[{3}]]></Content></xml>";
            int createTime = this.ConvertDateTimeToInt(DateTime.Now);
            return string.Format(formatString, toUserName, fromUserName, createTime, content);
        }

        public int ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        #endregion

        #region 安全验证
        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// <param name="token">自定义令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机字符数</param>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <returns></returns>
        public bool CheckSignature(string token, string timestamp, string nonce, string signature)
        {
            string[] sourceArray = { token, timestamp, nonce };
            Array.Sort(sourceArray);
            string source = string.Join("", sourceArray);
            string encrypt = this.SHA1Encrypt(source);
            return encrypt.Equals(signature);
        }

        /// <summary>
        /// 安全哈希算法散列加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string SHA1Encrypt(string source)
        {
            byte[] bytes = Encoding.Default.GetBytes(source);
            bytes = new SHA1CryptoServiceProvider().ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte num in bytes)
            {
                // 转16进制小写，补0
                builder.AppendFormat("{0:x2}", num);
            }
            return builder.ToString();
        }

        #endregion

        /// <summary>
        /// 根据用户名生成二维码图像
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public byte[] GetTicketImageData(string scene)
        {
            string ticket = CreateTicket(scene);
            string requestUrl = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + HttpUtility.UrlEncode(ticket);

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            webRequest.Method = "GET";
            using (WebResponse wr = webRequest.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)webRequest.GetResponse();
                WebClient mywebclient = new WebClient();
                byte[] data = mywebclient.DownloadData(myResponse.ResponseUri.ToString());
                return data;
            }
        }

        /// <summary>
        /// 根据scene创建一个Ticket
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public string CreateTicket(string scene)
        {
            string postJson = @"{""action_name"": ""QR_LIMIT_STR_SCENE"", ""action_info"": {""scene"": {""scene_str"": """ + scene + @"""}}}";
            string getTicketUrl = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + AccessToken;

            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;
            string result = myWebClient.UploadString(getTicketUrl, "POST", postJson);
            WeixinTicketMessage ticketMessage = JsonConvert.DeserializeObject<WeixinTicketMessage>(result);
            if (string.IsNullOrEmpty(ticketMessage.ticket))
            {
                throw new Exception(string.Format("自定义异常：ticketMessage.Ticken为空！getTicketUrl：{0}， postJson:{1}, result:{2}", getTicketUrl, postJson, result));
            }
            return ticketMessage.ticket;
        }  

    }
}