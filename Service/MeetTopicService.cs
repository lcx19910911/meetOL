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
    public class MeetTopicService : BaseService, IMeetTopicService
    {
        public MeetTopicService()
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
        public PageList<MeetTopic> GetPageList(int pageIndex, int pageSize, string title,string planId, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {

            using (DbRepository db = new DbRepository())
            {
                var query = db.MeetTopic.Where(x=>!x.IsDelete);
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(title));
                }
                if (planId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.PlanID.Equals(planId));
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
                var speakerIdList = pageResult.List.Select(x => x.SpeakerID).ToList();
                var dicSpeaker = db.Speaker.Where(x => !x.IsDelete && speakerIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                var meetIdList = pageResult.List.Select(x => x.MeetID).ToList();
                var dicMeet= db.Meet.Where(x => !x.IsDelete && meetIdList.Contains(x.ID)).ToDictionary(x => x.ID);
                pageResult.List.ForEach(x =>
                {
                    if (x.SpeakerID.IsNotNullOrEmpty() && dicSpeaker.ContainsKey(x.SpeakerID))
                    {
                        x.Speaker = dicSpeaker[x.SpeakerID];
                    }
                    if (x.MeetID.IsNotNullOrEmpty() && dicMeet.ContainsKey(x.MeetID))
                    {
                        x.Meet = dicMeet[x.MeetID];
                    }
                    
                });
                return pageResult;
            }
        }


        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public List<MeetTopic> GetList(Expression<Func<MeetTopic, bool>> predicate)
        {
            return GetList<MeetTopic>(predicate).ToList();
        }

        /// <summary>
        /// 获取用户所有的会议厅
        /// </summary>
        /// <returns></returns>
        public bool IsExits(Expression<Func<MeetTopic, bool>> predicate)
        {
            return IsExits<MeetTopic>(predicate);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add(MeetTopic model)
        {
            if (IsExits(x => x.Name == model.Name))
                return Result(false, ErrorCode.system_name_already_exist);
            Add<MeetTopic>(model);
            CacheHelper.Clear();
            return Result(true);
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update(MeetTopic model)
        {
            if (IsExits(x => x.Name == model.Name&&x.ID!=model.ID))
                return Result(false, ErrorCode.system_name_already_exist);
            Update<MeetTopic>(model.ID, model);
            CacheHelper.Clear();
            return Result(true);
        }


        public MeetTopic Find(string id)
        {
            return Find<MeetTopic>(x=>x.ID==id);
        }

        /// <summary>
        /// 删除会议厅
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public WebResult<bool> Delete(string IDs)
        {
            if (IDs.IsNullOrEmpty())
                return Result(false, ErrorCode.sys_param_format_error);
            Delete<MeetTopic>(IDs);

            foreach (var item in IDs.Split(','))
            {
                var model = Find(item);
                if (model != null)
                {
                    Delete<TopicUserJoin>(x => x.MeetTopicID == item);
                }
            }
            CacheHelper.Clear();
            return Result(true);
        }


        /// <summary>
        /// 搜索关键字
        /// </summary>
        /// <param name="shortKey"></param>
        /// <returns></returns>
        public List<SelectItem> GetSelectList(string meetId,string roomId,string planId,string speakerId)
        {

            List<SelectItem> list = new List<SelectItem>();
            var group = GetList(x => !x.IsDelete && (string.IsNullOrEmpty(meetId)?(1==1):x.MeetID.Equals(meetId)) && (string.IsNullOrEmpty(planId) ? (1 == 1) : x.PlanID.Equals(planId)) && (string.IsNullOrEmpty(roomId) ? (1 == 1) : x.RoomID.Equals(roomId)) && (string.IsNullOrEmpty(speakerId) ? (1 == 1) : (!string.IsNullOrEmpty(x.SpeakerID)&&x.SpeakerID.Equals(speakerId))));
            if (group != null)
            {
                list = group.OrderByDescending(x => x.CreatedTime).Select(
                    x => new SelectItem()
                    {
                        Text = x.Name,
                        Value = x.ID
                    }).ToList();
            }
            return list;
        }


    }
}

