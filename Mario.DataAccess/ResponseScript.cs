using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.DataAccess
{
    /// <summary>
    /// 返回给Mario客户端中的脚本实体
    /// </summary>
    public class ResponseScript
    {
        public int x1 = -1;
        public int y1 = -1;
        public int x2 = -1;
        public int y2 = -1;
        public int key = -1;
        public int interval;
        public int action = 1;
        public string cmd;
    }
}
