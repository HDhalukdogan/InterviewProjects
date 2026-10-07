using System;
using System.Collections.Generic;
using System.Text;

namespace Cart.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Category Category { get; set; } = Category.Electronics;
        public ICollection<CartItem> Items { get; set; } = [];
    }

    public enum Category
    {
        Electronics = 1, 
        Clothing, 
        Books
    }
}
