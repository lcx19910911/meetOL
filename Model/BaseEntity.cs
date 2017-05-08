using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BaseEntity
    {
        [Key]
        [NotChange]
        [Column("ID", TypeName = "char"), MaxLength(32)]
        public string ID { get; set; }

        
        [NotChange]
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        [NotChange]
        public System.DateTime CreatedTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [Required]
        public System.DateTime UpdatedTime { get; set; }
    }
}
