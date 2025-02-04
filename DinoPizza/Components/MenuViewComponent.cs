using DinoPizza.Abstract;
using DinoPizza.Domains;
using DinoPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace DinoPizza.Components
{
    [ViewComponent]
    public class MenuViewComponent : ViewComponent
    {
        private readonly IRepository<Product, long> _productRepo;

        public MenuViewComponent(IRepository<Product, long> productRepo)
        {
            _productRepo = productRepo;
        }

        public IViewComponentResult Invoke()
        {
            var products = _productRepo
                .GetAll()
                .Select(p => new ProductSimpleModel
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageSrc = p.ImageUrl
                })
                .ToList();

            return View(products);
        }
    }
}
