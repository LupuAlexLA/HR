using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Niva.Erp.Configuration;
using Niva.Erp.Web;

namespace Niva.Erp.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ErpDbContextFactory : IDesignTimeDbContextFactory<ErpDbContext>
    {
        public ErpDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(),"development");

            ErpDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ErpConsts.ConnectionStringName));

            return new ErpDbContext(builder.Options);
        }
    }
}
