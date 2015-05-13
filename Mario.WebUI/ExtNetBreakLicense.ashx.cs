using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mario.WebUI
{
    /// <summary>
    /// 破解EXT.NET,需要在Web.config配置
    /// </summary>
    public class ExtNetBreakLicense : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //在此处写入您的处理程序实现。
            context.Response.Write(string.Empty);
            context.Response.End();
        }  
    }
}