namespace Model
{
    using EnumPro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 投票参与记录
    /// </summary>
    [Table("PlanVoteUserJoin")]
    public partial class PlanVoteUserJoin : BaseEntity
    {
        /// <summary>
        /// 会议id
        /// </summary>
        [Display(Name = "话题id")]
        [Column("MeetID", TypeName = "char"), MaxLength(32)]
        public string MeetID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Display(Name = "用户id")]
        [Column("UserID", TypeName = "char"), MaxLength(32)]
        public string UserID { get; set; }

        /// <summary>
        /// 计划id
        /// </summary>
        [Display(Name = "计划id")]
        [Column("PlanID", TypeName = "char"), MaxLength(32)]
        public string PlanID { get; set; }

        /// <summary>
        /// 演讲者id
        /// </summary>
        [Display(Name = "演讲者id")]
        [Column("SpeakerID", TypeName = "char"), MaxLength(32)]
        public string SpeakerID { get; set; }

        [NotMapped]
        public User User { get; set; }
        [NotMapped]
        public Meet Meet { get; set; }

        [NotMapped]
        public Speaker Speaker { get; set; }


        [NotMapped]
        public MeetPlan MeetPlan { get; set; }
    }
}
