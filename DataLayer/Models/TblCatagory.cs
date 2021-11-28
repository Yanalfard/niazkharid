﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    [Table("TblCatagory", Schema = "dbo")]
    public partial class TblCatagory
    {
        public TblCatagory()
        {
            InverseParent = new HashSet<TblCatagory>();
            TblProduct = new HashSet<TblProduct>();
        }

        [Key]
        public int CatagoryId { get; set; }
        [Display(Name = "نام دسته بندی")]
        [Required(ErrorMessage = "نام دسته بندی را وارد کنید")]
        [MaxLength(100, ErrorMessage = "کارکتر های کمتری وارد کنید")]
        [MinLength(3, ErrorMessage = "لطفا کارکتر های بیشتری وارد کنید")]
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsOnFirstPage { get; set; }

        [ForeignKey(nameof(ParentId))]
        [InverseProperty(nameof(TblCatagory.InverseParent))]
        public virtual TblCatagory Parent { get; set; }
        [InverseProperty(nameof(TblCatagory.Parent))]
        public virtual ICollection<TblCatagory> InverseParent { get; set; }
        [InverseProperty("Catagory")]
        public virtual ICollection<TblProduct> TblProduct { get; set; }
    }
}
