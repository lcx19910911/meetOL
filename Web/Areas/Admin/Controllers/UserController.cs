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
    /// 用户
    /// </summary>
    [LoginFilter]
    public class UserController : BaseAdminController
    {
        public IUserService IUserService;

        public UserController(IUserService _IUserService)
        {
            this.IUserService = _IUserService;
        }
        // GET: 
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
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string name)
        {
            return JResult(IUserService.GetPageList(pageIndex, pageSize, name, UserType.Admin));
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(IUserService.Delete(ids));
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(IUserService.Find(id));
        }

        [HttpPost]
        public ActionResult Add(User model)
        {
            model.Type = UserType.Admin;
            model.Password = "111111";
            var result = IUserService.Add(model);
            return JResult(result);
        }
        [HttpPost]
        public ActionResult Update(User model)
        {
            var result = IUserService.Update(model);
            return JResult(result);
        }

        public ActionResult ChangePassword(string oldPassword, string newPassword, string cfmPassword, string id)
        {
            return JResult(IUserService.ChangePassword(oldPassword, newPassword, cfmPassword, id));
        }
    }
}