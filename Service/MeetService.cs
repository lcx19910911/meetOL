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
    /// 会议
    /// </summary>
    public class MeetService : BaseService, IMeetService
    {
        public MeetService()
        {
            base.ContextCurrent = HttpContext.Current;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">标题 - 搜索项</param>
        /// <param name="createdTimeStart">发布日期起 - 搜索项</param>
        /// <param name="createdTimeEnd">发布日期止 - 搜索项</param>
        /// <returns></returns>
        public PageList<Meet> GetPageList(int pageIndex, int pageSize, string title, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.Meet.Where(x => !x.IsDelete);
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(title));
                }
                var currentUser = GetCurrentUser();
                switch (currentUser.Type)
                {
                    case UserType.User:
                        query = query.Where(x => 1 == 2);
                        break;
                    case UserType.Admin:
                        query = query.Where(x => x.CreateUserId.Equals(currentUser.ID));
                        break;
                    case UserType.SuperAdmin:
                        break;
                }
                if (createdTimeStart != null)
                {
                    query = query.Where(x => x.OngoingTime >= createdTimeStart);
                }
                if (createdTimeEnd != null)
                {
                    createdTimeEnd = createdTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.OverTime < createdTimeEnd);
                }
                query = query.OrderByDescending(x => x.CreatedTime);

                var pageList = CreatePageList(query, pageIndex, pageSize);

                pageList.List.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.IsJoinAuditStr = x.IsJoinAudit.GetDescription();
                        x.IsAutoAllotStr = x.IsAutoAllot.GetDescription();
                        x.IsChangeQrcodeStr = x.IsChangeQrcode.GetDescription();
                    }
                });

                return pageList;

            }
        }


        /// <summary>
        /// 获取用户所有的会议
        /// </summary>
        /// <returns></returns>
        public List<Meet> GetList(Expression<Func<Meet, bool>> predicate)
        {
            return GetList<Meet>(predicate).ToList();
        }

        /// <summary>
        /// 获取用户所有的会议
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<Meet, bool>> predicate)
        {
            return IsExits<Meet>(predicate);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add(MeetModel model)
        {
            if (model.MeetPlans == null || model.MeetPlans.Count == 0)
            {
                return Result(false, ErrorCode.meetplan_empty);
            }
            if (model.Meet.OverTime < model.Meet.OngoingTime || model.Meet.OverTime < DateTime.Now)
                return Result(false, ErrorCode.meet_end_time_error);
            model.Meet.ID = Guid.NewGuid().ToString("N");
            model.Meet.CreateUserId = Client.LoginUser.ID;
            Add<Meet>(model.Meet);
            model.MeetPlans.ForEach(x =>
            {
                x.MeetID = model.Meet.ID;
                x.ID = Guid.NewGuid().ToString("N");
                Add<MeetPlan>(x);
                x.MeetTopics.ForEach(y =>
                {
                    y.MeetID = model.Meet.ID;
                    y.SpeakerID = x.SpeakerID;
                    y.PlanID = x.ID;
                    Add<MeetTopic>(y);
                });
            });

            CacheHelper.Clear();
            return Result(true);
        }


        public string GetLastMeetID()
        {
            return GetList<Meet>(x => !x.IsDelete).OrderByDescending(x => x.OngoingTime).FirstOrDefault()?.ID;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(MeetModel model)
        {
            if (model.MeetPlans == null || model.MeetPlans.Count == 0)
            {
                return Result(false, ErrorCode.meetplan_empty);
            }
            if (model.Meet.OverTime < model.Meet.OngoingTime || model.Meet.OverTime < DateTime.Now)
                return Result(false, ErrorCode.meet_end_time_error);
            var entity = Find(model.ID);
            if (entity == null)
                return Result(false, ErrorCode.sys_param_format_error);
            if (entity.Meet.CreateUserId != Client.LoginUser.ID)
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            model.Meet.CreateUserId = entity.Meet.CreateUserId;
            if (model.MeetPlans == null || model.MeetPlans.Count == 0)
            {
                return Result(false, ErrorCode.meetplan_empty);
            }
            Update<Meet>(model.ID, model.Meet);

            var planIdList = model.MeetPlans.Select(x => x.ID).ToList();
            var topicIdList = new List<string>();
            model.MeetPlans.ForEach(x =>
            {
                x.MeetID = model.Meet.ID;
                if (x.ID.IsNullOrEmpty())
                {
                    x.ID = Guid.NewGuid().ToString("N");
                    Add<MeetPlan>(x);
                    if (x.MeetTopics != null && x.MeetTopics.Count > 0)
                    {
                        x.MeetTopics.ForEach(y =>
                    {
                        y.MeetID = model.Meet.ID;
                        y.SpeakerID = x.SpeakerID;
                        y.PlanID = x.ID;
                        Add<MeetTopic>(y);
                    });
                    }
                }
                else
                {
                    Update<MeetPlan>(x.ID, x);
                    if (x.MeetTopics != null && x.MeetTopics.Count > 0)
                    {
                        x.MeetTopics.ForEach(y =>
                        {
                            y.MeetID = model.Meet.ID;
                            y.PlanID = x.ID;
                            if (y.ID.IsNullOrEmpty())
                            {
                                y.ID = Guid.NewGuid().ToString("N");
                                y.SpeakerID = x.SpeakerID;
                                Add<MeetTopic>(y);
                            }
                            else
                            {
                                Update<MeetTopic>(y.ID, y);
                            }
                            topicIdList.Add(y.ID);
                        });
                    }
                }
            });

            if (entity.MeetPlans != null && entity.MeetPlans.Count > 0)
            {
                var deletePlanIdList = entity.MeetPlans.Where(x => !planIdList.Contains(x.ID)).Select(x => x.ID).ToList();
                if (deletePlanIdList != null && deletePlanIdList.Count > 0)
                    Delete<MeetPlan>(string.Join(",", deletePlanIdList));
            }
            if (entity.MeetTopics != null && entity.MeetTopics.Count > 0)
            {
                var deleteTopicIdList = entity.MeetTopics.Where(x => !topicIdList.Contains(x.ID)).Select(x => x.ID).ToList();
                if (deleteTopicIdList != null && deleteTopicIdList.Count > 0)
                    Delete<MeetTopic>(string.Join(",", deleteTopicIdList));
            }
            CacheHelper.Clear();
            return Result(true);
        }


        string projectKey = CacheHelper.RenderKey("Cache", "Project");
        private List<MeetModel> GetCacheList()
        {
            return CacheHelper.Get<List<MeetModel>>(projectKey, CacheTimeOption.HalfDay, () =>
            {
                List<MeetModel> cacheModel = new List<MeetModel>();
                using (var db = new DbRepository())
                {

                    var list = db.Meet.Where(x => !x.IsDelete).OrderByDescending(x => x.CreatedTime).ToList();
                    var dic_Plan = db.MeetPlan.Where(x => !x.IsDelete).GroupBy(x => x.MeetID).ToDictionary(x => x.Key, x => x.OrderBy(y=>y.StratTime).ToList());
                    var dic_Speaker = db.Speaker.Where(x => !x.IsDelete).ToDictionary(x => x.ID);
                    var dic_Topic = db.MeetTopic.Where(x => !x.IsDelete).GroupBy(x => x.MeetID).ToDictionary(x => x.Key, x => x.OrderBy(y => y.CreatedTime).ToList());
                    var dic_Room = db.Room.Where(x => !x.IsDelete).ToDictionary(x => x.ID);
                    var dic_MeetJoin = db.MeetUserJoin.Where(x => !x.IsDelete).GroupBy(x => x.MeetID).ToDictionary(x => x.Key, x => x.OrderBy(y => y.CreatedTime).ToList());
                    list.ForEach(x =>
                    {
                        var model = new MeetModel()
                        {
                            ID = x.ID,
                            Meet = x,
                            Rooms = dic_Room.Values.Where(y => x.RoomIDs.Contains(y.ID)).ToList()
                        };

                        if (dic_Plan.ContainsKey(x.ID))
                        {
                            model.MeetPlans = dic_Plan[x.ID];
                            if (dic_Topic.ContainsKey(x.ID))
                                model.MeetTopics = dic_Topic[x.ID];

                            if (model.MeetPlans != null && model.MeetPlans.Count > 0&& model.MeetTopics!=null&& model.MeetTopics.Count>0)
                            {
                                model.MeetPlans.ForEach(y =>
                                {
                                    if (y.SpeakerID.IsNotNullOrEmpty() && dic_Speaker.ContainsKey(y.SpeakerID))
                                    {
                                        y.Speaker = dic_Speaker[y.SpeakerID];
                                    }
                                    if (dic_Topic.ContainsKey(x.ID))
                                    {
                                        y.MeetTopics = model.MeetTopics.Where(z => z.PlanID.Equals(y.ID)).ToList();
                                    }
                                });
                            }
                            var spearkIdList = model.MeetPlans.Select(y => y.SpeakerID).ToList();
                            model.Speakers = dic_Speaker.Values.Where(y => spearkIdList.Contains(y.ID)).ToList();
                        }

                        if (dic_MeetJoin.ContainsKey(x.ID))
                            model.MeetUserJoins = dic_MeetJoin[x.ID];

                        cacheModel.Add(model);
                    });
                }

                return cacheModel;
            });
        }

        public MeetModel Find(string id)
        {
            var model = GetCacheList().Find(x => x.ID.Equals(id));
            if (model == null)
                return null;
            var aryModel = model;
            if (aryModel.MeetPlans != null && aryModel.MeetPlans.Count > 0)
            {
                if (model.Rooms != null && model.Rooms.Count > 0)
                {
                    model.Rooms.ForEach(x =>
                    {
                        x.MeetPlans = aryModel.MeetPlans.Where(y => y.RoomID.Equals(x.ID)).OrderBy(y => y.StratTime).ToList();
                    });
                }
            }
            return aryModel;
        }



        
        /// <summary>
        /// 获得会议统计数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MeetReportModel GetReport(string id)
        {
            MeetReportModel model = new MeetReportModel();
            if (id.IsNullOrEmpty())
                return model;
            var entity = Find(id);
            if (entity == null)
                return model;
            if (entity.MeetUserJoins != null && entity.MeetUserJoins.Count > 0)
            {
                model.JoinCount = entity.MeetUserJoins.Count();
                model.SignCount = entity.MeetUserJoins.Where(x => x.HadSign == YesOrNoCode.Yes).Count();
            }
            if (entity.MeetPlans != null && entity.MeetPlans.Count > 0)
            {
                model.PlanList = entity.MeetPlans;
                model.PlanCount = entity.MeetPlans.Count();
            }
            if (entity.MeetTopics != null && entity.MeetTopics.Count > 0)
            {
                model.TopicCount = entity.MeetTopics.Count();
                var topicIdList = entity.MeetTopics.Select(x => x.ID).ToList();
                model.TopicJoinCount = GetCount<TopicUserJoin>(x => topicIdList.Contains(x.MeetTopicID) && x.State == UserJoinState.Pass);
                model.VoteCount = GetCount<PlanVoteUserJoin>(x => x.MeetID == id);
            }
            return model;
        }

        /// <summary>
        /// 获取该用户报名通过的会议集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MeetModel> GetListByUserId(string userId,YesOrNoCode? isSign)
        {
            if (userId.IsNullOrEmpty())
                return new List<MeetModel>();
            var meetList = GetCacheList() ;
            var returnList = new List<MeetModel>();
            if (meetList != null && meetList.Count > 0)
            {
                meetList.ForEach(x =>
                {
                    if (x.MeetUserJoins != null && x.MeetUserJoins.Count > 0)
                    {
                        x.UserJoin = x.MeetUserJoins.Where(y => y.UserID.Equals(userId) && y.State == UserJoinState.Pass).FirstOrDefault();
                        if (x.UserJoin != null)
                        {
                            if (isSign == null)
                                returnList.Add(x);
                            else
                            {
                                if (x.UserJoin.HadSign == isSign)
                                    returnList.Add(x);
                            }
                        }
                    }
                });
            }
            return returnList;
        }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        /// <param name="shortKey"></param>
        /// <returns></returns>
        public List<MeetTopic> GetListByMeetId(string id)
        {
            if (id.IsNullOrEmpty())
                return new List<MeetTopic>();
            var model = Find(id);
            if (model == null)
                return new List<MeetTopic>();
            var list = new List<MeetTopic>();
            if (model.MeetPlans == null)
                return new List<MeetTopic>();
            model.MeetPlans.ForEach(x =>
            {
                var topic = model.MeetPlans.Select(y => y.MeetTopics).FirstOrDefault();
                if (topic != null)
                {
                    topic.ForEach(y =>
                    {
                        y.Speaker = x.Speaker;
                    });
                    list.AddRange(topic);
                }
            });
            return list;
        }

        /// <summary>
        /// 删除会议
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            if (IDs.IsNullOrEmpty())
                return Result(false, ErrorCode.sys_param_format_error);
            Delete<Meet>(IDs);

            foreach (var item in IDs.Split(','))
            {
                var model = Find(item);
                if (model != null)
                {
                    Delete<MeetTopic>(x => x.MeetID == item);
                    Delete<TopicUserJoin>(x => x.MeetID == item);
                    Delete<MeetPlan>(x => x.MeetID == item);
                    Delete<PlanVoteUserJoin>(x => x.MeetID == item);
                }
            }

            CacheHelper.Clear();
            return Result(true);
        }


        /// <summary>
        ///导出 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<bool> ExportInto(List<ExportModel> list, string meetId
            )
        {
            if (list == null || meetId.IsNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository entities = new DbRepository())
            {
                string msg = string.Empty;
                list.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        var model = Find<User>(y => y.MobilePhone == x.Mobile);
                        if (model == null)
                        {
                            model = new User()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                Compnay = x.Company,
                                MobilePhone = x.Mobile,
                                Position = x.Position,
                                NickName = x.Name,
                                HeadImgUrl= "/Images/avtar.png",
                                RealName = x.Name
                            };
                            Add<User>(model);
                            Add<MeetUserJoin>(new MeetUserJoin()
                            {
                                MeetID = meetId,
                                UserID = model.ID,
                                State = UserJoinState.Pass,
                                AuditTime = DateTime.Now,
                            });
                        }
                        else
                        {
                            model.NickName = x.Name;
                            model.RealName = x.Name;
                            model.Compnay = x.Company;
                            model.Position = x.Position;
                            Update<User>(model.ID, model);
                            if (!IsExits<MeetUserJoin>(y => y.UserID == model.ID && y.MeetID == meetId))
                            {

                                Add<MeetUserJoin>(new MeetUserJoin()
                                {
                                    MeetID = meetId,
                                    UserID = model.ID,
                                    State = UserJoinState.Pass,
                                    AuditTime = DateTime.Now,
                                });
                            }
                            else
                            {
                                var join = Find<MeetUserJoin>(y => y.UserID == model.ID && y.MeetID == meetId);
                                join.State = UserJoinState.Pass;
                                Update<MeetUserJoin>(join.ID,join);
                            }
                        }
                    }
                });

                if (msg.IsNotNullOrEmpty())
                {
                    return Result(false, msg);
                }
                else
                {
                    return Result(true);
                }
            }
        }


        #region 报名


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> AddUserJoin(string meetId, string userId)
        {
            if (IsExits<MeetUserJoin>(x => x.MeetID == meetId && x.UserID == Client.LoginUser.ID))
                return Result(false, ErrorCode.meet_already_join);
            var meetModel = Find(meetId);
            if (meetModel == null)
                return Result(false, ErrorCode.sys_param_format_error);
            if (GetCount<MeetUserJoin>(x => !x.IsDelete && x.MeetID == meetId) > meetModel.Meet.MaxLimit)
            {
                return Result(false, ErrorCode.meet_join_max);
            }

            var model = new MeetUserJoin()
            {
                MeetID = meetId,
                UserID = Client.LoginUser.ID,
                State = meetModel.Meet.IsJoinAudit == YesOrNoCode.Yes ? UserJoinState.WaitAudit : UserJoinState.Pass,
            };
            Add<MeetUserJoin>(model);
            CacheHelper.Clear();
            return Result(true);
        }

        #endregion
    }
}

