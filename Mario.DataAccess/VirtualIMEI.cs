//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mario.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class VirtualIMEI
    {
        public int ID { get; set; }
        public int AppProjectsID { get; set; }
        public long IMEI { get; set; }
        public long IMSI { get; set; }
        public string MAC { get; set; }
        public string Brand { get; set; }
        public string Device { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int TaskStatus { get; set; }
        public int MobileDevicesID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public long Line1Number { get; set; }
        public string SimSerialNumber { get; set; }
        public string TelecomOperatorsName { get; set; }
        public int NetworkType { get; set; }
        public string PhoneNumberCity { get; set; }
        public Nullable<System.DateTime> RetainStartTime { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string OSVersion { get; set; }
        public long AndroidID { get; set; }
    }
}