using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpRepository.Repository;
using WallBase.Core.Domain.Media;

namespace WallBase.Logic
{
   public class WallpapersManager
   {
       private IRepository<Wallpaper, int> wallpapersRepository;

       public WallpapersManager(IRepository<Wallpaper, int> wallpapersRepository)
       {
           this.wallpapersRepository = wallpapersRepository;
       }

       public void Save(Wallpaper wallpaper)
       {
           this.wallpapersRepository.Add(wallpaper);
       }

       public IEnumerable<Wallpaper> GetAll()
       {
           return this.wallpapersRepository.GetAll();
       }
       public Wallpaper Get(int id)
       {
           return this.wallpapersRepository.Get(id);
       }
   }
}
