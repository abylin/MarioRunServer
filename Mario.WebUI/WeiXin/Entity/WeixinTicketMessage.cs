using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.WebUI.WeiXin.Entity
{
    public class WeixinTicketMessage
    {
        /// <summary>  
        /// 获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。  
        /// </summary>  
        public string ticket
        {
            get;
            set;
        }

        /// <summary>  
        /// 凭证有效时间，单位：秒  
        /// </summary>  
        public string expire_seconds
        {
            get;
            set;
        }
    }
}
