using System;
using System.Collections.Generic;

namespace ServerProject.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string? UnitName { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? UnitQuantityInStock { get; set; }

    public int? ProductId { get; set; }

    //public virtual Product? Product { get; set; }
}
