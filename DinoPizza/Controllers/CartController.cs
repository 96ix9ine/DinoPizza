using Azure.Core;
using DinoPizza.Abstract;
using DinoPizza.BusinessLogic;
using DinoPizza.DataAccessLayer;
using DinoPizza.Domains;
using DinoPizza.Helpers;
using DinoPizza.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Policy;
using static DinoPizza.Helpers.SessionExtension;

public class CartController : Controller
{
    private readonly IRepository<Product, long> _repo;
    private readonly IMapper _mapper;
    private readonly DinoDBContext _db;

    public CartController(
        IRepository<Product, long> repo,
        IMapper mapper,
        DinoDBContext context)
    {
        _repo = repo;
        _mapper = mapper;
        _db = context;
    }

    // Метод для обновления корзины в сессии
    private IActionResult UpdateCart(Action<Cart, ProductSimpleModel?> actionCart, long? id = null)
    {
        ProductSimpleModel? model = null;

        if (id.HasValue)
        {
            var entity = _repo.Read(id.Value);
            if (entity == null)
            {
                ViewData["ErrorMessage"] = $"Товар с Айди {id} не найден";
                return View("Error");
            }
            model = _mapper.Map<ProductSimpleModel>(entity);
        }

        // Унифицируем получение ClientId
        var clientId = ClientIdHelper.GetClientId(HttpContext.Session, User);

        // Загружаем корзину из сессии
        var cart = SessionHelper.GetCartFromSession(HttpContext.Session, clientId);

        // Если корзина не найдена, создаем новую
        if (cart == null)
        {
            cart = new Cart { ClientId = clientId };
        }

        // Выполняем действие с корзиной
        actionCart(cart, model);

        // Сохраняем корзину обратно в сессию
        SessionHelper.SetCartToSession(HttpContext.Session, cart, clientId);

        // Возвращаем пользователя обратно на страницу, откуда он пришел
        string? returnUrl = Request.Headers["Referer"];
        return string.IsNullOrEmpty(returnUrl)
            ? PartialView("_CartContentPartial", cart)
            : Redirect(returnUrl);
    }

    [HttpPost]
    public IActionResult Clear()
    {
        return UpdateCart((c, m) => c.Clear());
    }

    [HttpPost]
    public IActionResult AddItem(long id)
    {
        return UpdateCart((c, m) => c.AddProduct(m), id);
    }


    [HttpPost]
    public IActionResult RemoveItem(long id)
    {
        return UpdateCart((c, m) => c.RemoveProduct(m), id);
    }

    [HttpPost]
    public IActionResult RemoveRecord(long id)
    {
        return UpdateCart((c, m) => c.RemoveRecord(m), id);
    }

    // Получить идентификатор клиента из сессии
    private Guid GetClientIdFromSession()
    {
        // Попытка извлечь clientId из аутентификации (например, имя пользователя)
        var clientId = User?.Identity?.Name; // Пример для авторизации, где Name - это уникальный идентификатор пользователя

        // Проверяем, если clientId не пуст и является допустимым GUID
        if (!string.IsNullOrEmpty(clientId) && Guid.TryParse(clientId, out Guid parsedClientId))
        {
            return parsedClientId;  // Если это действительный GUID, возвращаем его
        }

        // Если аутентификация не найдена или clientId не является GUID, используем сессию
        var sessionId = HttpContext.Session.GetString("ClientId");
        if (string.IsNullOrEmpty(sessionId))
        {
            // Если еще нет идентификатора в сессии, создаем новый и сохраняем его
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("ClientId", sessionId);
        }

        // Возвращаем уникальный идентификатор, хранящийся в сессии
        return Guid.Parse(sessionId);  // Если это не GUID, он будет корректно преобразован здесь
    }

    [HttpGet]
    public IActionResult GetCartContent()
    {
        // Загружаем корзину из сессии
        var cart = SessionHelper.GetCartFromSession(HttpContext.Session, GetClientIdFromSession()) ?? new Cart();

        // Возвращаем частичное представление с содержимым корзины
        return PartialView("_CartContentPartial", cart);
    }

}
