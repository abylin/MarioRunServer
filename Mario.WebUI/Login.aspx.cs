using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, DirectEventArgs e)
        {
            LoginHandle();
        }

        protected void txt_KeyEnter(object sender, DirectEventArgs e)
        {
            LoginHandle();
        }

        public void LoginHandle()
        {
            string username = this.txtUsername.Text;
            string password = MD5Helper.Instance.HashString(this.txtPassword.Text);

            UserInfosManager manager = new UserInfosManager();
            try
            {
                UserInfos user = manager.VerifyPassword(username, password);
                if (user == null)
                {
                    X.Msg.Alert("登录失败", "用户名或密码错误，请检查后输入。").Show();
                }
                else
                {
                    windowsLogin.Close();
                    Session["CurrentUser"] = user;
                    RolesManager rolesManager = new RolesManager();
                    Session["IsAdmin"] = rolesManager.GetIsAdmin(user.ID, ProjectConst.ADMIN_ROLE_ID); 
                    SystemLogManager slm = new SystemLogManager();
                    slm.AddSystemLog(new SystemLog()
                    {
                        UserID = user.ID,
                        UserName = user.UserName,
                        LogTime = DateTime.Now,
                        Memo = "登录系统"
                    }
                    );
                    Response.Redirect("Pages/MainFrame.aspx");
                }
            }
            catch (Exception ex)
            {
                X.Msg.Alert("有异常", ex.Message + ex.StackTrace).Show();
            }
        }
    }
}