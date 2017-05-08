using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class MeetReportModel
    {
        /// <summary>
        /// 报名参加人数
        /// </summary>
        public int JoinCount { get; set; }
        /// <summary>
        /// 签到
        /// </summary>
        public int SignCount { get; set; }

        /// <summary>
        /// 计划数量
        /// </summary>
        public int PlanCount { get; set; }


        /// <summary>
        /// 话题数量
        /// </summary>
        public int TopicCount { get; set; }


        /// <summary>
        /// 话题数量
        /// </summary>
        public int TopicJoinCount { get; set; }


        /// <summary>
        /// 话题数量
        /// </summary>
        public int VoteCount { get; set; }
        
        public List<MeetPlan> PlanList { get; set; }
    }
}
