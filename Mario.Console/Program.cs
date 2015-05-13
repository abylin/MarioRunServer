using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using log4net;
using log4net.Config;

using Mario.DataAccess;
using Mario.Business;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(Watch = true)] 
namespace Mario.Console
{ 
    class Program
    {
        /// <summary>
        /// 每天凌晨00：03执行一次
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime startTime = DateTime.Now;
            System.Console.WriteLine(DateTime.Now.ToString() +":程序启动");
            ILog logger = LogManager.GetLogger("Logger");
            XmlConfigurator.Configure();
            SystemLogManager slm = new SystemLogManager();

            try
            {
                VirtualIMEIManager imeiManager = new VirtualIMEIManager();
                logger.Info("删除结束日期在30天之前的数据，以减少数据库压力....");
                int deleteCount1 = imeiManager.DeleteVirutalIMEIBeforeDay(yesterday.AddDays(-30));

                logger.Info("现在删除昨天没有做的新增IMEI....");
                int deleteCount2 = imeiManager.DeleteNotFinishedVirutalIMEI(yesterday);

                logger.Info("现在生成昨天的日报表....");
                DayReportManager dayReportManager = new DayReportManager();
                dayReportManager.BuildDayReport(yesterday);

                logger.Info("重新计算各型号权重概率....");
                MobileDeviceModelsManager mobileDeviceModelsManager = new MobileDeviceModelsManager();
                mobileDeviceModelsManager.ComputeWeight();

                logger.Info("计算从昨天的新增用户中，从今天起各自留存的天数....");
                TasksManager taskManager = new TasksManager();
                taskManager.ComputeRetentionVirutalIMEI(today);

                logger.Info("恢复所有IMEI状态,使得留存的记录可以在今天做继续做...."); // 也包括昨天某一个移动设备死机了,执行中的IMEI
                imeiManager.ResetAllVirutalIMEI();
                
                logger.Info("生成今天的新增数据(不含留存计算)....");
                int insertCount = taskManager.BuildNewAddVirutalIMEI(today);

                logger.Info("删除Excel临时文件。");
                string excelFilePath = ConfigurationManager.AppSettings["ExcelTempFilePath"];
                string[] files = Directory.GetFiles(excelFilePath);
                foreach (string file in files) 
                {
                    File.Delete(file);
                }

                // 记录日志
                string message = string.Format("计划任务于{0}开始，并于{1}结束。VirtualIMEI表删除了昨天之前结束的{2}条历史记录，昨天没有做的新增IMEI{3}条。新增了{4}条记录",
                    startTime, DateTime.Now, deleteCount1, deleteCount2, insertCount);
                slm.AddSystemLog(new SystemLog()
                {
                    UserID = 0,
                    UserName = "Mario.Console",
                    LogTime = DateTime.Now,
                    Memo = message
                });
                logger.Info(message);
                System.Console.WriteLine(DateTime.Now.ToString() + ":程序全部完成。");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + ex.StackTrace);
                slm.AddSystemLog(new SystemLog()
                {
                    UserID = 0,
                    UserName = "Mario.Console",
                    LogTime = DateTime.Now,
                    Memo = ex.Message + ex.StackTrace
                });
            }
        }
    }
}
