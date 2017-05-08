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
    public class PlanVoteUserJoinService : BaseService, IPlanVoteUserJoinService
    {
        public IMeetService IMeetService { get; set; }
        public PlanVoteUserJoinService()
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
        public PageList<PlanVoteUserJoin> GetPageList(int pageIndex, int pageSize, string name,string planId)
        {

            using (DbRepository db = new DbRepository())
            {
                var query = db.TopicVoteUserJoin.Where(x=>!x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    var selectUserIdList = db.User.Where(x => !x.IsDelete && x.RealName.Contains(name)).Select(x => x.ID).ToList();
                    query = query.Where(x => selectUserIdList.Contains(x.UserID));
                }
                if (planId.IsNotNullOrEmpty())
                { 
                    query = query.Where(x => x.PlanID.Equals(planId));
                }

                query = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        
                var pageResult=CreatePageList(query, pageIndex, pageSize);
                var userIdList = pageResult.List.Select(x => x.UserID).ToList();
                var userDic = db.User.Where(x => !x.IsDelete && userIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                var planIdList = pageResult.List.Select(x => x.PlanID).ToList();
                var planDic = db.MeetPlan.Where(x => !x.IsDelete && planIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                var speakerIdList = pageResult.List.Select(x => x.SpeakerID).ToList();
                var speakerDic = db.Speaker.Where(x => !x.IsDelete && speakerIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                var meetIdList = pageResult.List.Select(x => x.MeetID).ToList();
                var meetDic = db.Meet.Where(x => !x.IsDelete && meetIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                pageResult.List.ForEach(x =>
                {
                    if (x.MeetID.IsNotNullOrEmpty() && meetDic.ContainsKey(x.MeetID))
                    {
                        x.Meet = meetDic[x.MeetID];
                    }
                    if (x.UserID.IsNotNullOrEmpty()&&userDic.ContainsKey(x.UserID))
                    {
                        x.User = userDic[x.UserID];
                    }
                    if (x.PlanID.IsNotNullOrEmpty() && planDic.ContainsKey(x.PlanID))
                    {
                        x.MeetPlan = planDic[x.PlanID];
                    }
                    if (x.SpeakerID.IsNotNullOrEmpty() && speakerDic.ContainsKey(x.SpeakerID))
                    {
                        x.Speaker = speakerDic[x.SpeakerID];
                    }
                });

                return pageResult;
            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<PlanVoteUserJoin> GetList(Expression<Func<PlanVoteUserJoin, bool>> predicate)
        {
            return GetList<PlanVoteUserJoin>(predicate).ToList();
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<PlanVoteUserJoin, bool>> predicate)
        {
            return IsExits<PlanVoteUserJoin>(predicate);
        }



        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(PlanVoteUserJoin model)
        {
            Update<PlanVoteUserJoin>(model.ID, model);
            return Result(true);

        }

        public PlanVoteUserJoin Find(string id)
        {
            var model=Find<PlanVoteUserJoin>(x=>x.ID.Equals(id));
            if (model != null)
                model.User = Find<User>(x=>x.ID.Equals(model.UserID));
            return model;
        }

        /// <summary>
        /// 获取投票
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public bool CanVote(string planId,string userId)
        {
            return !IsExits(x =>!x.IsDelete&& x.PlanID.Equals(planId)&&x.UserID.Equals(userId));
        }
        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            Delete<PlanVoteUserJoin>(IDs);
            return Result(true);
        }

        /// <summary>
        /// 参与投票
        /// </summary>
        /// <returns></returns>
        public WebResult<bool> Add(string userId,string planId)
        {
            if (userId.IsNullOrEmpty()|| planId.IsNullOrEmpty())
                return Result(false, ErrorCode.sys_param_format_error);

            var plan = Find<MeetPlan>(x=>x.ID.Equals(planId));
            if(plan==null)
                return Result(false, ErrorCode.sys_param_format_error);

            if (IsExits(x => x.UserID.Equals(userId) && x.PlanID.Equals(planId)))
                return Result(false, ErrorCode.meet_had_vote);
            Add(new PlanVoteUserJoin() {
                UserID=userId,
                MeetID= plan.MeetID,
                SpeakerID= plan.SpeakerID,
                PlanID=planId
            });
            plan.VoteCount++;
            Update<MeetPlan>(planId, plan);
            return Result(true);
        }
    }
}

