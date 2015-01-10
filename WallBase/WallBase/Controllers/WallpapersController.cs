using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using WallBase.App_Start;
using WallBase.Config;
using WallBase.Core.Domain.Media;
using WallBase.Logic;
using WallBase.Models;

namespace WallBase.Controllers
{
    public class WallpapersController : Controller
    {
        private WallpapersService wallpapersManager;
        public WallpapersController(WallpapersService wallpapersManager)
        {
            this.wallpapersManager = wallpapersManager;
        }

        public FileResult Thumb(string name)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/" + name);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = "myfile.jpg";

            Bitmap b = ImageResizer.ImageBuilder.Current.Build(fileBytes,
                new ResizeSettings("width=300&height=200&mod=max"));
            ImageConverter converter = new ImageConverter();
            return File((byte[])converter.ConvertTo(b, typeof(byte[])), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public FileResult WallpaperImage(string name)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/" + name);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "www");
        }



        public ActionResult Wallpaper(string name)
        {
            var model = new ImageModel
            {
                Src = Url.Action("WallpaperImage", "Wallpapers", new { name }),
            };
            return this.View(model);
        }

        public ActionResult Image(string id)
        {
            var dir = Server.MapPath("/Images");
            var path = Path.Combine(dir, id + ".jpg");
            return base.File(path, "image/jpeg");
        }

        [HttpPost]
        public ActionResult UploadAction(List<HttpPostedFileBase> fileUpload)
        {
            // Your Code - / Save Model Details to DB

            // Handling Attachments - 
            //foreach (HttpPostedFileBase item in fileUpload)
            //{
            //    if (Array.Exists(model.FilesToBeUploaded.Split(','), s => s.Equals(item.FileName)))
            //    {
            //        //Save or do your action -  Each Attachment ( HttpPostedFileBase item ) 
            //    }
            //}
            return View("Index");
        }

        Dictionary<string, string> extensionLookup = new Dictionary<string, string>()
{
    {"image/jpg", ".jpg"},
    {"image/png", ".png"},
};
        [HttpPost]
        public ActionResult UploadImageMethod()
        {
            //sd.EnumerateFiles().FirstOrDefault().FullName
            if (Request.Files.Count != 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //todo move to filesystem.
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;

                    FileInfo fi = new FileInfo(fileName);
                    string ext = fi.Extension;
                    var wallpaper = new Wallpaper
                    {
                        Category = new Category
                        {
                            Name = "wee"
                        },
                        MimeType = file.ContentType,
                        Size = fileSize,
                        CreationDate = DateTime.Now,
                        Filename = fileName,
                        Extension = ext

                    };
                    this.wallpapersManager.Save(wallpaper);
                    //  var ext = extensionLookup[file.ContentType];
                    file.SaveAs(Server.MapPath(string.Format("{0}{1}{2}",Directories.WallpapersPath, wallpaper.Id, ext)));
                    var ImageJob = new ImageJob(file.InputStream, System.IO.File.Create(Server.MapPath(string.Format("{0}{1}{2}",Directories.ThumbsPath, wallpaper.Id, ext))), new ResizeSettings("width=300&height=200&mod=max"));
                    ImageJob.Build();
                }
                return Content("Success");
            }
            return Content("failed");
        }
    }

}