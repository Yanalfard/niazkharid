﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using ReflectionIT.Mvc.Paging;
using Services.Services;
using GhasreMobile.Utilities;

namespace GhasreMobile.Areas.Admin.Controllers
{
    [Area("Admin")]
    [PermissionChecker("admin")]
    public class DiscountController : Controller
    {
        Core _core = new Core();
        public IActionResult Index(int page = 1)
        {
            IEnumerable<TblDiscount> discounts = PagingList.Create(_core.Discount.Get().OrderByDescending(d=>d.DiscountId), 30, page);
            return View(discounts);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return ViewComponent("DiscountCreateAdmin");
        }

        [HttpPost]
        public IActionResult Create(TblDiscount discountt, int Till)
        {
            if (ModelState.IsValid)
            {
                TblDiscount Newdiscount = new TblDiscount();
                Newdiscount.Name = discountt.Name;
                Newdiscount.Discount = discountt.Discount;
                Newdiscount.Count = discountt.Count;
                Newdiscount.ValidTill = DateTime.Now.AddDays(Till);
                _core.Discount.Add(Newdiscount);
                _core.Save();
                return Redirect("/Admin/Discount");
            }
            return View(discountt);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return ViewComponent("DiscountEditAdmin", new { id = id });
        }

        [HttpPost]
        public IActionResult Edit(TblDiscount Editdiscount, int Till)
        {
            if (ModelState.IsValid)
            {
                _core.Discount.Update(Editdiscount);
                _core.Save();
                return Redirect("/Admin/Discount");
            }
            return View(Editdiscount);
        }

        public string Delete(int id)
        {
            TblDiscount product = _core.Discount.GetById(id);
            if (product.TblOrder.Any())
            {
                return "سفارشی برای این  تخفیف وجود دارد";
            }
            else
            {
                _core.Discount.Delete(product);
                _core.Save();
                return "true";
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _core.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
