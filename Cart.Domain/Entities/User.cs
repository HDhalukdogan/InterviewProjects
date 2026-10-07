namespace Cart.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public required string FullName { get; set; }
    }
}
