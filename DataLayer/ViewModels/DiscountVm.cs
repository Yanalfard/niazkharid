﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.ViewModels
{
    public class DiscountVm
    {
        public int DiscountId { get; set; }
        public long DiscountPrice { get; set; }
        public int Discount { get; set; }
        [Required(ErrorMessage = "کد تخفیف را وارد کنید")]
        [StringLength(20, ErrorMessage = "لطفا نام تخفیف را وارد کنید")]
        public string Name { get; set; }
        public long Sum { get; set; }
        public long SumWithDiscount { get; set; }
        public long Balance { get; set; }
        public int PostPrice { get; set; }
        public int PostPriceId { get; set; }
        public int SagfePost { get; set; }
        public bool? IsFractional { get; set; }

    }
}
