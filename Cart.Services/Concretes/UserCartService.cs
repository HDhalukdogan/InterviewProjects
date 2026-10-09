using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.CartItem;
using Cart.Services.DTOs.UserCart;
using Microsoft.EntityFrameworkCore;

namespace Cart.Services.Concretes
{
    public class UserCartService(IRepository<UserCart, Guid> repository, IMapper mapper, IServiceProvider serviceProvider) : CrudService<UserCart, UserCartDto, UserCartCreateDto, UserCartUpdateDto, Guid>(repository, mapper, serviceProvider), IUserCartService
    {
        private IProductService ProductService => LazyGetRequiredService<IProductService>();
        public async Task<UserCartDto?> AddCartItemToUserCartAsync(Guid userId, CartItemCreateDto cartItemCreateDto)
        {
            var userCart = await Repository.Include(s => s.Items).FirstOrDefaultAsync(s => s.UserId.Equals(userId));

            var product = await ProductService.GetByIdAsync(cartItemCreateDto.ProductId);
            UserCartDto? userCartDto;
            if (userCart is null)
            {
                var createDto = new UserCartCreateDto { UserId = userId };
                createDto.Items.Add(cartItemCreateDto);
                userCartDto = await CreateAsync(createDto, false);
            }
            else
            {
                var item = userCart.Items.FirstOrDefault(s => s.ProductId.Equals(cartItemCreateDto.ProductId));
                if (item is not null)
                {
                    item.Quantity = cartItemCreateDto.Quantity;
                    item.UnitPrice = cartItemCreateDto.UnitPrice;
                }
                else
                {
                    var cartItem = Mapper.Map<CartItem>(cartItemCreateDto);
                    userCart.Items.Add(cartItem);
                }
                userCartDto = await GetByIdAsync(userCart.Id);
            }
            await CompleteAsync();

            return userCartDto;
        }

        public override async Task<UserCartDto?> GetByIdAsync(Guid id)
        {
            var userCart = await Repository.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id.Equals(id));
            if (userCart is null)
                return null;
            return Mapper.Map<UserCartDto>(userCart);
        }
    }
}
