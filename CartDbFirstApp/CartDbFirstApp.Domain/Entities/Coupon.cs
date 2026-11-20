using System;
using System.Collections.Generic;

namespace CartDbFirstApp.Domain.Entities;

public partial class Coupon
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public int DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal? MinCartAmount { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public int UsageLimit { get; set; }

    public int UsedCount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
