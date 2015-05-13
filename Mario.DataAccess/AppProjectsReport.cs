using System;
using System.Collections.Generic;


namespace Mario.DataAccess
{
    /// <summary>
    /// App项目一览表实体
    /// </summary>
    public class AppProjectsReport : AppProjects
    {
        /// <summary>
        /// 昨日新增
        /// </summary>
        public int YesterdayNewAdd { get; set; }

        /// <summary>
        /// 昨日留存
        /// </summary>
        public int YesterdayRetention { get; set; }

        /// <summary>
        /// 承载的移动设备数量
        /// </summary>
        public int MobileDeviceCount { get; set; }
    }
}
