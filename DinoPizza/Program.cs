using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.Extensions.Options;
using System.Reflection;
using DinoPizza.Authorize;
using DinoPizza.BusinessLogic;
using DinoPizza.DataAccessLayer;
using DinoPizza.DataStorage;
using DinoPizza.Helpers;
using DinoPizza.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DinoPizza
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.SetupRepository();

            // ����������� DbContext
            builder.Services.AddDbContext<DinoDBContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ����������� Identity
            builder.Services.AddDefaultIdentity<AppUser>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@.?!#$%&'*+()������������������������������������Ũ�������������������������";
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DinoDBContext>()
            .AddDefaultUI();

            // ����������� Mapster
            MyMapper.RegisterSettings();
            builder.Services.AddMapster();

            // ����������� �������������� ��������
            builder.Services.AddTransient<FileStorageProvider>();
            builder.Services.AddTransient<DataSeeder>();
            builder.Services.AddTransient<ServiceContentManager>();
            builder.Services.AddTransient<ServiceAdmin>();
            builder.Services.AddScoped<YandexGeocoderService>();

            // ����������� CartService
            builder.Services.AddScoped<ICartService, CartService>();

            // ������������ ���� � ������
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromSeconds(60);
                opt.Cookie.HttpOnly = true;
                opt.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // ������������ ��������� ������ � ������������
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // ����������� �������������
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            // ��������� �������� ������������
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "cart-content",
                pattern: "{controller=Cart}/{action=GetCartContent}");

            app.MapRazorPages();

            // ����� ������� ������
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
            }



            app.Run();
        }
    }
}
