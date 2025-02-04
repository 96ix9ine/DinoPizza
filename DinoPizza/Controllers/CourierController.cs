using System.Linq;
using System.Security.Claims;
using DinoPizza.DataAccessLayer;
using DinoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DinoPizza.Controllers
{
    public class CourierController : Controller
    {
        private readonly DinoDBContext _db;

        public CourierController(DinoDBContext db)
        {
            _db = db;
        }
        public ActionResult OrdersCourierView()
        {
            // Получаем UserId (clientId) текущего пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Для извлечения UserId из Claims

            if (userId == null)
            {
                // Если нет аутентифицированного пользователя, можно вывести ошибку или перенаправить на страницу логина
                return RedirectToAction("Login", "Account");
            }

            // Найдем курьера по UserId
            var courier = _db.Couriers.FirstOrDefault(c => c.UserId == userId);

            if (courier == null)
            {
                ViewBag.Message = "Курьер не найден.";
                return View();
            }

            // Получаем заказы, связанные с этим курьером
            var orders = _db.Orders
                            .Include(o => o.Courier)
                            .Where(o => o.CourierId == courier.Id)
                            .ToList();

            if (!orders.Any())
            {
                ViewBag.Message = "У вас нет активных заказов.";
            }

            return View(orders);
        }

        [HttpPost]
        public ActionResult CompleteOrder(Guid orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null && order.Status == OrderStatus.InProgress)
            {
                order.Status = OrderStatus.Completed;
                var courier = _db.Couriers.Find(order.CourierId);
                if (courier != null)
                {
                    courier.IsAvailable = true;
                    _db.Couriers.Update(courier);
                }
                _db.SaveChanges();
            }
            return RedirectToAction("OrdersCourierView");
        }

    }
}
