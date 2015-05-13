using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Mario.WebUI.WeiXin.Entity
{
    [XmlRoot(ElementName = "xml")]
    public class WeiXinBaseMessage
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public int CreateTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

    }
}