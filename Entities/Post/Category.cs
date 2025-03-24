using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        //زبان تایپ قوی (Strong Type) : این نوع زبان روی کاری که میخاین روی متغیر و داده انجام بدین حساسه 
        //خطا را در جا نمایش میده 
        [ForeignKey(nameof(ParentCategoryId))]
        public Category ParentCategory { get; set; }//طرف یک 
        public ICollection<Category> ChildCategories { get; set; }
        public ICollection<Post> Posts { get; set; }//طرف n 
    }
}
