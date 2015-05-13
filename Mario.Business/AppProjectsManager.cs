using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class AppProjectsManager
    {
        #region AppProjects类操作

        public int AddOrUpdate(AppProjects project)
        {
            MarioEntities me = new MarioEntities();
            me.AppProjects.Attach(project);
            if (project.ID == 0)
            {
                me.Entry<AppProjects>(project).State = EntityState.Added;
            }
            else
            {
                me.Entry<AppProjects>(project).State = EntityState.Modified;
            }
            me.SaveChanges();
            return project.ID;
        }

        /// <summary>
        /// 删除项目以及相关的数据
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public int DeleteAppProjects(int projectID)
        {
            OperationSchemesManager osm = new OperationSchemesManager();
            osm.DeleteFromAppProjects(projectID); // 删除方案及报文

            MarioEntities me = new MarioEntities();
            me.AppProjectsInMobileDevices.RemoveRange(me.AppProjectsInMobileDevices.Where(a => a.AppProjectsID == projectID)); // 删除承载的应用
            me.AppProjectsRetention.RemoveRange(me.AppProjectsRetention.Where(r => r.AppProjectsID == projectID)); // 删除留存率
            me.VirtualIMEI.RemoveRange(me.VirtualIMEI.Where(v => v.AppProjectsID == projectID)); // 删除项目的新增及留存任务
            me.DayReport.RemoveRange(me.DayReport.Where(d => d.AppProjectsID == projectID)); // 删除日报表

            var result = me.AppProjects.Where(p => p.ID == projectID).FirstOrDefault(); // 删除项目
            me.AppProjects.Remove(result);

            return me.SaveChanges();
        }

        public AppProjects SelectSingleAppProject(int projectID)
        {
            MarioEntities me = new MarioEntities();
            AppProjects project = me.AppProjects.SingleOrDefault(p => p.ID == projectID);
            return project;
        }

        public List<AppProjects> SelectAllEntities(bool isDisplayPauseAppProject, int userID)
        {
            MarioEntities me = new MarioEntities();
            List<AppProjects> list = null;
            if (isDisplayPauseAppProject)
            {
                list = (from app in me.AppProjects
                         where (
                             (from apir in me.AppProjectsInRole
                              where
                                  ((from ur in me.UserInRole where ur.UserID == userID select ur.RoleID).Contains(apir.RoleID))
                              select apir.AppProjectsID).Contains(app.ID))
                         select app).OrderBy(p => p.ChineseName).ToList();
            }
            else
            {
                list = (from app in me.AppProjects
                        where (
                            (from apir in me.AppProjectsInRole
                             where
                                 ((from ur in me.UserInRole where ur.UserID == userID select ur.RoleID).Contains(apir.RoleID))
                             select apir.AppProjectsID).Contains(app.ID)) && app.Status == 1
                        select app).OrderBy(p => p.ChineseName).ToList();
            }

            return list;
        }

        public List<AppProjectsReport> SelectAppProjectsReport(bool isDisplayPauseAppProject, int userID)
        {
            List<AppProjects> appProjectsList = this.SelectAllEntities(isDisplayPauseAppProject, userID);
            List<AppProjectsReport> appProjectsReportList = new List<AppProjectsReport>();
            DayReportManager dayReportManager = new DayReportManager();
            DateTime yesterday = DateTime.Now.Date.AddDays(-1);
            foreach (AppProjects app in appProjectsList)
            {
                AppProjectsReport report = new AppProjectsReport();
                report.ID = app.ID;
                report.ChineseName = app.ChineseName;
                report.Status = app.Status;
                report.AddLimit = app.AddLimit;
                report.MobileDeviceCount = this.GetMobileDevicesCountFromAppProjects(app.ID);
                DayReport dayReport = dayReportManager.SelectSingleDayReport(yesterday, app.ID);
                if (dayReport == null)
                {
                    report.YesterdayNewAdd = 0;
                    report.YesterdayRetention = 0;
                }
                else
                {
                    report.YesterdayNewAdd = dayReport.AddCount;
                    report.YesterdayRetention = dayReport.Retention;
                }
                appProjectsReportList.Add(report);
            }

            return appProjectsReportList;
        }
        #endregion

        /// <summary>
        /// 获取指定日期（如今天）需要生成的APP项目列表
        /// </summary>
        /// <param name="buildDate"></param>
        /// <returns></returns>
        public List<AppProjects> GetAppProjectListToBuildIMEI(DateTime buildDate)
        {
            MarioEntities me = new MarioEntities();
            var result = from ap in me.AppProjects
                         where ap.Status == 1 && ap.StartDate <= buildDate
                         select ap;
            ;
            return result.ToList<AppProjects>();
        }

        #region AppProjectsInMobileDevices类操作

        public List<AppProjects> GetSelectedAppProjectsFromMobileDevices(int mobileDeviceID)
        {
            MarioEntities me = new MarioEntities();
            var result = from ap in me.AppProjects
                         where
                             ((from apmd in me.AppProjectsInMobileDevices 
                               where apmd.MobileDevicesID == mobileDeviceID 
                               select apmd.AppProjectsID).Contains(ap.ID))
                         select ap;
;
            return result.ToList<AppProjects>();
        }

        public List<AppProjects> GetAppProjectsFromMobileDevicesToTask(int mobileDeviceID)
        {
            MarioEntities me = new MarioEntities();
            var result = from ap in me.AppProjects
                         where
                             ((from apmd in me.AppProjectsInMobileDevices
                               where apmd.MobileDevicesID == mobileDeviceID
                               select apmd.AppProjectsID).Contains(ap.ID) && ap.Status == 1 && ap.StartDate <= DateTime.Now)
                         select ap;
            ;
            return result.ToList<AppProjects>();
        }

        public List<AppProjects> GetNotSelectAppProjectsFromMobileDevices(int mobileDeviceID)
        {
            MarioEntities me = new MarioEntities();
            var result = from ap in me.AppProjects
                         where
                             (!(from apmd in me.AppProjectsInMobileDevices
                               where apmd.MobileDevicesID == mobileDeviceID
                               select apmd.AppProjectsID).Contains(ap.ID))
                         select ap;
            ;
            return result.ToList<AppProjects>();
        }

        public void SetAppProjectsFromMobileDevices(int mobileDeviceID, List<AppProjects> appProjectsList)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsInMobileDevices.Where(apmd => apmd.MobileDevicesID == mobileDeviceID);
            me.AppProjectsInMobileDevices.RemoveRange(result);

            foreach (AppProjects app in appProjectsList)
            {
                AppProjectsInMobileDevices apimd = new AppProjectsInMobileDevices();
                apimd.MobileDevicesID = mobileDeviceID;
                apimd.AppProjectsID = app.ID;
                me.AppProjectsInMobileDevices.Add(apimd);
            }

            me.SaveChanges();
        }

        public List<AppProjectsInMobileDevices> GetMobileDevicesListFromAppProjects(int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            List<AppProjectsInMobileDevices> list = me.AppProjectsInMobileDevices.Where(apimd => apimd.AppProjectsID == appProjectID).ToList();
            return list;
        }

        public int GetMobileDevicesCountFromAppProjects(int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            return me.AppProjectsInMobileDevices.Where(apimd => apimd.AppProjectsID == appProjectID).Count();
        }

        public int AddOrUpdateAppProjectsInMobileDevices(AppProjectsInMobileDevices apimd)
        {
            MarioEntities me = new MarioEntities();
            me.AppProjectsInMobileDevices.Attach(apimd);
            if (apimd.ID == 0)
            {
                me.Entry<AppProjectsInMobileDevices>(apimd).State = EntityState.Added;
            }
            else
            {
                me.Entry<AppProjectsInMobileDevices>(apimd).State = EntityState.Modified;
            }
            me.SaveChanges();
            return apimd.ID;
        }

        public int DeleteAppProjectsInMobileDevices(int id)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsInMobileDevices.Where(p => p.ID == id).FirstOrDefault();
            me.AppProjectsInMobileDevices.Remove(result);
            return me.SaveChanges();
        }

        #endregion

        /// <summary>
        /// 复制一个原项目的信息到一个新的项目
        /// 新的项目会复制原项目的信息,并包括操作方案和报文,留存率设置等.名字为会在原中文名称加上'_副本'字样,且状态设置为已暂停.请在编辑后启用
        /// </summary>
        /// <param name="oldProjectID"></param>
        /// <returns>新的APP项目ID</returns>
        public int CopyAppProjects(int oldProjectID)
        {
            AppProjects oldAppProjects = this.SelectSingleAppProject(oldProjectID);
            AppProjects newAppProjects = new AppProjects();
            newAppProjects.ChineseName = oldAppProjects.ChineseName + "_副本";
            newAppProjects.AddLimit = oldAppProjects.AddLimit;
            newAppProjects.StartDate = DateTime.Now.Date;
            newAppProjects.Status = 2; // 已暂停
            newAppProjects.RetainDelayHour = oldAppProjects.RetainDelayHour;
            newAppProjects.Memo = oldAppProjects.Memo;
            int newAppProjectID = this.AddOrUpdate(newAppProjects);

            this.copyRetentionFromAppProjects(oldProjectID, newAppProjects.ID);
            this.copyBlackTimeFromAppProjects(oldProjectID, newAppProjects.ID);

            OperationSchemesManager osm = new OperationSchemesManager();
            List<OperationSchemes> schemesList = osm.SelectAllOperationSchemesFromAppProject(oldProjectID);
            foreach (OperationSchemes scheme in schemesList)
            {
                osm.CopyOperationSchemesToAppProject(scheme.ID, newAppProjects.ID);
            }
            return newAppProjectID;
        }

        #region 留存率

        public List<AppProjectsRetention> SelectRetentions(int projectID)
        {
            MarioEntities me = new MarioEntities();
            var result = (from r in me.AppProjectsRetention
                          where r.AppProjectsID == projectID
                          select r).OrderBy(r => r.Days);

            return result.ToList<AppProjectsRetention>();
        }

        public List<AppProjectsRetention> SelectRetentionsDescendingDay(int projectID)
        {
            MarioEntities me = new MarioEntities();
            var result = (from r in me.AppProjectsRetention
                          where r.AppProjectsID == projectID
                          select r).OrderByDescending(r => r.Days);

            return result.ToList<AppProjectsRetention>();
        }

        public AppProjectsRetention SelectSingleRetention(int retentionID)
        {
            MarioEntities me = new MarioEntities();
            AppProjectsRetention retention = me.AppProjectsRetention.SingleOrDefault(r => r.ID == retentionID);
            return retention;
        }

        /// <summary>
        /// 插入指定条数的留存率记录,默认值为0
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public void InsertRetentions(int projectID, int recordCount)
        {
            MarioEntities me = new MarioEntities();

            var result = me.AppProjectsRetention.Where(r => r.AppProjectsID == projectID);
            int maxDays = 0;
            if(result.Any())
            {
                maxDays = result.Max(r => r.Days);
            }
            
            for (int i = 0; i < recordCount; i++)
            {
                AppProjectsRetention rentention = new AppProjectsRetention();
                rentention.AppProjectsID = projectID;
                maxDays ++;
                rentention.Days = maxDays;
                rentention.Retention = 0;
                this.AddOrUpdateRentention(rentention);
            }
        }

        public int AddOrUpdateRentention(AppProjectsRetention rentention)
        {
            MarioEntities me = new MarioEntities();
            me.AppProjectsRetention.Attach(rentention);
            if (rentention.ID == 0)
            {
                me.Entry<AppProjectsRetention>(rentention).State = EntityState.Added;
            }
            else
            {
                me.Entry<AppProjectsRetention>(rentention).State = EntityState.Modified;
            }
            me.SaveChanges();
            return rentention.ID;
        }

        public int RemoveRententionFromLastDay(int appProjectID)
        {
            List<AppProjectsRetention> list = this.SelectRetentions(appProjectID);
            if (list.Count == 0)
            {
                return 0;
            }
            MarioEntities me = new MarioEntities();
            AppProjectsRetention rentention = list.Last(); // 移除最后一天的数据
            me.Entry<AppProjectsRetention>(rentention).State = EntityState.Deleted;
            return me.SaveChanges();
        }

        public int RemoveAllRententions(int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            me.AppProjectsRetention.RemoveRange(me.AppProjectsRetention.Where(a => a.AppProjectsID == appProjectID));
            return me.SaveChanges();
        }

        private void copyRetentionFromAppProjects(int oldAppProjectID, int newAppProjectID)
        {
            List<AppProjectsRetention> oldList = this.SelectRetentions(oldAppProjectID);
            foreach (AppProjectsRetention oldRetention in oldList)
            {
                AppProjectsRetention newRetention = new AppProjectsRetention();
                newRetention.AppProjectsID = newAppProjectID;
                newRetention.Days = oldRetention.Days;
                newRetention.Retention = oldRetention.Retention;
                this.AddOrUpdateRentention(newRetention);
            }
        }

        #endregion

        #region 不执行时间

        public List<AppProjectsBlackTime> SelectBlackTimes(int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            return me.AppProjectsBlackTime.Where(bt => bt.AppProjectsID == appProjectID).ToList();
        }


        public AppProjectsBlackTime SelectSingleBlackTime(int id)
        {
            MarioEntities me = new MarioEntities();
            return me.AppProjectsBlackTime.FirstOrDefault(r => r.ID == id);
        }

        public int AddOrUpdateBlackTime(AppProjectsBlackTime blackTime)
        {
            MarioEntities me = new MarioEntities();
            me.AppProjectsBlackTime.Attach(blackTime);
            if (blackTime.ID == 0)
            {
                me.Entry<AppProjectsBlackTime>(blackTime).State = EntityState.Added;
            }
            else
            {
                me.Entry<AppProjectsBlackTime>(blackTime).State = EntityState.Modified;
            }
            me.SaveChanges();
            return blackTime.ID;
        }

        public int DeleteBlackTime(int id)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsBlackTime.Where(p => p.ID == id).FirstOrDefault();
            me.AppProjectsBlackTime.Remove(result);
            return me.SaveChanges();
        }

        private void copyBlackTimeFromAppProjects(int oldAppProjectID, int newAppProjectID)
        {
            List<AppProjectsBlackTime> oldList = this.SelectBlackTimes(oldAppProjectID);
            foreach (AppProjectsBlackTime oldBlackTime in oldList)
            {
                AppProjectsBlackTime newBlackTime = new AppProjectsBlackTime();
                newBlackTime.AppProjectsID = newAppProjectID;
                newBlackTime.StartTime = oldBlackTime.StartTime;
                newBlackTime.EndTime = oldBlackTime.EndTime;
                this.AddOrUpdateBlackTime(newBlackTime);
            }
        }

        #endregion
    }
}
