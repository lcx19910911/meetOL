using Model;
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

        public List<MeetPlan> MeetPlans { get; set; }

        public List<MeetTopic> MeetTopics { get; set; }

        public List<Speaker> Speakers { get; set; }
        public List<Room> Rooms { get; set; }

        public List<MeetUserJoin> MeetUserJoins { get; set; }

        public List<string> userIdList { get; set; }
        public List<string> signUserIdList { get; set; }
    }
}
