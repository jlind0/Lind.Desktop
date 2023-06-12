using System;
using System.Collections.Generic;

namespace Lind.Example.Data.Models;

public partial class CustomerAddress : ExampleEntity
{
    public int CustomerId { get; set; }

    public int AddressId { get; set; }

    public string AddressType { get; set; } = null!;


    public virtual Address? Address { get; set; } = null!;

    public virtual Customer? Customer { get; set; } = null!;
}
