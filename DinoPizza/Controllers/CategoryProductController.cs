using DinoPizza.Abstract;
using DinoPizza.Domains;
using DinoPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace DinoPizza.Controllers
{
    public class CategoryProductController : Controller
    {
        private readonly IRepository<Category, long> _categoryRepo;
        private readonly IRepository<Product, long> _productRepo;

        public CategoryProductController(IRepository<Category, long> categoryRepo, IRepository<Product, long> productRepo)
        {
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
        }

        public IActionResult Index(long? categoryId)
        {
            var categories = _categoryRepo
                .GetAll()
                .Select(c => new CategoryModel
                {
                    CategoryId = c.CategoryId,
                    NameRus = c.NameRus
                })
                .ToList();

            // Если передан categoryId, то передаем его как активный
            ViewData["ActiveCategoryId"] = categoryId;

            return View(categories);
        }

        // Страница с продуктами по выбранной категории
        public IActionResult Products(long categoryId)
        {
            var category = _categoryRepo.GetAll()
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new CategoryModel
                {
                    CategoryId = c.CategoryId,
                    NameRus = c.NameRus
                })
                .FirstOrDefault();

            if (category == null)
                return NotFound();

            var products = _productRepo
                .GetAll()
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductSimpleModel
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToList();

            var model = new ProductsListModel
            {
                PageProducts = products,
                PageCount = (int)Math.Ceiling((double)products.Count / 10),  // Пагинация по 10 продуктов
                PageActive = 1,  // Активная страница (по умолчанию первая)
                CategoryId = categoryId  // Передаем выбранную категорию
            };

            ViewData["ActiveCategoryId"] = categoryId;

            return View(model);
        }

    }
}
