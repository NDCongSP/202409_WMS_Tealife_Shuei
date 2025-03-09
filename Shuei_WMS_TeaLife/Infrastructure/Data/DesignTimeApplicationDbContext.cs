using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class DesignTimeApplicationDbContext : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder =new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer(@"Server=49.212.161.31;Initial Catalog=FBT_DEV2;Persist Security Info=False;User ID=sa;Password=@shuei249;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=300;", b =>
        {
            b.MigrationsHistoryTable("__EFMigrationsHistoryWMS");
        });
        return new ApplicationDbContext(builder.Options);
    }
}
