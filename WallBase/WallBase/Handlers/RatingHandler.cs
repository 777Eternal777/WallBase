using System;
using System.Web;

namespace WallBase.Handlers
{
    public class RatingHandler : IHttpHandler
    {
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

            {
                string rating = context.Request.QueryString["rating"];
                int rate;
                if (rating != null && int.TryParse(rating, out rate))
                {
                    string id = context.Request.QueryString["id"];
                    if (id != null && id.Length == 36 && rate > 0 && rate < 6)
                    {
                        try
                        {
                            if (HasRated(id))
                            {
                                context.Response.Write(string.Format("{0}HASRATED", rate));
                                context.Response.End();
                            }
                            else
                            {
                                SetCookie(id, context);
                                context.Response.Write(string.Format("{0}OK", rate));
                                context.Response.End();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                context.Response.Write("FAIL");
            }
        }

        #endregion

        #endregion

        #region Methods

        private static bool HasRated(string postId)
        {
            HttpCookie ratingCookie = HttpContext.Current.Request.Cookies["rating"];

            if (ratingCookie != null)
            {
                return ratingCookie.Value.Contains(postId);
            }

            return false;
        }


        private static void SetCookie(string id, HttpContext context)
        {
            HttpCookie cookie = context.Request.Cookies["rating"] ?? new HttpCookie("rating");

            cookie.Expires = DateTime.Now.AddYears(2);
            cookie.Value += id;
            context.Response.Cookies.Add(cookie);
        }

        #endregion
    }
}