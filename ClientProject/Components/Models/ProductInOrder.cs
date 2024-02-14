using ServerProject.Models;

namespace ClientProject.Components.Models
{
    public class ProductInOrder
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public Unit Unit { get; set; }

        public ICollection<Unit> Units { get; set; }

        public int Quantity { get; set; }


    }
}
