using AutoMapper;
using Cart.Domain.Entities;
using Cart.Services.DTOs.CartItem;
using Cart.Services.DTOs.Coupon;
using Cart.Services.DTOs.Product;
using Cart.Services.DTOs.User;
using Cart.Services.DTOs.UserCart;

namespace Cart.Services
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<UserCart, UserCartDto>().ReverseMap();
            CreateMap<UserCartCreateDto, UserCart>();
            CreateMap<UserCartUpdateDto, UserCart>();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<CartItemCreateDto, CartItem>();
            CreateMap<CartItemUpdateDto, CartItem>();
            CreateMap<Coupon, CouponDto>().ReverseMap();
            CreateMap<CouponCreateDto, Coupon>();
            CreateMap<CouponUpdateDto, Coupon>();
        }
    }
}
