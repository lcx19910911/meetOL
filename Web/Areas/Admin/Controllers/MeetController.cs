﻿using Core;
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
using Service;
using Model;
using Domain;
using Core.Helper;
using System.IO;
using System.Collections;

namespace MeetOL.Areas.Admin.Controllers
{
    /// <summary>
    /// 会议
    /// </summary>
    public class MeetController : BaseAdminController
    {
        public IMeetPlanService IMeetPlanService;
        public IMeetService IMeetService;
        public IUserService IUserService;

        public MeetController(IMeetService _IMeetService, IUserService _IUserService, IMeetPlanService _IMeetPlanService)
        {
            this.IMeetService = _IMeetService;
            this.IUserService = _IUserService;
            this.IMeetPlanService = _IMeetPlanService;
        }


        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <param name="createdTimeStart">发布日期起 - 搜索项</param>
        /// <param name="createdTimeEnd">发布日期止 - 搜索项</param>
        /// <returns></returns>
        public JsonResult GetPageList(int pageIndex, int pageSize, string title, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            var pagelist = IMeetService.GetPageList(pageIndex, pageSize, title, createdTimeStart, createdTimeEnd);
            return JResult(pagelist);
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult Add(MeetModel model)
        {
            var result = IMeetService.Add(model);
            return JResult(result);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult Update(MeetModel model)
        {
            var result = IMeetService.Update(model);
            return JResult(result);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Find(string ID,bool isShowRoom,string planId)
        {
            var result = IMeetService.Find(ID);
            if (isShowRoom && result != null)
            {
                return JResult(new MeetModel() {
                    Rooms = result.Rooms,
                    Meet=result.Meet,
                    MeetPlans=null,
                    MeetTopics=null,
                    MeetUserJoins=null,
                    signUserIdList=null,
                    Speakers=null,
                    userIdList=null,
                    UserJoin=null,
                });
            }
            else if(planId.IsNotNullOrEmpty())
            {
                var model = result.MeetPlans.Find(x=>x.ID==planId);
                if (model != null)
                {
                    model.OngoingTime = result.Meet.OngoingTime;
                    model.OverTime = result.Meet.OverTime;
                    if (model.Speaker != null)
                        model.SpeakerName = model.Speaker.Name;
                    model.Speaker = null;

                    return JResult(new MeetPlan()
                    {
                        SpeakerName = model.Speaker.Name,
                        Speaker = null,
                        ID = model.ID,
                        MeetName = result.Meet.Name,
                        MeetTopics = model.MeetTopics,
                        Name = model.Name,
                        OngoingTime = result.Meet.OngoingTime,
                        OverTime = result.Meet.OverTime,
                        StratTime = model.StratTime,
                        VoteCount = model.VoteCount,
                        SpeakerID = model.SpeakerID,
                        RoomID = model.RoomID,
                        MeetID = model.MeetID,
                    });
                }
            }
            return JResult(new MeetPlan());
        }
        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult GetReport(string id)
        {
            var result = IMeetService.GetReport(id);
            return JResult(result);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            var effect = IMeetService.Delete(ids);
            return JResult(effect);
        }
        
        /// <summary>
        /// 修改投票数
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult UpdateCount(string id,int count)
        {
            var effect = IMeetPlanService.UpdateCount(id,count);
            return JResult(effect);
        }


        private Hashtable GetHT()
        {
            Hashtable hs = new Hashtable();
            hs["Name"] = "名称";
            hs["Mobile"] = "手机号";
            hs["Company"] = "公司";
            hs["Position"] = "职位";
            return hs;
        }

        public ActionResult ExportInto(string mark,string meetId)
        {
            HttpPostedFileBase file = Request.Files[0];
            string path = UploadHelper.Save(file, mark);
            string filePath = Path.Combine(Server.MapPath("~/") + path);
            var list = NPOIHelper<ExportModel>.FromExcel(GetHT(), filePath);
            //var msg = ;
            return JResult(IMeetService.ExportInto(list, meetId));
        }


        /// <summary>
        /// 修改投票数
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        //public ActionResult ExportAll(string id)
        //{
        //    var effect = IMeetTopicService.UpdateCount(id, count);
        //    return JResult(effect);
        //}
    }
}