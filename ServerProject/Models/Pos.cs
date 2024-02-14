namespace ServerProject.Models;

public partial class Pos
{
    public int PosId { get; set; }

    public string? CustomerId { get; set; }

    public int? EmployeeId { get; set; }

    public double? TotalPrice { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<PosDetail> PosDetails { get; set; } = new List<PosDetail>();
}
