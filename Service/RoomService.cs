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
    /// 会议厅
    /// </summary>
    public class RoomService : BaseService, IRoomService
    {
        public RoomService()
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
        public PageList<Room> GetPageList(int pageIndex, int pageSize, string title)
        {

            using (DbRepository db = new DbRepository())
            {
                var query = db.Room.Where(x=>!x.IsDelete);
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(title));
                }
                query = query.OrderByDescending(x => x.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                return CreatePageList(query, pageIndex, pageSize);
            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<Room> GetList(Expression<Func<Room, bool>> predicate)
        {
            return GetList<Room>(predicate).ToList();
        }

        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<Room, bool>> predicate)
        {
            return IsExits<Room>(predicate);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add(Room model)
        {
            if (IsExits(x => x.Name == model.Name))
                return Result(false, ErrorCode.system_name_already_exist);
            Add<Room>(model);
            CacheHelper.Clear();
            return Result(true);
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(Room model)
        {
            if (IsExits(x => x.Name == model.Name&&x.ID!=model.ID))
                return Result(false, ErrorCode.system_name_already_exist);
            Update<Room>(model.ID, model);
            CacheHelper.Clear();
            return Result(true);

        }

        public Room Find(string id)
        {
            return Find(id);
        }

        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            Delete<Room>(IDs);
            CacheHelper.Clear();
            return Result(true);
        }

        public List<ZTreeNode> GetZTreeChildren()
        {

            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = GetList(x=>!x.IsDelete);
            if (group != null)
            {
                ztreeNodes = group.OrderByDescending(x=>x.Sort).Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.ID
                    }).ToList();
            }
            return ztreeNodes;
        }

    }
}

