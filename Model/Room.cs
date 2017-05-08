namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 会议厅
    /// </summary>
    [Table("Room")]
    public partial class Room : BaseEntity
    {
        /// <summary>
        /// 会议厅名
        /// </summary>
        [Display(Name = "会议厅名")]
        [MaxLength(64)]
        [Required, Column("Name", TypeName = "varchar")]
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Display(Name = "图标")]
        [MaxLength(512), Column("Image", TypeName = "varchar")]
        public string Image { get; set; }

        /// <summary>
        /// 会议地点
        /// </summary>
        [Display(Name = "会议地点")]
        [MaxLength(512)]
        [Required, Column("Address", TypeName = "varchar")]
        public string Address { get; set; }

        /// <summary>
        /// 座号
        /// </summary>
        public int SeatCount { get; set; }

        /// <summary>
        /// 排除的座位集合
        /// </summary>
        [Display(Name = "排除的座位集合")]
        [MaxLength(512)]
        [Column("ExcludeList", TypeName = "varchar")]
        public string ExcludeList { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序"), Column("Sort", TypeName = "int")]
        public int Sort { get; set; }

        [NotMapped]
        public List<MeetPlan> MeetPlans { get; set; }
    }
}
