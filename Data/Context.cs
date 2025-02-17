using BackTareas.Models;
using Microsoft.EntityFrameworkCore;

namespace BackTareas.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }
        public DbSet<Work> Works { get; set; }  
        public DbSet<User> Users { get; set; }
    }
}
