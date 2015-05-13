using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mario.LoadTester
{
    class Program
    {
        static void Main(string[] args)
        {
            int threadCount = 100;
            DateTime startTime = DateTime.Now;
            Console.WriteLine(string.Format("测试开始，{0}个线程准备启动,开始时间为{1}:", threadCount.ToString(), startTime.TimeOfDay.ToString()));
            Thread[] threadArray = new Thread[threadCount];
            for(int i = 0; i < threadCount; i++)
            {
                LoadClient client = new LoadClient();
                threadArray[i] = new Thread(new ThreadStart(client.PostRequest));
                threadArray[i].Start();
                Thread.Sleep(10);
            }
            for (int i = 0; i < threadCount; i++)
            {
                threadArray[i].Join();
            }
            
            Console.WriteLine("主线程完成!");
            DateTime endTime = DateTime.Now;
            Console.WriteLine(string.Format("测试结束，共执行了{0}个线程请求,结束时间为{1},共耗时为{2}秒.", 
                threadCount.ToString(), endTime.TimeOfDay.ToString(), (endTime - startTime).TotalSeconds.ToString()));
            Console.ReadLine();
        }
    }
}
