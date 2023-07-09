using AdvancedAuth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAuth.Context
{
    public class AppDbContext : IdentityDbContext<User,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions options):base(options)
        {
        }


    }
}
