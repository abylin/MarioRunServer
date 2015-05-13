using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Mario.WebUI.WeiXin.Entity
{
    [XmlRoot(ElementName = "xml")]
    public class WeiXinTextMessage : WeiXinBaseMessage
    {
        /// <summary>
        /// 文本消息内容 
        /// </summary>
        public string Content { get; set; }      
            
    }
}