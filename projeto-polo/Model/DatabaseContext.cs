using Microsoft.EntityFrameworkCore;

namespace projeto_polo.Model
{
    class DatabaseContext : DbContext
    {
        public DbSet<Export> Exports { get; set; }
        public DbSet<ExportItem> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ProjetoPolo.db");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
