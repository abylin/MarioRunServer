using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mario.LoadTester
{
    public class LoadClient
    {
        public void PostRequest()
        {
            WebClient client = new WebClient();
            string[] mobileDeviceArray = { "100000", "100002", "100003", "100004" };
            Random random = new Random(DateTime.Now.Millisecond);

            Thread.Sleep(random.Next(100)); 
            string cId = mobileDeviceArray[random.Next(mobileDeviceArray.Length)];
            string url = "http://115.29.192.23/task.ashx?type=getTest&cId=" + cId;
            Console.WriteLine(string.Format("开始:线程号：{0}，移动设备编号{1}.", Thread.CurrentThread.ManagedThreadId.ToString(), cId));
            byte[] result = client.DownloadData(url);
            Console.WriteLine(string.Format("结束线程号：{0}，移动设备编号{1}.", Thread.CurrentThread.ManagedThreadId.ToString(), cId)); 
            // Console.WriteLine(Encoding.UTF8.GetString(result));
        }
    }
}
