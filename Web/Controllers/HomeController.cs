using Core.Extensions;
using Core.Util;
using Core.Web;
using IService;
using MeetOL.Filters;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeetOL.Controllers
{
    public class HomeController : BaseController
    {
        public IMeetService IMeetService;
        public IMeetUserJoinService IMeetUserJoinService;
        public HomeController(IMeetService _IMeetService, IMeetUserJoinService _IMeetUserJoinService)
        {
            this.IMeetService = _IMeetService;
            this.IMeetUserJoinService = _IMeetUserJoinService;
        }
        [GetMeetID]
        [LoginFilter]
        public ActionResult Index()
        {
            string meetId = "";
            if (this.Request["meetId"].IsNotNullOrEmpty())
            {
                meetId = this.Request["meetId"];
            }
            else
            {
                meetId = this.MeetID;
            }

            if (!IMeetService.IsExits(x => x.ID == meetId && !x.IsDelete))
            {
                meetId = IMeetService.GetLastMeetID();
                this.MeetID = meetId;
            }
            //LogHelper.WriteInfo("UserId-  " + this.LoginUser.ID);
            var model = IMeetService.Find(meetId);
            
            ViewBag.IsHadJoin = IMeetUserJoinService.IsHadJoin(meetId, this.LoginUser.ID);
            return View(model.Meet);
        }


        public void ClearCache()
        {
            CacheHelper.Clear();
        }
    }
}