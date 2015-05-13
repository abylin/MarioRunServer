using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Mario.DataAccess;

namespace Mario.Business
{
    public class PhoneNumberManager
    {
        private PhoneNumberSection phoneNumberSection;

        public PhoneNumberManager()
        {
            this.phoneNumberSection = this.getRandomPhoneNumberSection();
        }

        public long CreateIMSICode()
        {
            // IMSI：国际移动用户识别码，是识别移动用户的标志，IMSI=MCC（移动国家号码3位，中国为460）+MNC（移动网号2或3位）+MSIN（移动台识别号）。
            // MCC + MNC + MIN/ MSIN ，一个典型的 IMSI 号码为 460030912121001
            // MNC: Mobile Network Code ，移动网络码，移动用户的所属 PLMN 网号 ，共 2-3 位.
            Random random = new Random(DateTime.Now.Millisecond);
            string mcc = "4600"; // 包括MIN的第一位0
            string numberSectionString = this.phoneNumberSection.NumberSection.ToString();
            int mnc;
            if (numberSectionString.StartsWith("159") || numberSectionString.StartsWith("158") || numberSectionString.StartsWith("150") || numberSectionString.StartsWith("151")
                || numberSectionString.StartsWith("152") || (numberSectionString.StartsWith("134") && !numberSectionString.StartsWith("1349"))) // 1340 - 1348
            {
                mnc = 2;
            }
            else if (numberSectionString.StartsWith("135") || numberSectionString.StartsWith("136") || numberSectionString.StartsWith("137") || numberSectionString.StartsWith("138")
                || numberSectionString.StartsWith("139"))
            {
                mnc = 0;
            }
            else if (numberSectionString.StartsWith("157") || numberSectionString.StartsWith("188") || numberSectionString.StartsWith("187")  || numberSectionString.StartsWith("147"))
            {
                mnc = 7;
            }
            else if (numberSectionString.StartsWith("1349") || numberSectionString.StartsWith("133") || numberSectionString.StartsWith("180") || numberSectionString.StartsWith("153")
                || numberSectionString.StartsWith("189"))
            {
                mnc = 3;
            }
            else // 没有匹配到的号段根据运营商名随机生成
            {
                int[] mncArray = null;
                if (phoneNumberSection.TelecomOperators == "中国移动")
                {
                    mncArray = new int[] { 0, 2, 7 };
                }
                else if (phoneNumberSection.TelecomOperators == "中国电信")
                {
                    mncArray = new int[] { 3, 5 };
                }
                else if (phoneNumberSection.TelecomOperators == "中国联通")
                {
                    mncArray = new int[] { 1 };
                }
                mnc = mncArray[random.Next(mncArray.Length)];
            }

            // 09 + M0 M1 M2 M3 + ABCD , 其中的 M0、M1、M2、M3 和 MDN 号码中的 H0、H1、H2、H3 可存在对应关系， ABCD 四位为自由分配。
            string min = random.Next(0, 2).ToString(); // MIN第三位为0或1
            min += "9"; // MIN第四位为9
            min += random.Next(1000, 9999).ToString();
            min += random.Next(1000, 9999).ToString();
            return long.Parse(mcc + mnc.ToString() + min);
        }

        public string CreateSimCardNumber()
        {
            // ICCID：Integrate circuit card identity 集成电路卡识别码（固化在手机SIM卡中） ICCID为IC卡的唯一识别号码，共有20位数字组成，其编码格式为：XXXXXX 0MFSS YYGXX XXXXX
            // SIM卡上有20位数码。前面6位（898600）是中国移动的代号；（898601）是中国联通的代号；（898603）是中国电信的代号；
            // 第7位是业务接入号，在135、136、137、138、139中分别为5、6、7、8、9；第8位是SIM卡的功能位，一般为0，现在的预付费SIM卡为1；
            // 第9、10位是各省的编码;01：北京02：天津03：河北04：山西05：内蒙古06：辽宁07：吉林08：黑龙江09：上海10：江苏11：浙江12：安徽13：福建
            // 14： 江西15：山东16：河南17：湖北18：湖南19：广东20：广西21：海南22：四川23：贵州24：云南25：西藏26：陕西27：甘肃28：青海29：宁夏30：新疆
            // 31：重庆，第11、12位是年号；第13位是供应商代码；第14～19位则是用户识别码；第20位是校验位。
            string simCardNumber = "89860"; 
            if (phoneNumberSection.TelecomOperators == "中国移动")
            {
                simCardNumber += "0";
            }
            else if (phoneNumberSection.TelecomOperators == "中国电信")
            {
                simCardNumber += "1";
            }
            else if (phoneNumberSection.TelecomOperators == "中国联通")
            {
                simCardNumber += "3";
            }
            simCardNumber += phoneNumberSection.NumberSection / 10000 % 10; // 取区段的第3位做为SIM卡号的第7位
            Random random = new Random(DateTime.Now.Millisecond);
            simCardNumber += random.Next(2).ToString(); // 第8位为0或1；
            simCardNumber += phoneNumberSection.ProvinceSimCode.ToString().PadLeft(2, '0'); ; // 第9和10位为各省编码 
            simCardNumber += random.Next(16).ToString().PadLeft(2, '0'); // 第11、12位是年号
            simCardNumber += random.Next(99999999).ToString().PadLeft(8, '0'); // 后面8位
            return simCardNumber;
        }

        public long CreatePhoneNumber( )
        {
            Random random = new Random(DateTime.Now.Millisecond);
            long phoneNumber = Convert.ToInt64(phoneNumberSection.NumberSection) * 10000 + random.Next(10000);
            return phoneNumber;
        }

        public string GetTelecomOperatorsName()
        {
            return this.phoneNumberSection.TelecomOperators;
        }

        public string GetPhoneNumberCity()
        {
            return string.Format("{0} {1}", this.phoneNumberSection.Province, this.phoneNumberSection.City);
        }

        public int CreateNetworkType()
        {
            int[] networkTypeArray = null;
            if (phoneNumberSection.TelecomOperators == "中国移动")
            {
                networkTypeArray = new int[] { 1, 2, 3, 13 }; // enum NetworkType
            }
            else if (phoneNumberSection.TelecomOperators == "中国电信")
            {
                networkTypeArray = new int[] { 5, 6, 7, 12, 14 };
            }
            else if (phoneNumberSection.TelecomOperators == "中国联通")
            {
                networkTypeArray = new int[] { 1, 2, 3, 4, 8, 9, 10, 13, 15 };
            }
            Random random = new Random(DateTime.Now.Millisecond);
            return networkTypeArray[random.Next(networkTypeArray.Length)];
        }

        private PhoneNumberSection getRandomPhoneNumberSection()
        {
            MarioEntities me = new MarioEntities();
            Random random = new Random(DateTime.Now.Millisecond);
            int count = me.PhoneNumberSection.Count();
            return me.PhoneNumberSection.OrderBy(p=>p.ID).Skip(random.Next(count)).FirstOrDefault();
        }

    }
}
