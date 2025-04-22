using AutoMapper;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;

namespace ApiEshop.Mappings
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Store, StoreDto>();
            CreateMap<StoreDto, Store>();

            CreateMap<Category, CategoryDto>();

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.ProdCats.Select(pc => pc.Category)));


            CreateMap<Purchase, PurchaseDto>();
            CreateMap<PurchaseItem, PurchaseItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Image));


        }
    }
}
