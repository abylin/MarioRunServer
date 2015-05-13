using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Data.OleDb;
using System.Configuration;
using System.Data;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI.Pages
{
    public partial class ManageAppProjects : BasePage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!X.IsAjaxRequest)
            {
                bindGridAppProjectsList();
                InitSelectBoxToAppProject();
            }
        }

        private void InitSelectBoxToAppProject()
        {
            AppProjectsManager manager = new AppProjectsManager();
            UserInfos currentUser = this.GetCurrentUser();
            List<AppProjects> list = manager.SelectAllEntities(true, currentUser.ID);
            this.selectBoxToAppProject.Items.Clear();
            foreach (AppProjects project in list)
            {
                this.selectBoxToAppProject.Items.Add(new Ext.Net.ListItem(project.ChineseName, project.ID));
            }
        }

        #region gridAppProjects相关事件

        protected void chkAppProjectsDisplayPause_CheckedChanged(object sender, DirectEventArgs e)
        {
            bindGridAppProjectsList();
        }

        private void bindGridAppProjectsList()
        {
            bool isDisplayPauseAppProject = this.chkAppProjectsDisplayPause.Checked;
            AppProjectsManager manager = new AppProjectsManager();
            UserInfos currentUser = this.GetCurrentUser();
            this.gridAppProjects.GetStore().DataSource = manager.SelectAllEntities(isDisplayPauseAppProject, currentUser.ID);
            this.gridAppProjects.GetStore().DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridAppProjects_RowSelected(object sender, DirectEventArgs e)
        {
            this.setGridAppProjectsControl(true);
        }

        protected void gridAppProjects_Refresh(object sender, StoreReadDataEventArgs e)
        {
            this.bindGridAppProjectsList();
        }

        private void setGridAppProjectsControl(bool isRowSelected)
        {
            this.btnOperationMessageExportExcel.Disable();
            this.btnOperationMessageImportExcel.Disable();

            if (isRowSelected)
            {
                this.btnAppProjectsUpdate.Enable();
                this.btnAppProjectsDelete.Enable();
                this.btnAppProjectsRetention.Enable();
                this.btnOperationSchemesAdd.Enable();
                this.btnAppProjectsCopy.Enable();
                this.btnAppProjectsViewMobileDevices.Enable();
                this.btnAppProjectsSetBlackTime.Enable();

                // 绑定方案
                RowSelectionModel sm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
                int id = int.Parse(sm.SelectedRow.RecordID);
                this.bindOperationSchemesList(id);

                // 清空操作报文
                this.gridOperationMessages.DataBind();
                this.btnOperationMessagesAdd.Disable();
                this.btnOperationMessagesUpdate.Disable();
                this.btnOperationMessagesDelete.Disable();
                this.btnOperationMessageMoveUp.Disable();
                this.btnOperationMessageMoveDown.Disable();
            }
            else
            {
                this.btnAppProjectsUpdate.Disable();
                this.btnAppProjectsRetention.Disable();
                this.btnAppProjectsDelete.Disable();
                this.btnOperationSchemesAdd.Disable();
                this.btnAppProjectsCopy.Disable();
                this.btnAppProjectsViewMobileDevices.Disable();
                this.btnAppProjectsSetBlackTime.Disable();
            }
        }

        /// <summary>
        /// 绑定实体到子窗体
        /// </summary>
        /// <param name="appProjectsID"></param>
        private void bindAppProjects(int appProjectsID)
        {
            if (appProjectsID == 0)
            {
                this.txtAppProjectID.Text = ProjectConst.NEW_EMPTY_ID;
                this.txtAppProjectChineseName.Text = string.Empty;
                this.txtAppProjectAddLimit.Text = "1";
                this.dateFieldAppProjectStartDate.SelectedDate = DateTime.Now.Date;
                this.selectboxAppProjectStatus.Select(0);
                this.txtAppProjectsMemo.Text = string.Empty;
                this.txtAppProjectRetainDelayHour.Text = "0";
                this.subWindowAppProjects.Title = "新增项目";
            }
            else
            {
                this.txtAppProjectID.Text = appProjectsID.ToString();
                AppProjectsManager manager = new AppProjectsManager();
                AppProjects project = manager.SelectSingleAppProject(appProjectsID);
                this.txtAppProjectChineseName.Text = project.ChineseName;
                this.txtAppProjectAddLimit.Text = project.AddLimit.ToString();
                this.dateFieldAppProjectStartDate.SelectedDate = project.StartDate;
                this.selectboxAppProjectStatus.Select(project.Status.ToString());
                this.txtAppProjectsMemo.Text = project.Memo;
                this.txtAppProjectRetainDelayHour.Text = project.RetainDelayHour.ToString();
                this.subWindowAppProjects.Title = "修改项目";
            }
            this.subWindowAppProjects.Center();
            this.subWindowAppProjects.Show();
        }

        protected void btnAppProjectsAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindAppProjects(0);
        }

        protected void btnAppProjectsUpdate_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindAppProjects(id);
            }
        }

        protected void gridAppProjects_RowDblClick(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindAppProjects(id);
            }
        }

        protected void btnAppProjectsRetention_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindGridRetentions(id);
            }
        }

        protected void btnAppProjectsCopy_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                AppProjectsManager manager = new AppProjectsManager();
                int newAppProjectID = manager.CopyAppProjects(id);
                this.allocateAuthority(newAppProjectID);
                this.bindGridAppProjectsList();

                X.Msg.Info("复制完成!", "新的项目会复制原项目的信息,并包括操作方案和报文,留存率设置等.名字为会在原中文名称加上'_副本'字样,且状态设置为已暂停.请在编辑后启用", UI.Success).Show();
            }
        }

        protected void btnAppProjectsDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int appProjectID = int.Parse(rsm.SelectedRow.RecordID);
                AppProjectsManager manager = new AppProjectsManager();
                int result = manager.DeleteAppProjects(appProjectID);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                }
                // 删除此项目的权限
                RolesManager rolesManager = new RolesManager();
                rolesManager.RemoveAppProjectsFromAllRole(appProjectID);
                this.bindGridAppProjectsList();
                this.setGridAppProjectsControl(false);
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            } 
        }

        protected void btnAppProjectsViewMobileDevices_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int appProjectID = int.Parse(rsm.SelectedRow.RecordID);
                this.bindAppProjectsInMobileDevices(appProjectID);
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }
        }

        private void bindAppProjectsInMobileDevices(int appProjectID)
        {
            AppProjectsManager manager = new AppProjectsManager();
            List<AppProjectsInMobileDevices> list = manager.GetMobileDevicesListFromAppProjects(appProjectID);
            if (list.Count == 0)
            {
                X.Msg.Alert("系统提示", "该APP应用没有部署在任一台移动设备上!").Show();
            }
            else
            {
                this.subWindowMobileDeviceList.Center();
                this.subWindowMobileDeviceList.Show();

                this.gridMobileDeviceList.GetStore().DataSource = list;
                this.gridMobileDeviceList.GetStore().DataBind();
            }
        }

        protected void btnAppProjectsSetBlackTime_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int appProjectID = int.Parse(rsm.SelectedRow.RecordID);
                AppProjectsManager manager = new AppProjectsManager();
                List<AppProjectsBlackTime> list = manager.SelectBlackTimes(appProjectID);
                this.gridBlackTime.GetStore().DataSource = list;
                this.gridBlackTime.GetStore().DataBind();
                this.subWindowBlackTime.Center();
                this.subWindowBlackTime.Show();
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }
        }
        #endregion

        #region 子Windows AppProjects事件

        protected void btnAppProjectsSave_Click(object sender, DirectEventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                X.Msg.Alert("系统异常", "用户登录操时，请重新登录!").Show();
                return;
            }
            AppProjects project = null;
            AppProjectsManager manager = new AppProjectsManager();
            if (this.txtAppProjectID.Text == ProjectConst.NEW_EMPTY_ID)
            {
                project = new AppProjects();
                project.ID = 0;
            }
            else
            {
                project = manager.SelectSingleAppProject(int.Parse(this.txtAppProjectID.Text));
            }
            project.ChineseName = this.txtAppProjectChineseName.Text;
            project.AddLimit = int.Parse(this.txtAppProjectAddLimit.Text);
            project.StartDate = this.dateFieldAppProjectStartDate.SelectedDate;
            project.Status = int.Parse(this.selectboxAppProjectStatus.SelectedItem.Value);
            project.Memo = this.txtAppProjectsMemo.Text;
            project.RetainDelayHour = int.Parse(this.txtAppProjectRetainDelayHour.Text);

            int appProjectID = manager.AddOrUpdate(project);
            if (appProjectID == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!").Show();
                return;
            }
            else
            {
                if (this.txtAppProjectID.Text == ProjectConst.NEW_EMPTY_ID) // 新增项目要增加权限
                {
                    this.allocateAuthority(appProjectID);

                    #region 默认留存率
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 1, Retention = 90});
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 2, Retention = 85 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 3, Retention = 80 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 4, Retention = 75 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 5, Retention = 70 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 6, Retention = 65 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 7, Retention = 60 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 8, Retention = 55 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 9, Retention = 50 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 10, Retention = 45 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 11, Retention = 40 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 12, Retention = 35 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 13, Retention = 30 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 14, Retention = 25 });
                    manager.AddOrUpdateRentention(new AppProjectsRetention { AppProjectsID = appProjectID, Days = 15, Retention = 20 });
                    #endregion
                }
                this.AddSystemLog(string.Format("新增或修改了{0}号项目。", appProjectID));
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            }

            this.subWindowAppProjects.Hide();
            this.bindGridAppProjectsList();
        }

        /// <summary>
        /// 给指定的项目分配权限
        /// </summary>
        /// <param name="appProjectID"></param>
        private void allocateAuthority(int appProjectID)
        {
            RolesManager rolesManager = new RolesManager();
            UserInfos currentUser = this.GetCurrentUser();
            List<Roles> rolesList = rolesManager.GetRolesUserInfos(currentUser.ID);
            foreach (Roles role in rolesList)
            {
                rolesManager.AddOrUpdateAppProjectsInRole(
                        new AppProjectsInRole { AppProjectsID = appProjectID, RoleID = role.ID });
            }
            bool isAdmin = (bool)Session["IsAdmin"];
            if (!isAdmin) // 如果当前用户不是系统管理员，也要给系统管理员组赋权限
            {
                rolesManager.AddOrUpdateAppProjectsInRole(
                    new AppProjectsInRole { AppProjectsID = appProjectID, RoleID = ProjectConst.ADMIN_ROLE_ID });
            }
        }
        #endregion

        #region subWindowsRetention事件

        private void bindGridRetentions(int appProjectsID)
        {
            this.txtAppProjectID.Text = appProjectsID.ToString();
            AppProjectsManager manager = new AppProjectsManager();
            List<AppProjectsRetention> retentionList = manager.SelectRetentions(appProjectsID);
            this.gridRetention.GetStore().DataSource = retentionList;
            this.gridRetention.DataBind();

            this.subWindowRetention.Center();
            this.subWindowRetention.Show();
        }


        protected void btnRetentionAdd_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            int projectID = int.Parse(rsm.SelectedRow.RecordID);

            AppProjectsManager manager = new AppProjectsManager();
            manager.InsertRetentions(projectID, 1);
            this.bindGridRetentions(projectID);
        }

        [DirectMethod(Namespace = "Mario")]
        public void gridRetention_Edit(int id, string field, string oldValue, string newValue, object data)
        {
            //string message = "修改了为ID为{0}的记录，属性为: {1}，保存值为:{2}";

            AppProjectsManager manager = new AppProjectsManager();
            AppProjectsRetention retention = manager.SelectSingleRetention(id);
            retention.Retention = int.Parse(newValue);
            manager.AddOrUpdateRentention(retention);

            this.gridRetention.GetStore().GetById(id).Commit();
        }

        protected void btnRetentionDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            int projectID = int.Parse(sm.SelectedRow.RecordID);

            AppProjectsManager manager = new AppProjectsManager();
            int result = manager.RemoveRententionFromLastDay(projectID);

            this.bindGridRetentions(projectID);
        }

        protected void btnRetentionExportExcel_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            int appProjectsID = int.Parse(rsm.SelectedRow.RecordID);
            AppProjectsManager manager = new AppProjectsManager();
            List<AppProjectsRetention> retentionList = manager.SelectRetentions(appProjectsID);

            // 根据模板文件创建副本
            string templateFilePath = Server.MapPath("../Resources/ExcelFiles/Template/RetentionTemplate.xlsx");
            string relativeFilePath = string.Format("../Resources/ExcelFiles/Temp/{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            string outputFilePath = Server.MapPath(relativeFilePath);
            File.Copy(templateFilePath, outputFilePath, true);
            // 使用OleDb驱动程序连接到副本
            OleDbConnection conn = new OleDbConnection(string.Format(ProjectConst.EXCEL_CONNECTION_STRING, outputFilePath));
            using (conn)
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(null, conn);
                foreach (AppProjectsRetention retention in retentionList)
                {
                    cmd.CommandText = string.Format("INSERT INTO [Sheet1$]([天数], [留存率（%）]) VALUES({0},{1})",
                        retention.Days, retention.Retention);
                    cmd.ExecuteNonQuery();
                }
            }
            string downloadUrl = ConfigurationManager.AppSettings["ServerHostUrl"] + ResolveUrl(relativeFilePath);
            X.Msg.Alert("导出成功", string.Format("下载地址为：<a href='{0}'>{0}</a>", downloadUrl)).Show();
        }

        protected void btnRetentionImportExcel_Click(object sender, DirectEventArgs e)
        {
            if (this.btnRetentionImportExcel.HasFile)
            {
                int fileSize = Int32.Parse(this.btnRetentionImportExcel.PostedFile.ContentLength.ToString());
                if (fileSize > 2 * 1024 * 1024)
                {
                    X.Msg.Alert("提示信息", "上传文件过大！要上传的文件不能超过2M.").Show();
                    return;
                }
                string extensionName = Path.GetExtension(this.btnRetentionImportExcel.PostedFile.FileName).ToLower();//获取文件后缀
                if (extensionName != ".xlsx")
                {
                    X.Msg.Alert("提示信息", "文件格式不正确！只能为xlsx等Excel专用！建议先导出后文件进行修改,最后再导入.").Show();
                    return;
                }
                string remoteFileNamePath = Server.MapPath("../Resources/ExcelFiles/Temp/" + Path.GetFileName(this.btnRetentionImportExcel.PostedFile.FileName));
                if (File.Exists(remoteFileNamePath))
                {
                    File.Delete(remoteFileNamePath);
                }
                this.btnRetentionImportExcel.PostedFile.SaveAs(remoteFileNamePath);

                #region 导入数据

                OleDbConnection conn = new OleDbConnection(string.Format(ProjectConst.EXCEL_CONNECTION_STRING, remoteFileNamePath));
                DataTable dt = new DataTable();
                using (conn)
                {
                    OleDbDataAdapter da = new OleDbDataAdapter(
                    "SELECT * FROM [Sheet1$]", conn);
                    da.Fill(dt);
                }

                RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
                int appProjectID = int.Parse(rsm.SelectedRow.RecordID);

                AppProjectsManager manager = new AppProjectsManager();
                int deleteCount = manager.RemoveAllRententions(appProjectID); // 删除全部留存率
                int insertCount = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        AppProjectsRetention retention = new AppProjectsRetention();
                        retention.AppProjectsID = appProjectID;
                        retention.Days = int.Parse(dr[0].ToString());
                        retention.Retention = int.Parse(dr[1].ToString());
                        manager.AddOrUpdateRentention(retention);
                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        X.Msg.Info("导入异常", ex.Message, UI.Danger).Show();
                        continue;
                    }
                }

                #endregion

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "导入完成!",
                    Message = string.Format("共删除了{0}条原有记录，并导入了新增{1}条记录!", deleteCount, insertCount)
                });
                File.Delete(remoteFileNamePath); // 导入后删除文件

                this.bindGridRetentions(appProjectID);
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "导入失败",
                    Message = "没有需要上传的文件."
                });
            }
            this.btnRetentionImportExcel.Reset();
        }

        #endregion

        #region subWindowMobileDeviceList事件

        protected void btnMobileDeviceListAdd_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            int appProjectID = int.Parse(rsm.SelectedRow.RecordID);
            int mobileID = Convert.ToInt32(txtMobileDeviceID.Text);
            AppProjectsManager manager = new AppProjectsManager();
            List<AppProjectsInMobileDevices> existList = manager.GetMobileDevicesListFromAppProjects(appProjectID);
            foreach (AppProjectsInMobileDevices apimd in existList)
            {
                if (apimd.AppProjectsID == appProjectID && apimd.MobileDevicesID == mobileID)
                {
                    X.Msg.Alert("系统提示", "此设备已经添加过了。").Show();
                    return;
                }
            }
            MobileDevicesManager mobileManager = new MobileDevicesManager();
            List<MobileDevices> mobileDevicesList = mobileManager.SelectAllMobileDevices();
            bool isExistMobileDevice = false;
            foreach (MobileDevices mobileDevices in mobileDevicesList)
            {
                if (mobileID == mobileDevices.ID)
                {
                    isExistMobileDevice = true;
                    break;
                }
            }
            if (!isExistMobileDevice)
            {
                X.Msg.Alert("系统提示", "不存在此设备ID。").Show();
                return;
            }
            manager.AddOrUpdateAppProjectsInMobileDevices(new AppProjectsInMobileDevices { AppProjectsID = appProjectID, MobileDevicesID = mobileID });
            this.bindAppProjectsInMobileDevices(appProjectID); 
        }

        protected void btnMobileDeviceListDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridMobileDeviceList.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                AppProjectsManager manager = new AppProjectsManager();
                int result = manager.DeleteAppProjectsInMobileDevices(id);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                    RowSelectionModel sm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
                    int projectID = int.Parse(sm.SelectedRow.RecordID);
                    this.bindAppProjectsInMobileDevices(projectID); 
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            } 
        }


        #endregion

        #region subWindowsBlackTime事件

        private void bindGridBlackTime(int appProjectsID)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            int projectID = int.Parse(rsm.SelectedRow.RecordID);
            
            AppProjectsManager manager = new AppProjectsManager();
            List<AppProjectsBlackTime> list = manager.SelectBlackTimes(projectID);
            this.gridBlackTime.GetStore().DataSource = list;
            this.gridBlackTime.DataBind();
        }

        protected void btnBlackTimeAdd_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
            int projectID = int.Parse(rsm.SelectedRow.RecordID);

            AppProjectsManager manager = new AppProjectsManager();
            manager.AddOrUpdateBlackTime(new AppProjectsBlackTime { AppProjectsID = projectID, StartTime = new TimeSpan(2, 0, 0), EndTime = new TimeSpan(2, 0, 0) });
            this.bindGridBlackTime(projectID);
        }

        [DirectMethod(Namespace = "Mario")]
        public void gridBlackTime_Edit(int id, string field, string oldValue, string newValue, object data)
        {
            AppProjectsManager manager = new AppProjectsManager();
            AppProjectsBlackTime blackTime = manager.SelectSingleBlackTime(id);
            Type type = typeof(AppProjectsBlackTime);
            PropertyInfo property = type.GetProperty(field);
            object strongValue = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(newValue);
            property.SetValue(blackTime, strongValue);
            if (blackTime.StartTime < blackTime.EndTime)
            {
                manager.AddOrUpdateBlackTime(blackTime);
                this.gridBlackTime.GetStore().GetById(id).Commit();
            }
            else
            {
                X.Msg.Alert("系统提示", "开始时间必须小于结束时间").Show();
                this.gridBlackTime.GetStore().GetById(id).Reject();
            }
            
        }

        protected void btnBlackTimeDelete_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel csm = this.gridBlackTime.GetSelectionModel() as CellSelectionModel;
            int blackTimeID;
            try
            {
                blackTimeID = int.Parse(csm.SelectedCell.RecordID);
            }
            catch(Exception ex)
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
                return;
            }
            AppProjectsManager manager = new AppProjectsManager();
            int result = manager.DeleteBlackTime(blackTimeID);
            if (result == 0)
            {
                X.Msg.Alert("系统提示", "删除失败!").Show();
            }
            else
            {
                X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                RowSelectionModel sm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
                int projectID = int.Parse(sm.SelectedRow.RecordID);
                this.bindGridBlackTime(projectID);
            }
        }


        #endregion

        #region gridOperationSchemes相关事件

        private void bindOperationSchemesList(int appProjectsID)
        {
            OperationSchemesManager manager = new OperationSchemesManager();
            this.gridOperationSchemes.GetStore().DataSource = manager.SelectAllOperationSchemesFromAppProject(appProjectsID);
            this.gridOperationSchemes.GetStore().DataBind();
        }

        private void setGridOperationSchemesControl(bool isRowSelected)
        {
            if (isRowSelected)
            {
                this.btnOperationSchemesUpdate.Enable();
                this.btnOperationSchemesDelete.Enable();
                this.btnOperationMessagesAdd.Enable();
                RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                int id = int.Parse(sm.SelectedRow.RecordID);
                this.bindOperationMessagesList(id);
                this.btnOperationMessageExportExcel.Enable();
                this.btnOperationMessageImportExcel.Enable();
            }
            else
            {
                this.btnOperationSchemesUpdate.Disable();
                this.btnOperationSchemesDelete.Disable();
                this.btnOperationMessagesAdd.Disable();
                this.btnOperationMessageExportExcel.Disable();
                this.btnOperationMessageImportExcel.Disable();
            }
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridOperationSchemes_RowSelected(object sender, DirectEventArgs e)
        {
            this.setGridOperationSchemesControl(true);
        }

        /// <summary>
        /// 绑定实体到子窗体
        /// </summary>
        /// <param name="id"></param>
        private void bindOperationSchemes(int operationSchemesID)
        {
            if (operationSchemesID == 0)
            {
                this.txtOperationSchemesID.Text = ProjectConst.NEW_EMPTY_ID;
                this.txtOperationSchemesName.Text = string.Empty;
                this.selectBoxSchemeType.Select(0);
                // 所属项目ID
                RowSelectionModel rsm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
                this.txtOperationSchemesBelongToAppProjectID.Text = rsm.SelectedRow.RecordID;
                this.subWindowOperationSchemes.Title = "新增";
            }
            else
            {
                this.txtOperationSchemesID.Text = operationSchemesID.ToString();
                OperationSchemesManager manager = new OperationSchemesManager();
                OperationSchemes scheme = manager.SelectSingleEntity(operationSchemesID);
                this.txtOperationSchemesName.Text = scheme.Name;
                this.selectBoxSchemeType.Select(scheme.SchemeType.ToString());
                this.txtOperationSchemesBelongToAppProjectID.Text = scheme.AppProjectsID.ToString();
                this.subWindowOperationSchemes.Title = "修改";
            }
            this.subWindowOperationSchemes.Title += "方案(项目中至少要有一个留存方案)";
            this.subWindowOperationSchemes.Center();
            this.subWindowOperationSchemes.Show();
        }

        protected void btnOperationSchemesAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindOperationSchemes(0);
        }

        protected void btnOperationSchemesUpdate_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindOperationSchemes(id);
            }
        }

        protected void btnOperationSchemesDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                OperationSchemesManager manager = new OperationSchemesManager();
                int result = manager.Delete(id);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                    RowSelectionModel sm = this.gridAppProjects.GetSelectionModel() as RowSelectionModel;
                    int projectID = int.Parse(sm.SelectedRow.RecordID);
                    this.bindOperationSchemesList(projectID);

                    this.setGridOperationSchemesControl(false);
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }

        }
        #endregion

        #region 子Windows OperationSchemes事件

        protected void btnOperationSchemesSave_Click(object sender, DirectEventArgs e)
        {
            OperationSchemes scheme = null;
            OperationSchemesManager manager = new OperationSchemesManager();
            int operationSchemesID = 0;
            if (this.txtOperationSchemesID.Text == ProjectConst.NEW_EMPTY_ID)
            {
                scheme = new OperationSchemes();
                scheme.ID = operationSchemesID;
            }
            else
            {
                operationSchemesID = int.Parse(this.txtOperationSchemesID.Text);
                scheme = manager.SelectSingleEntity(operationSchemesID);
            }
            scheme.Name = this.txtOperationSchemesName.Text;
            scheme.AppProjectsID = int.Parse(this.txtOperationSchemesBelongToAppProjectID.Text);
            scheme.SchemeType = int.Parse(this.selectBoxSchemeType.SelectedItem.Value);

            // 同时复制方案
            if (this.chkIsCopyOperationSchemes.Checked)
            {
                int newProjectID = int.Parse(this.selectBoxToAppProject.SelectedItem.Value);
                manager.CopyOperationSchemesToAppProject(operationSchemesID, newProjectID);
            }

            int result = manager.AddOrUpdate(scheme);
            if (result == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!").Show();
            }
            else
            {
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            }

            this.subWindowOperationSchemes.Hide();
            this.bindOperationSchemesList(scheme.AppProjectsID);
        }

        #endregion

        #region gridOperationMessages相关事件

        private void bindOperationMessagesList(int operationSchemesID)
        {
            OperationMessagesManager manager = new OperationMessagesManager();
            this.gridOperationMessages.GetStore().DataSource = manager.SelectOperationMessagesFromOperationSchemes(operationSchemesID);
            this.gridOperationMessages.GetStore().DataBind();
        }

        private void setGridOperationMessagesControl(bool isRowSelected)
        {
            if (isRowSelected)
            {
                this.btnOperationMessagesUpdate.Enable();
                this.btnOperationMessagesDelete.Enable();
                this.btnOperationMessageMoveUp.Enable();
                this.btnOperationMessageMoveDown.Enable();
                this.btnOperationMessageCopy.Enable();
            }
            else
            {
                this.btnOperationMessagesUpdate.Disable();
                this.btnOperationMessagesDelete.Disable();
                this.btnOperationMessageMoveUp.Disable();
                this.btnOperationMessageMoveDown.Disable();
                this.btnOperationMessageCopy.Disable();
            }
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridOperationMessages_RowSelected(object sender, DirectEventArgs e)
        {
            this.setGridOperationMessagesControl(true);
        }

        /// <summary>
        /// 绑定实体到子窗体
        /// </summary>
        /// <param name="operationMessagesID"></param>
        private void bindOperationMessages(int operationMessagesID)
        {
            OperationMessagesManager manager = new OperationMessagesManager();
            if (operationMessagesID == 0)
            {
                this.txtOperationMessagesID.Text = ProjectConst.NEW_EMPTY_ID;
                // 所属方案ID
                RowSelectionModel rsm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                int schemeID = int.Parse(rsm.SelectedRow.RecordID);
                this.txtOperationMessagesBelongToSchemesID.Text = schemeID.ToString();
                this.txtStep.Text = (manager.GetMaxStep(schemeID) + 1).ToString();
                this.txtXPoint.Text = "1";
                this.txtYPoint.Text = "1";
                this.txtToXPoint.Text = "-1";
                this.txtToYPoint.Text = "-1";
                this.txtPhysicalKey.Text = "-1";
                this.txtInterval.Text = "1000";
                this.selectboxAction.Select(0);
                this.txtCommandScript.Text = string.Empty;
                this.txtMemo.Text = string.Empty;
                this.subWindowOperationMessages.Title = "新增";
            }
            else
            {
                this.txtOperationMessagesID.Text = operationMessagesID.ToString();
                OperationMessages message = manager.SelectSingleEntity(operationMessagesID);
                this.txtOperationMessagesBelongToSchemesID.Text = message.OperationSchemesID.ToString();
                this.txtStep.Value = message.Step;
                this.txtXPoint.Value = message.XPoint;
                this.txtYPoint.Value = message.YPoint;
                this.txtToXPoint.Value = message.ToXPoint;
                this.txtToYPoint.Value = message.ToYPoint;
                this.txtPhysicalKey.Value = message.PhysicalKey;
                this.txtInterval.Value = message.Interval;
                this.selectboxAction.Select(message.Action.ToString());
                this.txtCommandScript.Text = message.CommandScript;
                this.txtMemo.Text = message.Memo;
                this.subWindowOperationMessages.Title = "修改";
            }
            this.subWindowOperationMessages.Center();
            this.subWindowOperationMessages.Show();
        }

        protected void btnOperationMessagesAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindOperationMessages(0);
        }

        protected void btnOperationMessagesUpdate_Click(object sender, DirectEventArgs e)
        {
            ShowUpdateMessagesWindows();
        }


        private void ShowUpdateMessagesWindows()
        {
            RowSelectionModel rsm = this.gridOperationMessages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindOperationMessages(id);
            }
        }

        protected void btnOperationMessagesDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridOperationMessages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                OperationMessagesManager manager = new OperationMessagesManager();
                int result = manager.Delete(id);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                    RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                    int schemeID = int.Parse(sm.SelectedRow.RecordID);
                    this.bindOperationMessagesList(schemeID);

                    this.setGridOperationMessagesControl(false);
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }
        }

        protected void btnOperationMessageMoveUp_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridOperationMessages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                OperationMessagesManager manager = new OperationMessagesManager();
                string result = manager.MoveUp(id);
                if (result == null)
                {
                    RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                    int schemeID = int.Parse(sm.SelectedRow.RecordID);
                    this.bindOperationMessagesList(schemeID);

                }
                else
                {
                    X.Msg.Alert("系统提示", result).Show();
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }
        }

        protected void btnOperationMessageMoveDown_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridOperationMessages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                OperationMessagesManager manager = new OperationMessagesManager();
                string result = manager.MoveDown(id);
                if (result == null)
                {
                    RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                    int schemeID = int.Parse(sm.SelectedRow.RecordID);
                    this.bindOperationMessagesList(schemeID);

                }
                else
                {
                    X.Msg.Alert("系统提示", result).Show();
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }

        }

        protected void btnOperationMessageCopy_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridOperationMessages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                OperationMessagesManager manager = new OperationMessagesManager();
                manager.CopyMessage(id);
                RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                int schemeID = int.Parse(sm.SelectedRow.RecordID);
                this.bindOperationMessagesList(schemeID);

            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }

        }

        protected void btnOperationMessageExportExcel_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
            int schemeID = int.Parse(sm.SelectedRow.RecordID);
            OperationMessagesManager manager = new OperationMessagesManager();
            List<OperationMessages> operationMessagesList = manager.SelectOperationMessagesFromOperationSchemes(schemeID);
            if (operationMessagesList.Count == 0)
            {
                X.Msg.Alert("导出失败", "没有需要导出的数据.").Show();
                return;
            }

            // 根据模板文件创建副本
            string templateFilePath = Server.MapPath("../Resources/ExcelFiles/Template/OperationMessagesTemplate.xlsx");
            string relativeFilePath = string.Format("../Resources/ExcelFiles/Temp/OperationMessages{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            string outputFilePath = Server.MapPath(relativeFilePath);
            File.Copy(templateFilePath, outputFilePath, true);
            // 使用OleDb驱动程序连接到副本
            OleDbConnection conn = new OleDbConnection(string.Format(ProjectConst.EXCEL_CONNECTION_STRING, outputFilePath));
            try
            {
                conn.Open();
                // 增加记录
                OleDbCommand cmd = new OleDbCommand(null, conn);
                foreach (OperationMessages om in operationMessagesList)
                {
                    cmd.CommandText = string.Format("INSERT INTO [Sheet1$]([步骤序号], [起始X座标], [起始Y座标], [目的X座标], [目的Y座标], [物理按键(数字)], [操作间隔(毫秒)], [动作(数字)], [命令脚本], [备注]) VALUES({0},{1},{2},{3}, {4}, {5},{6}, {7},'{8}','{9}')",
                        om.Step, om.XPoint, om.YPoint, om.ToXPoint, om.ToYPoint, om.PhysicalKey, om.Interval, om.Action, om.CommandScript, om.Memo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                X.Msg.Alert("出现异常", ex.Message + ex.StackTrace).Show();
                return;
            }
            finally
            {
                conn.Close();
            }
            string downloadUrl = ConfigurationManager.AppSettings["ServerHostUrl"] + ResolveUrl(relativeFilePath);
            X.Msg.Alert("导出成功", string.Format("下载地址为：<a href='{0}'>{0}</a>", downloadUrl)).Show();
            this.bindOperationMessagesList(schemeID);
            this.setGridOperationMessagesControl(false);
        }

        protected void btnOperationMessageImportExcel_Click(object sender, DirectEventArgs e)
        {
            if (this.btnOperationMessageImportExcel.HasFile)
            {
                int fileSize = Int32.Parse(this.btnOperationMessageImportExcel.PostedFile.ContentLength.ToString());
                if (fileSize > 2 * 1024 * 1024)
                {
                    X.Msg.Alert("提示信息", "上传文件过大！要上传的文件不能超过2M.").Show();
                    return;
                }
                string extensionName = Path.GetExtension(this.btnOperationMessageImportExcel.PostedFile.FileName).ToLower();//获取文件后缀
                if (extensionName != ".xlsx")
                {
                    X.Msg.Alert("提示信息", "文件格式不正确！只能为xlsx等Excel专用！建议先导出后文件进行修改,最后再导入.").Show();
                    return;
                }
                string remoteFileNamePath = Server.MapPath("../Resources/ExcelFiles/Temp/" + Path.GetFileName(this.btnOperationMessageImportExcel.PostedFile.FileName));
                if (File.Exists(remoteFileNamePath))
                {
                    File.Delete(remoteFileNamePath);
                }
                this.btnOperationMessageImportExcel.PostedFile.SaveAs(remoteFileNamePath);

                #region 导入数据

                OleDbConnection conn = new OleDbConnection(string.Format(ProjectConst.EXCEL_CONNECTION_STRING, remoteFileNamePath));
                DataTable dt = new DataTable();
                using (conn)
                {
                    OleDbDataAdapter da = new OleDbDataAdapter(
                    "SELECT * FROM [Sheet1$]", conn);
                    da.Fill(dt);
                }

                RowSelectionModel sm = this.gridOperationSchemes.GetSelectionModel() as RowSelectionModel;
                int schemeID = int.Parse(sm.SelectedRow.RecordID);
                OperationMessagesManager manager = new OperationMessagesManager();
                int deleteCount = manager.DeleteFromOperationSchemes(schemeID); // 删除现有的报文
                int insertCount = 0;

                foreach(DataRow dr in dt.Rows)
                {
                    try
                    {
                        OperationMessages om = new OperationMessages();
                        // [步骤序号], [起始X座标], [起始Y座标], [目的X座标], [目的Y座标], [物理按键(数字)], [操作间隔(毫秒)], [动作(数字)], [命令脚本]
                        om.Step = int.Parse(dr[0].ToString());
                        om.XPoint = int.Parse(dr[1].ToString());
                        om.YPoint = int.Parse(dr[2].ToString());
                        om.ToXPoint = int.Parse(dr[3].ToString());
                        om.ToYPoint = int.Parse(dr[4].ToString());
                        om.PhysicalKey = int.Parse(dr[5].ToString());
                        om.Interval = int.Parse(dr[6].ToString());
                        om.Action = int.Parse(dr[7].ToString());
                        om.CommandScript = dr[8].ToString();
                        om.Memo = dr[9].ToString();
                        om.OperationSchemesID = schemeID;
                        manager.AddOrUpdate(om);
                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        X.Msg.Alert("导入异常", ex.Message).Show();
                        continue;
                    }
                }
                
                #endregion

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "导入完成!",
                    Message = string.Format("共删除了{0}条原有报文，并导入了新增{1}条报文!", deleteCount, insertCount)
                });
                File.Delete(remoteFileNamePath); // 导入后删除文件

                this.bindOperationMessagesList(schemeID);
                this.setGridOperationMessagesControl(false);
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "导入失败",
                    Message = "没有需要上传的文件."
                });
            }
            this.btnOperationMessageImportExcel.Reset();
        }
        #endregion

        #region 子Windows OperationMessage事件

        protected void btnOperationMessagesSave_Click(object sender, DirectEventArgs e)
        {
            int operationMessagesID = 0;

            OperationMessages message = null;
            OperationMessagesManager manager = new OperationMessagesManager();
            if (this.txtOperationMessagesID.Text == ProjectConst.NEW_EMPTY_ID)
            {
                message = new OperationMessages();
                message.ID = operationMessagesID;
            }
            else
            {
                operationMessagesID = int.Parse(this.txtOperationMessagesID.Text);
                message = manager.SelectSingleEntity(operationMessagesID);
            }
            message.OperationSchemesID = int.Parse(this.txtOperationMessagesBelongToSchemesID.Text);
            message.Step = int.Parse(this.txtStep.Text);
            message.XPoint = int.Parse(this.txtXPoint.Text);
            message.YPoint = int.Parse(this.txtYPoint.Text);
            message.ToXPoint = int.Parse(this.txtToXPoint.Text);
            message.ToYPoint = int.Parse(this.txtToYPoint.Text);
            message.PhysicalKey = int.Parse(this.txtPhysicalKey.Text);
            message.Interval = int.Parse(this.txtInterval.Text);
            message.Action = int.Parse(this.selectboxAction.SelectedItem.Value);
            message.CommandScript = this.txtCommandScript.Text;
            message.Memo = this.txtMemo.Text;

            manager.SaveMessage(message);
            X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();

            this.subWindowOperationMessages.Hide();
            this.bindOperationMessagesList(message.OperationSchemesID);
        }

        [DirectMethod(Namespace = "Mario")]
        public void gridOperationMessages_Edit(int id, string field, string oldValue, string newValue, object data)
        {
            OperationMessagesManager manager = new OperationMessagesManager();
            OperationMessages om = manager.SelectSingleEntity(id);

            Type type = typeof(OperationMessages);
            PropertyInfo property = type.GetProperty(field);
            object strongValue = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(newValue);
            property.SetValue(om, strongValue);
            manager.AddOrUpdate(om);

            this.gridOperationMessages.GetStore().GetById(id).Commit();
        }

        #endregion

    }
}