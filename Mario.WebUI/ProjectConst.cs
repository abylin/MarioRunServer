using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mario.WebUI
{
    /// <summary>
    /// 项目中公用的常量
    /// </summary>
    public class ProjectConst
    {
        /// <summary>
        /// Excel文件连接字符串模板
        /// </summary>
        public const string EXCEL_CONNECTION_STRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES'";

        /// <summary>
        /// ID为空进的文本
        /// </summary>
        public const string NEW_EMPTY_ID = "系统自动生成";

        /// <summary>
        /// 系统管理员组的ID
        /// </summary>
        public const int ADMIN_ROLE_ID = 1;
    }
}