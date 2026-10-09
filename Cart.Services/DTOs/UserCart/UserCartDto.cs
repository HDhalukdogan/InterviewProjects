
using Cart.Services.DTOs.CartItem;

namespace Cart.Services.DTOs.UserCart
{
    public class UserCartDto : BaseDto<Guid>
    {
        public Guid UserId { get; set; }
        public string? AppliedCouponCode { get; set; }
        public ICollection<CartItemDto> Items { get; set; } = [];
    }
}
