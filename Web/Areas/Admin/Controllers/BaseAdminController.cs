using Core;
using Core.Extensions;
using Core.Web;
using MeetOL.Filters;
using Repository;
using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetOL.Controllers;
using MPUtil.UserMng;

namespace MeetOL.Areas.Admin.Controllers
{
    [LoginFilter]
    public class BaseAdminController : BaseController
    {

    }
}