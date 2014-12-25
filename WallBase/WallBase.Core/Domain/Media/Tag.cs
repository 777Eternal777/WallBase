using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallBase.Core.Domain.Media
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }

        public virtual List<Wallpaper> Wallpapers { get; set; }
    }
}
