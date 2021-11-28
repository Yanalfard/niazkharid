﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer.ViewModels
{
    public class ChargeWalletVm
    {
        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1000, Int32.MaxValue, ErrorMessage = "حداقل مبلغ شارژ 1000 هزار تومان می باشد")]
        public int? Amount { get; set; }
        public int? OrderId { get; set; }
    }

    public class WalletVm
    {
        public int Amount { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }

    }
}
