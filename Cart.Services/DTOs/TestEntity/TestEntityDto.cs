using Cart.Services.DTOs;

namespace Cart.Services.DTOs.TestEntity
{
    public class TestEntityDto : BaseDto<Guid>
    {
        public string TestName { get; set; } = null!;
    }
}