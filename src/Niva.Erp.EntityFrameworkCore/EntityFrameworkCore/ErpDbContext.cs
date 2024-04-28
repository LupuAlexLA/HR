using System.Threading.Tasks;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Niva.Erp.Authorization.Roles;
using Niva.Erp.Authorization.Users;
using Niva.Erp.EntityFrameworkCore.DbFunctions;
using Niva.Erp.Models;
using Niva.Erp.MultiTenancy;

namespace Niva.Erp.EntityFrameworkCore
{
    public class ErpDbContext : AbpZeroDbContext<Tenant, Role, User, ErpDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<Masuratoare> Masuratori { get; set; }

        public DbSet<MasuratoareInterpretare> MasuratoriInterpretari { get; set; }

        public DbSet<Studiu> Studii { get; set; }

        public DbSet<Tara> Tari { get; set; }

        public DbSet<Judet> Judete { get; set; }

        public ErpDbContext(DbContextOptions<ErpDbContext> options)
            : base(options)
        {
        }

        
        

     

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ...
            MyDbFunctions.Register(modelBuilder);
            // ...

            //preluate din vechi
            //modelBuilder.Properties<string>().Where(n => !n.Name.EndsWith("Lob")).Configure(s => s.HasMaxLength(2000));
           


            //modelBuilder.Configurations.Add(new RoleEfConfig());
            //modelBuilder.Configurations.Add(new UserEfConfig());
            //modelBuilder.Configurations.Add(new PersonEfConfig());

            //modelBuilder.Configurations.Add(new NaturalPersonEfConfig());
            //modelBuilder.Configurations.Add(new AppClientsEfConfig());
            //modelBuilder.Configurations.Add(new ThirdPartyEfConfig());

            //modelBuilder.Configurations.Add(new BankConfig());

            //modelBuilder.Configurations.Add(new ImoAssetStockConfig());

           

     

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }

    public static class SqlServerModelBuilderExtensions
    {
        public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale)
        {
            return builder.HasColumnType($"decimal({precision},{scale})");
        }

        public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale)
        {
            return builder.HasColumnType($"decimal({precision},{scale})");
        }


    }


}
