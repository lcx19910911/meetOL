using Core;
using Core.Model;
using Domain;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IService
{
    /// <summary>
    /// 会议厅接口
    /// </summary>
    public interface IRoomService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        PageList<Room> GetPageList(int pageIndex, int pageSize, string title);


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        List<Room> GetList(Expression<Func<Room, bool>> predicate = null);

        bool IsExits(Expression<Func<Room, bool>> predicate);

        WebResult<bool> Add(Room model);


        WebResult<bool> Update(Room model);


        WebResult<bool> Delete(string ids);


        Room Find(string id);


        List<ZTreeNode> GetZTreeChildren();
    }
}
