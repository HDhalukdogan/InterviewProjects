namespace Cart.Domain.Entities
{
    public class TestEntity: BaseEntity<Guid>
    {
        public required string TestName { get; set; }
    }
}
