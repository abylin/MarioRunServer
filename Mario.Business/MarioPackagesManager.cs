using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class MarioPackagesManager
    {
        public int AddOrUpdate(MarioPackages pack)
        {
            MarioEntities me = new MarioEntities();
            me.MarioPackages.Attach(pack);
            if (pack.ID == 0)
            {
                me.Entry<MarioPackages>(pack).State = EntityState.Added;
            }
            else
            {
                me.Entry<MarioPackages>(pack).State = EntityState.Modified;
            }
            me.SaveChanges();
            return pack.ID;
        }

        public int Delete(int marioPackagesID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.MarioPackages.Where(m => m.ID == marioPackagesID).FirstOrDefault();
            me.MarioPackages.Remove(result);
            return me.SaveChanges();
        }

        public MarioPackages SelectSingleMarioPackages(int marioPackagesID)
        {
            MarioEntities me = new MarioEntities();
            MarioPackages pack = me.MarioPackages.SingleOrDefault(m => m.ID == marioPackagesID); 
            return pack;
        }

        public MarioPackages GetLastestMarioPackages()
        {
            MarioEntities me = new MarioEntities();
            MarioPackages pack = me.MarioPackages.OrderByDescending(m => m.Version).FirstOrDefault();
            return pack;
        }

        public List<MarioPackages> SelectAllMarioPackages()
        {
            MarioEntities me = new MarioEntities();
            return me.MarioPackages.OrderByDescending(m => m.Version).ToList();
        }
    }
}
