using Cart.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cart.Services.DTOs.Coupon
{
    public class CouponCreateDto
    {
        public required string Code { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;
        public decimal DiscountValue { get; set; }
        public decimal? MinCartAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int UsageLimit { get; set; } = 10;
        public int UsedCount { get; set; } = 0;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(10);
    }
}
