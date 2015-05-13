using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class DayReportManager
    {
        public int BuildDayReport(DateTime buildDate)
        {
            AppProjectsManager appProjectsManager = new AppProjectsManager();
            VirtualIMEIManager virtualIMEIManager = new VirtualIMEIManager();

            List<AppProjects> appProjectsList = appProjectsManager.GetAppProjectListToBuildIMEI(buildDate);

            int recordCount = 0;
            foreach (AppProjects appProject in appProjectsList)
            {
                DayReport dayReport = new DayReport();
                dayReport.AppProjectsID = appProject.ID;
                dayReport.CollectDate = buildDate;
                dayReport.AddCount = virtualIMEIManager.GetNewAddCountFromAppProjectForReport(appProject.ID, buildDate);
                dayReport.Retention = virtualIMEIManager.GetRetentionCountFromAppProject(appProject.ID, buildDate);
                this.AddOrUpdate(dayReport);
                
            }
            return recordCount;
        }

        public int AddOrUpdate(DayReport report)
        {
            MarioEntities me = new MarioEntities();
            me.DayReport.Attach(report);
            if (report.ID == 0)
            {
                me.Entry<DayReport>(report).State = EntityState.Added;
            }
            else
            {
                me.Entry<DayReport>(report).State = EntityState.Modified;
            }
            me.SaveChanges();
            return report.ID;
        }

        public List<DayReport> SelectDayReportList(DateTime startDate, DateTime endDate, int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.DayReport.Where(d => d.AppProjectsID == appProjectID && d.CollectDate >= startDate && d.CollectDate <= endDate).OrderBy(d => d.CollectDate);
            return result.ToList<DayReport>();
        }

        public DayReport SelectSingleDayReport(DateTime selectDate, int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            return me.DayReport.Where(d => d.AppProjectsID == appProjectID && d.CollectDate == selectDate).SingleOrDefault();
        }

        public List<DayReport> SelectDayReportListOrderDesc(DateTime startDate, DateTime endDate, int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.DayReport.Where(d => d.AppProjectsID == appProjectID && d.CollectDate >= startDate && d.CollectDate <= endDate).OrderByDescending(d => d.CollectDate);
            return result.ToList<DayReport>();
        }
    }
}
