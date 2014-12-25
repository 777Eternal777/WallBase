using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallBase.Core.Domain.Media
{
    public class Category : BaseEntity
    {
        public Category()
        {
            this.Wallpapers=new List<Wallpaper>();
        }
        public string Name { get; set; }
        public virtual IList<Wallpaper> Wallpapers { get; set; }
    }
}
