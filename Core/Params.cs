using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Security;

namespace Core
{
    public class Params
    {
        /// <summary>
        /// 域名
        /// </summary>
        public static readonly string DomianName = ConfigurationManager.AppSettings["DomianName"];

        /// <summary>
        /// 时间戳有效时间c
        /// </summary>
        public const int TimspanExpiredMinutes = 60;
        /// <summary>
        /// token失效时间
        /// </summary>
        public const int ExpiredDays = 7;

        public static readonly string SecretKey = ConfigurationManager.AppSettings["SecretKey"];
        public static readonly string SiteDomian = ConfigurationManager.AppSettings["SiteDomian"];

        /// <summary>
        /// 跟平台通信密钥
        /// </summary>
        public static readonly string WeixinAppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];

        /// <summary>
        /// 平台地址
        /// </summary>
        public static readonly string WeixinAppId = ConfigurationManager.AppSettings["WeixinAppId"];

        /// <summary>
        /// 登陆cookie
        /// </summary>
        public static readonly string UserCookieName = "wechat_user";
    }
}
