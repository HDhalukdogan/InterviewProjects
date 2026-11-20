using System;
using System.Collections.Generic;

namespace CartDbFirstApp.Domain.Entities;

public partial class Cart
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? AppliedCouponCode { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
