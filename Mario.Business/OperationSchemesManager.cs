using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class OperationSchemesManager
    {
        public int AddOrUpdate(OperationSchemes scheme)
        {
            MarioEntities me = new MarioEntities();
            me.OperationSchemes.Attach(scheme);
            if (scheme.ID == 0)
            {
                me.Entry<OperationSchemes>(scheme).State = EntityState.Added;
            }
            else
            {
                me.Entry<OperationSchemes>(scheme).State = EntityState.Modified;
            }
            me.SaveChanges();
            return scheme.ID;
        }

        /// <summary>
        /// 删除方案及报文
        /// </summary>
        /// <param name="schemeID"></param>
        /// <returns></returns>
        public int Delete(int schemeID)
        {
            // 删除方案里的报文
            OperationMessagesManager omm = new OperationMessagesManager();
            omm.DeleteFromOperationSchemes(schemeID);

            MarioEntities me = new MarioEntities();
            var result = me.OperationSchemes.Where(s => s.ID == schemeID).FirstOrDefault();
            me.OperationSchemes.Remove(result);
            return me.SaveChanges();
        }

        public int DeleteFromAppProjects(int projectID)
        {
            MarioEntities me = new MarioEntities();
            List<OperationSchemes> list = me.OperationSchemes.Where(s => s.AppProjectsID == projectID).ToList();
            foreach (OperationSchemes scheme in list)
            {
                this.Delete(scheme.ID);
 
            }
            return list.Count();
        }

        public OperationSchemes SelectSingleEntity(int schemeID)
        {
            MarioEntities me = new MarioEntities();
            OperationSchemes scheme = me.OperationSchemes.SingleOrDefault(s => s.ID == schemeID);
            return scheme;
        }

        public List<OperationSchemes> SelectOperationSchemesFromAppProject(int projectID, bool isNewAdd)
        {
            MarioEntities me = new MarioEntities();
            if (isNewAdd)
            {
                List<OperationSchemes> list = me.OperationSchemes.Where(s => s.AppProjectsID == projectID && s.SchemeType == 1).ToList();
                if (list.Count == 0) // 如果没有特定的新增方案，也执行留存
                {
                    return me.OperationSchemes.Where(s => s.AppProjectsID == projectID && s.SchemeType == 0).ToList();
                }
                else
                {
                    return list;
                }
            }
            else
            {
                return me.OperationSchemes.Where(s => s.AppProjectsID == projectID && s.SchemeType == 0).ToList();
            }
        }

        public List<OperationSchemes> SelectAllOperationSchemesFromAppProject(int projectID)
        {
            MarioEntities me = new MarioEntities();
            return me.OperationSchemes.Where(s => s.AppProjectsID == projectID).ToList();
        }
        
        /// <summary>
        /// 复制操作方案到新的项目中
        /// </summary>
        /// <param name="schemeID"></param>
        /// <param name="projectID"></param>
        public void CopyOperationSchemesToAppProject(int schemeID, int projectID)
        {
            OperationSchemes oldScheme = this.SelectSingleEntity(schemeID);
            OperationSchemes newScheme = new OperationSchemes();
            newScheme.AppProjectsID = projectID;
            newScheme.Name = oldScheme.Name;
            newScheme.SchemeType = oldScheme.SchemeType;
            this.AddOrUpdate(newScheme);

            // 复制操作方案里的操作报文
            OperationMessagesManager omm = new OperationMessagesManager();
            List<OperationMessages> messageList = omm.SelectOperationMessagesFromOperationSchemes(oldScheme.ID);
            foreach (OperationMessages message in messageList)
            {
                this.copyOperationMessagesToOperationSchemes(message, newScheme);
            }

        }

        private void copyOperationMessagesToOperationSchemes(OperationMessages oldMessage, OperationSchemes newScheme)
        {
            OperationMessages newMessage = new OperationMessages();
            newMessage.OperationSchemesID = newScheme.ID;
            newMessage.Step = oldMessage.Step;
            newMessage.XPoint = oldMessage.XPoint;
            newMessage.YPoint = oldMessage.YPoint;
            newMessage.ToXPoint = oldMessage.ToXPoint;
            newMessage.ToYPoint = oldMessage.ToYPoint;
            newMessage.PhysicalKey = oldMessage.PhysicalKey;
            newMessage.Interval = oldMessage.Interval;
            newMessage.Action = oldMessage.Action;
            newMessage.CommandScript = oldMessage.CommandScript;
            newMessage.Memo = oldMessage.Memo;

            OperationMessagesManager omm = new OperationMessagesManager();
            omm.AddOrUpdate(newMessage);
        }
    }
}
