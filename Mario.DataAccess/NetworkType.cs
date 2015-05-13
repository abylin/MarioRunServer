using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.DataAccess
{
    /// <summary>
    /// 网络类型
    /// </summary>
    public enum NetworkType
    {
        /// <summary>
        /// 不明网络
        /// </summary>
        NETWORK_TYPE_UNKNOWN = 0,
        /// <summary>
        /// 移动/联通
        /// </summary>
        NETWORK_TYPE_GPRS = 1,
        /// <summary>
        /// 移动/联通
        /// </summary>
        NETWORK_TYPE_EDGE = 2,
        /// <summary>
        /// 移动/联通/电信
        /// </summary>
        NETWORK_TYPE_UMTS = 3,
        /// <summary>
        /// 联通/电信
        /// </summary>
        NETWORK_TYPE_CDMA = 4,
        /// <summary>
        /// 电信
        /// </summary>
        NETWORK_TYPE_EVDO_0 = 5,
        /// <summary>
        /// 电信
        /// </summary>
        NETWORK_TYPE_EVDO_A = 6,
        /// <summary>
        /// 电信
        /// </summary>
        NETWORK_TYPE_1xRTT = 7,
        /// <summary>
        /// 联通
        /// </summary>
        NETWORK_TYPE_HSDPA = 8,
        /// <summary>
        /// 联通
        /// </summary>
        NETWORK_TYPE_HSUPA = 9,
        /// <summary>
        /// 联通
        /// </summary>
        NETWORK_TYPE_HSPA = 10,
        /// <summary>
        /// 不用
        /// </summary>
        NETWORK_TYPE_IDEN = 11,
        /// <summary>
        /// 电信
        /// </summary>
        NETWORK_TYPE_EVDO_B = 12,
        /// <summary>
        /// 移动/联通
        /// </summary>
        NETWORK_TYPE_LTE = 13,
        /// <summary>
        /// 电信
        /// </summary>
        NETWORK_TYPE_EHRPD = 14,
        /// <summary>
        /// 联通
        /// </summary>
        NETWORK_TYPE_HSPAP = 15

    }
}
