using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace DinoPizza.Authorize
{
    public static class ClaimsHelper
    {
        public static List<IdentityRoleClaim<string>> GetAllPermissions()
        {
            var productEdit = GetPermissions(typeof(AppPermissions.ProductEdit));
            var categoryEdit = GetPermissions(typeof(AppPermissions.CategoryEdit));

            var allPermissions = productEdit.Union(categoryEdit).ToList(); //соедлинили два набора прав

            return allPermissions;
        }

        public static List<IdentityRoleClaim<string>> GetPermissions(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var list = fields.Select(x => new IdentityRoleClaim<string>
            {
                ClaimValue = x.Name,
                ClaimType = type.Name
            }).ToList();

            return list;
        }
    }
}
