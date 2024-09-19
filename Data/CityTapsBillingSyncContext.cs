using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CityTapsBillingSync.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace CityTapsBillingSync.Data
{
    public class CityTapsBillingSyncContext : IdentityDbContext
    {
        public CityTapsBillingSyncContext(DbContextOptions<CityTapsBillingSyncContext> options) : base(options) { }

        public DbSet<CTaps_Reading> CTaps_Reading { get; set; } = default!;
        public DbSet<BS_WaterReadingExport> BS_WaterReadingExport { get; set; } = default!;
        public DbSet<CTaps_UploadInstance> CTaps_UploadInstance { get; set; } = default!;
        public DbSet<BS_WaterReadingExportData> BS_WaterReadingExportData { get; set; } = default!;
        public DbSet<CTaps_DebtorCityTap> BS_DebtorCityTap { get; set; } = default!;
        public DbSet<BS_Month> BS_Month { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //Ignore the Pay_Month entity during migrations
        //    modelBuilder.Ignore<CTReading>();
        //    modelBuilder.Ignore<BS_WaterReadingExport>();
        //    //modelBuilder.Ignore<BS_UploadInstance>();
        //    modelBuilder.Ignore<BS_WaterReadingExportData>();
        //    modelBuilder.Ignore<BS_DebtorCityTap>();
        //    modelBuilder.Ignore<BS_Month>();
        //    modelBuilder.Ignore<ApplicationUser>();

        //}
    }
}
