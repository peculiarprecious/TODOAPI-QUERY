using Microsoft.EntityFrameworkCore;
using TODOAPI_QUERY.Models;

namespace TODOAPI_QUERY.DatabaseContext
{
    public class TodoDbContext:DbContext
    {
        // Constructor passing options to the base DbContext
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        // The database table matching your requested entity name
        public DbSet<TodoItem> TodoItems { get; set; } 
    }
}
