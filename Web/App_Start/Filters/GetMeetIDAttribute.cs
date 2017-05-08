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
    /// 获取会议id
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GetMeetIDAttribute : ActionFilterAttribute
    {

        private IMeetService MeetService = new MeetService();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;
            string requestUrl = filterContext.HttpContext.Request.Url.ToString();
            if (filterContext.HttpContext.Request["meetId"].IsNotNullOrEmpty())
            {
                if (controller.MeetID != null)
                {
                    if (!controller.MeetID.Equals(filterContext.HttpContext.Request["meetId"]))
                    {

                        if (!MeetService.IsExits(x => x.ID == filterContext.HttpContext.Request["meetId"] && !x.IsDelete))
                        {
                            controller.MeetID = MeetService.GetLastMeetID();
                        }
                        else
                        {
                            controller.MeetID = filterContext.HttpContext.Request["meetId"];
                        }
                    }
                    else
                    {

                        if (!MeetService.IsExits(x => x.ID == controller.MeetID && !x.IsDelete))
                        {
                            controller.MeetID = MeetService.GetLastMeetID();
                        }
                    }
                }
                else
                {
                    if (!MeetService.IsExits(x => x.ID == filterContext.HttpContext.Request["meetId"] && !x.IsDelete))
                    {
                        controller.MeetID = MeetService.GetLastMeetID();
                    }
                    else
                    { 
                        controller.MeetID = filterContext.HttpContext.Request["meetId"];
                    }
                }
            }
            else
            {
                if (controller.MeetID == null)
                {
                    controller.MeetID = MeetService.GetLastMeetID();
                }
            }           
        }
    }
}