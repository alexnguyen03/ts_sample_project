namespace ServerProject.Models;

public partial class PosDetail
{
    public int PosDetailId { get; set; }

    public int? ProductId { get; set; }

    public string? UnitName { get; set; }

    public double? PricePerUnit { get; set; }

    public int? Quantity { get; set; }

    public double? TotalPrice { get; set; }

    public string? BatchNumber { get; set; }

    public int? PosId { get; set; }

    public virtual Pos? Pos { get; set; }

    public virtual Product? Product { get; set; }
}
