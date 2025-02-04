using System.Linq;
using DinoPizza.DataAccessLayer;
using DinoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DinoPizza.Controllers
{
    public class ManagerController : Controller
    {
        private readonly DinoDBContext _db;

        public ManagerController(DinoDBContext db)
        {
            _db = db;
        }

        // Список активных заказов
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
            var completedOrders = _db.Orders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled)
                .ToList(); // Получаем завершенные или отмененные заказы
            return View(completedOrders); // Передаем заказы в представление
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

        [HttpPost]
        public ActionResult SaveOrderStatuses(OrdersListModel model)
        {
            if (model?.Orders != null && ModelState.IsValid)
            {
                foreach (var order in model.Orders)
                {
                    var orderToUpdate = _db.Orders.Find(order.Id);
                    if (orderToUpdate != null && orderToUpdate.Status != order.Status)
                    {
                        orderToUpdate.Status = order.Status; // Обновляем статус заказа
                    }
                }

                _db.SaveChanges(); // Сохраняем изменения в базе данных
            }
            else
            {
                // Обработка ошибки, если model или Orders равны null
                ModelState.AddModelError("", "Невозможно обработать заказы.");
            }

            return RedirectToAction("IndexManagerView");
        }
    }
}
