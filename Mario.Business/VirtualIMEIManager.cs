using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;
using Newtonsoft.Json;

namespace Mario.Business
{
    public class VirtualIMEIManager
    {
        #region 生成IMEI、MAC、AndroidID算法
        /// <summary>
        /// 生成IMEI校验码
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private char getLuhnDigit(string x)
        {
            int sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                int n = Convert.ToInt32(x.Length - 1 - i);
                if (i % 2 == 0)
                {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }
                sum += n;
            }
            return ((sum * 9) % 10).ToString().ToArray()[0];
        }

        /// <summary>
        /// 生成一个随机的IMEI
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public long CreateIMEICode(string brand)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            string[] rbi = null;
            if (brand == "Samsung")
            {
                rbi = new string[] { "35" };

            }
            else
            {
                rbi = new string[] { "86" };
            }
            string imei = rbi[random.Next(rbi.Length)];
            while (imei.Length < 14)
                imei += random.Next(10);
            imei += getLuhnDigit(imei);
            return long.Parse(imei);
        }


        public string CreateMAC()
        {
            // MAC地址为12个16进制数，中间用：分隔
            Random random = new Random(DateTime.Now.Millisecond);
            char[] macArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            char[] lanArray = new char[] { '0', '2', '4', '6', '8', 'A', 'C', 'E' };  // 网卡是单播地址，第二位必须为偶数
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i <= 12; i++)
            {
                if (i == 2)
                {
                    sb.Append(lanArray[random.Next(lanArray.Length)]);
                }
                else
                {
                    sb.Append(macArray[random.Next(macArray.Length)]);
                }
                if (i % 2 == 0 && i != 12)
                {
                    sb.Append(":");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成一个AndroidID
        /// </summary>
        /// <returns></returns>
        public long CreateAndroidID()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            long result = (long)random.Next() << 32;
            result += random.Next();
            return result;
        }

        #endregion

        public int AddOrUpdate(VirtualIMEI imei)
        {
            MarioEntities me = new MarioEntities();
            me.VirtualIMEI.Attach(imei);
            if (imei.ID == 0)
            {
                me.Entry<VirtualIMEI>(imei).State = EntityState.Added;
            }
            else
            {
                me.Entry<VirtualIMEI>(imei).State = EntityState.Modified;
            }
            me.SaveChanges();
            return imei.ID;
        }

        public int UpdateList(List<VirtualIMEI> imeiList)
        {
            MarioEntities me = new MarioEntities();
            foreach (VirtualIMEI imei in imeiList)
            {
                me.Entry<VirtualIMEI>(imei).State = EntityState.Modified;
            }
            return me.SaveChanges();
        }

        /// <summary>
        /// // 将imeiList的值随机顺序插入数据库  
        /// </summary>
        /// <param name="imeiList"></param>
        public int RamdomInsert(List<VirtualIMEI> imeiList)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            using(MarioEntities me = new MarioEntities())
            {
                me.Configuration.AutoDetectChangesEnabled = false;
                me.Configuration.ValidateOnSaveEnabled = false;
                while (imeiList.Count > 0)
                {
                    VirtualIMEI imei = imeiList[random.Next(imeiList.Count)];
                    me.VirtualIMEI.Add(imei);
                    imeiList.Remove(imei);
                }
                int result = me.SaveChanges();
                me.Configuration.AutoDetectChangesEnabled = true;
                me.Configuration.ValidateOnSaveEnabled = true;
                return result;
            }
        }


        /// 删除指定日期里没有完成的新增的IMEI
        /// </summary>
        /// <param name="buildDate">指定日期（通常是昨天）</param>
        /// <returns>删除的记录数</returns>
        public int DeleteNotFinishedVirutalIMEI(DateTime notFinishDate)
        {
            // VirutalIMEI删除条件(需同时满足)：1、VirutalIMEI.TaskStatus不为成功完成; 2、StartDate等于指定日期
            MarioEntities me = new MarioEntities();
            var result = me.VirtualIMEI.Where( vi => vi.TaskStatus != 2 && vi.StartDate == notFinishDate);
            me.VirtualIMEI.RemoveRange(result);
            return me.SaveChanges();
        }

        /// <summary>
        /// 删除结束日期在指定日期之前的记录，以减少数据库压力
        /// </summary>
        /// <param name="beforeEndDay"></param>
        /// <returns></returns>
        public int DeleteVirutalIMEIBeforeDay(DateTime beforeEndDay)
        {
            // VirutalIMEI删除条件(需同时满足)：1、VirutalIMEI.TaskStatus不为成功完成; 2、StartDate等于指定日期
            MarioEntities me = new MarioEntities();
            var result = me.VirtualIMEI.Where(vi => vi.EndDate < beforeEndDay);
            me.VirtualIMEI.RemoveRange(result);
            return me.SaveChanges();
        }

        /// <summary>
        /// 获取指定应用指定日期的新增数（给日报表用）
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <param name="reportDate"></param>
        /// <returns></returns>
        public int GetNewAddCountFromAppProjectForReport(int appProjectID, DateTime reportDate)
        {
            MarioEntities me = new MarioEntities();
            int count = me.VirtualIMEI.Count(vi => vi.AppProjectsID == appProjectID && vi.StartDate == reportDate && vi.TaskStatus == 2);
            return count;
        }

        /// <summary>
        /// 获取今天指定项目的新增任务数(不管是否执行)
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <returns></returns>
        public int GetTodayNewAddCount(int appProjectID, DateTime today)
        {
            MarioEntities me = new MarioEntities();
            int count = me.VirtualIMEI.Count(vi => vi.AppProjectsID == appProjectID && vi.StartDate == today);
            return count;
        }

        /// <summary>
        /// 获取指定应用指定日期的留存数（给日报表用）
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetRetentionCountFromAppProject(int appProjectID, DateTime reportDate)
        {
            MarioEntities me = new MarioEntities();
            int count = me.VirtualIMEI.Count(vi => vi.AppProjectsID == appProjectID && vi.StartDate < reportDate && vi.EndDate >= reportDate && vi.TaskStatus == 2); 
            return count;
        }

        /// <summary>
        /// 恢复所有IMEI状态,使得留存的记录可以在今天做继续做
        /// </summary>
        /// <param name="notFinishDate"></param>
        /// <returns></returns>
        public int ResetAllVirutalIMEI()
        {
            // VirutalIMEI删除条件(需同时满足)：1、VirutalIMEI.TaskStatus不为成功完成; 2、StartDate等于指定日期
            MarioEntities me = new MarioEntities();
            List<VirtualIMEI> list = me.VirtualIMEI.ToList();
            foreach (VirtualIMEI imei in list)
            {
                imei.TaskStatus = 0;
                imei.UpdateTime = null;
                imei.MobileDevicesID = 0;
            }
            return me.SaveChanges();
        }

        /// <summary>
        /// 本操作将会将指定APP项目的“执行中”状态的任务全部设置为“待执行”
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <returns></returns>
        public int ResetFailedTasks(int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            List<VirtualIMEI> list = me.VirtualIMEI.Where(v=>v.AppProjectsID == appProjectID && v.TaskStatus == 1).ToList();
            foreach (VirtualIMEI imei in list)
            {
                imei.TaskStatus = 0;
                imei.UpdateTime = null;
                imei.MobileDevicesID = 0;
            }
            return me.SaveChanges();
        } 

        /// <summary>
        /// 当移动设备发起请求时，抽取一条留存记录给它。并在数据库中做标记
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public VirtualIMEI DrawOutRetentionRecord(AppProjects appProject, DateTime today, bool isCommitDatabase, int mobileDeviceID)
        {
            MarioEntities me = new MarioEntities();
            VirtualIMEI imei = null;
            if (appProject.RetainDelayHour == 0)
            {
                imei = me.VirtualIMEI.Where(vi => vi.AppProjectsID == appProject.ID && vi.StartDate < today && vi.EndDate >= today && vi.TaskStatus == 0).FirstOrDefault();
            }
            else // 项目中有设置留存启动时间
            {
                imei = me.VirtualIMEI.Where(vi => vi.AppProjectsID == appProject.ID && vi.StartDate < today && vi.EndDate >= today && vi.TaskStatus == 0
                    &&(vi.RetainStartTime == null || vi.RetainStartTime < DateTime.Now) ).FirstOrDefault(); 
            }
            if (isCommitDatabase && imei != null)
            {
                imei.TaskStatus = 1;
                imei.UpdateTime = DateTime.Now;
                imei.MobileDevicesID = mobileDeviceID;
                this.AddOrUpdate(imei);
            }
            return imei;
        }

        /// <summary>
        /// 当移动设备发起请求时，抽取一条新增记录给它。并在数据库中做标记
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public VirtualIMEI DrawOutNewAddRecord(AppProjects appProject, DateTime today, bool isCommitDatabase, int mobileDeviceID)
        {
            MarioEntities me = new MarioEntities();
            VirtualIMEI imei = me.VirtualIMEI.Where(vi => vi.AppProjectsID == appProject.ID && vi.StartDate == today && vi.TaskStatus == 0).FirstOrDefault();
            if (isCommitDatabase && imei != null)
            {
                imei.TaskStatus = 1;
                imei.MobileDevicesID = mobileDeviceID;
                imei.UpdateTime = DateTime.Now;
                if (appProject.RetainDelayHour != 0)
                {
                    imei.RetainStartTime = DateTime.Now.AddHours(appProject.RetainDelayHour);
                }
                this.AddOrUpdate(imei);
            }
            return imei;
        }

        /// <summary>
        /// 查询某一天指定App的任务
        /// </summary>
        /// <param name="appProjectID"></param>
        /// <param name="selectedDay"></param>
        /// <param name="isNewAdd">True为选择新增任务，False选择留存任务</param>
        /// <returns></returns>
        public List<VirtualIMEI> SelectEntitys(int appProjectID, DateTime selectedDay, bool isNewAdd)
        {
            MarioEntities me = new MarioEntities();
            List<VirtualIMEI> imeiList = null;
            if (isNewAdd)
            {
                imeiList = me.VirtualIMEI.Where(vi => vi.AppProjectsID == appProjectID && vi.StartDate == selectedDay).OrderByDescending(vi => vi.TaskStatus).ToList();
            }
            else
            {
                imeiList = me.VirtualIMEI.Where(vi => vi.AppProjectsID == appProjectID && vi.StartDate < selectedDay && vi.EndDate >= selectedDay).ToList();
            }
            return imeiList;
        }

        public VirtualIMEI SelectSingleEntity(int imeiID)
        {
            MarioEntities me = new MarioEntities();
            return me.VirtualIMEI.Where(vi => vi.ID == imeiID).FirstOrDefault();
        }

        public string SetIMEIStatus(int imeiID, bool isSuccess, int mobileDeviceID)
        {
            MarioEntities me = new MarioEntities();
            VirtualIMEI imei = me.VirtualIMEI.SingleOrDefault(vi => vi.ID == imeiID);
            if(imei == null)
            {
                return JsonConvert.SerializeObject(new ResponseMessage { resultCode = 103, resultInfo = "找不到此tId(imeiID)" });
            }
            else
            {
                if (isSuccess)
                {
                    imei.TaskStatus = 2;
                }
                else
                {
                    imei.TaskStatus = 0;
                }
                imei.UpdateTime = DateTime.Now;
                imei.MobileDevicesID = mobileDeviceID;
                this.AddOrUpdate(imei);
                return JsonConvert.SerializeObject(new ResponseMessage { resultCode = 0, resultInfo = string.Empty });
            }
        }

    }
}
