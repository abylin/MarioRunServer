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
    public class TasksManager
    {
        /// <summary>
        /// 给移动设备分配刷机任务
        /// </summary>
        /// <param name="mobileDeviceID"></param>
        /// <returns></returns>
        public ResponseMessage GetMobileDeviceTask(MobileDevices mobileDevice, bool isCommitDatabase)
        {
            MobileDevicesManager mobileDevicesManager = new MobileDevicesManager();
            if (mobileDevice.InUse == false)
            {
                return new ResponseMessage { resultCode = 10, resultInfo = "此移动设备被禁用" };
            }

            AppProjectsManager appProjectsManager = new AppProjectsManager();
            List<AppProjects> appProjectsList = appProjectsManager.GetAppProjectsFromMobileDevicesToTask(mobileDevice.ID);
            if (appProjectsList.Count == 0)
            {
                return new ResponseMessage { resultCode = 11, resultInfo = "移动设备上没有安装可刷机的APP应用" };
            }

            VirtualIMEIManager imeiManager = new VirtualIMEIManager();
            VirtualIMEI imei = null;
            AppProjects appProject = null;
            bool isNewAdd = false;
            TimeSpan nowTimeSpan = DateTime.Now.TimeOfDay;
            foreach (AppProjects app in appProjectsList)
            {
                List<AppProjectsBlackTime> blackTimeLists = appProjectsManager.SelectBlackTimes(app.ID);
                bool needSkipThisApp = false;
                foreach (AppProjectsBlackTime blackTime in blackTimeLists)
                {
                    if (blackTime.StartTime <= nowTimeSpan && nowTimeSpan < blackTime.EndTime) // 这个APP当前处在不执行时间
                    {
                        if (appProjectsList.Count == 1)
                        {
                            TimeSpan restSpan = blackTime.EndTime - nowTimeSpan;
                            return new ResponseMessage
                            {
                                resultCode = 21,
                                resultInfo = string.Format("移动设备还需要休眠：{0}:{1}:{2}",
                                    restSpan.Hours, restSpan.Minutes, restSpan.Seconds)
                            };
                        }
                        else
                        {
                            appProjectsList.Remove(app); //这个APP不做安排
                            needSkipThisApp = true;
                            break;
                        }
                    }
                }
                if (needSkipThisApp)
                {
                    continue;
                }
                // 从这个APP应用对应的留存记录中取一条执行
                imei = imeiManager.DrawOutRetentionRecord(app, DateTime.Now.Date, isCommitDatabase, mobileDevice.ID);
                appProject = app;
                if (imei != null)
                {
                    break;
                }

            }
            if (imei == null) // 如果都没有留存操作了，就进行新增
            {
                foreach (AppProjects app in appProjectsList)
                {

                    // 从这个APP应用对应的新增记录中取一条执行
                    imei = imeiManager.DrawOutNewAddRecord(app, DateTime.Now.Date, isCommitDatabase, mobileDevice.ID);
                    appProject = app;
                    if (imei != null)
                    {
                        isNewAdd = true;
                        break;
                    }
                }
            }
            if (imei == null)
            {
                return new ResponseMessage { resultCode = 12, resultInfo = "当前没有需要移动设备做的任务安排APP应用" };
            }
            else
            {
                return this.BuildJsonMessage(imei, appProject, mobileDevice.ID, isNewAdd);
            }
        }

        /// <summary>
        /// 在指定的日期生成虚拟的新增IMEI，包括启动时间，但不包含结束时间
        /// </summary>
        /// <param name="buildDate"></param>
        public int BuildNewAddVirutalIMEI(DateTime buildDate)
        {
            AppProjectsManager appProjectsManager = new AppProjectsManager();
            List<AppProjects> appProjectsList = appProjectsManager.GetAppProjectListToBuildIMEI(buildDate);

            List<VirtualIMEI> imeiList = new List<VirtualIMEI>();
            foreach (AppProjects appProject in appProjectsList)
            {
                imeiList.AddRange(this.buildNewAddIMEIList(appProject, buildDate));
            }
            VirtualIMEIManager virtualIMEIManager = new VirtualIMEIManager();
            return virtualIMEIManager.RamdomInsert(imeiList);
        }

        /// <summary>
        /// 在已有新增基础上，计算留存时间
        /// </summary>
        /// <param name="computeDate">要计算的日期</param>
        /// <returns></returns>
        public void ComputeRetentionVirutalIMEI(DateTime computeDate)
        {
            AppProjectsManager appProjectsManager = new AppProjectsManager();
            List<AppProjects> appProjectsList = appProjectsManager.GetAppProjectListToBuildIMEI(computeDate.AddDays(-1));


            foreach (AppProjects appProject in appProjectsList)
            {
                this.computeRetentionIMEIList(appProject, computeDate);
            }

        }

        public int BuildNewAddVirutalIMEIForSingleAppProject(int appProjectID, DateTime buildDate)
        {
            AppProjectsManager appProjectsManager = new AppProjectsManager();
            AppProjects appProject = appProjectsManager.SelectSingleAppProject(appProjectID);
            List<VirtualIMEI> imeiList = this.buildNewAddIMEIList(appProject, buildDate);
            int imeiListCount = imeiList.Count;
            VirtualIMEIManager virtualIMEIManager = new VirtualIMEIManager();
            virtualIMEIManager.RamdomInsert(imeiList);
            return imeiListCount;
        }

        /// <summary>
        /// 生成新增任务（不包含留存计算，即结束日期未设置）
        /// </summary>
        /// <param name="appProject"></param>
        /// <param name="buildDate"></param>
        /// <returns></returns>
        private List<VirtualIMEI> buildNewAddIMEIList(AppProjects appProject, DateTime buildDate)
        {
            VirtualIMEIManager virtualIMEIManager = new VirtualIMEIManager();

            // 今天需要生成的记录数 = APP项目的每日新增上限数 - 当前已生成的新增任务数
            int todayNeedNewAddCount = appProject.AddLimit - virtualIMEIManager.GetTodayNewAddCount(appProject.ID, buildDate);
            List<VirtualIMEI> imeiList = new List<VirtualIMEI>();
            for (int i = 0; i < todayNeedNewAddCount; i++)
            {
                VirtualIMEI imei = this.BuildSingleVirtualIMEIWithoutAppProject(buildDate);
                imei.AppProjectsID = appProject.ID;
                imei.StartDate = buildDate;  
                imei.EndDate = DateTime.MaxValue;
                imeiList.Add(imei);
            }
            return imeiList;
        }


        private int computeRetentionIMEIList(AppProjects appProject, DateTime computeDate)
        {
            VirtualIMEIManager virtualIMEIManager = new VirtualIMEIManager();
            AppProjectsManager appProjectsManager = new AppProjectsManager();

            // 提取出昨天的新增，用来计算留存
            List<VirtualIMEI> imeiList = virtualIMEIManager.SelectEntitys(appProject.ID, computeDate.AddDays(-1), true);

            List<AppProjectsRetention> retentionList = appProjectsManager.SelectRetentionsDescendingDay(appProject.ID);
            int lastRetention = 0;
            int imeiIndex = 0;
            foreach (AppProjectsRetention retention in retentionList)
            {
                // 根据留存率和昨天的新增任务数算出分配到某一天要结束的任务数
                int todayNewAddCount = (retention.Retention - lastRetention) * imeiList.Count / 100;
                lastRetention = retention.Retention;
                for (int i = 0; i < todayNewAddCount; i++)
                {
                    imeiList[imeiIndex].EndDate = computeDate.AddDays(retention.Days - 1);
                    imeiIndex++;
                }
            }

            // 非留存的第一天新增数据
            while (imeiIndex < imeiList.Count)
            {
                imeiList[imeiIndex].EndDate = computeDate.AddDays(-1);
                imeiIndex++;
            }

            return virtualIMEIManager.UpdateList(imeiList);
        }


        public VirtualIMEI BuildSingleVirtualIMEIWithoutAppProject(DateTime buildDate)
        {
            MobileDeviceModelsManager modelManager = new MobileDeviceModelsManager();
            VirtualIMEI imei = new VirtualIMEI();
            //imei.AppProjectsID = appProject.ID; 不需要包含App项目信息
            MobileDeviceModels model = modelManager.GetRandomMobileDeviceModels();
            VirtualIMEIManager virtualIMEIManager = new VirtualIMEIManager();
            imei.IMEI = virtualIMEIManager.CreateIMEICode(model.Brand);
            imei.MAC = virtualIMEIManager.CreateMAC();

            PhoneNumberManager phoneNumberManager = new PhoneNumberManager();
            imei.Line1Number = phoneNumberManager.CreatePhoneNumber();
            imei.IMSI = phoneNumberManager.CreateIMSICode();
            imei.SimSerialNumber = phoneNumberManager.CreateSimCardNumber();
            imei.TelecomOperatorsName = phoneNumberManager.GetTelecomOperatorsName();
            imei.NetworkType = phoneNumberManager.CreateNetworkType();
            imei.PhoneNumberCity = phoneNumberManager.GetPhoneNumberCity();
            imei.Brand = model.Brand;
            imei.Device = model.Device;
            imei.Width = model.Width;
            imei.Height = model.Height;
            imei.OSVersion = model.OSVersion;
            imei.AndroidID = virtualIMEIManager.CreateAndroidID();
            imei.StartDate = buildDate;
            //imei.EndDate = buildDate.AddDays(retention.Days); 不需要包含App项目信息
            imei.TaskStatus = 0;
            imei.MobileDevicesID = 0;

            return imei;
        }

        /// <summary>
        /// 生成JSON报文
        /// </summary>
        /// <param name="imei"></param>
        /// <param name="appProject">如果为空，则不</param>
        /// <returns></returns>
        public ResponseMessage BuildJsonMessage(VirtualIMEI imei, AppProjects appProject, int mobileDeviceID, bool isNewAdd)
        {
            
            ResponseMessage message = new ResponseMessage();
            if (appProject != null)
            {
                ResponseScript[] scripts = this.getAppProjectScript(appProject.ID, isNewAdd);
                message.script = scripts;
            }
            message.resultCode = 0;
            message.resultInfo = string.Empty;
            message.tId = imei.ID;
            message.brand = imei.Brand;
            message.model = imei.Brand + " " + imei.Device;
            message.device = imei.Device;
            message.imei = imei.IMEI.ToString();
            message.imsi = imei.IMSI.ToString();
            message.width = imei.Width;
            message.height = imei.Height;
            message.release = imei.OSVersion;
            message.mac = imei.MAC;
            message.line1Number = imei.Line1Number.ToString();
            message.simSerialNumber = imei.SimSerialNumber.ToString();
            message.networkOperatorName = imei.TelecomOperatorsName;
            message.networkType = imei.NetworkType.ToString();
            message.androidId = imei.AndroidID.ToString("x16");
            MarioPackagesManager manager = new MarioPackagesManager();
            MarioPackages lastestMario = manager.GetLastestMarioPackages();
            message.version = lastestMario.Version;
            message.marioUrl = lastestMario.DownloadUrl;
            message.field = mobileDeviceID.ToString(); // 原先预留放第几天的留存，现在不用了。改成放移动设备编号。  
            message.resultCode = 0; // 成功
            message.resultInfo = null;
            return message;
        }

        /// <summary>
        /// 生成JSON报文的脚本操作
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        private ResponseScript[] getAppProjectScript(int projectID, bool isNewAdd)
        {
            OperationSchemesManager osm = new OperationSchemesManager();
            List<OperationSchemes> schemeList = osm.SelectOperationSchemesFromAppProject(projectID, isNewAdd);
            if (schemeList.Count == 0)
            {
                return null;
            }
            OperationSchemes scheme = null;
            if (schemeList.Count == 1)
            {
                scheme = schemeList[0];
            }
            else
            {
                Random random = new Random(DateTime.Now.Millisecond);
                scheme = schemeList[random.Next(schemeList.Count)]; // 随机取一个方案
            }
            OperationMessagesManager omm = new OperationMessagesManager();
            List<OperationMessages> messagesList = omm.SelectOperationMessagesFromOperationSchemes(scheme.ID);
            List<ResponseScript> scriptList = new List<ResponseScript>();
            foreach (OperationMessages message in messagesList)
            {
                ResponseScript script = new ResponseScript();
                script.x1 = message.XPoint;
                script.y1 = message.YPoint;
                script.x2 = message.ToXPoint;
                script.y2 = message.ToYPoint;
                script.key = message.PhysicalKey;
                script.interval = message.Interval;
                script.action = message.Action;
                script.cmd = message.CommandScript;
                scriptList.Add(script);
            }

            return scriptList.ToArray();
        }
    }
}
