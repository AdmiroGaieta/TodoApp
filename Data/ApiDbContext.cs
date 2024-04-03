using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoAPP.Models;

namespace TodoAPP.Data
{
    public class ApiDbContext : IdentityDbContext<IdentityUser>
    {
        public virtual DbSet<ItemData> Items { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    }
}
