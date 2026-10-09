namespace Cart.Services.DTOs.User
{
    public class UserDto : BaseDto<Guid>
    {
        public string FullName { get; set; } = null!;
    }
}
