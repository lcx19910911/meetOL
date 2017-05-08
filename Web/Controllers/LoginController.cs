using EnumPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IService;
using Core.Helper;
using Core;
using MeetOL.Filters;
using Core.Model;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using NLog;
using Core.Util;

namespace MeetOL.Controllers
{
    public class LoginController : BaseController
    {

        public IUserService IUserService;

        public LoginController(IUserService _IUserService)
        {
            this.IUserService = _IUserService;
        }


        [GetMeetID]
        // GET: Login
        public ActionResult Index()
        {
            //string data = "{\"openid\":\"on50NvxImo5qMV35CMXo4nQ - ROeE\",\"nickname\":\"刘城熙\",\"sex\":1,\"language\":\"zh_CN\",\"city\":\"Fuzhou\",\"province\":\"Fujian\",\"country\":\"China\",\"headimgurl\":\"http://wx.qlogo.cn/mmopen/tkakSePChPEFtCdC6J5gCzh562xXYkZsuVq1BK3jQicOkwOD7Pr50jJa5lssMpDqPOGrLJaNtfgYf6xkrKXHCia9DbciaCqhKy5/0\",\"privilege\":[]}";
            //JObject obj2 = JsonConvert.DeserializeObject(data) as JObject;
            //if (obj2 != null)
            //{
            //    var model = new Model.User()
            //    {
            //        ID = Guid.NewGuid().ToString("N"),
            //        NickName = obj2["nickname"].ToString(),
            //        OpenId = obj2["openid"].ToString(),
            //        Sex = obj2["sex"].GetInt(),
            //        Province = obj2["province"].ToString(),
            //        City = obj2["city"].ToString(),
            //        Country = obj2["country"].ToString(),
            //        HeadImgUrl = obj2["headimgurl"].ToString(),
            //        RealName = obj2["nickname"].ToString()
            //    };
            //    IUserService.Add(model);
            //    this.LoginUser = model;
            //    this.Response.Redirect("/home/index");
            //}
            if (Request.UserAgent.IsNotNullOrEmpty() && Request.UserAgent.ToLower().Contains("micromessenger"))
            {
                WeixinLoginAction();
            }
            return View();
        }


        public void WeixinLoginAction()
        {
            string code = this.Request.QueryString["code"];


            //LogHelper.WriteInfo("MeetID-  " + this.MeetID);
            //本地没有member cookies
            if (this.LoginUser==null)
            {
                //请求回来
                if (!string.IsNullOrEmpty(code))
                {
                    var url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + Params.WeixinAppId + "&secret=" + Params.WeixinAppSecret + "&code=" + code + "&grant_type=authorization_code";

                  
                    string responseResult = WebHelper.GetPage(url);
                    
                    if (responseResult.Contains("access_token"))
                    {
                        JObject obj2 = JsonConvert.DeserializeObject(responseResult) as JObject;

                        var access_token = obj2["access_token"].ToString();
                        string openId = obj2["openid"].ToString();
                        var user = IUserService.FindByOpenId(openId);
                        if (user == null)
                        {
                            string userResponseResult = WebHelper.GetPage("https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + openId + "&lang=zh_CN");
                            JObject obj3 = JsonConvert.DeserializeObject(userResponseResult) as JObject;
                            if (obj3!=null)
                            {
                                var model = new Model.User()
                                {
                                    ID = Guid.NewGuid().ToString("N"),
                                    NickName = obj3["nickname"].ToString(),
                                    OpenId = obj3["openid"].ToString(),
                                    Sex = obj3["sex"].GetInt(),
                                    Province = obj3["province"].ToString(),
                                    City = obj3["city"].ToString(),
                                    Country = obj3["country"].ToString(),
                                    HeadImgUrl = obj3["headimgurl"].ToString()
                                };
                                IUserService.Add(model);
                                this.LoginUser = model;
                                this.Response.Redirect("/home/index");
                            }
                            else
                            {
                                this.Response.Redirect("/base/_404");
                            }
                        }
                        else
                        {
                            LoginUser = user;
                            this.Response.Redirect("/home/index");
                        }

                    }
                    else
                    {
                        this.Response.Redirect("/base/_404");
                    }
                }
                else if (!string.IsNullOrEmpty(this.Request.QueryString["state"]))
                {
                    this.Response.Redirect("https://" +Params.DomianName);
                }
                else
                {
                    string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + Params.WeixinAppId + "&redirect_uri=" + HttpUtility.UrlEncode(this.Request.Url.ToString().Replace(":" + this.Request.Url.Port.ToString(), "")) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                    this.Response.Redirect(url);
                }

            }

        }


        public string GetApplicationPath()
        {
            string applicationPath = "/";
            if (Request.RequestContext != null)
            {
                try
                {
                    applicationPath = Request.ApplicationPath;
                }
                catch
                {
                    applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                }
            }
            if (applicationPath == "/")
            {
                return string.Empty;
            }
            return applicationPath.ToLower(CultureInfo.InvariantCulture);
        }



        /// <summary>
        /// 登录提交
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param> 
        /// <returns></returns>
        public JsonResult Submit(string account, string password)
        {
            var result = IUserService.Login(account, password);
            if (result.Result != null)
            {
                this.LoginUser = result.Result;
            }
            return JResult(result);

        }


        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Quit()
        {
            this.LoginUser = null;
            return View("Index");
        }
    }
}