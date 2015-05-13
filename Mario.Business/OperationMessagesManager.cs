using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;


namespace Mario.Business
{
    public class OperationMessagesManager
    {
        public int AddOrUpdate(OperationMessages message)
        {
            MarioEntities me = new MarioEntities();
            me.OperationMessages.Attach(message);
            if (message.ID == 0)
            {
                me.Entry<OperationMessages>(message).State = EntityState.Added;
            }
            else
            {
                me.Entry<OperationMessages>(message).State = EntityState.Modified;
            }
            me.SaveChanges();
            return message.ID;
        }

        public void SaveMessage(OperationMessages message)
        {
            MarioEntities me = new MarioEntities();
            // 查找同一方案里是否有存在相同步骤号的报文
            int count = me.OperationMessages.Where(om => om.OperationSchemesID == message.OperationSchemesID && om.ID != message.ID && om.Step == message.Step).Count();
            if(count >= 1)
            { 
                List<OperationMessages> repeatStepMessageList = me.OperationMessages.Where(om => om.OperationSchemesID == message.OperationSchemesID && om.ID != message.ID && om.Step >= message.Step).ToList();
                foreach(OperationMessages addMessage in repeatStepMessageList)
                {
                    addMessage.Step += 1;
                }
                me.SaveChanges();
            }
            this.AddOrUpdate(message);
            return;
        }

        public void CopyMessage(int messageID)
        {
            OperationMessages oldMessage = this.SelectSingleEntity(messageID);
            OperationMessages newMessage = new OperationMessages();
            newMessage.OperationSchemesID = oldMessage.OperationSchemesID;
            newMessage.Step = oldMessage.Step;
            newMessage.XPoint = oldMessage.XPoint;
            newMessage.YPoint = oldMessage.YPoint;
            newMessage.ToXPoint = oldMessage.ToXPoint;
            newMessage.ToYPoint = oldMessage.ToYPoint;
            newMessage.PhysicalKey = oldMessage.PhysicalKey;
            newMessage.Interval = oldMessage.Interval;
            newMessage.Action = oldMessage.Action;
            newMessage.CommandScript = oldMessage.CommandScript;

            this.SaveMessage(newMessage);
        }

        public int Delete(int messageID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.OperationMessages.Where(m => m.ID == messageID).FirstOrDefault();
            me.OperationMessages.Remove(result);
            return me.SaveChanges();
        }

        public int DeleteFromOperationSchemes(int schemeID)
        {
            MarioEntities me = new MarioEntities();
            me.OperationMessages.RemoveRange(me.OperationMessages.Where(m => m.OperationSchemesID == schemeID));
            return me.SaveChanges();
        }

        public OperationMessages SelectSingleEntity(int messageID)
        {
            MarioEntities me = new MarioEntities();
            OperationMessages message = me.OperationMessages.SingleOrDefault(m => m.ID == messageID);
            return message;
        }

        public List<OperationMessages> SelectOperationMessagesFromOperationSchemes(int operationSchemesID)
        {
            MarioEntities me = new MarioEntities();
            var result = me.OperationMessages.Where(m => m.OperationSchemesID == operationSchemesID).OrderBy(m => m.Step);

            return result.ToList<OperationMessages>();
        }

        public string MoveUp(int messageID)
        {
            MarioEntities me = new MarioEntities();
            OperationMessages swapMessage = me.OperationMessages.Where(m => m.ID == messageID).FirstOrDefault();
            var result = me.OperationMessages.Where(m => m.Step < swapMessage.Step && m.OperationSchemesID == swapMessage.OperationSchemesID);
            List<OperationMessages> selectedMessageList = result.ToList();
            if (selectedMessageList.Count == 0)
            {
                return "待选择的记录已经是在最前面了";
            }
            OperationMessages maxMessage  = null;
            foreach (OperationMessages selectedMessage in selectedMessageList)
            {
                if (maxMessage == null)
                {
                    maxMessage = selectedMessage;
                    continue;
                }
                if (maxMessage.Step < selectedMessage.Step)
                {
                    maxMessage = selectedMessage;
                }
            }

            // 交换两个步骤序号
            int temp = maxMessage.Step;
            maxMessage.Step = swapMessage.Step;
            swapMessage.Step = temp;
            me.SaveChanges();

            return null;
        }

        public string MoveDown(int messageID)
        {
            MarioEntities me = new MarioEntities();
            OperationMessages swapMessage = me.OperationMessages.Where(m => m.ID == messageID).FirstOrDefault();
            var result = me.OperationMessages.Where(m => m.Step > swapMessage.Step && m.OperationSchemesID == swapMessage.OperationSchemesID);
            List<OperationMessages> selectedMessageList = result.ToList();
            if (selectedMessageList.Count == 0)
            {
                return "待选择的记录已经是在最后面了";
            }
            OperationMessages minMessage = null;
            foreach (OperationMessages selectedMessage in selectedMessageList)
            {
                if (minMessage == null)
                {
                    minMessage = selectedMessage;
                    continue;
                }
                if (minMessage.Step > selectedMessage.Step)
                {
                    minMessage = selectedMessage;
                }
            }

            // 交换两个步骤序号
            int temp = minMessage.Step;
            minMessage.Step = swapMessage.Step;
            swapMessage.Step = temp;
            me.SaveChanges();

            return null;
        }

        public int GetMaxStep(int schemeID)
        {
            MarioEntities me = new MarioEntities();
            int maxStep = 0;
            if (me.OperationMessages.Where( m => m.OperationSchemesID == schemeID).Count() > 0)
            {
                maxStep = (from m in me.OperationMessages
                               where m.OperationSchemesID == schemeID
                               select m.Step).Max();
            }           
            return maxStep;
        }
    }
}
