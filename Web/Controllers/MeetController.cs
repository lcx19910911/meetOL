using Core.Extensions;
using Core.Web;
using Core;
using IService;
using MeetOL.Filters;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Domain;
using Core.Helper;
using EnumPro;

namespace MeetOL.Controllers
{
    [GetMeetID]
    [LoginFilter]
    public class MeetController : BaseController
    {
        public IMeetUserJoinService IMeetUserJoinService;
        public ITopicUserJoinService ITopicUserJoinService;
        public IPlanVoteUserJoinService IPlanVoteUserJoinService;
        public IMeetService IMeetService;
        public IUserService IUserService;

        public MeetController(IMeetUserJoinService _IMeetUserJoinService, IMeetService _IMeetService, ITopicUserJoinService _ITopicUserJoinService, IPlanVoteUserJoinService _IPlanVoteUserJoinService, IUserService _IUserService)
        {
            this.IMeetUserJoinService = _IMeetUserJoinService;
            this.IMeetService = _IMeetService;
            this.ITopicUserJoinService = _ITopicUserJoinService;
            this.IPlanVoteUserJoinService = _IPlanVoteUserJoinService;
            this.IUserService = _IUserService;
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult Index()
        {
            var model = IMeetService.Find(this.MeetID);
            return View(model);
        }

        public ActionResult Sign()
        {
            if (LoginUser.Type == UserType.User)
                return View("Index");
            ViewBag.Url = "";
            ViewBag.Sign = false;
            var meet = IMeetService.Find(this.MeetID);
            if (meet == null)
                return View("Index");
            ViewBag.IsChangeQrCode = meet.Meet.IsChangeQrcode==YesOrNoCode.Yes?1:0;
            CacheTimeOption timeCode = meet.Meet.IsChangeQrcode == EnumPro.YesOrNoCode.Yes ? CacheTimeOption.HalfDay : CacheTimeOption.TenSecond;
            var code = CacheHelper.Get<string>("sign_code", timeCode, () =>
            {
                return Guid.NewGuid().ToString("N").SubString(6); ;
            });
            ViewBag.Url = "http://" + Params.DomianName + "/Meet/ForSign?info=" + CryptoHelper.AES_Encrypt(code + "," + this.MeetID, "11111111");
            return View();
        }

        public ActionResult GetSignUrl()
        {
            if (LoginUser.Type == UserType.User)
                return View("Index");
            var code = CacheHelper.Get<string>("sign_code", CacheTimeOption.TenSecond, () =>
            {
                return Guid.NewGuid().ToString("N").SubString(6);
            });
            string url= "http://" + Params.DomianName + "/Meet/ForSign?info="+ CryptoHelper.AES_Encrypt(code+","+this.MeetID,"11111111");
            return JResult(url);
        }

        public ActionResult ForSign(string info)
        {
            if (info.IsNullOrEmpty())
                return _404();
            var valiteCode = CacheHelper.Get<string>("sign_code");
            var ary = CryptoHelper.AES_Decrypt(info, "11111111");
            if(ary.IsNullOrEmpty())
                return _404();
            var list = ary.Split(',');
            if (list.Length!=2)
                return _404();
            if (valiteCode.IsNotNullOrEmpty() && valiteCode == list[0])
            {
                return View(IMeetUserJoinService.Sign(list[1]).Result);
            }
            else
            {
                return View(false);
            }
        }

        /// <summary>
        /// 增加评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [JoinFilter]
        public ActionResult AddTopicJoin(string topicId, string content)
        {
            return JResult(ITopicUserJoinService.Add(LoginUser.ID, topicId, content));
        }

        /// <summary>
        /// 增加评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult Vote()
        {
            var model = IMeetService.Find(this.MeetID);
            if (model.MeetPlans == null)
                return View("Index");
            var list = model.MeetPlans.OrderByDescending(x => x.VoteCount).ToList();
            list.ForEach(x =>
            {
                x.CanVote = IPlanVoteUserJoinService.CanVote(x.ID, LoginUser.ID);
            });
            return View(list);
        }

        /// <summary>
        /// 增加评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult AddPlanJoin(string planId)
        {
            return JResult(IPlanVoteUserJoinService.Add(LoginUser.ID, planId));
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult GetTopicPageList(int pageIndex, int pageSize,string topicId)
        {
            return JResult(ITopicUserJoinService.GetPageList(pageIndex, pageSize,"", topicId,null,null,null));
        }


        /// <summary>
        /// 我的会议
        /// </summary>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult MyMeet()
        {
            return View();
        }
        /// <summary>
        /// 我的会议
        /// </summary>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult MyList(YesOrNoCode? type)
        {
            ViewBag.Type = type;
            var model = IMeetService.GetListByUserId(this.LoginUser.ID);
            return View(model);
        }
        

        #region 页面

        /// <summary>
        /// 座位图
        /// </summary>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult SeatImg()
        {
            var model = IMeetService.Find(this.MeetID);
            if (model != null && model.Meet.PlaceImage.IsNotNullOrEmpty())
            {
                ViewBag.Path = model.Meet.PlaceImage;
            }
            else
            {
                ViewBag.Path = "";
            }
            return View();
        }


        /// <summary>
        /// 下载页面
        /// </summary>
        /// <returns></returns>
        [JoinFilter]
        public ActionResult Download()
        {
            var model = IMeetService.Find(this.MeetID);
            if (model != null&& model.Meet.DownLoadInfos.IsNotNullOrEmpty())
            {
                return View(model.Meet.DownLoadInfos.DeserializeJson<List<DownLoadModel>>());            
            }
            return View(new List<DownLoadModel>());
        }

        
        public ActionResult Join()
        {
            ViewBag.IsMax = false;
            if(IMeetUserJoinService.IsMax(this.MeetID,LoginUser.ID))
            {
                ViewBag.IsMax = true;
                return View(new MeetUserJoin());
            }
            var model = IMeetUserJoinService.Find(this.MeetID, LoginUser.ID);
            if (LoginUser.MobilePhone.IsNullOrEmpty())
                ViewBag.IsBind = false;
            else
                ViewBag.IsBind = true;
            return View(model);
        }
        [HttpPost]
        public ActionResult doApply()
        {
            return JResult(IMeetUserJoinService.Add(this.MeetID, LoginUser.ID));
        }

        [HttpPost]
        public ActionResult doBindApply(string phone, string position, string realName, string compnay)
        {
            var model = IMeetUserJoinService.Find(this.MeetID, LoginUser.ID);
            if (model == null)
            {
                IMeetUserJoinService.Add(this.MeetID, LoginUser.ID);
            }
            return JResult(IUserService.Bind(phone, position, realName, compnay, LoginUser.ID));
        }

        [JoinFilter]
        public ActionResult FlowList()
        {
            var model = IMeetService.Find(this.MeetID);
            ViewBag.DateList = model.MeetPlans.OrderBy(x=>x.StratTime).Select(x => x.StratTime.Date).Distinct().ToList();
            ViewBag.DateDic = model.MeetPlans.GroupBy(x => x.StratTime.Date).ToDictionary(x=>x.Key,x=>x.Select(y=>y.RoomID).ToList());
            return View(model);
        }
        [JoinFilter]
        public ActionResult RoomPlans(string roomId,DateTime day)
        {
            var model = IMeetService.Find(this.MeetID);
            if (model.MeetPlans == null)
                return View("Index");

            var selectPlans = model.MeetPlans.Where(x => x.RoomID == roomId && x.StratTime.Date == day.Date).OrderBy(x=>x.StratTime).ToList();
            return View(selectPlans);
        }

        [JoinFilter]
        public ActionResult TopicList()
        {
            var model = IMeetService.Find(this.MeetID);
            if (model.MeetTopics == null|| model.MeetPlans == null)
                return View("Index");
            List<MeetTopic> topicList = new List<MeetTopic>();
            var planList = model.MeetPlans.Where(x => x.StratTime.Date >= DateTime.Now.Date).OrderBy(x => x.StratTime).ToList();
            if (planList!=null)
            {
                planList.ForEach(x =>
                {
                    var addRange = x.MeetTopics;
                    addRange.ForEach(y =>
                    {
                        y.MeetPlan = x;
                        y.Speaker = x.Speaker;
                        y.TopicUserJoins = ITopicUserJoinService.GetListByTopicId(y.ID);
                        y.Room = model.Rooms.Find(z => z.ID == y.RoomID);
                    });
                    topicList.AddRange(addRange);

                });
            }
            return View(topicList);
        }
        #endregion
    }
}