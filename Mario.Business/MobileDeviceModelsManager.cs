using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class MobileDeviceModelsManager
    {

        public int AddOrUpdate(MobileDeviceModels model)
        {
            MarioEntities me = new MarioEntities();
            int count = me.MobileDeviceModels.Count(m => m.Brand == model.Brand && m.Device == model.Device && m.ID != model.ID);
            if (count > 0)
            {
                return 0;
            }
            me.MobileDeviceModels.Attach(model);
            if (model.ID == 0)
            {
                me.Entry<MobileDeviceModels>(model).State = EntityState.Added;
            }
            else
            {
                me.Entry<MobileDeviceModels>(model).State = EntityState.Modified;
            }
            me.SaveChanges();
            return model.ID;
        }

        public int Delete(int mobileDeviceModelsID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.MobileDeviceModels.Where(m => m.ID == mobileDeviceModelsID).FirstOrDefault();
            me.MobileDeviceModels.Remove(result);
            return me.SaveChanges();
        }

        public MobileDeviceModels SelectSingleEntity(int mobileDeviceModelsID)
        {
            MarioEntities me = new MarioEntities();
            MobileDeviceModels models = me.MobileDeviceModels.SingleOrDefault(m => m.ID == mobileDeviceModelsID);
            return models;
        }

        public List<MobileDeviceModels> SelectAllEntities()
        {
            MarioEntities me = new MarioEntities();
            var result = (from m in me.MobileDeviceModels
                         select m).OrderBy(m => m.ID);

            return result.ToList<MobileDeviceModels>();
        }

        public MobileDeviceModels GetRandomMobileDeviceModels()
        {
            MarioEntities me = new MarioEntities();
            Random random = new Random(DateTime.Now.Millisecond);
            int skipCount = random.Next(this.GetWeightSum());
            MobileDeviceModels models = me.MobileDeviceModels.FirstOrDefault(m => m.RandomStart <= skipCount && m.RandomEnd >= skipCount);
            return models;
        }

        /// <summary>
        /// 根据权重计算随机数权重
        /// </summary>
        /// <returns></returns>
        public int ComputeWeight()
        {
            MarioEntities me = new MarioEntities();
            List<MobileDeviceModels> mobileDevicesList = me.MobileDeviceModels.ToList();
            int randomStart = 0;
            foreach (MobileDeviceModels deviceModel in mobileDevicesList)
            {
                deviceModel.RandomStart = randomStart;
                deviceModel.RandomEnd = randomStart + deviceModel.Weight - 1;
                randomStart += deviceModel.Weight;
            }
            return me.SaveChanges();
        }

        /// <summary>
        /// 获取总的权重值
        /// </summary>
        /// <returns></returns>
        public int GetWeightSum()
        {
            MarioEntities me = new MarioEntities();
            return me.MobileDeviceModels.Sum(m => m.Weight);
        }

    }
}
