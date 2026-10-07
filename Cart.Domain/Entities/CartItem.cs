namespace Cart.Domain.Entities
{
    public class CartItem : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public UserCart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
