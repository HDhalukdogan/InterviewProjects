using Cart.Domain.Entities;
using Cart.Services.DTOs.Coupon;

namespace Cart.Services.Abstracts
{
    public interface ICouponService : ICrudService<Coupon, CouponDto, CouponCreateDto, CouponUpdateDto, Guid>
    {
    }
}
