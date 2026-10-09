using Cart.Services.DTOs.CartItem;

namespace Cart.Services.DTOs.UserCart
{
    public class UserCartCreateDto
    {
        public Guid UserId { get; set; }
        public string? AppliedCouponCode { get; set; }
        public ICollection<CartItemCreateDto> Items { get; set; } = [];
    }
}
