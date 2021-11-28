﻿using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhasreMobile.ViewComponents.View.BrandList
{
    public class BrandListView : ViewComponent
    {
        private Core db = new Core();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("~/Views/Shared/Components/BrandListView/BrandListView.cshtml", db.Brand.Get().OrderByDescending(i => i.BrandId)));
        }
    }
}
