namespace Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 话题
    /// </summary>
    [Table("MeetTopic")]
    public partial class MeetTopic : BaseEntity
    {

        /// <summary>
        /// 会议id
        /// </summary>
        [Required, Column("MeetID", TypeName = "char"), MaxLength(32)]
        public string MeetID { get; set; }

        /// <summary>
        /// 会议厅人
        /// </summary>
        [Required,Column("RoomID", TypeName = "char"), MaxLength(32)]
        public string RoomID { get; set; }

        /// <summary>
        /// 计划
        /// </summary>
        [Required, Column("PlanID", TypeName = "char"), MaxLength(32)]
        public string PlanID { get; set; }

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
        public Meet Meet { get; set; }

        [NotMapped]
        public Room Room { get; set; }


        [NotMapped]
        [JsonIgnore]
        public MeetPlan MeetPlan { get; set; }
        [NotMapped]
        public List<TopicUserJoin> TopicUserJoins { get; set; }
    }
}
