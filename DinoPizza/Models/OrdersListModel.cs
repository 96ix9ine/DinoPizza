using System.Collections.Generic;
using DinoPizza.Models;

namespace DinoPizza.Models
{
    public class OrdersListModel
    {
        public IEnumerable<Order>? Orders { get; set; }
        public IEnumerable<Courier>? AvailableCouriers { get; set; }
    }
}
