using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 会议
    /// </summary>
    public class MeetModel
    {
        public string ID { get; set; }

        public Meet Meet { get; set; }

        [JsonIgnore]
        public List<MeetPlan> MeetPlans { get; set; }

        public List<MeetTopic> MeetTopics { get; set; }

        public List<Speaker> Speakers { get; set; }
        public List<Room> Rooms { get; set; }

        public List<MeetUserJoin> MeetUserJoins { get; set; }

        public List<string> userIdList { get; set; }
        public List<string> signUserIdList { get; set; }

        public MeetUserJoin UserJoin { get; set; }
    }

    public class MeetRoomModel : BaseEntity
    {

        public string Name { get; set; }
        public List<MeetPlanModel> MeetPlans { get; set; }
    }

    public class MeetPlanModel:BaseEntity
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StratTime { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 演讲人
        /// </summary>
        public string SpeakerID { get; set; }


        public string SpeakerName { get; set; }

        public List<MeetTopicModel> MeetTopics { get; set; }
        public int VoteCount { get; set; } 
    }

    /// <summary>
    /// 话题
    /// </summary>
    public partial class MeetTopicModel : BaseEntity
    {
        /// <summary>
        /// 计划名称
        /// </summary>
        public string Name { get; set; }
    }
}
