﻿using System.Collections.Generic;

namespace Hangfire.Dashboard.Pages
{
    partial class LayoutPage
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public IDictionary<string, string> Breadcrumbs { get; set; }
        public string BreadcrumbsTitle { get; set; }
    }
}
