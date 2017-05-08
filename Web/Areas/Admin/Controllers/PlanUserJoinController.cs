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
    public class PlanUserJoinController : BaseAdminController
    {
        
        public IPlanVoteUserJoinService IPlanUserJoinService;

        public PlanUserJoinController(IPlanVoteUserJoinService _IPlanUserJoinService)
        {
            this.IPlanUserJoinService = _IPlanUserJoinService;
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
        public JsonResult GetPageList(int pageIndex, int pageSize,string name,string planId)
        {
            var pagelist = IPlanUserJoinService.GetPageList(pageIndex, pageSize, name, planId);
            return JResult(pagelist);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="unid"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            var model = IPlanUserJoinService.Delete(ids);
            return JResult(model);
        }
        
    }
}