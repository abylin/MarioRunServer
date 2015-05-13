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
    public partial class ManageRoles : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!X.IsAjaxRequest)
            {
                bindRolesList();
            }
        }


        #region 左边角色相关事件

        protected void btnRoleSubmit_Click(object sender, DirectEventArgs e)
        {
            RolesManager manager = new RolesManager();
            Roles role = null;
            if (this.txtRoleID.Text == ProjectConst.NEW_EMPTY_ID)
            {
                role = new Roles();
            }
            else
            {
                int roldID = int.Parse(this.txtRoleID.Text);
                role = manager.SelectSingleEntity(roldID);
            }
            role.Name = this.txtRoleName.Text;
            role.Memo = this.txtRoleMemo.Text;
            int result = manager.AddOrUpdate(role);
            if (result == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!").Show();
            }
            else
            {
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            }

            this.subWindowRoles.Hide();
            this.bindRolesList();
        }

        private void bindRolesList()
        {
            RolesManager manager = new RolesManager();
            this.gridRoles.GetStore().DataSource = manager.SelectAllEntities();
            this.gridRoles.GetStore().DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridRoles_RowSelected(object sender, DirectEventArgs e)
        {
            bindAppProjectsInRole();
        }

        private void bindAppProjectsInRole()
        {
            RowSelectionModel sm = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            if (sm.SelectedRows.Count == 0)
            {
                this.btnRolesModify.Disable();
                this.btnRolesDelete.Disable();
                this.btnRoleConfigUser.Disable();
                this.btnAppProjectsInRolesConfig.Disable();
            }
            else
            {
                this.btnRolesModify.Enable();
                this.btnRolesDelete.Enable();
                this.btnRoleConfigUser.Enable();
                this.btnAppProjectsInRolesConfig.Enable();

                int roleID = int.Parse(sm.SelectedRow.RecordID);
                this.bindAppProjectsInRolesList(roleID);
            }
        }

        /// <summary>
        /// 绑定实体到子窗体
        /// </summary>
        private void bindSingleRole(int roleID)
        {
            RolesManager manager = new RolesManager();
            Roles role = null;
            if (roleID == 0)
            {
                this.txtRoleID.Text = ProjectConst.NEW_EMPTY_ID;
                this.txtRoleName.Text = string.Empty;
                this.txtRoleMemo.Text = string.Empty;
                role = new Roles();
            }
            else
            {
                role = manager.SelectSingleEntity(roleID);
                this.txtRoleID.Text = role.ID.ToString();
                this.txtRoleName.Text = role.Name;
                this.txtRoleMemo.Text = role.Memo;
            }
            this.subWindowRoles.Title = "修改";
            this.subWindowRoles.Center();
            this.subWindowRoles.Show();
        }


        protected void btnRolesAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindSingleRole(0);
        }

        protected void btnRolesModify_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindSingleRole(id);
            }
        }

        protected void btnRolesDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                RolesManager manager = new RolesManager();
                int result = manager.Delete(id);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                    bindRolesList();
                    bindAppProjectsInRole();
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }
        }

        

        #endregion

        #region 角色配置用户事件

        protected void GridPanelUserRight_SubmitData(object sender, StoreSubmitDataEventArgs e)
        {
            List<UserInfos> selectedUserInfosList = e.Object<UserInfos>();

            RowSelectionModel sm = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            int roleID = int.Parse(sm.SelectedRow.RecordID);
            RolesManager manager = new RolesManager();
            manager.SetUserInfosFromRoles(roleID, selectedUserInfosList);
            X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            this.subWindowUserSelector.Hide();
        }


        protected void btnRoleConfigUser_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            int roleID = int.Parse(sm.SelectedRow.RecordID);
            this.bindRoleConfig(roleID);

            this.subWindowUserSelector.Center();
            this.subWindowUserSelector.Show();
        }

        private void bindRoleConfig(int roleID)
        {
            // 绑定待选和已选的APP项目
            RolesManager manager = new RolesManager();
            this.GridPanelUserLeft.GetStore().DataSource = manager.GetNotSelectUserInfosFromRoles(roleID);
            this.GridPanelUserLeft.GetStore().DataBind();

            this.GridPanelUserRight.GetStore().DataSource = manager.GetSelectedUserInfosFromRoles(roleID);
            this.GridPanelUserRight.GetStore().DataBind();
        }
        #endregion

        #region 右边AppProjects相关事件

        protected void SubmitSelectedAppProjectsData(object sender, StoreSubmitDataEventArgs e)
        {
            List<AppProjects> selectedAppProjectsList = e.Object<AppProjects>();

            RowSelectionModel sm = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            int roleID = int.Parse(sm.SelectedRow.RecordID);
            RolesManager manager = new RolesManager();
            manager.SetAppProjectsFromRoles(roleID, selectedAppProjectsList);
            X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            this.subWindowAppProjectsSelector.Hide();
            this.bindAppProjectsInRolesList(roleID);
        }

        private void bindAppProjectsInRolesList(int roleID)
        {
            RolesManager manager = new RolesManager();
            this.gridAppProjectsInRoles.GetStore().DataSource = manager.GetSelectedAppProjectsFromRoles(roleID);
            this.gridAppProjectsInRoles.DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridAppProjectsInRoles_RowSelected(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.gridAppProjectsInRoles.GetSelectionModel() as RowSelectionModel;
            if (sm.SelectedRows.Count == 0)
            {
                this.btnAppProjectsInRolesDelete.Disable();
            }
            else
            {
                this.btnAppProjectsInRolesDelete.Enable();
            }
        }

        protected void btnAppProjectsInRolesConfig_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.gridRoles.GetSelectionModel() as RowSelectionModel;

            int roleID = int.Parse(sm.SelectedRow.RecordID);

            // 绑定待选和已选的APP项目
            RolesManager manager = new RolesManager();
            this.GridPanel1.GetStore().DataSource = manager.GetNotSelectAppProjectsFromRoles(roleID);
            this.GridPanel1.GetStore().DataBind();

            this.GridPanel2.GetStore().DataSource = manager.GetSelectedAppProjectsFromRoles(roleID);
            this.GridPanel2.GetStore().DataBind();

            this.subWindowAppProjectsSelector.Center();
            this.subWindowAppProjectsSelector.Show();
        }

        protected void btnAppProjectsInRolesDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjectsInRoles.GetSelectionModel() as RowSelectionModel;
            RowSelectionModel rsmRole = this.gridRoles.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1 && rsmRole.SelectedRows.Count == 1)
            {
                int appProjectID = int.Parse(rsm.SelectedRow.RecordID);
                int roleID = int.Parse(rsmRole.SelectedRow.RecordID);
                RolesManager manager = new RolesManager();
                int result = manager.RemoveAppProjectsFromRole(roleID, appProjectID);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "移除权限失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "移除权限成功!", UI.Success).Show();
                    this.bindAppProjectsInRolesList(roleID);
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }

        }
        #endregion
    }
}