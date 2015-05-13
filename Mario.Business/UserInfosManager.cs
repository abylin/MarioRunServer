using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class UserInfosManager
    {
        /// <summary>
        /// 验证用户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>密码正确返回UserInfos,用户名错误或密码返回null</returns>
        public UserInfos VerifyPassword(string username, string password)
        {
            MarioEntities me = new MarioEntities();
            UserInfos user = me.UserInfos.SingleOrDefault(u => u.UserName == username);
            if (user == null || user.Password != password)
            {
                // 用户名或密码错误
                return null;
            }
            else
            {
                return user;
            }
        }

        public int AddOrUpdateUser(UserInfos user)
        {
            MarioEntities me = new MarioEntities();
            me.UserInfos.Attach(user);
            if (user.ID == 0)
            {
                me.Entry<UserInfos>(user).State = EntityState.Added;
            }
            else
            {
                me.Entry<UserInfos>(user).State = EntityState.Modified;
            }
            me.SaveChanges();
            return user.ID;
        }

        public int DeleteUser(int userID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.UserInfos.Where(u => u.ID == userID).FirstOrDefault();
            me.UserInfos.Remove(result);
            return me.SaveChanges();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>如果修改成功，则返回空字符串；否则返回异常信息</returns>
        public string ChangePassword(string username, string oldPassword, string newPassword)
        {
            MarioEntities me = new MarioEntities();
            UserInfos user = me.UserInfos.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return "不能修改,找不到此用户名";
            }
            if (user.Password != oldPassword)
            {
                return "原密码不正确";
            }
            try
            {
                user.Password = newPassword;
                me.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public UserInfos SelectSingleUser(int userID)
        {
            MarioEntities me = new MarioEntities();
            UserInfos user = me.UserInfos.SingleOrDefault(u => u.ID == userID); 
            return user;
        }

        public UserInfos SelectSingleUser(string userName)
        {
            MarioEntities me = new MarioEntities();
            UserInfos user = me.UserInfos.SingleOrDefault(u => u.UserName == userName);
            return user;
        }

        public List<UserInfos> SelectAllUsers()
        {
            MarioEntities me = new MarioEntities();
            var result = from u in me.UserInfos
                         select u;

            return result.ToList<UserInfos>();
        }
    }
}
