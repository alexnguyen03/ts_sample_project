namespace ServerProject.Models.Response
{
    public class ProductInPOSResponse
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int? SupplierId { get; set; }

        public string? CategoryName { get; set; }

        public string? QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>();


    }
}
