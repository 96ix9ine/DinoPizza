using Microsoft.AspNetCore.Identity;

namespace DinoPizza.Authorize
{
    public class AppUser : IdentityUser
    {
        public bool IsClient { get; set; }

        public string? FirstName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
