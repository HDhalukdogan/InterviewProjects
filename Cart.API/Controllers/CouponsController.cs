using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.Coupon;

namespace Cart.API.Controllers
{
    public class CouponsController(ICouponService couponService)
        : BaseCrudController<Coupon, CouponDto, CouponCreateDto, CouponUpdateDto, Guid>(couponService)
    {
    }
}
