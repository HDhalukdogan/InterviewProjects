namespace CartDbFirstApp.Service.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string? AppliedCouponCode { get; set; }
    }
}
