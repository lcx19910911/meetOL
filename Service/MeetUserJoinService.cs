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
using Core.Util;

namespace Service
{
    /// <summary>
    /// 会议厅
    /// </summary>
    public class MeetUserJoinService : BaseService, IMeetUserJoinService
    {
        public IMeetService IMeetService { get; set; }
        public MeetUserJoinService()
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
        public PageList<MeetUserJoin> GetPageList(int pageIndex, int pageSize, string name, string meetName, string meetId, UserJoinState? state, YesOrNoCode? isSign, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {

            using (DbRepository db = new DbRepository())
            {
                var query = db.MeetUserJoin.Where(x=>!x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    var selectUserIdList = db.User.Where(x => !x.IsDelete && x.RealName.Contains(name)).Select(x => x.ID).ToList();
                    query = query.Where(x => selectUserIdList.Contains(x.UserID));
                }
                if (meetName.IsNotNullOrEmpty())
                {
                    var selectMeetIdList = db.Meet.Where(x => !x.IsDelete && x.Name.Contains(meetName)).Select(x => x.ID).ToList();
                    query = query.Where(x => selectMeetIdList.Contains(x.MeetID));
                }
                if (meetId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.MeetID.Equals(meetId));
                }
                if (state != null&&(int)state!=-1)
                { 
                        query = query.Where(x => x.State== state);
                }
                if (isSign != null && (int)isSign != -1)
                {
                    query = query.Where(x => x.HadSign == isSign);
                }
                if (createdTimeStart != null)
                {
                    query = query.Where(x => x.CreatedTime >= createdTimeStart);
                }
                if (createdTimeEnd != null)
                {
                    createdTimeEnd = createdTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < createdTimeEnd);
                }

                query = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        
                var pageResult=CreatePageList(query, pageIndex, pageSize);
                var userIdList = pageResult.List.Select(x => x.UserID).ToList();
                var userDic = db.User.Where(x => !x.IsDelete && userIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                var meetIdList = pageResult.List.Select(x => x.MeetID).ToList();
                var meetDic = db.Meet.Where(x => !x.IsDelete && meetIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                pageResult.List.ForEach(x =>
                {
                    if(x.UserID.IsNotNullOrEmpty()&&userDic.ContainsKey(x.UserID))
                    {
                        x.User = userDic[x.UserID];
                    }
                    if (x.MeetID.IsNotNullOrEmpty() && meetDic.ContainsKey(x.MeetID))
                    {
                        x.Meet = meetDic[x.MeetID];
                    }
                    x.StateStr = x.State.GetDescription();
                    x.HadSignStr = x.HadSign.GetDescription();
                });

                return pageResult;
            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<MeetUserJoin> GetList(Expression<Func<MeetUserJoin, bool>> predicate)
        {
            return GetList<MeetUserJoin>(predicate).ToList();
        }

        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<MeetUserJoin> GetListByMeetID(string meetId, YesOrNoCode? hadSign, UserJoinState? state)
        {
            var list=GetList<MeetUserJoin>(x=> (string.IsNullOrEmpty(meetId) ? 1 == 1 : x.MeetID  == meetId)&&(state != null&& (int)state != -1 ? x.State== state : 1==1) && (hadSign != null && (int)hadSign != -1 ? x.HadSign == hadSign : 1 == 1)).ToList();
            if (list != null && list.Count > 0)
            {
                var userIdList = list.Select(x => x.UserID).ToList();
                var userDic = GetList<User>(x => userIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                list.ForEach(x =>
                {
                    if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                    {
                        x.UserName = userDic[x.UserID].RealName;
                        x.Compnay = userDic[x.UserID].Compnay;
                        x.Position = userDic[x.UserID].Position;
                        x.UserPhone = userDic[x.UserID].MobilePhone;
                    }
                    x.HadSignStr = x.HadSign.GetDescription();
                    x.StateStr = x.State.GetDescription();
                });
            }
            return list;
        }

        public MeetUserJoin Find(string meetId, string userId)
        {
            return Find<MeetUserJoin>(x => (string.IsNullOrEmpty(userId) ? 1 == 1 : x.UserID == userId)&& (string.IsNullOrEmpty(meetId) ? 1 == 1 : x.MeetID == meetId));
        }

        public WebResult<bool> Add(string meetId, string userId)
        {
            if (meetId.IsNullOrEmpty()|| userId.IsNullOrEmpty())
                return Result(false, ErrorCode.sys_param_format_error);
            if (IsExits(x => x.MeetID == meetId && x.UserID == userId))
                return Result(false, ErrorCode.meet_already_join);

            var meet = Find<Meet>(x => x.ID.Equals(meetId));
            if (meet == null)
                return Result(false, ErrorCode.sys_param_format_error);

            var hadCount = GetCount<MeetUserJoin>(x => !x.IsDelete && x.MeetID.Equals(meetId));
            if (hadCount > meet.MaxLimit&&meet.MaxLimit!=0)
            {
                return Result(false, ErrorCode.meet_join_max);
            }


            Add(new MeetUserJoin()
            {
                MeetID = meetId,
                UserID = userId,
                State= meet.IsJoinAudit==YesOrNoCode.No?UserJoinState.Pass:UserJoinState.WaitAudit
            });
            CacheHelper.Clear();
            return Result(true);
        }

        /// <summary>
        /// 人数是否超过限制
        /// </summary>
        /// <param name="meetId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMax(string meetId,string userId)
        {
            if (meetId.IsNullOrEmpty()||userId.IsNullOrEmpty())
                return true;
            if (!IsExits(x => x.MeetID == meetId && x.UserID == userId))
            {
                var meet = Find<Meet>(x => x.ID.Equals(meetId));
                if (meet == null)
                    return true;
                var hadCount = GetCount<MeetUserJoin>(x => !x.IsDelete && x.MeetID.Equals(meetId));
                if (hadCount > meet.MaxLimit&&meet.MaxLimit!=0)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 人数是否超过限制
        /// </summary>
        /// <param name="meetId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsHadJoin(string meetId, string userId)
        {
            if (meetId.IsNullOrEmpty() || userId.IsNullOrEmpty())
                return false;
            return IsExits(x => x.MeetID == meetId && x.UserID == userId);
        }
        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<MeetUserJoin, bool>> predicate)
        {
            return IsExits<MeetUserJoin>(predicate);
        }



        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(MeetUserJoin model)
        {
            if (IsExits(x => x.MeetID == model.MeetID && x.UserID == Client.LoginUser.ID&&x.ID!=model.ID))
                return Result(false, ErrorCode.meet_already_join);
            Update<MeetUserJoin>(model.ID, model);
            CacheHelper.Clear();
            return Result(true);

        }

        public MeetUserJoin Find(string id)
        {
            var model=Find<MeetUserJoin>(x=>x.ID==id);
            if (model != null)
                model.User = Find<User>(x=>x.ID.Equals(model.UserID));
            return model;
        }

        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            Delete<MeetUserJoin>(IDs);
            CacheHelper.Clear();
            return Result(true);
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        /// <param name="isPass"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebResult<bool> Audit(UserJoinState state, string id)
        {
            var model = Find(id);
            if (model == null)
                return Result(false, ErrorCode.sys_param_format_error);
            if (model.AuditTime!=null)
                return Result(false, ErrorCode.join_had_audit);
            model.State = state;
            model.AuditTime = DateTime.Now;

            Update(id, model);
            CacheHelper.Clear();
            return Result(true);
        }


        /// <summary>
        /// 审核时间
        /// </summary>
        /// <param name="isPass"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebResult<bool> Sign(string id,string userId)
        {
            //LogHelper.WriteInfo("sign:"+ Client.LoginUser.ID+"  meetid:"+id);
            var model = Find<MeetUserJoin>(x=>x.UserID.Equals(userId) &&x.MeetID.Equals(id));
            if (model == null)
                return Result(false, ErrorCode.sys_param_format_error);
            if (model.HadSign == YesOrNoCode.Yes)
                return Result(true);
           
            model.HadSign = YesOrNoCode.Yes;
            model.SignTime = DateTime.Now;
            Update(model.ID, model);
            CacheHelper.Clear();
            return Result(true);
        }
    }
}

