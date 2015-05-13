using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mario.DataAccess;
using System.Data.Entity;

namespace Mario.Business
{
    public class RolesManager
    {
        public int AddOrUpdate(Roles role)
        {
            MarioEntities me = new MarioEntities();
            me.Roles.Attach(role);
            if (role.ID == 0)
            {
                me.Entry<Roles>(role).State = EntityState.Added;
            }
            else
            {
                me.Entry<Roles>(role).State = EntityState.Modified;
            }
            me.SaveChanges();
            return role.ID;
        }

        public int Delete(int roleID)
        {
            MarioEntities me = new MarioEntities();

            // 删除角色与APP项目的对应关系
            me.AppProjectsInRole.RemoveRange(me.AppProjectsInRole.Where(a => a.RoleID == roleID));

            var result = me.Roles.Where(m => m.ID == roleID).FirstOrDefault();
            me.Roles.Remove(result);
            return me.SaveChanges();
        }

        public Roles SelectSingleEntity(int roleID)
        {
            MarioEntities me = new MarioEntities();
            Roles role = me.Roles.SingleOrDefault(m => m.ID == roleID);
            return role;
        }

        public List<Roles> SelectAllEntities()
        {
            MarioEntities me = new MarioEntities();
            return me.Roles.ToList();
        }

        #region App项目的权限

        /// <summary>
        /// 设置角色可以访问的APP同时，会清除原有设置
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="appProjectsList"></param>
        public void SetAppProjectsFromRoles(int roleID, List<AppProjects> appProjectsList)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsInRole.Where(apir => apir.RoleID == roleID);
            me.AppProjectsInRole.RemoveRange(result);

            foreach (AppProjects app in appProjectsList)
            {
                AppProjectsInRole apir = new AppProjectsInRole();
                apir.RoleID = roleID;
                apir.AppProjectsID = app.ID;
                me.AppProjectsInRole.Add(apir);
            }

            me.SaveChanges();
        }

        public List<AppProjects> GetSelectedAppProjectsFromRoles(int roleID)
        {
            MarioEntities me = new MarioEntities();
            var result = from ap in me.AppProjects
                         where
                             ((from apir in me.AppProjectsInRole
                               where apir.RoleID == roleID
                               select apir.AppProjectsID).Contains(ap.ID))
                         select ap;
            ;
            return result.ToList<AppProjects>();
        }

        public List<AppProjects> GetNotSelectAppProjectsFromRoles(int roleID)
        {
            MarioEntities me = new MarioEntities();
            var result = from ap in me.AppProjects
                         where
                             (!(from apir in me.AppProjectsInRole
                                where apir.RoleID == roleID
                                select apir.AppProjectsID).Contains(ap.ID))
                         select ap;
            ;
            return result.ToList<AppProjects>();
        }

        public int AddOrUpdateAppProjectsInRole(AppProjectsInRole apir)
        {
            MarioEntities me = new MarioEntities();
            me.AppProjectsInRole.Attach(apir);
            if (apir.ID == 0)
            {
                me.Entry<AppProjectsInRole>(apir).State = EntityState.Added;
            }
            else
            {
                me.Entry<AppProjectsInRole>(apir).State = EntityState.Modified;
            }
            me.SaveChanges();
            return apir.ID;

        }

        public int RemoveAppProjectsFromRole(int roleID, int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsInRole.Where(m => m.RoleID == roleID && m.AppProjectsID == appProjectID).FirstOrDefault();
            me.AppProjectsInRole.Remove(result);
            return me.SaveChanges();
        }

        public int RemoveAppProjectsFromAllRole(int appProjectID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.AppProjectsInRole.Where(m => m.AppProjectsID == appProjectID).FirstOrDefault();
            me.AppProjectsInRole.Remove(result);
            return me.SaveChanges();
        }

        #endregion 

        #region 用户权限的角色

        /// <summary>
        /// 设置角色内的用户同时，会清除原有设置
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="appProjectsList"></param>
        public void SetUserInfosFromRoles(int roleID, List<UserInfos> userList)
        {
            MarioEntities me = new MarioEntities();
            var result = me.UserInRole.Where(r => r.RoleID == roleID);
            me.UserInRole.RemoveRange(result);

            foreach (UserInfos user in userList)
            {
                UserInRole uir = new UserInRole();
                uir.RoleID = roleID;
                uir.UserID = user.ID;
                me.UserInRole.Add(uir);
            }
            me.SaveChanges();
        }

        public List<UserInfos> GetSelectedUserInfosFromRoles(int roleID)
        {
            MarioEntities me = new MarioEntities();
            var result = from u in me.UserInfos
                         where
                             ((from uir in me.UserInRole
                               where uir.RoleID == roleID
                               select uir.UserID).Contains(u.ID))
                         select u;
            return result.ToList<UserInfos>();
        }

        public List<UserInfos> GetNotSelectUserInfosFromRoles(int roleID)
        {
            MarioEntities me = new MarioEntities();
            var result = from u in me.UserInfos
                         where
                             (!(from uir in me.UserInRole
                                where uir.RoleID == roleID
                                select uir.UserID).Contains(u.ID))
                         select u;
            return result.ToList<UserInfos>();
        }

        public List<AppProjects> GetAppProjectsFromUserInfos(int userID)
        {
            MarioEntities me = new MarioEntities();
            var result = from app in me.AppProjects
                         where (
                             (from apir in me.AppProjectsInRole
                              where
                                  ((from ur in me.UserInRole where ur.UserID == userID select ur.RoleID).Contains(apir.RoleID))
                              select apir.AppProjectsID).Contains(app.ID))
                         select app;
            return result.ToList<AppProjects>();
        }

        public List<Roles> GetRolesUserInfos(int userID)
        {
            MarioEntities me = new MarioEntities();
            var result = from r in me.Roles
                         where
                             ((from uir in me.UserInRole
                               where uir.UserID == userID
                               select uir.RoleID).Contains(r.ID))
                         select r;
            return result.ToList<Roles>();
        }

        public bool GetIsAdmin(int userID, int adminRoleID)
        {
            List<Roles> roleList = this.GetRolesUserInfos(userID);
            foreach (Roles role in roleList)
            {
                if (role.ID == adminRoleID)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion 
    }
}
