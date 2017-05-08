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
using System.IO;
using System.Collections;

namespace MeetOL.Areas.Admin.Controllers
{
    /// <summary>
    /// 分类控制器
    /// </summary>
    [LoginFilter]
    public class UserJoinController : BaseAdminController
    {
        
        public IMeetUserJoinService IMeetUserJoinService;

        public UserJoinController(IMeetUserJoinService _IMeetUserJoinService)
        {
            this.IMeetUserJoinService = _IMeetUserJoinService;
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
        public JsonResult GetPageList(int pageIndex, int pageSize, string name,string meetName, string meetId, UserJoinState? state,YesOrNoCode? isSign, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            var pagelist = IMeetUserJoinService.GetPageList(pageIndex, pageSize, name, meetName, meetId, state, isSign, createdTimeStart, createdTimeEnd);
            return JResult(pagelist);
        }

        public ActionResult Export(string meetId, UserJoinState? state, YesOrNoCode? isSign)
        {
            var list=IMeetUserJoinService.GetListByMeetID(meetId, isSign, state);
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            //if (!Directory.Exists(filePath))
            //    Directory.CreateDirectory(filePath);

            NPOIHelper<MeetUserJoin>.GetExcel(list, GetUserHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }

        private Hashtable GetUserHT()
        {
            Hashtable hs = new Hashtable();
            hs["UserName"] = "姓名";
            hs["UserPhone"] = "手机";
            hs["Compnay"] = "单位";
            hs["Position"] = "职位";
            hs["CreatedTime"] = "报名时间";
            hs["StateStr"] = "审核状态";
            hs["HadSignStr"] = "是否已签到";
            hs["SignTime"] = "签到时间";
            return hs;
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="model"</param>
        /// <returns></returns>
        public JsonResult Find(string id)
        {
            var result = IMeetUserJoinService.Find(id);
            return JResult(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="unid"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            var model = IMeetUserJoinService.Delete(ids);
            return JResult(model);
        }
        
        public ActionResult Audit(UserJoinState state, string id)
        {
            var model = IMeetUserJoinService.Audit(state, id);
            return JResult(model);
        }
    }
}