using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.Coupon;

namespace Cart.Services.Concretes
{
    public class CouponService(IRepository<Coupon, Guid> repository, IMapper mapper, IServiceProvider serviceProvider) : CrudService<Coupon, CouponDto, CouponCreateDto, CouponUpdateDto, Guid>(repository, mapper, serviceProvider), ICouponService
    {
    }
}
