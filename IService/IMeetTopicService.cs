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
    public interface IMeetTopicService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <returns></returns>
        PageList<MeetTopic> GetPageList(int pageIndex, int pageSize, string title,string planId, DateTime? createdTimeStart, DateTime? createdTimeEnd);


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        List<MeetTopic> GetList(Expression<Func<MeetTopic, bool>> predicate = null);

        bool IsExits(Expression<Func<MeetTopic, bool>> predicate);

        WebResult<bool> Add(MeetTopic model);


        WebResult<bool> Update(MeetTopic model);


        WebResult<bool> Delete(string ids);


        MeetTopic Find(string id);


        List<SelectItem> GetSelectList(string meetId, string roomId, string planId, string speakerId);
    }
}
