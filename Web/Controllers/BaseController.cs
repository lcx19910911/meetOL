
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetOL.Filters;
using Core.Model;
using Core.Code;
using Core.Extensions;
using Domain;
using Model;
using Core;

namespace MeetOL.Controllers
{
    [Timer]
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        //
        // GET: /Web/
        public ActionResult JError()
        {
            return JResult(false);
        }

        public ActionResult _404()
        {
            return View();
        }

        public ActionResult OAuthExpired()
        {
            return View("OAuthExpired");
        }

        

        public ActionResult _505()
        {
            return View();
        }

        /// <summary>
        /// 返回部分视图的错误页
        /// </summary>
        /// <returns></returns>
        public PartialViewResult PartialError()
        {
            return null;
        }

        #region Json返回

        /// <summary>
        /// 返回异常编号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected internal JsonResult JResult(ErrorCode code)
        {
            return Json(new
            {
                Code = code,
                ErrorDesc = code.GetDescription()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回异常编号附带自定义消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="appendMsg"></param>
        /// <returns></returns>
        protected internal JsonResult JResult(ErrorCode code, string appendMsg)
        {
            return Json(new
            {
                Code = code,
                ErrorDesc = code.GetDescription(),
                Append = appendMsg
            }, JsonRequestBehavior.AllowGet);
        }


        protected internal JsonResult JResult<T>(T model)
        {
            return Json(new
            {
                Code = ErrorCode.sys_success,
                Result = model
            }, JsonRequestBehavior.AllowGet);
        }

        protected internal JsonResult JResult<T>(WebResult<T> model)
        {
            if (model.OccurError)
            {
                return JResult(model.Code, model.Append);
            }
            return Json(new
            {
                Code = ErrorCode.sys_success,
                Result = model,
            }, JsonRequestBehavior.AllowGet);
        }



        #endregion

        protected internal ViewResult View<T>(WebResult<T> model)
        {
            if (model.OccurError)
            {
                return View("Error");
            }
            return View(model.Result);
        }

        protected internal ActionResult ReLogin()
        {
            return RedirectToAction("Login", "Home");
        }



        private User _loginUser = null;

        public User LoginUser
        {
            get
            {

                return _loginUser != null ? _loginUser : LoginHelper.GetCurrentUser();
            }
            set
            {
                LoginHelper.CreateUser(value);
            }
        }

        private string _meetId = null;

        public string MeetID
        {
            get
            {
                return _meetId.IsNotNullOrEmpty() ? _meetId : LoginHelper.GetCurrentMeetId();
            }
            set
            {
                LoginHelper.CreateMeetId(value);
            }
        }
    }
}