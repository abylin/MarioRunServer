using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;
using System.Threading;

namespace Mario.WebUI.Pages
{

    public partial class ViewTodayTasks : BasePage
    {
        public StoreFilterData[] storeTaskDataArray
        {
            get
            {
                return new StoreFilterData[]{
                    new StoreFilterData{id = 0 ,text = "待执行"},
                    new StoreFilterData{id = 1 ,text = "执行中"},
                    new StoreFilterData{id = 2 ,text = "已完成"}
                };
            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                initSelectBoxAppProject(0);
            }
        }

        private void initSelectBoxAppProject(int selectedIndex)
        {
            AppProjectsManager manager = new AppProjectsManager();
            UserInfos currentUser = this.GetCurrentUser();
            List<AppProjects> list = manager.SelectAllEntities(false, currentUser.ID);
            this.selectBoxAppProject.Items.Clear();
            foreach (AppProjects project in list)
            {
                this.selectBoxAppProject.Items.Add(new Ext.Net.ListItem(project.ChineseName, project.ID));
            }
            if (this.selectBoxAppProject.Items.Count > selectedIndex)
            {
                this.selectBoxAppProject.Select(selectedIndex);
            }  
        }

        protected void GridVirtualIMEI_Refresh(object sender, StoreReadDataEventArgs e)
        {
            this.bindTaskGrid();
        }

        private void bindTaskGrid()
        {
            int appProjectID = int.Parse(this.selectBoxAppProject.SelectedItem.Value);
            bool isNewAdd = false;
            if (this.selectboxQueryType.SelectedItem.Value == null)
            {
                this.selectBoxAppProject.Select(0);
            }
            else
            { 
                isNewAdd = bool.Parse(this.selectboxQueryType.SelectedItem.Value);
            }
            VirtualIMEIManager manager = new VirtualIMEIManager();
            List<VirtualIMEI> imeiList = manager.SelectEntitys(appProjectID, DateTime.Now.Date, isNewAdd);
            // 统计各种记录条数
            int finishCount = 0; // 已完成
            int doingCount = 0; // 执行中
            foreach (VirtualIMEI imei in imeiList)
            {
                if (imei.TaskStatus == 2) finishCount++;
                if (imei.TaskStatus == 1) doingCount++;
            }
            this.lblSummary.Text = string.Format("已完成{0}条，执行中{1}条，待执行{2}条。", 
                finishCount, doingCount, imeiList.Count - finishCount - doingCount);
            this.GridTasks.GetStore().DataSource = imeiList;
            this.GridTasks.DataBind();
        }

