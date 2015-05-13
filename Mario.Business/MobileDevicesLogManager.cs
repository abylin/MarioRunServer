using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mario.DataAccess;

namespace Mario.Business
{
    public class MobileDevicesLogManager
    {
        public int AddLog(MobileDevicesLog log)
        {
            MarioEntities me = new MarioEntities();
            me.MobileDevicesLog.Add(log);
            return me.SaveChanges();
        }

        /// <summary>
        /// 查询指定时间的日志(显示前1000条)
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public List<MobileDevicesLog> SelectLogs(DateTime startTime, DateTime endTime)
        {
            MarioEntities me = new MarioEntities();
            var result = (from logs in me.MobileDevicesLog
                          where logs.LogTime >= startTime && logs.LogTime <= endTime
                          select logs
                         ).OrderByDescending(logs => logs.LogTime).Take(1000);
            List<MobileDevicesLog> list = result.ToList<MobileDevicesLog>();
            return list;
        }

        public MobileDevicesLog SelectSingleLog(int logID)
        {
            MarioEntities me = new MarioEntities();
            return me.MobileDevicesLog.Where(l => l.ID == logID).FirstOrDefault();
        }
    }
}
