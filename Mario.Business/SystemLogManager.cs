using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mario.DataAccess;

namespace Mario.Business
{
    public class SystemLogManager
    {
        public int AddSystemLog(SystemLog log)
        {
            MarioEntities me = new MarioEntities();
            me.SystemLog.Add(log);
            return me.SaveChanges();
        }

        /// <summary>
        /// 查询指定时间的系统日志(显示前1000条)
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public List<SystemLog> SelectSystemLogs(DateTime startTime, DateTime endTime)
        {
            MarioEntities me = new MarioEntities();
            var result = (from sl in me.SystemLog
                          where sl.LogTime >= startTime && sl.LogTime <= endTime
                          select sl
                         ).OrderByDescending(sl => sl.LogTime).Take(1000);
            List<SystemLog> list = result.ToList<SystemLog>();
            return list;
        }

        public SystemLog SelectSingleLog(int logID)
        {
            MarioEntities me = new MarioEntities();
            return me.SystemLog.Where(l => l.ID == logID).FirstOrDefault();
        }
    }
}
