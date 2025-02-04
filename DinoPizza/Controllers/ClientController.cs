using DinoPizza.DataAccessLayer;
using DinoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DinoPizza.Controllers
{
    public class ClientController : Controller
    {
        private readonly DinoDBContext _db;

        public ClientController(DinoDBContext db)
        {
            _db = db;
        }

        // Список клиентов
        public IActionResult IndexClientView()
        {
            var clients = _db.Clients.ToList();
            return View(clients);
        }

        // Создание клиента
        public IActionResult CreateClientView()
        {
            return View();
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

        // Детали клиента
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
    }
}
