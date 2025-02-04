using Mapster;
using DinoPizza.Authorize;
using DinoPizza.Domains;
using DinoPizza.Models;

namespace DinoPizza.Mapping
{
    public class MyMapper 
    {
        public static void RegisterSettings()
        {
            TypeAdapterConfig.GlobalSettings
                .NewConfig<Product, ProductSimpleModel>()
                .Map(dst => dst.Id, src => src.ProductId)
                .Map(dst => dst.ImageSrc, src => src.ImageUrl)
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<Product, ProductDetailsModel>()
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<Product, ProductEditModel>()
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<Category, CategoryModel>()
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<Category, CategorySimpleModel>().Map(x => x.Name, x => x.NameRus)
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<AppUser, UserSimpleModel>()
                .Map(x => x.UserId, x => x.Id)
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<AppUser, UserEditModel>()
                .Map(x => x.UserId, x => x.Id)
                .Map(x => x.Phone, x => x.PhoneNumber)
                .RequireDestinationMemberSource(true);

            TypeAdapterConfig.GlobalSettings
                .NewConfig<UserEditModel, AppUser>()
                .Ignore(x => x.Id)
                .Map(x => x.PhoneNumber, x => x.Phone);
        }
    }
}
