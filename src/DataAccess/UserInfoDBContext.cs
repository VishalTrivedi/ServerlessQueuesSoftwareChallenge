using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class UserInfoDBContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserInfoDBContext()
    {
        
    }

    public UserInfoDBContext(DbContextOptions<UserInfoDBContext> options)
        : base(options)
    {
        
    }
}
