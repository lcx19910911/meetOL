namespace Model
{
    using EnumPro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 用户参与记录
    /// </summary>
    [Table("MeetUserJoin")]
    public partial class MeetUserJoin : BaseEntity
    {
        /// <summary>
        /// 会议id
        /// </summary>
        [Display(Name = "活动id")]
        [Column("MeetID", TypeName = "char"), MaxLength(32)]
        public string MeetID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Display(Name = "用户id")]
        [Column("UserID", TypeName = "char"), MaxLength(32)]
        public string UserID { get; set; }

        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public string UserPhone { get; set; }
        [NotMapped]
        public string Position { get; set; }
        [NotMapped]
        public string Compnay { get; set; }

        /// <summary>
        /// 会议厅id
        /// </summary>
        [Display(Name = "会议厅id")]
        [Column("RoomeID", TypeName = "char"), MaxLength(32)]
        public string RoomeID { get; set; }


        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int SeatNum { get; set; }

        [NotMapped]
        public string StateStr { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public UserJoinState State { get; set; } = UserJoinState.WaitAudit;


        [NotMapped]
        public string HadSignStr { get; set; }
        /// <summary>
        /// 是否签到
        /// </summary>
        public YesOrNoCode HadSign { get; set; } = YesOrNoCode.No;

        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime? SignTime { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        [NotMapped]
        public User User { get; set; }
        [NotMapped]
        public Meet Meet { get; set; }
    }

    public enum UserJoinState
    {
        [Description("等待审核")]
        WaitAudit =0,

        [Description("审核允许参加")]
        Pass =1,

        [Description("审核不允许参加")]
        Reject =2
    }
}
