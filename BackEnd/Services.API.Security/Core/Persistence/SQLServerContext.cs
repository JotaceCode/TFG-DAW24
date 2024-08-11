using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Services.API.Security.Core.Entities;

namespace Services.API.Security.Core.Persistence
{
    // Context for interact with the SQL Server Database
    public class SQLServerContext: IdentityDbContext<UserEntity>
    {
        public SQLServerContext(DbContextOptions options): base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
