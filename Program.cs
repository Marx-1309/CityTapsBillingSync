using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CityTapsBillingSync.Data;
using CityTapsBillingSync.Services.IService;
using CityTapsBillingSync.Services;
using CityTapsBillingSync.Service.IService;
using CityTapsBillingSync.Models;
using Microsoft.AspNetCore.Identity;
namespace CityTapsBillingSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
   

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped</*IBaseService, */BaseService>();
            builder.Services.AddHttpClient<ICityTapsService, CityTapService>();
            //builder.Services.AddHttpClient<IAuthService, AuthService>();
            builder.Services.AddScoped<ICityTapsService, CityTapService>();


            
            builder.Services.AddDbContext<CityTapsBillingSyncContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CityTapsBillingSyncContext")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<CityTapsBillingSyncContext>();
            builder.Services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);
                opt.SignIn.RequireConfirmedEmail = false;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

             app.Run();
        }
    }
}
