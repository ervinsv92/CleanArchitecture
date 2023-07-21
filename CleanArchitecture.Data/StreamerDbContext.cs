

using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Data
{
    public class StreamerDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=localhost\sqlexpress; Initial Catalog=Streamer; Integrated Security=True")
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        //FluentAPI relacion de uno a muchos/muchos a muchos, es opcional si se siguen las convenciones de los nombres de los atributos de los objetos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Streamer>().HasMany(m => m.Videos).WithOne(m => m.Streamer).HasForeignKey(m => m.StreamerId).IsRequired().OnDelete(DeleteBehavior.Restrict);//para la llave de uno a muchos

            modelBuilder.Entity<Video>().HasMany(p => p.Actores).WithMany(t => t.Videos).UsingEntity<VideoActor>(pt => pt.HasKey(e => new { e.ActorId, e.VideoId }));//para la llave de muchos a muchos
        }
        public DbSet<Streamer>? Streamers { get; set; }
        public DbSet<Video>? Videos { get; set; }
        public DbSet<Actor>? Actor { get; set; }
        public DbSet<Director>? Director { get; set; }
    }
}
