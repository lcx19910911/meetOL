namespace Model
{
    using EnumPro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 会议
    /// </summary>
    [Table("Meet")]
    public partial class Meet : BaseEntity
    {
        /// <summary>
        /// 创建用户id
        /// </summary>
        [Display(Name = "创建用户id")]
        [Required, Column("CreateUserId", TypeName = "char"), MaxLength(32)]
        public string CreateUserId { get; set; }


        /// <summary>
        /// 会议地点
        /// </summary>
        [Display(Name = "会议地点")]
        [MaxLength(512)]
        [Required, Column("Address", TypeName = "varchar")]
        public string Address { get; set; }

        /// <summary>
        /// 会议名称
        /// </summary>
        [Display(Name = "会议名称")]
        [MaxLength(64)]
        [Required, Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 报名是否需要审核
        /// </summary>
        public YesOrNoCode IsJoinAudit { get; set; } = YesOrNoCode.No;
        [NotMapped]
        public string IsJoinAuditStr { get; set; }



        /// <summary>
        /// 签到二维码是否变化
        /// </summary>
        public YesOrNoCode IsChangeQrcode { get; set; } = YesOrNoCode.No;
        [NotMapped]
        public string IsChangeQrcodeStr { get; set; }

        /// <summary>
        /// 座位图
        /// </summary
        [MaxLength(512)]
        [Column("PlaceImage", TypeName = "varchar")]
        public string PlaceImage { get; set; }

        /// <summary>
        /// 是否自动分配
        /// </summary>
        public YesOrNoCode IsAutoAllot { get; set; } = YesOrNoCode.Yes;
        [NotMapped]
        public string IsAutoAllotStr { get; set; }

        /// <summary>
        /// 会议信息
        /// </summary>
        [Display(Name = "会议信息")]
        [Column("Content", TypeName = "text")]
        public string Content { get; set; }

        [Column("DownLoadInfos", TypeName = "text")]
        public string DownLoadInfos { get; set; }

        /// <summary>
        /// 会议厅
        /// </summary>
        [MaxLength(2014)]
        [Required, Column("RoomIDs", TypeName = "varchar")]
        public string RoomIDs { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [Display(Name = "活动开始时间")]
        [Required]
        public System.DateTime OngoingTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        [Display(Name = "活动结束时间")]
        [Required]
        public System.DateTime OverTime { get; set; }


        public int MaxLimit { get; set; }
    }
}
