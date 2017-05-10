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
    /// 会议接口
    /// </summary>
    public interface IMeetService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <param name="createdTimeStart">发布日期起 - 搜索项</param>
        /// <param name="createdTimeEnd">发布日期止 - 搜索项</param>
        /// <returns></returns>
        PageList<Meet> GetPageList(int pageIndex, int pageSize, string title, DateTime? createdTimeStart, DateTime? createdTimeEnd);


        /// <summary>
        /// 获取用户所有的会议
        /// </summary>
        /// <returns></returns>
        List<Meet> GetList(Expression<Func<Meet , bool>> predicate=null);

        bool IsExits(Expression<Func<Meet, bool>> predicate);

        WebResult<bool> Add(MeetModel model);


        WebResult<bool> Update(MeetModel model);


        WebResult<bool> Delete(string ids);


        MeetModel Find(string id);


        string GetLastMeetID();


        MeetReportModel GetReport(string id);


        WebResult<bool> AddUserJoin(string meetId, string userId);

        List<MeetTopic> GetListByMeetId(string id);

        List<MeetModel> GetListByUserId(string userId);

        WebResult<bool> ExportInto(List<ExportModel> list, string meetId);
    }
}
