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

            // Регистрация DbContext
            builder.Services.AddDbContext<DinoDBContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Регистрация Identity
            builder.Services.AddDefaultIdentity<AppUser>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@.?!#$%&'*+()абвгдеёжзийклмнопрстуфхцчшщьыэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЫЭЮЯ";
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DinoDBContext>()
            .AddDefaultUI();

            // Регистрация Mapster
            MyMapper.RegisterSettings();
            builder.Services.AddMapster();

            // Регистрация дополнительных сервисов
            builder.Services.AddTransient<FileStorageProvider>();
            builder.Services.AddTransient<DataSeeder>();
            builder.Services.AddTransient<ServiceContentManager>();
            builder.Services.AddTransient<ServiceAdmin>();
            builder.Services.AddScoped<YandexGeocoderService>();

            // Регистрация CartService
            builder.Services.AddScoped<ICartService, CartService>();

            // Конфигурация кэша и сессий
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromSeconds(60);
                opt.Cookie.HttpOnly = true;
                opt.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Конфигурация обработки ошибок и безопасности
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Стандартная маршрутизация
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            // Настройка маршрута контроллеров
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "cart-content",
                pattern: "{controller=Cart}/{action=GetCartContent}");

            app.MapRazorPages();

            // Вызов сеедера данных
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
            }



            app.Run();
        }
    }
}
