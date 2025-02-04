using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using DinoPizza.Authorize;
using DinoPizza.Domains;
using Microsoft.EntityFrameworkCore;
using DinoPizza.Models;
using DinoPizza.Models.Product;
using DinoPizza.DataStorage;

namespace DinoPizza.DataAccessLayer
{
    public class DataSeeder
    {
        private readonly DinoDBContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly FileStorageProvider _fileStorageProvider;

        public DataSeeder(DinoDBContext context, IServiceProvider serviceProvider, FileStorageProvider fileStorageProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _fileStorageProvider = fileStorageProvider;
        }

        public async Task SeedAsync()
        {
            await SeedUserRolesAsync();  // Инициализация ролей
            SeedCategories();  // Инициализация категорий
            await LoadProductsFromJson();  // Загрузка товаров из JSON
        }

        private async Task LoadProductsFromJson()
        {
            string jsonFilePath = Path.Combine(_fileStorageProvider.GetPath(), "Products", "products.json");
            if (!System.IO.File.Exists(jsonFilePath))
            {
                Console.WriteLine("JSON файл не найден");
                return;
            }

            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var products = JsonConvert.DeserializeObject<List<ProductJsonModel>>(jsonContent);

            // Сохраняем в базу данных
            foreach (var product in products)
            {
                var category = _context.Categories.FirstOrDefault(c => c.NameRus == product.Category);
                if (category != null)
                {
                    // Проверяем, существует ли продукт с таким же Article (уникальный идентификатор)
                    var existingProduct = await _context.Products
                                                         .FirstOrDefaultAsync(p => p.Article == product.Article);

                    // Если продукт уже существует, пропускаем его добавление
                    if (existingProduct != null)
                    {
                        Console.WriteLine($"Продукт с артикулом {product.Article} уже существует. Пропускаем...");
                        continue;
                    }

                    var newProduct = new Product
                    {
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Article = product.Article,
                        ImageUrl = product.ImageFileName,
                        CategoryId = category.CategoryId
                    };

                    _context.Products.Add(newProduct);
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Товары загружены в базу данных");
        }


        private async Task SeedUserRolesAsync()
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (roleManager.Roles.Any())
            {
                return;
            }

            string[] roleNames =
            {
                AuthConstants.Roles.Admin,
                AuthConstants.Roles.ContentManager,
                AuthConstants.Roles.Courier,
                "user"
            };

            foreach (string roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{roleName}'.");
                    }
                }
            }

            var userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();
            string defaultPassword = "Cnfhrjd20041!";

            var userRoles = new Dictionary<string, string>
            {
                { "sergstar2012@mail.ru", AuthConstants.Roles.Admin }
            };

            foreach (var userRole in userRoles)
            {
                var user = await userManager.FindByEmailAsync(userRole.Key);
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = userRole.Key,
                        Email = userRole.Key,
                        PhoneNumber = "+79514623254",
                        Address = "Благих 85",
                        FirstName = "Сергей",
                        IsClient = true
                    };
                    var createUserResult = await userManager.CreateAsync(user, defaultPassword);
                    if (!createUserResult.Succeeded)
                    {
                        throw new Exception($"Failed to create user '{userRole.Key}'.");
                    }

                    var client = new ClientModel
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address
                    };

                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                }

                if (!await userManager.IsInRoleAsync(user, userRole.Value))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(user, userRole.Value);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add user '{userRole.Key}' to role '{userRole.Value}'.");
                    }
                }
            }
        }

        private void SeedCategories()
        {
            var categories = new List<string>
            {
                "Пиццы", "Закуски", "Напитки", "Кофе", "Коктейли", "Десерты", "Соусы", "Комбо"
            };

            foreach (var categoryName in categories)
            {
                // Проверяем, если категория уже существует
                var existingCategory = _context.Categories.FirstOrDefault(c => c.NameRus == categoryName);
                if (existingCategory == null)
                {
                    // Добавляем новую категорию
                    var category = new Category { NameRus = categoryName };
                    _context.Categories.Add(category);
                }
            }

            _context.SaveChanges();
        }
    }
}
