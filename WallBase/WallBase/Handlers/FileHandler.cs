using System;
using System.IO;
using System.Net;
using System.Web;

namespace WallBase.Handlers
{
    public class FileHandler : IHttpHandler
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
            if (!string.IsNullOrEmpty(context.Request.QueryString["file"]))
            {
                string fileName = context.Request.QueryString["file"];
                OnServing(fileName);
                fileName = !fileName.StartsWith("/") ? string.Format("/{0}", fileName) : fileName;
                try
                {
                    FileStream file = File.Open("", FileMode.Append);
                    if (file != null)
                    {
                        context.Response.AppendHeader("Content-Disposition",
                            string.Format("inline; filename=\"{0}\"", file.Name));
                        SetContentType(context, file.Name);


                        OnServed(fileName);
                    }
                    else
                    {
                        OnBadRequest(fileName);
                    }
                }
                catch (Exception)
                {
                    OnBadRequest(fileName);
                }
            }
        }

        public static bool SetConditionalGetHeaders(DateTime date)
        {
            DateTime now = date.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;


            if (date > now || date.Year < 1900)
            {
                date = now;
            }

            HttpResponse response = HttpContext.Current.Response;
            HttpRequest request = HttpContext.Current.Request;

            string etag = string.Format("\"{0}\"", date.Ticks);
            string incomingEtag = request.Headers["If-None-Match"];

            DateTime incomingLastModifiedDate;
            DateTime.TryParse(request.Headers["If-Modified-Since"], out incomingLastModifiedDate);

            response.Cache.SetLastModified(date);
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetETag(etag);

            if (String.Compare(incomingEtag, etag) == 0 || incomingLastModifiedDate == date)
            {
                response.Clear();
                response.StatusCode = (int) HttpStatusCode.NotModified;
                return true;
            }

            return false;
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


        private static void SetContentType(HttpContext context, string fileName)
        {
            context.Response.AddHeader(
                "Content-Type", fileName.EndsWith(".pdf") ? "application/pdf" : "application/octet-stream");
        }

        #endregion
    }
}