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
    /// 演讲者控制器
    /// </summary>
    [LoginFilter]
    public class SpeakerController : BaseAdminController
    {
        
        public ISpeakerService ISpeakerService;

        public SpeakerController(ISpeakerService _ISpeakerService)
        {
            this.ISpeakerService = _ISpeakerService;
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
        public JsonResult GetPageList(int pageIndex, int pageSize, string name)
        {
            var pagelist = ISpeakerService.GetPageList(pageIndex, pageSize, name);
            return JResult(pagelist);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Add(Speaker model)
        {
            var result = ISpeakerService.Add(model);
            return JResult(result);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Update(Speaker model)
        {
            var result = ISpeakerService.Update(model);
            return JResult(result);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Find(string id)
        {
            var result = ISpeakerService.Find(id);
            return JResult(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="unid"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            var model = ISpeakerService.Delete(ids);
            return JResult(model);
        }

        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetZTreeChildren(string key)
        {
            return JResult(ISpeakerService.GetSelectList(key));
        }
    }
}