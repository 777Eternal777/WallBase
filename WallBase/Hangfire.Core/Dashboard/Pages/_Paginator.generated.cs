﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hangfire.Dashboard.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class Paginator : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");


WriteLiteral("<div class=\"btn-toolbar\">\r\n");


            
            #line 4 "..\..\Dashboard\Pages\_Paginator.cshtml"
     if (_pager.TotalPageCount > 1)
    {

            
            #line default
            #line hidden
WriteLiteral("        <div class=\"btn-group paginator\">\r\n");


            
            #line 7 "..\..\Dashboard\Pages\_Paginator.cshtml"
             foreach (var page in _pager.PagerItems)
            {
                switch (page.Type)
                {
                    case Pager.ItemType.Page:

            
            #line default
            #line hidden
WriteLiteral("                        <a href=\"");


            
            #line 12 "..\..\Dashboard\Pages\_Paginator.cshtml"
                            Write(_pager.PageUrl(page.PageIndex));

            
            #line default
            #line hidden
WriteLiteral("\" class=\"btn btn-default ");


            
            #line 12 "..\..\Dashboard\Pages\_Paginator.cshtml"
                                                                                     Write(_pager.CurrentPage == page.PageIndex ? "active" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            ");


            
            #line 13 "..\..\Dashboard\Pages\_Paginator.cshtml"
                       Write(page.PageIndex);

            
            #line default
            #line hidden
WriteLiteral("  \r\n                        </a>\r\n");


            
            #line 15 "..\..\Dashboard\Pages\_Paginator.cshtml"
                        break;
                    case Pager.ItemType.NextPage:

            
            #line default
            #line hidden
WriteLiteral("                        <a href=\"");


            
            #line 17 "..\..\Dashboard\Pages\_Paginator.cshtml"
                            Write(_pager.PageUrl(page.PageIndex));

            
            #line default
            #line hidden
WriteLiteral("\" class=\"btn btn-default ");


            
            #line 17 "..\..\Dashboard\Pages\_Paginator.cshtml"
                                                                                     Write(page.Disabled ? "disabled" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            Next\r\n                        </a>\r\n");


            
            #line 20 "..\..\Dashboard\Pages\_Paginator.cshtml"
                        break;
                    case Pager.ItemType.PrevPage:

            
            #line default
            #line hidden
WriteLiteral("                        <a href=\"");


            
            #line 22 "..\..\Dashboard\Pages\_Paginator.cshtml"
                            Write(_pager.PageUrl(page.PageIndex));

            
            #line default
            #line hidden
WriteLiteral("\" class=\"btn btn-default ");


            
            #line 22 "..\..\Dashboard\Pages\_Paginator.cshtml"
                                                                                     Write(page.Disabled ? "disabled" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                            Prev\r\n                        </a>\r\n");


            
            #line 25 "..\..\Dashboard\Pages\_Paginator.cshtml"
                        break;
                    case Pager.ItemType.MorePage:

            
            #line default
            #line hidden
WriteLiteral("                        <a href=\"#\" class=\"btn btn-default disabled\">\r\n          " +
"                  …\r\n                        </a>\r\n");


            
            #line 30 "..\..\Dashboard\Pages\_Paginator.cshtml"
                        break;
                }
            }

            
            #line default
            #line hidden
WriteLiteral("        </div>\r\n");



WriteLiteral("        <div class=\"btn-toolbar-spacer\"></div>\r\n");


            
            #line 35 "..\..\Dashboard\Pages\_Paginator.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div class=\"btn-toolbar-label\">Total jobs: ");


            
            #line 37 "..\..\Dashboard\Pages\_Paginator.cshtml"
                                          Write(_pager.TotalRecordCount);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n</div>\r\n");


        }
    }
}
#pragma warning restore 1591
