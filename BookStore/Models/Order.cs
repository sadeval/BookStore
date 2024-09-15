using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }

        // Связь с классом Book
        public Book Book { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
