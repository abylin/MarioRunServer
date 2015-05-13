using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mario.DataAccess;
using Mario.Business;
using Newtonsoft.Json;

namespace Mario.WebUI
{
    /// <summary>
    /// Mario移动客户端与服务端的通信接口
    /// </summary>
    public class Task : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string type = context.Request.QueryString["type"];
            MobileDevicesManager mobileDevicesManager = new MobileDevicesManager();
            try
            {     
                if (type == "getTask") // 获取手机任务
                {
                    int mobileDeviceID = Convert.ToInt32(context.Request.QueryString["cId"]);
                    MobileDevices mobileDevice = mobileDevicesManager.SelectSingleMobileDevices(mobileDeviceID);
                    if (mobileDevice == null)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 102, resultInfo = "此移动设备不存在或还没有注册,cID:" + mobileDeviceID }));
                        return;
                    }
                    if (mobileDevice.NeedReboot) // 服务器下发了重启指令
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 99, resultInfo = "服务器下发了重启指令" }));
                        mobileDevice.NeedReboot = false;
                        mobileDevice.LastResponseTime = DateTime.Now;
                        mobileDevicesManager.AddOrUpdate(mobileDevice);
                    }
                    else if (DateTime.Now.TimeOfDay < new TimeSpan(0, 15, 00)) // 每天凌晨00:00:00 － 00:15:00 不发任务
                    {
                        TimeSpan restSpan = new TimeSpan(0, 15, 00) - DateTime.Now.TimeOfDay;
                        context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 21, resultInfo = string.Format("移动设备还需要休眠：{0}:{1}:{2}",
                            restSpan.Hours, restSpan.Minutes, restSpan.Seconds)
                        })); 
                    }
                    else
                    {
                        TasksManager mananger = new TasksManager();
                        ResponseMessage message = mananger.GetMobileDeviceTask(mobileDevice, true);
                        string responeText = JsonConvert.SerializeObject(message);
                        context.Response.Write(responeText);
                        mobileDevicesManager.UpdateLastResponseTime(mobileDeviceID, DateTime.Now); // 更新最后响应时间
                    }
                }
                else if (type == "feedback") // 客户端反馈是否完成
                {
                    string isSuccess = context.Request.QueryString["isSuccess"]; // 1:成功 2:失败
                    int imeiId = Convert.ToInt32(context.Request.QueryString["tId"]); // 虚拟手机编号IMEIER_ID，是自增长主键，不是虚拟的IMEI号。
                    int mobileDeviceID = int.Parse(context.Request.QueryString["cId"]); 

                    VirtualIMEIManager imeiManager = new VirtualIMEIManager();
                    string responeText = imeiManager.SetIMEIStatus(imeiId, isSuccess == "1", mobileDeviceID);
                    mobileDevicesManager.UpdateLastResponseTime(mobileDeviceID, DateTime.Now); // 更新最后响应时间 
                    context.Response.Write(responeText);
                    
                }
                else if (type == "getTest") // 功能与GetTask类似,但是不保存,只做测试用
                {
                    int mobileDeviceID = Convert.ToInt32(context.Request.QueryString["cId"]);
                    MobileDevices mobileDevices = mobileDevicesManager.SelectSingleMobileDevices(mobileDeviceID);
                    TasksManager mananger = new TasksManager();
                    ResponseMessage message = mananger.GetMobileDeviceTask(mobileDevices, false);
                    string responeText = JsonConvert.SerializeObject(message);
                    context.Response.Write(responeText);
                }
                else if (type == "getCID")  // 移动客户端申请注册,向服务器申请一个ID
                {
                    MobileDevices device = new MobileDevices();
                    device.LastResponseTime = DateTime.Now;
                    device.InUse = true;
                    device.Memo = string.Empty;
                    device.RealIMEI = long.Parse(context.Request.QueryString["real_imei"].ToString());
                    device.RealModel = context.Request.QueryString["real_model"].Trim(); 
                    if (device.RealModel == string.Empty)
                    {  
                        context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 104, resultInfo = "注册报文中没有真实型号" })); 
                        return;
                    }
                    MobileDevices existMobileDevices = mobileDevicesManager.SelectSingleMobileDevices(device.RealIMEI);
                    if (existMobileDevices != null) // 这部移动设备以前已注册过，所以直接返回以前的编号
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 20, resultInfo = existMobileDevices.ID.ToString() })); 
                        return;
                    }
                    int mobileID = mobileDevicesManager.AddOrUpdate(device);
                    mobileDevicesManager.UpdateLastResponseTime(mobileID, DateTime.Now); // 更新最后响应时间 
                    // 至少要返回6位字符，数据库直接从100000开始编号。  测试环境从800000开始编号 
                    context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 0, resultInfo = mobileID.ToString() })); 
                }
                else if (type == "postLogMessage") // 移动设备客户端上报日志
                {
                    int mobileDeviceID = int.Parse(context.Request.QueryString["cId"]);
                    string logMessage = context.Request.QueryString["logMessage"];
                    MobileDevicesLogManager mobileDevicesLogManager = new MobileDevicesLogManager();
                    int logResult = mobileDevicesLogManager.AddLog(new MobileDevicesLog { 
                        MobileDevicesID = mobileDeviceID,
                        LogTime = DateTime.Now,
                        Memo = logMessage
                    });
                    context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 0, resultInfo = string.Empty })); 
                }
                else if (type == "getPhoneInfo") // 随机生成一个不带项目信息的虚拟手机信息，给黄世忠测试用
                {
                    TasksManager taskMananger = new TasksManager();
                    VirtualIMEI imei = taskMananger.BuildSingleVirtualIMEIWithoutAppProject(DateTime.Now.Date);
                    ResponseMessage message = taskMananger.BuildJsonMessage(imei, null, 0, false);
                    string responeText = JsonConvert.SerializeObject(message);
                    context.Response.Write(responeText);
                }
            }
            catch (Exception ex)
            {
                string errorInfo = type + "接口异常： " + context.Request.QueryString.ToString() + ex.Message + ex.StackTrace;
                context.Response.Write(JsonConvert.SerializeObject(new ResponseMessage { resultCode = 200, resultInfo = errorInfo }));
                this.addSystemLog(errorInfo);
            }    
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void addSystemLog(string memo)
        {
            SystemLogManager slm = new SystemLogManager();
            slm.AddSystemLog(new SystemLog()
            {
                UserID = 0,
                UserName = "Task.ashx",
                LogTime = DateTime.Now,
                Memo = memo
            });
        }
    }
}