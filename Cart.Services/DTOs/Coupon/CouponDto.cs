using Cart.Domain.Entities;

namespace Cart.Services.DTOs.Coupon
{
    public class CouponDto : BaseDto<Guid>
    {
        public required string Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinCartAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
