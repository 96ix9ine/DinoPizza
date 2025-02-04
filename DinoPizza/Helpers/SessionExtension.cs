using Microsoft.AspNetCore.Http;
using DinoPizza.BusinessLogic;
using Newtonsoft.Json;
using DinoPizza.Models;
using System.Security.Claims;

namespace DinoPizza.Helpers
{
    public static class SessionExtension
    {
        public static void SaveObject<T>(
            this ISession session,  T something) 
            where T : class, new()
        {
            string jsonString = JsonConvert
                .SerializeObject(something);

            // ключ - имя класса
            string key = something.GetType().Name;
            session.SetString(key,jsonString);
        }

        public static T? LoadObject<T>(
            this ISession session, 
            bool isCreateNew = true) 
            where T : class, new()
        {
            string key = typeof(T).Name;
            string? jsonString = session.GetString(key);
            if (jsonString == null)
            {
                return isCreateNew ?
                    Activator.CreateInstance<T>() :
                    null;
            }
            else
            {
                return JsonConvert
                    .DeserializeObject<T>(jsonString);
            }
        }
        public static class ClientIdHelper
        {
            public static Guid GetClientId(ISession session, ClaimsPrincipal user)
            {
                // Проверяем, аутентифицирован ли пользователь
                if (user?.Identity?.IsAuthenticated == true)
                {
                    var clientIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(clientIdString) && Guid.TryParse(clientIdString, out var parsedClientId))
                    {
                        session.SetString("ClientId", clientIdString); // Сохраняем в сессии для консистентности
                        return parsedClientId;
                    }
                    throw new InvalidOperationException("Аутентифицированный пользователь не имеет корректного идентификатора.");
                }

                // Если пользователь не аутентифицирован, используем или создаём идентификатор в сессии
                var sessionId = session.GetString("ClientId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    session.SetString("ClientId", sessionId);
                }
                return Guid.Parse(sessionId);
            }
        }


        public static class SessionHelper
        {
            public static Cart GetCartFromSession(ISession session, Guid clientId)
            {
                var cartJson = session.GetString($"Cart_{clientId}");
                if (string.IsNullOrEmpty(cartJson))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<Cart>(cartJson);
            }

            public static void SetCartToSession(ISession session, Cart cart, Guid clientId)
            {
                var cartJson = JsonConvert.SerializeObject(cart);
                session.SetString($"Cart_{clientId}", cartJson);
            }
        }

    }
}
