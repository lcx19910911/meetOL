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
using Service;
using Model;
using EnumPro;

namespace MeetOL.Areas.Admin.Controllers
{
    /// <summary>
    /// 分类控制器
    /// </summary>
    [LoginFilter]
    public class TopicUserJoinController : BaseAdminController
    {
        
        public ITopicUserJoinService ITopicUserJoinService;

        public TopicUserJoinController(ITopicUserJoinService _ITopicUserJoinService)
        {
            this.ITopicUserJoinService = _ITopicUserJoinService;
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
        /// <param name="name">名称 - 搜索项</param>
        /// <returns></returns>
        public JsonResult GetPageList(int pageIndex, int pageSize,string name,string topicId, UserJoinState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            var pagelist = ITopicUserJoinService.GetPageList(pageIndex, pageSize, name, topicId, state, createdTimeStart, createdTimeEnd);
            return JResult(pagelist);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="unid"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            var model = ITopicUserJoinService.Delete(ids);
            return JResult(model);
        }
        
        public ActionResult Audit(UserJoinState state, string id)
        {
            var model = ITopicUserJoinService.Audit(state, id);
            return JResult(model);
        }
    }
}