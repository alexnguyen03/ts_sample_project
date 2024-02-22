using Newtonsoft.Json;

namespace ServerProject.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    public string? QuantityPerUnit { get; set; }

    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    public Category? Category { get; set; }
    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    [JsonIgnore]
    public virtual ICollection<PosDetail> PosDetails { get; set; } = new List<PosDetail>();
    [JsonIgnore]
    public virtual Supplier? Supplier { get; set; }
    //[JsonIgnore]
    public ICollection<Unit> Units { get; set; } = new List<Unit>();

}
