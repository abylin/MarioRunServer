using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Mario.WebUI.WeiXin
{
    /// <summary>
    /// QRImageHandler 的摘要说明
    /// </summary>
    public class QRImageHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            { 
                string scene = context.Request.Params["scene"];
                byte[] data = WeiXinHelper.Instance.GetTicketImageData(scene);
                MemoryStream mem = new MemoryStream(data);
                mem.Position = 0;
                Bitmap bmp = new Bitmap(mem);
                bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                mem.Close();
            }
            catch(Exception ex)
            {
                context.Response.Write(ex.Message + ex.StackTrace);
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