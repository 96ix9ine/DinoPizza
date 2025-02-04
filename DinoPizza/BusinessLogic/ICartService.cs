using DinoPizza.Models;
using System.Threading.Tasks;

namespace DinoPizza.BusinessLogic
{
    public interface ICartService
    {
        Task CreateOrUpdateCartForUser(string userId);
        Task<Cart> GetCartByUserIdAsync(string userId);
    }
}
