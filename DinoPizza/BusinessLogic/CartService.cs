using System.Threading.Tasks;
using DinoPizza.Models;
using System.Linq;
using Microsoft.AspNetCore.Cors.Infrastructure;
using DinoPizza.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace DinoPizza.BusinessLogic
{
    public class CartService : ICartService
    {
        private readonly DinoDBContext _context;

        public CartService(DinoDBContext context)
        {
            _context = context;
        }

        public async Task CreateOrUpdateCartForUser(string userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.ClientId.ToString() == userId);

            if (cart == null)
            {
                cart = new Cart { ClientId = Guid.Parse(userId) };
                _context.Carts.Add(cart);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.ClientId.ToString() == userId);
        }
    }
}
