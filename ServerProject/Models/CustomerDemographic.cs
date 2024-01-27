using System;
using System.Collections.Generic;

namespace ServerProject.Models;

public partial class CustomerDemographic
{
    public string CustomerTypeId { get; set; } = null!;

    public string? CustomerDesc { get; set; }

    public virtual List<Customer> Customers { get; set; } = new List<Customer>();
}
