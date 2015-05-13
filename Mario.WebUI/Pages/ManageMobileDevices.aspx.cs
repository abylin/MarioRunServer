using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;
using Newtonsoft.Json;

namespace Mario.WebUI.Pages
{
    public partial class ManageMobileDevices : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!X.IsAjaxRequest)
            {
                bindMobileDevicesList();
            }
        }

        #region 左边移动设备相关事件

        protected void btnMobileDevicesSubmit_Click(object sender, DirectEventArgs e)
        {
            int mobileDeviceID = int.Parse(this.txtMobileDevicesID.Text);
            MobileDevicesManager manager = new MobileDevicesManager();
            MobileDevices mobileDevice = manager.SelectSingleMobileDevices(mobileDeviceID);
            mobileDevice.Memo = this.txtMobileDevicesMemo.Text;
            mobileDevice.InUse = this.chkInUse.Checked;
            int result = manager.AddOrUpdate(mobileDevice);
            if (result == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!").Show();
            }
            else
            {
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            }

            this.subWindowMobileDevices.Hide();
            this.bindMobileDevicesList();
        }

        private void bindMobileDevicesList()
        {
            MobileDevicesManager manager = new MobileDevicesManager();
            this.gridMobileDevices.GetStore().DataSource = manager.SelectAllMobileDevices();
            this.gridMobileDevices.GetStore().DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridMobileDevices_RowSelected(object sender, DirectEventArgs e)
        {
            bindGridMobileDevices();
        }

        private void bindGridMobileDevices()
        {
            CheckboxSelectionModel sm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            if (sm.SelectedRows.Count == 1)
            {
                this.btnMobileDeviceModify.Enable();
                this.btnMobileDeviceDelete.Enable();
                this.btnMobileDeviceJSONMessage.Enable();
                this.btnMobileDeviceReboot.Enable();
                this.btnAppDevicesInMobileDevicesConfig.Enable();
                int deviceID = int.Parse(sm.SelectedRows[0].RecordID);
                this.bindAppProjectsInMobileDevicesList(deviceID);             
            }
            else
            {
                this.btnMobileDeviceModify.Disable();
                this.btnMobileDeviceDelete.Disable();
                this.btnMobileDeviceJSONMessage.Disable();
                if (sm.SelectedRows.Count > 1)
                {
                    this.btnMobileDeviceReboot.Enable();
                }
                else
                {
                    this.btnMobileDeviceReboot.Disable();
                }
                this.btnAppDevicesInMobileDevicesConfig.Disable(); 
            }
        }

        /// <summary>
        /// 绑定实体到子窗体
        /// </summary>
        /// <param name="mobileDevicesID"></param>
        private void bindMobileDevice(int mobileDevicesID)
        {
            MobileDevicesManager manager = new MobileDevicesManager();
            MobileDevices device = manager.SelectSingleMobileDevices(mobileDevicesID);

            this.txtMobileDevicesID.Text = mobileDevicesID.ToString();
            this.txtMobileDevicesMemo.Text = device.Memo;
            this.txtRealIMEI.Text = device.RealIMEI.ToString();
            this.txtRealModel.Text = device.RealModel;
            DateTime convertTime = device.LastResponseTime ?? DateTime.MinValue;
            if (convertTime == DateTime.MinValue)
            {
                this.txtLastResponseTime.Text = string.Empty;
            }
            else
            {
                this.txtLastResponseTime.Text = convertTime.ToString("yyyy-MM-dd HH:mm:SS");
            }
            
            this.chkInUse.Checked = device.InUse;
            this.subWindowMobileDevices.Title = "修改";
            this.subWindowMobileDevices.Center();
            this.subWindowMobileDevices.Show();
        }


        protected void btnMobileDeviceModify_Click(object sender, DirectEventArgs e)
        {
            CheckboxSelectionModel csm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            if (csm.SelectedRows.Count == 1)
            {
                int id = int.Parse(csm.SelectedRows[0].RecordID);
                this.bindMobileDevice(id);
            }
        }

        protected void btnMobileDeviceReboot_Click(object sender, DirectEventArgs e)
        {
            CheckboxSelectionModel csm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            if (csm.SelectedRows.Count >0)
            {
                foreach (SelectedRow row in csm.SelectedRows)
                {
                    int id = int.Parse(row.RecordID);
                    MobileDevicesManager manager = new MobileDevicesManager();
                    manager.SetReboot(id, true); 
                }
                X.Msg.Info("系统提示", csm.SelectedRows.Count.ToString() + "台设备设置重启成功!", UI.Success).Show();
            }
        }


        protected void gridMobileDevices_RowDblClick(object sender, DirectEventArgs e)
        {
            CheckboxSelectionModel rsm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRows[0].RecordID);
                this.bindMobileDevice(id);
            }
        }

        protected void btnMobileDeviceDelete_Click(object sender, DirectEventArgs e)
        {
            CheckboxSelectionModel rsm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRows[0].RecordID);
                MobileDevicesManager manager = new MobileDevicesManager();
                int result = manager.Delete(id);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                    bindMobileDevicesList();
                    bindGridMobileDevices();
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "请选择一台要操作的设备。").Show();
            }
        }

        protected void btnMobileDeviceJSONMessage_Click(object sender, DirectEventArgs e)
        {
            CheckboxSelectionModel csm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            if (csm.SelectedRows.Count == 1)
            {
                int mobileDeviceID = int.Parse(csm.SelectedRows[0].RecordID);
                MobileDevicesManager mobileDevicesManager = new MobileDevicesManager();
                MobileDevices mobileDevices = mobileDevicesManager.SelectSingleMobileDevices(mobileDeviceID);
                TasksManager tasksManager = new TasksManager();
                ResponseMessage message = tasksManager.GetMobileDeviceTask(mobileDevices, false);
                string responeText = JsonConvert.SerializeObject(message);

                X.Msg.Alert("移动设备将收到的JSON报文（建议复制到文本编辑器中查看）", responeText).Show();
            }
            else
            {
                X.Msg.Alert("系统提示", "请选择一台要操作的设备").Show();
            }
            

        }

        #endregion

        #region 右边AppProjects相关事件

        protected void SubmitSelectedAppProjectsData(object sender, StoreSubmitDataEventArgs e)
        {
            //string json = e.Json;
            //string xml = e.Xml.ToString(); ;
            List<AppProjects> selectedAppProjectsList = e.Object<AppProjects>();

            CheckboxSelectionModel sm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;
            int deviceID = int.Parse(sm.SelectedRows[0].RecordID);
            AppProjectsManager manager = new AppProjectsManager();
            manager.SetAppProjectsFromMobileDevices(deviceID, selectedAppProjectsList);

            X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();

            this.subWindowAppProjectsSelector.Hide();
            this.bindAppProjectsInMobileDevicesList(deviceID);
        }

        private void bindAppProjectsInMobileDevicesList(int deviceID)
        {
            AppProjectsManager manager = new AppProjectsManager();
            this.gridAppProjectsInMobileDevices.GetStore().DataSource = manager.GetSelectedAppProjectsFromMobileDevices(deviceID);
            this.gridAppProjectsInMobileDevices.DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridAppProjectsInMobileDevices_RowSelected(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.gridAppProjectsInMobileDevices.GetSelectionModel() as RowSelectionModel;
            if (sm.SelectedRows.Count == 0)
            {
                this.btnAppDevicesInMobileDevicesDelete.Disable();
            }
            else
            {
                this.btnAppDevicesInMobileDevicesDelete.Enable();
            }
        }

        protected void btnAppDevicesInMobileDevicesConfig_Click(object sender, DirectEventArgs e)
        {
            CheckboxSelectionModel sm = this.gridMobileDevices.GetSelectionModel() as CheckboxSelectionModel;

            int deviceID = int.Parse(sm.SelectedRows[0].RecordID);

            // 绑定待选和已选的APP项目
            AppProjectsManager manager = new AppProjectsManager();
            this.GridPanel1.GetStore().DataSource = manager.GetNotSelectAppProjectsFromMobileDevices(deviceID);
            this.GridPanel1.GetStore().DataBind();

            this.GridPanel2.GetStore().DataSource = manager.GetSelectedAppProjectsFromMobileDevices(deviceID);
            this.GridPanel2.GetStore().DataBind();

            this.subWindowAppProjectsSelector.Center();
            this.subWindowAppProjectsSelector.Show();
        }

        protected void btnAppDevicesInMobileDevicesDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.gridAppProjectsInMobileDevices.GetSelectionModel() as RowSelectionModel;
            RowSelectionModel rsmMobileDevice = this.gridMobileDevices.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1 && rsmMobileDevice.SelectedRows.Count == 1)
            {
                int appProjectID = int.Parse(rsm.SelectedRow.RecordID);
                int deviceID = int.Parse(rsmMobileDevice.SelectedRows[0].RecordID);
                MobileDevicesManager manager = new MobileDevicesManager();
                int result = manager.RemoveAppProjectsFromMobileDevice(deviceID, appProjectID);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                    this.bindAppProjectsInMobileDevicesList(deviceID);
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