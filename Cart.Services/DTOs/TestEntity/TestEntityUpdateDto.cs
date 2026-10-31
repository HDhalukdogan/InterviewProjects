using Cart.Services.DTOs;

namespace Cart.Services.DTOs.TestEntity
{
    public class TestEntityUpdateDto : BaseDto<Guid>
    {
        public required string TestName { get; set; }
    }
}