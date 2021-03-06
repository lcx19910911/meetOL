﻿using Core.Model;
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
    /// 接口
    /// </summary>
    public interface IPlanVoteUserJoinService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="topicId">标题 - 搜索项</param>
        /// <returns></returns>
        PageList<PlanVoteUserJoin> GetPageList(int pageIndex, int pageSize,string name,string planId);

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        PlanVoteUserJoin Find(string id);

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        bool CanVote(string planId, string userId);

        WebResult<bool> Delete(string ids);

        WebResult<bool> Add(string userId, string planId);
    }
}
