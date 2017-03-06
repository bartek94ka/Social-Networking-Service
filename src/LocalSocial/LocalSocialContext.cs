using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using LocalSocial.Models;
using Microsoft.Data.Entity.Infrastructure;

namespace LocalSocial
{
    public class LocalSocialContext : IdentityDbContext<User>
    {
        // Your context has been configured to use a 'LocalSocialContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'LocalSocial.LocalSocialContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'LocalSocialContext' 
        // connection string in the application configuration file.


        public DbSet<Post> Post { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<UserFriends> UserFriends { get; set; }
        public DbSet<PostTags> PostTags { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=tcp:poznan.database.windows.net,1433;Initial Catalog=LocalSocial;Persist Security Info=False;User ID=poznan;Password=Kaczka1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFriends>().HasKey(x => new { x.UserId, x.FriendId });

            modelBuilder.Entity<PostTags>().HasKey(x => new { x.PostId, x.TagId });

            //modelBuilder.Entity<UserFriends>()
            //    .HasOne(pc => pc.user)
            //    .WithMany(p => p.Friends)
            //    .HasForeignKey(pc => pc.FriendId);

            //modelBuilder.Entity<UserFriends>()
            //    .HasOne(pc => pc.friend)
            //    .WithMany(c => c.Friends)
            //    .HasForeignKey(pc => pc.UserId);

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
