namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 演讲人
    /// </summary>
    [Table("Speaker")]
    public partial class Speaker : BaseEntity
    {


        /// <summary>
        /// 首字母缩写
        /// </summary>
        [Display(Name = "首字母缩写"), Column("HeadImages", TypeName = "varchar"), MaxLength(512)]
        public string HeadImages { get; set; } = "/images/defulat.jpg";

        /// <summary>
        /// 演讲人名称
        /// </summary>
        [Required,Display(Name = "演讲人名称"), Column("Name", TypeName = "varchar"), MaxLength(64)]
        public string Name { get; set; }


        /// <summary>
        /// 首字母缩写
        /// </summary>
        [Display(Name = "首字母缩写"), Column("ShortKey", TypeName = "varchar"), MaxLength(32)]
        public string ShortKey { get; set; }


        /// <summary>
        /// 职位
        /// </summary>
        [Display(Name = "演讲人职位"), Column("Position", TypeName = "varchar"), MaxLength(1024)]
        public string Position { get; set; }



        /// <summary>
        /// 职位
        /// </summary>
        [Display(Name = "演讲人手机"), Column("Phone", TypeName = "varchar"), MaxLength(11)]
        public string Phone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注"), Column("Remark", TypeName = "varchar"), MaxLength(256)]
        public string Remark { get; set; }
    }
}
