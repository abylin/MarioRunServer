using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using Mario.Business;
using Mario.DataAccess;
using Ext.Net;

namespace Mario.WebUI.Pages
{
    /// <summary>
    /// 页面基类，主要用于做权限控制
    /// </summary>
    public class BasePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["CurrentUser"] == null) // 用户未登录 
                {
                    // DEBUG
                    //UserInfosManager manager = new UserInfosManager();
                    //UserInfos testUser = manager.SelectSingleUser(1);
                    //Session["CurrentUser"] = testUser;
                    //return;

                    string loginScript = "<script type='text/javascript'>window.top.location.href='" + ConfigurationManager.AppSettings["ServerHostUrl"] + "/login.aspx" + "'</script>";
                    Response.Write(loginScript);
                    Response.Flush();
                    Response.End();
                    return;
                }

                // 判断当前用户的角色，禁止访问无权限页面。
                UserInfos currentUser = (UserInfos)Session["CurrentUser"];
                string url = Request.Url.ToString();
                bool isAdmin = (bool)Session["IsAdmin"];
                if (!isAdmin)
                {
                    if (url.Contains("ManageUserInfos.aspx") || url.Contains("ViewSystemLogs.aspx")
                        || url.Contains("ViewMobileDevicesLogs.aspx") || url.Contains("ManageRoles.aspx"))
                    {
                        Response.Redirect("../Pages/PermissionDenied.html");
                    }
                } 
            }
        }

        public void AddSystemLog(string memo)
        {
            UserInfos currentUser = (UserInfos)Session["CurrentUser"];
            if (currentUser != null)
            {
                SystemLogManager slm = new SystemLogManager();
                slm.AddSystemLog(new SystemLog()
                {
                    UserID = currentUser.ID,
                    UserName = currentUser.UserName,
                    LogTime = DateTime.Now,
                    Memo = memo
                });
            }
        }

        public UserInfos GetCurrentUser()
        {
            if (Session["CurrentUser"] == null)
            {
                X.Msg.Alert("系统超时", "系统超时，请重新登录").Show();
                Response.Redirect("../login.aspx");
            }
            return (UserInfos)Session["CurrentUser"];
        }
    }
}