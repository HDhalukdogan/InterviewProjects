namespace Cart.Services.DTOs
{
    public class BaseDto<T> where T: struct
    {
        public T Id { get; set; }
    }
}