        private void bindSingleTask(int imeiID)
        {
            VirtualIMEIManager manager = new VirtualIMEIManager();
            VirtualIMEI imeiTask = manager.SelectSingleEntity(imeiID);
            this.txtAppProjectsID.Text = imeiTask.AppProjectsID.ToString();
            this.txtModel.Text = imeiTask.Brand + " " + imeiTask.Device;
            this.txtStartDate.Text = imeiTask.StartDate.ToString("yyyy-MM-dd");
            if (imeiTask.EndDate.Year == 9999)
            {
                this.txtEndDate.Text = "新增任务待明天计算";
            }
            else
            {
                this.txtEndDate.Text = imeiTask.EndDate.ToString("yyyy-MM-dd");
            }
            if(imeiTask.TaskStatus == 0)
            {
                this.txtTaskStatus.Text = "待执行";
            }
            else if(imeiTask.TaskStatus == 1)
            {
                this.txtTaskStatus.Text = "执行中";
            }
            else if(imeiTask.TaskStatus == 2)
            {
                this.txtTaskStatus.Text = "已完成";
            }
            
            this.txtMobileDevicesID.Text = imeiTask.MobileDevicesID.ToString();
            DateTime doTime = imeiTask.UpdateTime ?? DateTime.MinValue;
            if(doTime == DateTime.MinValue)
            {
                this.txtUpdateTime.Text = string.Empty;
            }
            else
            {
                this.txtUpdateTime.Text = doTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            this.txtIMEI.Text = imeiTask.IMEI.ToString();
            this.txtIMSI.Text = imeiTask.IMSI.ToString();
            this.txtMAC.Text = imeiTask.MAC;
            this.txtLine1Number.Text = imeiTask.Line1Number.ToString();
            this.txtSimSerialNumber.Text = imeiTask.SimSerialNumber;
            this.txtTelecomOpertaionorsName.Text = imeiTask.TelecomOperatorsName;
            this.txtNetworkType.Text = ((NetworkType)imeiTask.NetworkType).ToString();
            this.txtPhoneNumberCity.Text = imeiTask.PhoneNumberCity;
            this.txtAndroidID.Text = imeiTask.AndroidID.ToString("x16");
            DateTime retainStartTime = imeiTask.RetainStartTime ?? DateTime.MinValue;
            if (retainStartTime == DateTime.MinValue)
            {
                this.txtRetainStartTime.Text = string.Empty;
            }
            else
            {
                this.txtRetainStartTime.Text = retainStartTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            this.txtResolution.Text = imeiTask.Width + " * " + imeiTask.Height;
            this.txtOSVersion.Text = imeiTask.OSVersion;
            this.subWindowDetail.Show();
                                               
        }

        protected void btnQuery_Click(object sender, DirectEventArgs e)
        {
            bindTaskGrid();
            this.initSelectBoxAppProject(this.selectBoxAppProject.SelectedItem.Index);
            this.selectBoxAppProject.Update();
        }

        #region 手工生成任务
        protected void btnBuildTodayNewAdd_Click(object sender, DirectEventArgs e)
        {
            this.Session["startTime"] = DateTime.Now;
            ThreadPool.QueueUserWorkItem(LongAction);
            this.ResourceManager1.AddScript("{0}.startTask('longaction');", this.TaskManager1.ClientID); 
        }


        private void LongAction(object state)
        {
            int appProjectID = int.Parse(this.selectBoxAppProject.SelectedItem.Value);
            TasksManager manager = new TasksManager();
            int buildCount = manager.BuildNewAddVirutalIMEIForSingleAppProject(appProjectID, DateTime.Now.Date);
            string logMessage = null;
            if (buildCount == 0)
            {
                logMessage = "今天生成的新增任务已达到此APP项目的新增上限，不能再生成了。";
            }
            else
            {
                logMessage = string.Format("新增任务生成完成，第{0}号APP项目共生成了{1}条。", appProjectID.ToString(), buildCount.ToString());
                this.AddSystemLog(logMessage);
                bindTaskGrid();
            }
            this.Session["logMessage"] = logMessage;
            this.Session.Remove("startTime");
        }

        protected void TaskManager1_Refresh(object sender, DirectEventArgs e)
        {
            if (this.Session["startTime"] == null)
            {
                this.ResourceManager1.AddScript("{0}.stopTask('longaction');", this.TaskManager1.ClientID);
                if (this.Session["logMessage"] != null)
                {
                    X.Js.Call("updateMask", this.Session["logMessage"].ToString());
                }
                
            }
            else
            {
                DateTime startTime = Convert.ToDateTime(this.Session["startTime"]);
                X.Js.Call("updateMask", string.Format("正在生成任务中，已进行了{0}秒......<br/>每分钟大约可以生成600条任务。<br/>注意：各APP会在每天凌晨自动生成每日任务。此功能仅用于今天刚刚新建，且需要马上执行的APP项目等特殊情况。<br/>任务完成后可进行任务查询以确认生成情况。", 
                    (DateTime.Now - startTime).Seconds));
                
            }
        }

        #endregion

        protected void btnResetFailedTasks_Click(object sender, DirectEventArgs e)
        {
            int appProjectID = int.Parse(this.selectBoxAppProject.SelectedItem.Value);
            VirtualIMEIManager manager = new VirtualIMEIManager();
            int buildCount = manager.ResetFailedTasks(appProjectID);
            string logMessage = string.Format("重置任务状态生成完成，第{0}号APP项目共重置了{1}条。", appProjectID.ToString(), buildCount.ToString());
            this.AddSystemLog(logMessage);
            bindTaskGrid();
            X.Msg.Info("任务完成", logMessage, UI.Success).Show();
        }

        protected void GridTasks_RowDblClick(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.GridTasks.GetSelectionModel() as RowSelectionModel;
            int imeiID = int.Parse(sm.SelectedRow.RecordID);
            this.bindSingleTask(imeiID);
        }

    }
}