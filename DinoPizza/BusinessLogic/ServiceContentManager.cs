using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DinoPizza.Domains;
using DinoPizza.Models;
using DinoPizza.Authorize;
using DinoPizza.DataAccessLayer;
using DinoPizza.Models.Categories;

namespace DinoPizza.BusinessLogic
{
    public class ServiceContentManager
    {
        private readonly DinoDBContext _context;
        private readonly IMapper _mapper;

        public ServiceContentManager(DinoDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private void PrepareRelations(ProductEditModel model, Product entity)
        {
            model.SelectedCategoryId = entity.Category?.CategoryId ?? 0;
            model.CategoriesList = _context.Categories
                .Select(c => new SelectListItem()
                {
                    Text = c.NameRus,
                    Value = c.CategoryId.ToString(),
                    Selected = c.CategoryId == model.SelectedCategoryId
                })
                .ToList();

            model.CategoriesList.Insert(0, new SelectListItem
            {
                Text = "",
                Value = null,
                Selected = entity.Category == null
            });
        }

        public ProductEditModel GetProductEditModel(long productId)
        {
            var entity = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == productId);

            if (entity == null) throw new Exception("Product not found");

            var model = _mapper.Map<ProductEditModel>(entity);
            PrepareRelations(model, entity);
            return model;
        }

        public void UpdateProduct(ClaimsPrincipal user, ProductEditModel model)
        {
            var entity = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == model.ProductId);

            if (entity == null) throw new Exception("Product not found");

            FillProductProperties(user, entity, model);
        }

        private void FillProductProperties(ClaimsPrincipal user, Product entity, ProductEditModel model)
        {
            int oldPrice = entity.Price;

            _context.Entry(entity).CurrentValues.SetValues(model);

            if (user == null || !user.CanEditProductPrice())
            {
                entity.Price = oldPrice;
            }

            if (model.FileImage != null)
            {
                UploadFileImage(model);
                entity.ImageUrl = model.ImageUrl;
            }

            if (entity.Category == null || entity.Category.CategoryId != model.SelectedCategoryId)
            {
                entity.Category = model.SelectedCategoryId != null
                    ? _context.Categories.Find(model.SelectedCategoryId)
                    : null;
            }

            _context.SaveChanges();
        }

        private void UploadFileImage(ProductEditModel model)
        {
            string extension = Path.GetExtension(model.FileImage.FileName);
            string filename = Guid.NewGuid() + extension;
            model.ImageUrl = filename;

            string fullFilename = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "FileStorage",
                "ProductImages",
                filename);

            using (var stream = File.Create(fullFilename))
            {
                model.FileImage.CopyTo(stream);
            }
        }

        public ProductEditModel CreateProductEditModel()
        {
            var model = new ProductEditModel();
            PrepareRelations(model, new Product());
            return model;
        }

        public void CreateProduct(ClaimsPrincipal user, ProductEditModel model)
        {
            var entity = new Product();
            _context.Products.Add(entity);
            FillProductProperties(user, entity, model);
        }

        public ManageCategoriesModel GetManageCategoriesModel()
        {
            var categories = _context.Categories.ToList();
            return new ManageCategoriesModel
            {
                Categories = categories
            };
        }
    }
}
