using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;

using Mario.DataAccess;

namespace Mario.WebUI.Pages
{
    public partial class MainFrame : BasePage
    {
        protected  new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                UserInfos currentUser = this.GetCurrentUser();
                this.panelMain.Title = string.Format("您好,{0}", currentUser.ChineseName);
                bool isAdmin = (bool)Session["IsAdmin"];
                ////if (isAdmin)
                ////{
                ////    this.MenuItem10.Hidden = false;
                ////    this.MenuItem11.Hidden = false;
                ////    this.MenuItem12.Hidden = false;
                ////    this.MenuItem13.Hidden = false;
                ////    this.MenuItem14.Hidden = false;
                ////}
            }
        }

        protected void logout_Click(object sender, DirectEventArgs e)
        {
            //清空当前用户Session
            Session.Clear();
            Response.Redirect("../login.aspx");
        }
    }
}