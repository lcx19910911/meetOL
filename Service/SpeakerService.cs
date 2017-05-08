using Core.Model;
using Domain;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;
using EnumPro;
using Core.Helper;
using Core.Web;
using IService;
using Extension;
using System.Web;
using System.Threading;
using Model;
using Core;
using System.Data.Entity;
using Service;
using System.Linq.Expressions;
using Core.Code;

namespace Service
{
    /// <summary>
    /// 演讲者
    /// </summary>
    public class SpeakerService : BaseService, ISpeakerService
    {
        public SpeakerService()
        {
            base.ContextCurrent = HttpContext.Current;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        public PageList<Speaker> GetPageList(int pageIndex, int pageSize, string title)
        {

            using (DbRepository db = new DbRepository())
            {
                var query = db.Speaker.Where(x=>!x.IsDelete);
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(title));
                }
                query = query.OrderBy(x => x.ShortKey).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                return CreatePageList(query, pageIndex, pageSize);
            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<Speaker> GetList(Expression<Func<Speaker, bool>> predicate)
        {
            return GetList<Speaker>(predicate).ToList();
        }

        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<Speaker, bool>> predicate)
        {
            return IsExits<Speaker>(predicate);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add(Speaker model)
        {
            //if (IsExits(x => x.Name == model.Name))
            //    return Result(false, ErrorCode.system_name_already_exist);
            Add<Speaker>(model);
            CacheHelper.Clear();
            return Result(true);
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(Speaker model)
        {
            //if (IsExits(x => x.Name == model.Name&&x.ID!=model.ID))
            //    return Result(false, ErrorCode.system_name_already_exist);
            Update<Speaker>(model.ID, model);
            CacheHelper.Clear();
            return Result(true);

        }

        public Speaker Find(string id)
        {
            return Find<Speaker>(x=>x.ID.Equals(id));
        }

        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            Delete<Speaker>(IDs);
            CacheHelper.Clear();
            return Result(true);
        }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        /// <param name="shortKey"></param>
        /// <returns></returns>
        public List<SelectItem> GetSelectList(string shortKey)
        {

            List<SelectItem> list = new List<SelectItem>();
            var group = GetList(x=>!x.IsDelete&&x.ShortKey.Contains(shortKey));
            if (group != null)
            {
                list = group.OrderByDescending(x=>x.ShortKey).Select(
                    x => new SelectItem()
                    {
                        Text = x.Name,
                        Value = x.ID
                    }).ToList();
            }
            return list;
        }

    }
}

