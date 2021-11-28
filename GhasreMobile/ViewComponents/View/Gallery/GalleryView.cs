﻿using DataLayer.Models;
using GhasreMobile.Utilities;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhasreMobile.ViewComponents.View.Gallery
{
    public class GalleryView : ViewComponent
    {
        private Core db = new Core();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Random rand = new Random();
            List<TblAlbum> images = db.Album.Get(i => i.IsProduct == false).OrderByDescending(i => i.AlbumId).Take(6).ToList();
            images.ShuffleList();
            return await Task.FromResult((IViewComponentResult)View("~/Views/Shared/Components/GalleryView/GalleryView.cshtml", images));
        }
    }
}
