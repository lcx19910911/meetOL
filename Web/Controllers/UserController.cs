using Core.Web;
using IService;
using MeetOL.Filters;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Extensions;

namespace MeetOL.Controllers
{
    [LoginFilter]
    public class UserController : BaseController
    {
        public IUserService IUserService;
        public IMeetUserJoinService IMeetUserJoinService;

        public UserController(IUserService _IUserService, IMeetUserJoinService _IMeetUserJoinService)
        {
            this.IUserService = _IUserService;
            this.IMeetUserJoinService = _IMeetUserJoinService;
        }

    }
}