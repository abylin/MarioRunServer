using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI.Pages
{
    public partial class ViewDayReport : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                this.DateFieldStart.SelectedDate = DateTime.Now.AddDays(-14).Date;
                this.DateFieldEnd.SelectedDate = DateTime.Now.AddDays(-1).Date;
                InitSelectBoxAppProject();
                if (this.selectBoxAppProject.Items.Count > 0)
                {
                    this.bindChartData(this.DateFieldStart.SelectedDate, this.DateFieldEnd.SelectedDate, int.Parse(this.selectBoxAppProject.Items[0].Value));
                }
            }
        }

        private void InitSelectBoxAppProject()
        {
            AppProjectsManager manager = new AppProjectsManager();
            UserInfos currentUser = this.GetCurrentUser();
            List<AppProjects> list = manager.SelectAllEntities(true, currentUser.ID);
            this.selectBoxAppProject.Items.Clear();
            foreach (AppProjects project in list)
            {
                this.selectBoxAppProject.Items.Add(new Ext.Net.ListItem(project.ChineseName, project.ID));
            }
            if (this.selectBoxAppProject.Items.Count > 0)
            {
                this.selectBoxAppProject.Select(0);
            }
        }


        private void bindChartData(DateTime startDate, DateTime endDate, int appProjectID)
        {
            DayReportManager manager = new DayReportManager();

            List<DayReport> descList = manager.SelectDayReportListOrderDesc(startDate, endDate, appProjectID);
            this.GridPanelDayReport.GetStore().DataSource = descList;
            this.GridPanelDayReport.DataBind();

            if (descList.Count > 4) // 有4条以上记录才显示图表
            {
                List<DayReport> ascList = manager.SelectDayReportList(startDate, endDate, appProjectID);
                this.ChartReport.GetStore().DataSource = ascList;
                this.ChartReport.DataBind();
                this.ChartReport.Hidden = false;
            }
            else
            {
                this.ChartReport.Hidden = true;
            }
        }

        protected void btnQuery_Click(object sender, DirectEventArgs e)
        {
            this.bindChartData(this.DateFieldStart.SelectedDate, this.DateFieldEnd.SelectedDate, int.Parse(this.selectBoxAppProject.SelectedItem.Value));
        }

        protected void btnToExcel_Click(object sender, EventArgs e)
        {
            DayReportManager manager = new DayReportManager();
            List<DayReport> list = manager.SelectDayReportList(this.DateFieldStart.SelectedDate, this.DateFieldEnd.SelectedDate, int.Parse(this.selectBoxAppProject.SelectedItem.Value));

            if (list.Count == 0)
            {
                X.Msg.Alert("系统提示", "没有需要导出的数据，请选择查询条件。").Show();
                return;
            }

            HttpResponse response = Page.Response;
            response.ContentEncoding = Encoding.GetEncoding("GB2312");
            string fileName = DateTime.Now.ToString("Report_yyyy-MM-dd_HHmmss") + ".xls";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string columnHeaders = "编号\t统计日期\t新增数\t留存数\n";

            // 向HTTP输出流中写入取得的数据信息
            response.Write(columnHeaders);

            // 逐行处理数据  
            foreach (DayReport report in list)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(report.ID.ToString());
                sb.Append("\t");
                sb.Append(report.CollectDate.ToLongDateString());
                sb.Append("\t");
                sb.Append(report.AddCount);
                sb.Append("\t");
                sb.Append(report.Retention);
                sb.Append("\n");

                response.Write(sb.ToString());

            }
            response.End();
        }
    }
}