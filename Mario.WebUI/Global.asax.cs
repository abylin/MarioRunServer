using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
            Exception ex = Server.GetLastError().GetBaseException();
            string errorLog = "异常信息: " + ex.Message + ex.StackTrace + "。";
            //清除前一个异常
            Server.ClearError();
            SystemLogManager slm = new SystemLogManager();
            slm.AddSystemLog(new SystemLog()
            {
                UserID = 0,
                UserName = HttpContext.Current.Request.Url.ToString(),
                LogTime = DateTime.Now,
                Memo = errorLog
            }
            );
        }
    }
}