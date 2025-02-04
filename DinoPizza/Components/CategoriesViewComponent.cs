using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using DinoPizza.Abstract;
using DinoPizza.Domains;
using DinoPizza.Models;

namespace DinoPizza.Components
{
    [ViewComponent]
    public class CategoriesViewComponent 
        : ViewComponent
    {
        private readonly IRepository<Category, long> _repo;
        private readonly IMapper _mapper;

        public CategoriesViewComponent(IRepository<Category, long> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke()
        {
            // Получаем все категории без фильтрации
            var categories = _repo
                .GetAll()  // Получаем все категории
                .Select(_mapper.Map<CategoryModel>)  // Преобразуем в нужную модель
                .ToList();

            return View(categories);  // Отображаем в представлении
        }
    }
}
