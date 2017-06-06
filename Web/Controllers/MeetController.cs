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
using System.Web.Security;

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
            ViewBag.IsChangeQrCode = meet.Meet.IsChangeQrcode == YesOrNoCode.Yes ? 1 : 0;
            CacheTimeOption timeCode = meet.Meet.IsChangeQrcode == EnumPro.YesOrNoCode.No ? CacheTimeOption.HalfDay : CacheTimeOption.TenSecond;
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
            string url = "http://" + Params.DomianName + "/Meet/ForSign?info=" + CryptoHelper.AES_Encrypt(code + "," + this.MeetID, "11111111");
            return JResult(url);
        }

        public ActionResult ForSign(string info, string userId)
        {
            if (info.IsNullOrEmpty())
                return _404();
            if (userId.IsNullOrEmpty())
                userId = LoginUser.ID;
            if (userId.IsNullOrEmpty())
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
                return View(IMeetUserJoinService.Sign(list[1], userId).Result);
            }
            else
            {
                return View(false);
            }
        }
        public ActionResult SignResult()
        {
            var meet = IMeetService.Find(this.MeetID);
            if (meet == null)
                return View("Index");
            if (meet.MeetUserJoins != null && meet.MeetUserJoins.Count > 0)
            {
                var usrIdList = meet.MeetUserJoins.Select(x => x.UserID).ToList();
                var userDic = IUserService.GetList(x => usrIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                if (meet.MeetUserJoins != null && meet.MeetUserJoins.Count > 0)
                {
                    meet.MeetUserJoins.ForEach(x =>
                    {
                        if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                        {
                            x.User = userDic[x.UserID];
                        }
                    });

                    return View(meet.MeetUserJoins);
                }
            }
            
                return View(new List<MeetUserJoin>());
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
        public ActionResult MyMeet()
        {
            return View();
        }
        /// <summary>
        /// 我的会议
        /// </summary>
        /// <returns></returns>
        public ActionResult MyList(YesOrNoCode? isSign)
        {
            ViewBag.IsSign = isSign;
            var model = IMeetService.GetListByUserId(this.LoginUser.ID, isSign);
            return View(model);
        }

        public ActionResult ScanQRcode()
        {
            ViewBag.AppId = Params.WeixinAppId;
            string cacheToken = this.GetCacheToken(Params.WeixinAppId, Params.WeixinAppSecret);
            ViewBag.TimeStamp = ConvertDateTimeInt(DateTime.Now).ToString();
            ViewBag.NonceStr = Guid.NewGuid().ToString("N").Substring(0, 10);
            string token = "";
            ViewBag.Signature = this.GetSignature(this.Request.Url.ToString().Replace(":" + this.Request.Url.Port.ToString(), ""), cacheToken, ViewBag.TimeStamp, ViewBag.NonceStr, out token);
            return View();
        }

        public string GetSignature(string url, string token, string timestamp, string nonce, out string str)
        {
            string str3 = this.GetJsApi_ticket(token);
            string str4 = "jsapi_ticket=" + str3;
            string str5 = "noncestr=" + nonce;
            string str6 = "timestamp=" + timestamp;
            string str7 = "url=" + url;
            string[] strArray = new string[] { str4, str5, str6, str7 };
            str = string.Join("&", strArray);
            string str8 = str;
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToLower();
        }


        private string GetCacheToken(string appid, string secret)
        {
            string str = GetToken_Message(appid, secret);
            if ((!string.IsNullOrEmpty(str) && str.Contains("errmsg")) && str.Contains("errcode"))
            {
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
                if (((dictionary != null) && dictionary.ContainsKey("errcode")) && dictionary.ContainsKey("errmsg"))
                {
                    return str;
                }
                return str;
            }
            if (string.IsNullOrEmpty(str))
            {
                str = "";
            }
            return str;
        }


        public string GetJsApi_ticket(string token)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", token);
            string str2 = WebHelper.GetPage(url);
            if (!string.IsNullOrEmpty(str2))
            {
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(str2);
                if ((dictionary != null) && dictionary.ContainsKey("ticket"))
                {
                    return dictionary["ticket"];
                }
            }
            return string.Empty;
        }

        public static string GetToken_Message(string appid, string secret)
        {
            string token = GetToken(appid, secret);
            if (token.Contains("access_token"))
            {
                token = token.DeserializeJson<Token>().access_token;
            }
            return token;
        }

        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(0x7b2, 1, 1));
            TimeSpan span = (TimeSpan)(time - time2);
            return (int)span.TotalSeconds;
        }

        public static string GetToken(string appid, string secret)
        {
            return CacheHelper.Get<string>("weixinToken", CacheTimeOption.TenMinutes, () =>
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
                var str = WebHelper.GetPage(url, null);
                return str;
            });
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
            if (model.MeetPlans == null)
                return RedirectToAction("Index","Home");
            List<MeetTopic> topicList = new List<MeetTopic>();
            var planList = model.MeetPlans.OrderBy(x => x.StratTime).ToList();
            if (planList!=null)
            {
                planList.ForEach(x =>
                {
                    var addRange = x.MeetTopics;
                    if (addRange != null)
                    {
                        addRange.ForEach(y =>
                        {
                            y.Speaker = x.Speaker;
                            y.TopicUserJoins = ITopicUserJoinService.GetListByTopicId(y.ID);
                            y.Room = model.Rooms.Find(z => z.ID == y.RoomID);
                        });
                        topicList.AddRange(addRange);
                    }
                });
            }
            return View(topicList);
        }
        #endregion
    }
}