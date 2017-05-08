using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Core.Extensions;
using Repository;
using Core.Model;
using MPUtil.UserMng;
using Core.Helper;
using Core;
using Model;

namespace Service
{
    public class CookieHelper
    {

        //更新用户的cookie
        public static void CreateUserId(string id)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(Params.UserCookieName);
                using (DbRepository db = new DbRepository())
                {
                    cookie.Value = id;
                    cookie.Expires = DateTime.Now.AddYears(1);
                }
                // 写登录Cookie
                HttpContext.Current.Response.Cookies.Remove(cookie.Name);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch { }
        }

        /// <summary>
        /// 获取当前用户id
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserId()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.UserCookieName];
            if (cookie == null)
                return null;
            return cookie.Value;
        }

        
    }
}
