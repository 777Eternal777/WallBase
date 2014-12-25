using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallBase.Core.Domain.Media;

namespace EfContext
{
    public class WallbaseDB : DbContext
    {
        public WallbaseDB()
            : base()
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {//    modelBuilder.Entity<Friend>().HasRequired(q => q.User).WithMany(w => w.UserFriends).HasForeignKey(q => q.UserId).WillCascadeOnDelete(false);
            //          modelBuilder.Entity<UserRewards>().HasRequired(q => q.User).WithMany(w => w.Rewards).HasForeignKey(q => q.UserId);
            //  modelBuilder.Entity<Friend>().HasKey(t => new { t.UserId, t.FriendId });
            //modelBuilder.Entity<User>().HasMany(m => m.RequestUsers).WithMany().Map(
            //   p =>
            //   {
            //       p.MapLeftKey("UserId");
            //       p.MapRightKey("RequestUserId");
            //       p.ToTable("FriendRequests");
            //   });
         //   modelBuilder.Entity<UserCardInTeam>().HasRequired(q => q.Card).WithMany(w => w.TeamCards).HasForeignKey(q => q.CardId).WillCascadeOnDelete(true);
            modelBuilder.Entity<Wallpaper>().ToTable("Wallpaper1");
        }
        public WallbaseDB(string connectionstring)
            : base(connectionstring)
        {

        }
        public DbSet<Wallpaper> Wallpapers;

        public DbSet<Tag> Tags;

        public DbSet<Category> Categories;
    }
}
