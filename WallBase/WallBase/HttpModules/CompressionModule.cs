using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Web;
using System.Web.UI;

namespace WallBase.HttpModules
{
    public sealed class CompressionModule : IHttpModule
    {
        #region Constants and Fields

        private const string Deflate = "deflate";


        private const string Gzip = "gzip";

        #endregion

        #region Public Methods

        public static void CompressResponse(HttpContext context)
        {
            if (IsEncodingAccepted(Deflate))
            {
                context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
                WillCompressResponse = true;
                SetEncoding(Deflate);
            }
            else if (IsEncodingAccepted(Gzip))
            {
                context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                WillCompressResponse = true;
                SetEncoding(Gzip);
            }
        }

        #endregion

        #region Private Methods

        private static bool WillCompressResponse
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    return false;
                }
                return context.Items["will-compress-resource"] != null && (bool) context.Items["will-compress-resource"];
            }
            set
            {
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    return;
                }
                context.Items["will-compress-resource"] = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpModule

        void IHttpModule.Dispose()
        {
        }


        void IHttpModule.Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += ContextPostReleaseRequestState;
            context.Error += context_Error;
        }

        private void context_Error(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication) sender).Context;
            Exception ex = context.Server.GetLastError();


            if (WillCompressResponse)
            {
                context.Response.Filter = null;
                WillCompressResponse = false;
            }
        }

        #endregion

        #endregion

        #region Methods

        private static bool IsEncodingAccepted(string encoding)
        {
            HttpContext context = HttpContext.Current;
            return context.Request.Headers["Accept-encoding"] != null &&
                   context.Request.Headers["Accept-encoding"].Contains(encoding);
        }


        private static void SetEncoding(string encoding)
        {
            HttpContext.Current.Response.AppendHeader("Content-encoding", encoding);
        }


        private static void ContextPostReleaseRequestState(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication) sender).Context;
            Debug.WriteLine("precessing request --->" + context.Request.Path);


            if (context.Request.Path.Contains("/Scripts/") || context.Request.Path.Contains("/Styles/") ||
                context.Request.Path.EndsWith(".js.axd"))
            {
                if (!context.Request.Path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.EndsWith(".swf", StringComparison.OrdinalIgnoreCase))
                {
                    SetHeaders(context);
                    CompressResponse(context);
                }
            }


            if (context.CurrentHandler is Page && context.Request["HTTP_X_MICROSOFTAJAX"] == null &&
                context.Request.HttpMethod == "GET")
            {
                CompressResponse(context);

                if (!context.Request.Path.Contains("/admin/"))
                {
                    WillCompressResponse = true;
                }
            }
        }

        private static void SetHeaders(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpCachePolicy cache = response.Cache;

            cache.VaryByHeaders["Accept-Encoding"] = true;
            cache.SetExpires(DateTime.UtcNow.AddDays(30));
            cache.SetMaxAge(new TimeSpan(365, 0, 0, 0));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            string etag = string.Format("\"{0}\"", context.Request.Path.GetHashCode());
            string incomingEtag = context.Request.Headers["If-None-Match"];

            cache.SetETag(etag);
            cache.SetCacheability(HttpCacheability.Public);

            if (String.Compare(incomingEtag, etag) != 0)
            {
                return;
            }

            response.Clear();
            response.StatusCode = (int) HttpStatusCode.NotModified;
            response.SuppressContent = true;
        }

        #endregion
    }
}