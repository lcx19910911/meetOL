using Core.Model;
using Domain;
using EnumPro;
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
    public interface IMeetUserJoinService
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
        PageList<MeetUserJoin> GetPageList(int pageIndex, int pageSize, string name, string meetName, string meetId, UserJoinState? state, YesOrNoCode? isSign, DateTime? createdTimeStart, DateTime? createdTimeEnd);

        /// <summary>
        /// 获取会议所有的报名
        /// </summary>
        /// <returns></returns>
        MeetUserJoin Find(string meetId, string userId);


        /// <summary>
        /// 获取会议所有的报名
        /// </summary>
        /// <returns></returns>
        WebResult<bool> Add(string meetId, string userId);

        /// <summary>
        /// 获取会议所有的报名
        /// </summary>
        /// <returns></returns>
        MeetUserJoin Find(string id);

        /// <summary>
        /// 获取会议所有的报名
        /// </summary>
        /// <returns></returns>
        List<MeetUserJoin> GetListByMeetID(string meetId,YesOrNoCode? hadSign, UserJoinState? state);       


        WebResult<bool> Delete(string ids);


        WebResult<bool> Audit(UserJoinState state, string id);

        WebResult<bool> Sign(string id);

        bool IsMax(string meetId,string userId);

        bool IsHadJoin(string meetId, string userId);
    }
}
