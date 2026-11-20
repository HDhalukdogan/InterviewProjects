using CartDbFirstApp.Service.DTOs;

namespace CartDbFirstApp.Service.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartDto>> GetAllAsync();

    }
}
