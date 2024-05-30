using Atlas.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Database
{
    internal class DatabaseContext : DbContext
    {
        public DbSet<SavedAppsDb> Apps { get; set; }

        public string DbPath { get; }
        public DatabaseContext() 
        {
            //var folder = Environment.SpecialFolder.;
            //Environment.GetFolderPath(folder);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            DbPath = Path.Join(path, "atlas.db");

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    }
}
