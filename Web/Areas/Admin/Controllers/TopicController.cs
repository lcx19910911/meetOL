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

namespace MeetOL.Areas.Admin.Controllers
{
    /// <summary>
    /// 分类控制器
    /// </summary>
    [LoginFilter]
    public class TopicController : BaseAdminController
    {
        
        public IMeetTopicService ITopicService;

        public TopicController(IMeetTopicService _ITopicService)
        {
            this.ITopicService = _ITopicService;
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
        public JsonResult GetPageList(int pageIndex, int pageSize, string name,string planId, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            var pagelist = ITopicService.GetPageList(pageIndex, pageSize, name, planId, createdTimeStart, createdTimeEnd);
            return JResult(pagelist);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Add(MeetTopic model)
        {
            var result = ITopicService.Add(model);
            return JResult(result);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Update(MeetTopic model)
        {
            var result = ITopicService.Update(model);
            return JResult(result);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Find(string id)
        {
            var result = ITopicService.Find(id);
            return JResult(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="unid"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            var model = ITopicService.Delete(ids);
            return JResult(model);
        }
    }
}