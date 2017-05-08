using Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MeetOL.Filters
{
    public class ExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                return;
            var ex = filterContext.Exception ?? new Exception("no further information exists");
            filterContext.ExceptionHandled = true;
            
            Core.Util.LogHelper.WriteException(ex);
            RedirectResult redirectResult = new RedirectResult("/base/_505");
            filterContext.Result = redirectResult;
        }
    }
}