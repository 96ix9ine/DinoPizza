using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DinoPizza.Abstract;
using DinoPizza.Domains;
using DinoPizza.BusinessLogic;
using DinoPizza.Models;
using DinoPizza.Authorize;
using DinoPizza.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DinoPizza.Controllers
{
    [Authorize(Roles = $"{AuthConstants.Roles.Admin},{AuthConstants.Roles.ContentManager}")]
    public class ContentManagerController : Controller
    {
        private readonly ServiceContentManager _contentManager;
        private readonly IRepository<Product, long> _repoProducts;
        private readonly IRepository<Category, long> _repoCategories;
        private readonly IMapper _mapper;
        private readonly DinoDBContext _db;

        public ContentManagerController(
            ServiceContentManager contentManager,
            IRepository<Product, long> repoProducts,
            IRepository<Category, long> repoCategories,
            IMapper mapper,
            DinoDBContext db)
        {
            _contentManager = contentManager;
            _repoProducts = repoProducts;
            _repoCategories = repoCategories;
            _mapper = mapper;
            _db = db;
        }

        // Просмотр и редактирование категории
        public IActionResult EditCategoryView(long categoryId)
        {
            var categoryModel = _repoCategories.Read(categoryId).Adapt<CategoryModel>();
            return View(categoryModel);
        }

        // Создание новой категории
        public IActionResult CreateCategoryView()
        {
            var model = new CategoryModel();
            return View(nameof(EditCategoryView), model);
        }

        [HttpPost]
        public IActionResult EditCategoryView(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _mapper.Map<Category>(model);

                if (entity.CategoryId == 0)
                    _repoCategories.Create(entity);
                else
                    _repoCategories.Update(entity);

                string url = Url.Action(nameof(ManageCategoriesView), "ContentManager");
                return Redirect(url);
            }

            return View(model);
        }

        // Управление категориями
        public IActionResult ManageCategoriesView()
        {
            var categories = _repoCategories
                .GetAll()
                .ProjectToType<CategorySimpleModel>()
                .ToList();

            return View(categories);
        }

        // Страница создания нового товара
        public IActionResult CreateProductView()
        {
            var model = _contentManager.CreateProductEditModel();
            model.ReturnUrl = Request.Headers["Referer"];
            return View("ProductEditView", model);
        }

        // Страница редактирования товара
        public IActionResult ProductEditView(long id)
        {
            var model = _contentManager.GetProductEditModel(id);
            model.ReturnUrl = Request.Headers["Referer"];
            return View(model);
        }

        [HttpPost]
        public IActionResult ProductEditView(ProductEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProductId == 0)
                    _contentManager.CreateProduct(User, model);
                else
                    _contentManager.UpdateProduct(User, model);

                string returnUrl = model.ReturnUrl ?? Url.Action("List", "Products");
                return Redirect(returnUrl);
            }

            return View(model);
        }

        // Главная страница управления контентом
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult IndexManagerView()
        {
            // Создание и инициализация модели
            var viewModel = new OrdersListModel
            {
                Orders = _db.Orders
                    .Include(o => o.Client)
                    .Include(o => o.Courier)
                    .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress)
                    .ToList(),
                AvailableCouriers = _db.Couriers.Where(c => c.IsAvailable).ToList()
            };

            return View(viewModel); // Передача модели в представление
        }


        // Отображение страницы с курьерами
        public ActionResult CouriersManagerView()
        {
            var couriers = _db.Couriers.ToList(); // Получаем список курьеров
            return View(couriers); // Передаем курьеров в представление
        }

        // Архив заказов
        public ActionResult ArchiveManagerView()
        {
            // Получаем завершенные или отмененные заказы
            var completedOrders = _db.Orders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled)
                .ToList();

            // Десериализация OrderDetails в список OrderProduct для каждого заказа
            foreach (var order in completedOrders)
            {
                if (!string.IsNullOrEmpty(order.OrderDetails))
                {
                    // Десериализация OrderDetails в список OrderProduct
                    var orderProducts = JsonConvert.DeserializeObject<List<OrderProduct>>(order.OrderDetails);
                    order.OrderDetails = string.Join(", ", orderProducts.Select(p => $"{p.Product.Name} - {p.Quantity} x {p.Product.Price} руб.")); // Формируем строку с названием продукта, количеством и ценой
                }
            }

            return View(completedOrders); // Передаем заказы с преобразованными данными в представление
        }

        // Назначение курьера (POST-запрос)
        [HttpPost]
        public ActionResult AssignCourier(int orderId, int courierId)
        {
            var order = _db.Orders.Find(orderId);
            var courier = _db.Couriers.Find(courierId);

            if (order != null && courier != null && courier.IsAvailable)
            {
                order.CourierId = courier.Id;
                order.Status = OrderStatus.InProgress;
                courier.IsAvailable = false;
                _db.SaveChanges();
            }
            return RedirectToAction("IndexManagerView");
        }

        /* ----------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------- */
        /* ----------------------------------------------------------------------- */

        public IActionResult CreateClientView()
        {
            return View();
        }

        public IActionResult IndexClientView()
        {
            var clients = _db.Clients.ToList();
            return View(clients);
        }

        [HttpPost]
        public IActionResult Create(ClientModel client)
        {
            if (ModelState.IsValid)
            {
                _db.Clients.Add(client);
                _db.SaveChanges();
                return RedirectToAction("IndexClientView");
            }
            return View(client);
        }

        public IActionResult DetailsClientView(int id)
        {
            var client = _db.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        public ActionResult ClientOrdersListView(Guid clientId)
        {
            // Получение клиента с его заказами
            var clientOrders = _db.Orders
                .Include(o => o.Courier) // Подгружаем курьера
                .Include(o => o.Client)  // Подгружаем данные клиента
                .Where(o => o.ClientId == clientId) // Фильтруем заказы по клиенту
                .ToList();

            if (!clientOrders.Any())
            {
                return NotFound("Заказы клиента не найдены."); // Если заказов нет
            }

            ViewBag.ClientName = clientOrders.First().Client.FirstName; // Имя клиента для отображения
            return View(clientOrders); // Передаем заказы в представление
        }

        [HttpPost]
        public ActionResult ChangeOrderStatus(int orderId, string newStatus)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                // Преобразуем строковое значение статуса обратно в Enum
                if (Enum.TryParse(newStatus, out OrderStatus status))
                {
                    order.Status = status;
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("IndexManagerView");
        }
    }
}
