namespace Cart.Services.DTOs.User
{
    public class UserUpdateDto : BaseDto<Guid>
    {
        public required string FullName { get; set; }
    }
}
