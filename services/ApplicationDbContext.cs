using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

namespace ToDoApp.services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
                
        }
        public DbSet<ToDo> ToDos { get; set; }
    }
}
