namespace Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 会议安排
    /// </summary>
    [Table("MeetPlan")]
    public partial class MeetPlan : BaseEntity
    {
        /// <summary>
        /// 会议id
        /// </summary>
        [Required, Column("MeetID", TypeName = "char"), MaxLength(32)]
        public string MeetID { get; set; }

        /// <summary>
        /// 会议厅
        /// </summary>
        [Required,Column("RoomID", TypeName = "char"), MaxLength(32)]
        public string RoomID { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StratTime { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        [Display(Name = "计划名称")]
        [MaxLength(50)]
        [Required, Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 演讲人
        /// </summary>
        [Column("SpeakerID", TypeName = "char"), MaxLength(32)]
        public string SpeakerID { get; set; }

        [NotMapped]
        public Speaker Speaker { get; set; }
        [NotMapped]
        public string SpeakerName { get; set; }
        [NotMapped]
        public string MeetName { get; set; }
        [NotMapped]
        public List<MeetTopic> MeetTopics { get; set; }

        [NotMapped]
        public List<User> VoteUserList { get; set; }

        [NotMapped]
        public bool CanVote { get; set; }

        public int VoteCount { get; set; } = 0;

        [NotMapped]

        public DateTime OngoingTime { get; set; }
        [NotMapped]
        public DateTime OverTime { get; set; }
    }
}
