namespace Cart.Domain.Entities
{
    public class UserCart : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string? AppliedCouponCode { get; set; }
        public ICollection<CartItem> Items { get; set; } = [];
    }
}
