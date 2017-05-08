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
    public interface IMeetPlanService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        PageList<MeetPlan> GetPageList(int pageIndex, int pageSize, string name,string meetName,string id, DateTime? createdTimeStart, DateTime? createdTimeEnd);


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        List<MeetPlan> GetList(Expression<Func<MeetPlan, bool>> predicate = null);

        bool IsExits(Expression<Func<MeetPlan, bool>> predicate);

        WebResult<bool> Add(MeetPlan model);


        WebResult<bool> Update(MeetPlan model);


        WebResult<bool> Delete(string ids);


        MeetPlan Find(string id);


        List<SelectItem> GetSelectList(string meetId, string roomId, string speakerId);


        WebResult<bool> UpdateCount(string id, int count);
    }
}
