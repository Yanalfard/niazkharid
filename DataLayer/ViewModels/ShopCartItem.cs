﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
    public class ShopCartItem
    {
        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int Count { get; set; }
    }
}
