using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

using Mario.WebUI.WeiXin.Entity;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI.WeiXin
{
    /// <summary>
    /// 与微信服务器的通信接口
    /// </summary>
    public class WeiXinHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string token = ConfigurationManager.AppSettings["WeiXinToken"]; //从配置文件获取Token

                string signature = HttpContext.Current.Request.QueryString["signature"]; // 例子：0167f97a6273a7319717096351b25deb022ad10b
                string timestamp = HttpContext.Current.Request.QueryString["timestamp"]; // 例子：1430104920
                string nonce = HttpContext.Current.Request.QueryString["nonce"]; // 例子：:672413572
                if (WeiXinHelper.Instance.CheckSignature(token, timestamp, nonce, signature)) // 验证通过，是微信服务器所发
                {
                    if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
                    {
                        WeiXinBaseMessage inputMessage = WeiXinHelper.Instance.GetWeiXinInputMessage(context);
                        string serverWeiXinUserName = inputMessage.ToUserName; // 例子：gh_20dd46924ae0
                        string outputMessage = WeiXinHelper.Instance.BuildTextMessage(inputMessage.FromUserName, serverWeiXinUserName, "没有可回复的消息!");

                        if (inputMessage is WeiXinBaseEvent) // 收到的消息是事件
                        {
                            WeiXinBaseEvent inputEvent = inputMessage as WeiXinBaseEvent;

                            if (inputEvent.EventName == "subscribe")
                            {
                                #region 刚订阅，第一次进微信公众号
                                // OpenID例子：oMgpSs2X5k3dKEUIrFGMYlMDXwlQ
                                outputMessage = WeiXinHelper.Instance.BuildTextMessage(inputMessage.FromUserName, serverWeiXinUserName, "您好，欢迎关注进入Mario微信公众号！，您的OpenID为："
                                    + inputMessage.FromUserName);

                                #endregion
                            }
                            else
                            {
                                string[] keyVals = inputEvent.EventKey.Split('_');
                                string userName = "";
                                if (keyVals.Length == 1)
                                    userName = keyVals[0];
                                else
                                    userName = keyVals[1];
                                userName = HttpUtility.UrlDecode(userName);
                                string openID = inputEvent.FromUserName;
                                
                                UserInfosManager userInfosManager = new UserInfosManager();

                                if (userName != null)
                                {
                                    if (userName.StartsWith("#")) // 
                                    {
                                        #region 登陆处理
                                        //UserInfos userInfo = userInfosManager.SelectBindOpenIDUser(openID);
                                        //WeiXinUserState userState = new WeiXinUserState();
                                        //userState.User = userInfo;
                                        //if (userInfo != null && userInfo.WeiXinOpenID == openID)
                                        //{
                                        //    userState.Info = "登陆成功";
                                        //    UserDict.AddOrUpdate(userName, userState);
                                        //    responseText = WeiXinHelper.Instance.BuildTextMessage(baseMessage.FromUserName, baseMessage.ToUserName, "登陆成功");
                                        //}
                                        //else
                                        //{
                                        //    userState.Info = "登陆失败，不存在相应的手机绑定";
                                        //    UserDict.AddOrUpdate(userName, userState);
                                        //    responseText = WeiXinHelper.Instance.BuildTextMessage(baseMessage.FromUserName, baseMessage.ToUserName, userState.Info);
                                        //}
                                        #endregion
                                    }
                                    else 
                                    {
                                        #region 用户绑定处理

                                        UserInfos user = userInfosManager.SelectSingleUser(userName);
                                        user.WeiXinOpenID = openID;
                                        try
                                        {
                                            int result = userInfosManager.AddOrUpdateUser(user);
                                            if (result > 0)
                                            {
                                                outputMessage = WeiXinHelper.Instance.BuildTextMessage(inputMessage.FromUserName, serverWeiXinUserName, 
                                                    string.Format("用户名{0}与微信OpenID{1}绑定成功！EventKey为：{2}。", userName, openID, inputEvent.EventKey));
                                            }
                                        }
                                        catch(Exception ex)
                                        {
                                            outputMessage = WeiXinHelper.Instance.BuildTextMessage(inputMessage.FromUserName, serverWeiXinUserName, 
                                                    string.Format("绑定失败！用户名{0}，微信OpenID{1}，EventKey为：{2}。异常：{3}", userName, openID, inputEvent.EventKey, ex.Message + ex.StackTrace));
                                        }

                                        #endregion
                                    }
                                }
                            }
                        }
                        else if (inputMessage is WeiXinTextMessage) // 收到的消息是文本消息
                        {
                            WeiXinTextMessage inputTextMessage = inputMessage as WeiXinTextMessage;
                            string outputText = string.Format("收到消息，OK! 用户OpenID:{0}, content:{1}, 服务器ID:{2}",
                                inputMessage.FromUserName, inputTextMessage.Content, serverWeiXinUserName);
                            outputMessage = WeiXinHelper.Instance.BuildTextMessage(inputTextMessage.FromUserName, serverWeiXinUserName, outputText);
                        }

                        context.Response.Write(outputMessage);
                        context.Response.Flush();
                        context.Response.End();
                    }
                    else // GET操作, 一般只做第一次验证用
                    {
                        string echoString = HttpContext.Current.Request.QueryString["echoStr"]; // 随机字符串，POST操作不会带这个参数
                        if (!string.IsNullOrEmpty(echoString))
                        {
                            context.Response.Write(echoString);
                            context.Response.Flush();
                            context.Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemLogManager slm = new SystemLogManager();
                slm.AddSystemLog(new SystemLog()
                {
                    UserID = 0,
                    UserName = "WeiXin",
                    LogTime = DateTime.Now,
                    Memo = ex.Message + ex.StackTrace
                }
                );
                context.Response.Write(ex.Message + ex.StackTrace);
                context.Response.Flush();
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}