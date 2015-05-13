using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI.Pages
{
    public partial class ChangePassword : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected void btnSubmit_Click(object sender, DirectEventArgs e)
        {
            UserInfos user = this.GetCurrentUser();
            UserInfosManager userManager = new UserInfosManager();
            string oldPassword = MD5Helper.Instance.HashString(this.txtOldPassword.Text.Trim());
            string newPassword = MD5Helper.Instance.HashString(this.txtNewPassword.Text.Trim());
            string result = userManager.ChangePassword(user.UserName, oldPassword, newPassword);
            if (result == string.Empty)
            {
                X.Msg.Info("系统提示", "密码修改成功", UI.Success).Show();
                this.txtOldPassword.Text = string.Empty;
                this.txtNewPassword.Text = string.Empty;
                this.txtNewPasswordAgain.Text = string.Empty;
                
            }
            else
            {
                this.txtOldPassword.MarkInvalid(result);
                X.Msg.Info("系统提示", result, UI.Danger).Show();
            }
        }

        protected void btnBindWeiXin_Click(object sender, DirectEventArgs e)
        {
            // 在Session中清除原来的微信OpenID
            UserInfos currentUser = (UserInfos)Session["CurrentUser"];
            currentUser.WeiXinOpenID = string.Empty;
            Session["CurrentUser"] = currentUser;

            if (btnBindWeiXin.Text == "绑定微信")
            {
                subWindowWeiXinQRCode.Center();
                subWindowWeiXinQRCode.Show();
            }
            else if (btnBindWeiXin.Text == "取消绑定微信")
            {
                UserInfosManager userInfosManager = new UserInfosManager();
                int result = userInfosManager.AddOrUpdateUser(currentUser);
                if (result > 0)
                {
                    X.Msg.Info("系统提示", "取消绑定成功").Show();
                    this.btnBindWeiXin.Text = "绑定微信";
                }
            }
        }

        protected void subWindowWeiXinQRCode_Show(object sender, DirectEventArgs e)
        {
            UserInfos currentUser = (UserInfos)Session["CurrentUser"];
            QRCodeImage.ImageUrl = "~/WeiXin/QRImageHandler.ashx?scene=" + currentUser.UserName;
            QRCodeImage.Update();
            this.QRImageTaskManager.StartTask("WaitScanQRImageTask");
        }

        protected void subWindowWeiXinQRCode_Hide(object sender, DirectEventArgs e)
        {
            this.QRImageTaskManager.StopTask("WaitScanQRImageTask");
        }

        protected void WaitScanQRImageTask_Update(object sender, DirectEventArgs e)
        {
            UserInfos currentUser = (UserInfos)Session["CurrentUser"];
            if (!string.IsNullOrEmpty(currentUser.WeiXinOpenID)) // 说明绑定操作在WeiXinHandler.ashx已完成
            {
                this.QRImageTaskManager.StopTask("WaitScanQRImageTask");
                X.Msg.Info("系统提示", "绑定成功！", UI.Success).Show();
                this.btnBindWeiXin.Text = "取消绑定微信";
                this.btnBindWeiXin.Update();
            }
        }

    }
}