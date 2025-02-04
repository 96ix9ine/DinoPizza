using Microsoft.AspNetCore.Mvc;
using DinoPizza.BusinessLogic;
using DinoPizza.Helpers;
using DinoPizza.Models;

namespace DinoPizza.Components
{
    [ViewComponent]
    public class CartWidgetViewComponent
        :ViewComponent
    {
        private readonly ISession? _session;

        public CartWidgetViewComponent(IHttpContextAccessor contextAccessor)
        {
            _session = contextAccessor?
                .HttpContext?
                .Session;
        }

        public IViewComponentResult Invoke()
        {
            var cart = _session == null ?
                 new Cart() :
                _session.LoadObject<Cart>();

            return View(cart);
        }
    }
}
