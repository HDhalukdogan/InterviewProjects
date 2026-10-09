using Cart.Services.DTOs.CartItem;

namespace Cart.Services.DTOs.UserCart
{
    public class UserCartUpdateDto : BaseDto<Guid>
    {
        public Guid UserId { get; set; }
        public string? AppliedCouponCode { get; set; }
        public ICollection<CartItemUpdateDto> Items { get; set; } = [];
    }
}
