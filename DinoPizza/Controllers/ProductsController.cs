using DinoPizza.Abstract;
using DinoPizza.Domains;
using DinoPizza.Models;
using DinoPizza.DataStorage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using DinoPizza.Models.Product;

namespace DinoPizza.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository<Product, long> _productRepo;
        private readonly IRepository<Category, long> _categoryRepo;
        private readonly FileStorageProvider _fileStorageProvider;

        public ProductsController(IRepository<Product, long> productRepo, IRepository<Category, long> categoryRepo, FileStorageProvider fileStorageProvider)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _fileStorageProvider = fileStorageProvider;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepo.GetAll().ToList();
            ViewData["Categories"] = categories;  // Передаем категории в макет через ViewData
            return View();
        }

        // Загрузка товаров из JSON
        public IActionResult LoadProductsFromJson()
        {
            string jsonFilePath = Path.Combine(_fileStorageProvider.GetPath(), "Products", "products.json");
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return NotFound("JSON файл не найден");
            }

            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var products = JsonConvert.DeserializeObject<List<ProductJsonModel>>(jsonContent);

            // Сохраняем в базу данных
            foreach (var product in products)
            {
                var category = _categoryRepo.GetAll().FirstOrDefault(c => c.NameRus == product.Category);
                if (category != null)
                {
                    var newProduct = new Product
                    {
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Article = product.Article,
                        ImageUrl = product.ImageFileName,
                        CategoryId = category.CategoryId
                    };

                    _productRepo.Create(newProduct);
                }
            }

            return Ok("Товары загружены в базу данных");
        }

        // Отображение товаров по категории
        public IActionResult ProductsInCategory(long categoryId)
        {
            var category = _categoryRepo.GetAll().FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound("Категория не найдена");
            }

            var products = _productRepo.GetAll().Where(p => p.CategoryId == categoryId).ToList();
            var model = new ProductsListModel
            {
                PageProducts = products.Select(p => new ProductSimpleModel
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageSrc = p.ImageUrl // Путь к картинке товара
                }).ToList(),
                PageCount = (int)Math.Ceiling((double)products.Count / 10),  // Пагинация по 10 товаров
                PageActive = 1  // Активная страница (по умолчанию первая)
            };

            return View(model);
        }
    }
}
