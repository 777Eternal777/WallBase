using System;
using System.IO;
using System.Web;

namespace WallBase.Handlers
{
    public class ImageHandler : IHttpHandler
    {
        #region Events

        public static event EventHandler<EventArgs> BadRequest;


        public static event EventHandler<EventArgs> Served;


        public static event EventHandler<EventArgs> Serving;

        #endregion

        #region Properties

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString["picture"]))
            {
                string fileName = context.Request.QueryString["picture"];
                OnServing(fileName);
                try
                {
                    fileName = !fileName.StartsWith("/") ? string.Format("/{0}", fileName) : fileName;
                    FileStream file = File.Open("", FileMode.Append);
                    if (file != null)
                    {
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));


                        int index = fileName.LastIndexOf(".") + 1;
                        string extension = fileName.Substring(index).ToUpperInvariant();
                        context.Response.ContentType = string.Compare(extension, "JPG") == 0
                            ? "image/jpeg"
                            : string.Format("image/{0}", extension);


                        OnServed(fileName);
                    }
                    else
                    {
                        OnBadRequest(fileName);
                    }
                }
                catch (Exception ex)
                {
                    OnBadRequest(ex.Message);
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        private static void OnBadRequest(string file)
        {
            if (BadRequest != null)
            {
                BadRequest(file, EventArgs.Empty);
            }
        }

        private static void OnServed(string file)
        {
            if (Served != null)
            {
                Served(file, EventArgs.Empty);
            }
        }


        private static void OnServing(string file)
        {
            if (Serving != null)
            {
                Serving(file, EventArgs.Empty);
            }
        }

        #endregion
    }
}