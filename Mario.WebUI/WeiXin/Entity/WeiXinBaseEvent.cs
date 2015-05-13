using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mario.WebUI.WeiXin.Entity
{
    public class WeiXinBaseEvent : WeiXinBaseMessage
    {
        /// <summary>
        /// 事件名称，如subscribe
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public string EventKey { get; set; }

    }
}