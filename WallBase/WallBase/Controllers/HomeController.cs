using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharpRepository.Repository;
using WallBase.App_Start;
using WallBase.Logic;
using WallBase.Models;

namespace WallBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly WallpapersService WallpapersService;
        public HomeController( WallpapersService wallpapersService)
        {
            WallpapersService = wallpapersService;
        }


        [OutputCache(Duration = 60)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        [OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult UploadsListing(){

            return View();
        }
        public ActionResult Upload()
        {

            return View();
        }

        [Route("wallpaper/{name}")]
        public ActionResult Wallpaper(string name)
        {
            return RedirectToAction("Wallpaper", "Wallpapers");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            var model = new List<ImageModel>();
            var wallpapers = WallpapersService.GetAll();
            foreach (var file in wallpapers)
            {
                model.Add(new ImageModel
                {
                    ThumbSrc = "/Content/thumbs/" + file.Id+file.Extension,
                    Src = "/Content/wallpapers/" + file.Id + file.Extension,

                  //  ThumbSrc = Url.Action("Thumb", "Wallpapers", new { @name = file.Id }),
                   // Src = Url.Action("Wallpaper", "Wallpapers", new { @name = file.Id }),
                });
            }

            return View(model);
        }


        //   [Route("home/wallpapers/{size}/{day}")]  //thumb/
        //public FileResult Wallpapers(string size, int day) //string thumb2
        //{
        //    string path = HttpContext.Server.MapPath("~/App_Data/LaQ-6CnlCho.jpg");
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
        //    string fileName = "myfile.jpg";
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}


    }

}