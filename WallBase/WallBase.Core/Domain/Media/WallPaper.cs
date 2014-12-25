using System;
using System.Runtime.CompilerServices;
using Nop.Core;

namespace WallBase.Core.Domain.Media
{
    /// <summary>
    /// Represents a picture
    /// </summary>
    public partial class Wallpaper : BaseEntity
    {
        public string MimeType { get; set; }

        public string Filename { get; set; }
        public string CDNSrc { get; set; }

        public int FavsCount { get; set; }
        public Purity Purity { get; set; }
        public DateTime CreationDate { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public long ViewCount { get; set; }
        public virtual Category Category { get; set; }

        public virtual Tag Tag { get; set; }
        public string SourceSrc { get; set; }

        public float Size { get; set; }
        public string Extension { get; set; }
    }
}
