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
    /// 演讲者接口
    /// </summary>
    public interface ISpeakerService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        PageList<Speaker> GetPageList(int pageIndex, int pageSize, string title);


        /// <summary>
        /// 获取集合
        /// </summary>
        /// <returns></returns>
        List<Speaker> GetList(Expression<Func<Speaker, bool>> predicate = null);

        bool IsExits(Expression<Func<Speaker, bool>> predicate);

        WebResult<bool> Add(Speaker model);


        WebResult<bool> Update(Speaker model);


        WebResult<bool> Delete(string ids);


        Speaker Find(string id);


        List<SelectItem> GetSelectList(string shortKey);
    }
}
