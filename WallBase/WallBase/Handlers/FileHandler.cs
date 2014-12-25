using System;
using System.IO;
using System.Net;
using System.Web;

namespace WallBase.Handlers
{
    /// <summary>
    /// The FileHandler serves all files that is uploaded from
    ///     the admin pages except for images.
    /// </summary>
    /// <remarks>
    /// By using a HttpHandler to serve files, it is very easy
    ///     to add the capability to stop bandwidth leeching or
    ///     to create a statistics analysis feature upon it.
    /// </remarks>
    public class FileHandler : IHttpHandler
    {
        #region Events

        /// <summary>
        ///     Occurs when the requested file does not exist;
        /// </summary>
        public static event EventHandler<EventArgs> BadRequest;

        /// <summary>
        ///     Occurs when a file is served;
        /// </summary>
        public static event EventHandler<EventArgs> Served;

        /// <summary>
        ///     Occurs when the requested file does not exist;
        /// </summary>
        public static event EventHandler<EventArgs> Serving;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that 
        ///     implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> 
        ///     object that provides references to the intrinsic server objects 
        ///     (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public static bool SetConditionalGetHeaders(DateTime date)
        {
            var now = date.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

            // SetLastModified() below will throw an error if the 'date' is a future date.
            // If the date is 1/1/0001, Mono will throw a 404 error
            if (date > now || date.Year < 1900)
            {
                date = now;
            }

            var response = HttpContext.Current.Response;
            var request = HttpContext.Current.Request;

            var etag = string.Format("\"{0}\"", date.Ticks);
            var incomingEtag = request.Headers["If-None-Match"];

            DateTime incomingLastModifiedDate;
            DateTime.TryParse(request.Headers["If-Modified-Since"], out incomingLastModifiedDate);

            response.Cache.SetLastModified(date);
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetETag(etag);

            if (String.Compare(incomingEtag, etag) == 0 || incomingLastModifiedDate == date)
            {
                response.Clear();
                response.StatusCode = (int)HttpStatusCode.NotModified;
                return true;
            }

            return false;
        }
        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString["file"]))
            {
                var fileName = context.Request.QueryString["file"];
                OnServing(fileName);
                fileName = !fileName.StartsWith("/") ? string.Format("/{0}", fileName) : fileName;
                try
                {

                    var file = File.Open("", FileMode.Append);//BlogService.GetFile(string.Format("{0}files{1}",Blog.CurrentInstance.StorageLocation, fileName));
                    if (file != null)
                    {
                        context.Response.AppendHeader("Content-Disposition", string.Format("inline; filename=\"{0}\"", file.Name));
                        SetContentType(context, file.Name);
                    //    if (SetConditionalGetHeaders(file.Name))
                          //  return;

                  //      context.Response.BinaryWrite(file.Read());
                        OnServed(fileName);
                    }
                    else
                    {
                        OnBadRequest(fileName);
                     //   context.Response.Redirect(string.Format("{0}error404.aspx", Utils.AbsoluteWebRoot));
                    }
                }
                catch (Exception)
                {
                    OnBadRequest(fileName);
                 //   context.Response.Redirect(string.Format("{0}error404.aspx", Utils.AbsoluteWebRoot));
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Called when [bad request].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnBadRequest(string file)
        {
            if (BadRequest != null)
            {
                BadRequest(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [served].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnServed(string file)
        {
            if (Served != null)
            {
                Served(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [serving].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnServing(string file)
        {
            if (Serving != null)
            {
                Serving(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the content type depending on the filename's extension.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        private static void SetContentType(HttpContext context, string fileName)
        {
            context.Response.AddHeader(
                "Content-Type", fileName.EndsWith(".pdf") ? "application/pdf" : "application/octet-stream");
        }

        #endregion
    }
}