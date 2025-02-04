using System;
using System.Linq;
using DinoPizza.Authorize;
using DinoPizza.BusinessLogic;
using DinoPizza.DataAccessLayer;
using DinoPizza.Helpers;
using DinoPizza.Domains;
using DinoPizza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static DinoPizza.Helpers.SessionExtension;
using System.Security.Claims;

namespace DinoPizza.Controllers
{
    public class OrderController : Controller
    {
        private readonly DinoDBContext db;
        private readonly UserManager<AppUser> _userManager;
        private readonly YandexGeocoderService _yandexGeocoderService;

        public OrderController(DinoDBContext context, UserManager<AppUser> userManager, YandexGeocoderService yandexGeocoderService)
        {
            db = context;
            _userManager = userManager;
            _yandexGeocoderService = yandexGeocoderService;
        }

        private Guid GetClientIdFromSession()
        {
            
            var clientId = User?.Identity?.Name;

            if (!string.IsNullOrEmpty(clientId) && Guid.TryParse(clientId, out Guid parsedClientId))
            {
                return parsedClientId;
            }

            var sessionId = HttpContext.Session.GetString("ClientId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("ClientId", sessionId);
            }

            return Guid.Parse(sessionId);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var clientId = ClientIdHelper.GetClientId(HttpContext.Session, User);

            var cart = SessionHelper.GetCartFromSession(HttpContext.Session, clientId);

            if (cart == null || !cart.Records.Any())
            {
                return PartialView("_CartContentPartial", new Cart());
            }

            // Передаем данные клиента в представление
            var clientModel = new ClientModel
            {
                FirstName = user.FirstName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            ViewData["Client"] = clientModel;

            return View("CheckoutOrderView", cart);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string customerName, string address, DateTime deliveryTime, PaymentMethod paymentMethod)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Получаем клиента из базы данных
            var client = await db.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null)
            {
                ModelState.AddModelError("Client", "Клиент не найден.");
                return View("CheckoutOrderView");
            }

            var clientId = ClientIdHelper.GetClientId(HttpContext.Session, User);

            var cart = SessionHelper.GetCartFromSession(HttpContext.Session, clientId);
            if (cart == null || !cart.Records.Any())
            {
                ViewData["ErrorMessage"] = "Ваша корзина пуста.";
                return PartialView("~/Views/Cart/_CartContentPartial.cshtml", new Cart());
            }

            if (string.IsNullOrEmpty(address))
            {
                ModelState.AddModelError("Address", "Адрес доставки не может быть пустым.");
                return PartialView("~/Views/Cart/_CartContentPartial.cshtml", cart);
            }

            if (string.IsNullOrEmpty(customerName))
            {
                ModelState.AddModelError("CustomerName", "Имя клиента не может быть пустым.");
                return PartialView("~/Views/Cart/_CartContentPartial.cshtml", cart); 
            }

            deliveryTime = deliveryTime == DateTime.MinValue ? DateTime.Now.ToLocalTime() : deliveryTime;

            var availableCourier = await db.Couriers
                .FirstOrDefaultAsync(c => c.IsAvailable);

            if (availableCourier == null)
            {
                ModelState.AddModelError("Courier", "Нет доступных курьеров.");
                return PartialView("~/Views/Cart/_CartContentPartial.cshtml", cart);
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ClientId = client.ClientId,
                CustomerName = customerName,
                Address = address,
                OrderDetails = JsonConvert.SerializeObject(cart.Records),
                OrderTime = DateTime.Now,
                CourierId = availableCourier.Id,
                DeliveryTime = deliveryTime,
                PaymentMethod = paymentMethod,
                Status = OrderStatus.InProgress
            };

            availableCourier.IsAvailable = false;
            db.Couriers.Update(availableCourier);

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            cart.Clear();
            SessionHelper.SetCartToSession(HttpContext.Session, cart, client.ClientId);

            return RedirectToAction("OrderDetailsView", "Order", new { orderId = order.Id });
        }

        public async Task<IActionResult> OrderDetailsView(Guid orderId)
        {
            var order = await db.Orders
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var orderProducts = JsonConvert.DeserializeObject<List<OrderProduct>>(order.OrderDetails);

            ViewBag.OrderProducts = orderProducts;

            return View(order);
        }


        public ActionResult CheckoutDetails(Guid orderId)
        {
            var order = db.Orders
                .Include(o => o.Client)
                .Include(o => o.Courier)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Заказ не найден.");
            }

            return View(order);
        }

        public ActionResult OrdersList()
        {
            var orders = db.Orders
                .Include(o => o.Client)
                .Include(o => o.Courier)
                .ToList();

            return View(new OrdersListModel
            {
                Orders = orders,
                AvailableCouriers = db.Couriers.ToList()
            });
        }
        [HttpPost]
        public ActionResult UpdateOrderStatus(List<Order> Orders)
        {
            foreach (var order in Orders)
            {
                var existingOrder = db.Orders.Include(o => o.Courier).FirstOrDefault(o => o.Id == order.Id);

                if (existingOrder == null)
                {
                    return NotFound("Заказ не найден.");
                }

                // Обновляем статус заказа
                existingOrder.Status = order.Status;

                // Если статус заказа изменен на Completed или Cancelled
                if (existingOrder.Status == OrderStatus.Completed || existingOrder.Status == OrderStatus.Cancelled)
                {
                    // Если курьер назначен на заказ, делаем его доступным
                    if (existingOrder.Courier != null)
                    {
                        existingOrder.Courier.IsAvailable = true;
                        db.Couriers.Update(existingOrder.Courier); // Обновляем курьера в базе данных
                    }
                }

                db.SaveChanges(); // Сохраняем изменения в базе данных
            }

            return RedirectToAction("IndexManagerView", "ContentManager");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateAddress(string newAddress)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.Address = newAddress;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Checkout");
        }

    }
}
