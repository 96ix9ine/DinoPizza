using Microsoft.EntityFrameworkCore;
using DinoPizza.Abstract;
using DinoPizza.DataAccessLayer;
using DinoPizza.Domains;
using DinoPizza.Models;

namespace DinoPizza.Helpers
{
    public static class WebBuilderHelper
    {
        public static void DbSeedWithScope(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var container = scope.ServiceProvider;

                var seeder = container.GetService<DataSeeder>();
                if (seeder != null)
                {
                    seeder.SeedAsync().Wait();
                }
            }
        }

        private static void AddRepository<T>(IServiceCollection services)
            where T : MyEntity<long>
        {
            services.AddTransient<IRepository<T, long>, RepositoryGenericSql<T, long>>();
        }

        public static void SetupRepository(this WebApplicationBuilder builder)
        {
            // Проверяем, какой репозиторий нужно использовать
            var section = builder.Configuration.GetSection("RepositoryType");

            if (section.Value == "Sql")
            {
                // Строка подключения из конфигурации
                var connectionString = builder
                    .Configuration
                    .GetConnectionString("DefaultConnection") ??
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                builder.Services.AddDbContext<DinoDBContext>(
                    options => options.UseSqlServer(connectionString));

                builder.Services.AddScoped<DinoDBContext>();

                // Репозитории для необходимых сущностей
                AddRepository<Product>(builder.Services);
                AddRepository<Category>(builder.Services);

                // Если вам нужно работать с клиентами, заказами и курьерами, можно раскомментировать следующие строки:
                /*
                AddRepository<ClientModel>(builder.Services);
                AddRepository<Order>(builder.Services);
                AddRepository<Feedback>(builder.Services);
                AddRepository<Courier>(builder.Services);
                */
            }
            else
            {
                throw new Exception("Not supported repository type!");
            }
        }
    }
}
