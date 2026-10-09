namespace Cart.Services.DTOs.CartItem
{
    public class CartItemCreateDto
    {
        public Guid ProductId { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
