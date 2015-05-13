﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI.Pages
{
    public partial class ViewSystemLogs : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                this.DateField1.SelectedDate = DateTime.Now.AddDays(-14);
                this.DateField2.SelectedDate = DateTime.Now;
                this.bindGridData();
            }
        }

        protected void GridPanelLog_Refresh(object sender, StoreReadDataEventArgs e)
        {
            this.bindGridData();
        }

        private void bindGridData()
        {
            DateTime startTime = this.DateField1.SelectedDate.Date;
            DateTime endTime = this.DateField2.SelectedDate.Date.AddDays(1).AddSeconds(-1);

            SystemLogManager manager = new SystemLogManager();
            List<SystemLog> logs = manager.SelectSystemLogs(startTime, endTime);
            Store store = this.GridPanelLog.GetStore();
            store.DataSource = logs;
            store.DataBind();
        }

        protected void btnSearch_Click(object sender, DirectEventArgs e)
        {
            this.bindGridData();
        }

        protected void btnToExcel_Click(object sender, EventArgs e)
        {
            DateTime startTime = this.DateField1.SelectedDate.Date;
            DateTime endTime = this.DateField2.SelectedDate.Date.AddDays(1).AddSeconds(-1);

            SystemLogManager manager = new SystemLogManager();
            List<SystemLog> logs = manager.SelectSystemLogs(startTime, endTime);
            if (logs.Count == 0)
            {
                X.Msg.Alert("系统提示", "没有需要导出的数据，请选择查询条件。").Show();
                return;
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            Page.EnableViewState = false;
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");//设置输出流为简体中文 
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".xls";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string columnHeaders = "日志号\t记录时间\t操作人\t备注\n";

            // 向HTTP输出流中写入取得的数据信息
            HttpContext.Current.Response.Write(columnHeaders);

            // 逐行处理数据  
            foreach (SystemLog log in logs)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(log.ID.ToString());
                sb.Append("\t");
                sb.Append(log.LogTime.ToString());
                sb.Append("\t");
                sb.Append(log.UserName);
                sb.Append("\t");
                sb.Append(log.Memo);
                sb.Append("\n");

                HttpContext.Current.Response.Write(sb.ToString());

            }
            HttpContext.Current.Response.End();
        }

        protected void GridPanelLog_RowDblClick(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.GridPanelLog.GetSelectionModel() as RowSelectionModel;
            int logID = int.Parse(sm.SelectedRow.RecordID);
            this.bindSingleLog(logID);
        }

        private void bindSingleLog(int logID)
        {
            SystemLogManager manager = new SystemLogManager();
            SystemLog log = manager.SelectSingleLog(logID);
            this.txtLogID.Text = log.ID.ToString();
            this.txtUserName.Text = log.UserName;
            this.txtLogTime.Text = log.LogTime.ToString("yyyy-MM-dd HH:mm:ss");
            this.txtMemo.Text = log.Memo;
            this.subWindowDetail.Show();
        }

    }
}