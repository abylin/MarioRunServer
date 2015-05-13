using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class MobileDevicesManager
    {
        public int AddOrUpdate(MobileDevices device)
        {
            MarioEntities me = new MarioEntities();
            me.MobileDevices.Attach(device);
            if (device.ID == 0)
            {
                me.Entry<MobileDevices>(device).State = EntityState.Added;
            }
            else
            {
                me.Entry<MobileDevices>(device).State = EntityState.Modified;
            }
            me.SaveChanges();
            return device.ID;
        }

        public int Delete(int deviceID)
        {
            MarioEntities me = new MarioEntities();

            // 删除移动设备中的APP安装关系
            me.AppProjectsInMobileDevices.RemoveRange(me.AppProjectsInMobileDevices.Where(a => a.MobileDevicesID == deviceID));

            var result = me.MobileDevices.Where(m => m.ID == deviceID).FirstOrDefault();
            me.MobileDevices.Remove(result);
            return me.SaveChanges();
        }


        public MobileDevices SelectSingleMobileDevices(int deviceID)
        {
            MarioEntities me = new MarioEntities();
            MobileDevices device = me.MobileDevices.SingleOrDefault(m => m.ID == deviceID);
            return device;
        }

        public MobileDevices SelectSingleMobileDevices(long realIMEI)
        {
            if (realIMEI == 0)
            {
                return null;
            }
            MarioEntities me = new MarioEntities();
            MobileDevices device = me.MobileDevices.SingleOrDefault(m => m.RealIMEI == realIMEI);
            return device;
        }

        public List<MobileDevices> SelectAllMobileDevices()
        {
            MarioEntities me = new MarioEntities();
            var result = (from m in me.MobileDevices
                          select m);

            return result.ToList<MobileDevices>();
        }


        public int RemoveAppProjectsFromMobileDevice(int deviceID, int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsInMobileDevices.Where(m => m.MobileDevicesID == deviceID && m.AppProjectsID == appProjectID).FirstOrDefault();
            me.AppProjectsInMobileDevices.Remove(result);
            return me.SaveChanges();
        }

        public void UpdateLastResponseTime(int deviceID, DateTime responseTime)
        {
            MobileDevices device = this.SelectSingleMobileDevices(deviceID);
            device.LastResponseTime = responseTime;
            this.AddOrUpdate(device); ;
        }

        /// <summary>
        /// 设置移动设备在获取下次任务时，发送重启报文
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="needReoot"></param>
        public void SetReboot(int deviceID, bool needReoot)
        {
            MobileDevices device = this.SelectSingleMobileDevices(deviceID);
            device.NeedReboot = needReoot;
            this.AddOrUpdate(device); ;
        }
    }
}
