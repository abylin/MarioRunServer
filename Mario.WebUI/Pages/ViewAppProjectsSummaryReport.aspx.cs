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
    public partial class ViewAppProjectsSummaryReport : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                this.bindGridData();
            }

        }

        private void bindGridData()
        {
            UserInfos currentUser = (UserInfos)Session["CurrentUser"];
            AppProjectsManager manager = new AppProjectsManager();
            List<AppProjectsReport> list = manager.SelectAppProjectsReport(false, currentUser.ID);
            this.GridPanelSummaryReport.GetStore().DataSource = list;

            this.GridPanelSummaryReport.DataBind();
        }

        protected void GridPanelSummaryReport_Refresh(object sender, StoreReadDataEventArgs e)
        {
            this.bindGridData();
        }
    }
}