using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string? Publisher { get; set; }
        public decimal Price { get; set; }

        // Связи
        public virtual ICollection<Author> Authors { get; set; } = new HashSet<Author>();
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
