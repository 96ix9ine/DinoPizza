using System.Security.Claims;

namespace DinoPizza.Authorize
{
    public static class PermissionHelper
    {
        public static bool CanEditProductPrice(this ClaimsPrincipal user)
        {
            return user.Claims.Any(x => x.Value == AppPermissions.ProductEdit.Price);
        }
    }
}
