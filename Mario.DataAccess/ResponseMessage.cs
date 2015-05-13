using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.DataAccess
{
    /// <summary>
    /// 返回给Mario客户端中的报文
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        /// 0到100是各种成功状态，默认是0
        /// 101以上是失败状态
        /// 0：执行成功。1：
        /// </summary>
        public int resultCode;

        /// <summary>
        /// 返回结果说明，会显示在手机上
        /// </summary>
        public string resultInfo;
        
        /// <summary>
        /// 对应VirtualIMEI的ID
        /// </summary>
        public int tId;

        /// <summary>
        /// 手机品牌
        /// </summary>
        public string brand;

        /// <summary>
        /// 手机品牌+型号
        /// </summary>
        public string model;

        /// <summary>
        /// 手机型号
        /// </summary>
        public string device;

        public string imei;
        public string imsi;
        public string mac;
        public string line1Number;
        public string simSerialNumber;
        public string networkOperatorName;
        public string networkType;

        /// <summary>
        /// 标志字段，该字段下发给玛丽奥，客户端执行完之后要返回该字段信息。原先用来设计记录是第几天留存。
        /// </summary>
        public string field;

        /// <summary>
        /// Mario客户端版本
        /// </summary>
        public int version;

        /// <summary>
        /// Mario客户端下载地址
        /// </summary>
        public string marioUrl;

        /// <summary>
        /// 分辨率宽度
        /// </summary>
        public int width;

        /// <summary>
        /// 分辨率高度
        /// </summary>
        public int height;

        /// <summary>
        /// Android系统版本
        /// </summary>
        public string release;

        /// <summary>
        /// 转换成16进制的AndroidID, 纯小写16个字符（0－9，a-f）。
        /// </summary>
        public string androidId;
        /// <summary>
        /// 操作报文
        /// </summary>
        public ResponseScript[] script;
    }
}
