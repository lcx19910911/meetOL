 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetOL.Controllers;
using Service;
using IService;
using Core;
using Core.Extensions;

namespace MeetOL.Filters
{
    /// <summary>
    /// 是否参加了会议过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JoinFilterAttribute : ActionFilterAttribute
    {

        public IMeetUserJoinService IService=new MeetUserJoinService();


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;
            string requestUrl = filterContext.HttpContext.Request.Url.ToString();
            if (controller.LoginUser != null)
            {
                if (controller.MeetID.IsNotNullOrEmpty())
                {
                    //判断是否
                    var join = IService.Find(controller.MeetID, controller.LoginUser.ID);
                    if (join==null)
                    {
                        filterContext.Result = new RedirectResult("/meet/join");
                    }
                    else
                    {
                        if (join.State != Model.UserJoinState.Pass)
                        {
                            filterContext.Result = new RedirectResult("/meet/join");
                        }
                        else
                        {
                            //if(join.HadSign==EnumPro.YesOrNoCode.No)
                            //{
                            //    filterContext.Result = new RedirectResult("/meet/index");
                            //}
                        }
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("/");
                }
            }
        }
    }
}