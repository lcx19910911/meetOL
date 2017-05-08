namespace Model
{
    using EnumPro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 话题参与记录
    /// </summary>
    [Table("TopicUserJoin")]
    public partial class TopicUserJoin : BaseEntity
    {
        /// <summary>
        /// 话题id
        /// </summary>
        [Display(Name = "话题id")]
        [Column("MeetTopicID", TypeName = "char"), MaxLength(32)]
        public string MeetTopicID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Display(Name = "用户id")]
        [Column("UserID", TypeName = "char"), MaxLength(32)]
        public string UserID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Display(Name = "用户id")]
        [Column("MeetID", TypeName = "char"), MaxLength(32)]
        public string MeetID { get; set; }


        /// <summary>
        /// 用户id
        /// </summary>
        [Display(Name = "用户id")]
        [Column("PlanID", TypeName = "char"), MaxLength(32)]
        public string PlanID { get; set; }
        /// <summary>
        /// 参与内容
        /// </summary>
        [Display(Name = "参与内容")]
        [MaxLength(512)]
        [Required, Column("Content", TypeName = "varchar")]
        public string Content { get; set; }

        public UserJoinState State { get; set; } = UserJoinState.WaitAudit;

        [NotMapped]
        public string StateStr { get; set; }

        [NotMapped]
        public User User { get; set; }

        [NotMapped]
        public Meet Meet { get; set; }
        [NotMapped]
        public MeetTopic MeetTopic { get; set; }
    }
}
