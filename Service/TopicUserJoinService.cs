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
    public class TopicUserJoinService : BaseService, ITopicUserJoinService
    {
        public TopicUserJoinService()
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
        public PageList<TopicUserJoin> GetPageList(int pageIndex, int pageSize, string name, string topicId,UserJoinState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {

            using (DbRepository db = new DbRepository())
            {
                var query = db.TopicUserJoin.Where(x=>!x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    var selectUserIdList = db.User.Where(x => !x.IsDelete && x.RealName.Contains(name)).Select(x => x.ID).ToList();
                    query = query.Where(x => selectUserIdList.Contains(x.UserID));
                }
                if (topicId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.MeetTopicID.Equals(topicId));
                }
                if (state != null)
                {
                    query = query.Where(x => x.State == state);
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
                query = query.OrderByDescending(x => x.CreatedTime);
        
                var pageResult=CreatePageList(query, pageIndex, pageSize);
                var userIdList = pageResult.List.Select(x => x.UserID).ToList();
                var userDic = db.User.Where(x =>userIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                var topicIdList = pageResult.List.Select(x => x.MeetTopicID).ToList();
                var topicDic = db.MeetTopic.Where(x =>  topicIdList.Contains(x.ID)).ToDictionary(x => x.ID);

                var meetIdList = pageResult.List.Select(x => x.MeetID).ToList();
                var meetDic = db.Meet.Where(x => meetIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                pageResult.List.ForEach(x =>
                {
                    if(x.UserID.IsNotNullOrEmpty()&&userDic.ContainsKey(x.UserID))
                    {
                        x.User = userDic[x.UserID];
                    }
                    if (x.MeetTopicID.IsNotNullOrEmpty() && topicDic.ContainsKey(x.MeetTopicID))
                    {
                        x.MeetTopic = topicDic[x.MeetTopicID];
                    }
                    if (x.MeetID.IsNotNullOrEmpty() && meetDic.ContainsKey(x.MeetID))
                    {
                        x.Meet = meetDic[x.MeetID];
                    }
                    x.StateStr = x.State.GetDescription();
                });

                return pageResult;
            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<TopicUserJoin> GetList(Expression<Func<TopicUserJoin, bool>> predicate)
        {
            return GetList<TopicUserJoin>(predicate).ToList();
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<TopicUserJoin, bool>> predicate)
        {
            return IsExits<TopicUserJoin>(predicate);
        }



        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(TopicUserJoin model)
        {
            Update<TopicUserJoin>(model.ID, model);
            CacheHelper.Clear();
            return Result(true);

        }

        public TopicUserJoin Find(string id)
        {
            var model=Find<TopicUserJoin>(x=>x.ID.Equals(id));
            if (model != null)
                model.User = Find<User>(x=>x.ID.Equals(model.UserID));
            return model;
        }


        public List<TopicUserJoin> GetListByTopicId(string topicId)
        {
            if (topicId.IsNullOrEmpty())
                return null;
            var list=GetList(x => !x.IsDelete && x.State == UserJoinState.Pass && x.MeetTopicID == topicId);
            if (list != null&&list.Count>0)
            {
                var userIdList = list.Select(x => x.UserID).ToList();
                var userDic = GetList<User>(x => !x.IsDelete && userIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                list.ForEach(x =>
                {
                    if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                    {
                        x.User = userDic[x.UserID];
                    }
                });
            }

            return list;
        }

        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            Delete<TopicUserJoin>(IDs);
            CacheHelper.Clear();
            return Result(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WebResult<bool> Add(string userId, string topicId, string content)
        {
            if (userId.IsNullOrEmpty()|| topicId.IsNullOrEmpty() || content.IsNullOrEmpty())
                return Result(false, ErrorCode.sys_param_format_error);
            var topic = Find<MeetTopic>(x => x.ID.Equals(topicId));
            if(topic==null)
                return Result(false, ErrorCode.sys_param_format_error);
            Add(new TopicUserJoin() {
                UserID=userId,
                MeetTopicID=topicId,
                Content=content,
                MeetID=topic.MeetID,
                PlanID=topic.PlanID       
            });
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
            if (model.State!= UserJoinState.WaitAudit)
                return Result(false, ErrorCode.join_had_audit);
            model.State = state;
            Update(id, model);
            CacheHelper.Clear();
            return Result(true);
        }
    }
}

