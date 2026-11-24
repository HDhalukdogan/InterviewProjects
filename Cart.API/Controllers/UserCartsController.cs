using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.CartItem;
using Cart.Services.DTOs.UserCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cart.API.Controllers
{
    public class UserCartsController(IUserCartService userCartService, IUserService userService)
        : BaseCrudController<UserCart, UserCartDto, UserCartCreateDto, UserCartUpdateDto, Guid>(userCartService)
    {
        // JWT çerezinden okunan kimlik durumunu kontrol eden endpoint
        [AllowAnonymous]
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity.Name
                });
            }
            return Ok(new { isAuthenticated = false, message = "Oturum açılmamış." });
        }

        // Giriş yapan kullanıcının kendi sepetini getiren endpoint
        [Authorize]
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Oturum açmış kullanıcı kimliği doğrulanamadı." });
            }

            var fullName = User.FindFirst(ClaimTypes.Email)?.Value 
                          ?? User.FindFirst(ClaimTypes.Name)?.Value 
                          ?? User.Identity?.Name 
                          ?? "Identity User";

            // Sadece giriş yapan kullanıcı varsa Cart DB'sindeki Users tablosuna ekle / doğrula
            await userService.EnsureUserExistsAsync(userId, fullName);

            var allCarts = await userCartService.GetAllAsync();
            var myCart = allCarts.FirstOrDefault(c => c.UserId == userId);
            return Ok(myCart);
        }

        // Sepete ürün ekleme: SADECE giriş yapan Identity kullanıcısı varsa ekler ve Users tablosuna kaydeder.
        [Authorize]
        [HttpPut("addItemToUserCart")]
        public async Task<IActionResult> AddItemToUserCart([FromBody] CartItemCreateDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Sepete ürün eklemek için geçerli bir kullanıcı oturumu gereklidir." });
            }

            var fullName = User.FindFirst(ClaimTypes.Email)?.Value 
                          ?? User.FindFirst(ClaimTypes.Name)?.Value 
                          ?? User.Identity?.Name 
                          ?? "Identity User";

            // SADECE giriş yapan Identity kullanıcısı varsa Cart DB'sindeki Users tablosuna kaydet/doğrula
            await userService.EnsureUserExistsAsync(userId, fullName);

            var response = await userCartService.AddCartItemToUserCartAsync(userId, dto);
            return Ok(response);
        }
    }
}
