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
    public partial class ManageUserInfos : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!X.IsAjaxRequest)
            {
                bindGridUserInfoList();
            }
        }

        #region Grid相关事件

        private void bindGridUserInfoList()
        {
            UserInfosManager manager = new UserInfosManager();
            this.GridUserInfos.GetStore().DataSource = manager.SelectAllUsers();
            this.GridUserInfos.GetStore().DataBind();
        }

        /// <summary>
        /// 点击选择某一个用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridUserinfos_RowSelected(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.GridUserInfos.GetSelectionModel() as RowSelectionModel;
            if (sm.SelectedRows.Count == 0)
            {
                this.btnUpdate.Disable();
                this.btnDelete.Disable();
            }
            else
            {
                this.btnUpdate.Enable();
                this.btnDelete.Enable();
            }
        }

        /// <summary>
        /// 绑定用户信息到子窗体
        /// </summary>
        /// <param name="userID"></param>
        private void bindUserInfo(int userID)
        {
            
            if (userID == 0)
            {
                this.txtUserID.Text = ProjectConst.NEW_EMPTY_ID;
                this.txtUserName.Text = string.Empty;
                this.txtChineseName.Text = string.Empty;
                subWindowUserInfo.Title = "新增";
                this.txtPassword.Visible = true;
            }
            else
            {
                this.txtUserID.Text = userID.ToString();
                UserInfosManager manager = new UserInfosManager();
                UserInfos user = manager.SelectSingleUser(userID);
                this.txtUserName.Text = user.UserName;
                this.txtChineseName.Text = user.ChineseName;
                subWindowUserInfo.Title = "修改";
                this.txtPassword.Visible = false;
            }
            subWindowUserInfo.Center();
            subWindowUserInfo.Show();
        }

        protected void btnAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindUserInfo(0);
        }

        protected void btnUpdate_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.GridUserInfos.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int userID = int.Parse(rsm.SelectedRow.RecordID);
                this.bindUserInfo(userID);
            }
        }

        protected void btnDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.GridUserInfos.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int userID = int.Parse(rsm.SelectedRow.RecordID);
                UserInfosManager manager = new UserInfosManager();
                int result = manager.DeleteUser(userID);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的用户").Show();
            }
            this.bindGridUserInfoList();
        }
        #endregion

        #region 子Windows事件

        protected void btnWinSave_Click(object sender, DirectEventArgs e)
        {
            int userID = 0;
            if (this.txtUserID.Text != ProjectConst.NEW_EMPTY_ID)
            {
                userID = int.Parse(this.txtUserID.Text); 
            }

            UserInfos user = null;
            UserInfosManager manager = new UserInfosManager();
            if (userID == 0)
            {
                user = new UserInfos();
                user.ID = userID;
                user.Password = MD5Helper.Instance.HashString("1234");
            }
            else
            {
                user = manager.SelectSingleUser(userID);
            }
  
            user.UserName = this.txtUserName.Text;
            user.ChineseName = this.txtChineseName.Text;
            user.WeiXinOpenID = this.txtWeiXinOpenID.Text;
            
            int result = manager.AddOrUpdateUser(user);
            if (result == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!").Show();
            }
            else
            {
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            }

            subWindowUserInfo.Hide();
            this.bindGridUserInfoList();
        }

        #endregion

    }
}