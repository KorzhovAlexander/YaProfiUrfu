using Microsoft.EntityFrameworkCore;
using YaProfiUrfu.Entity;

namespace YaProfiUrfu
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
    }
}