using System;
using System.Collections.Generic;

namespace Lind.Example.Data.Models;

public partial class Customer : ExampleEntity
{
    public int CustomerId { get; set; }

    public bool NameStyle { get; set; }

    public string? Title { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string? Suffix { get; set; }

    public string? CompanyName { get; set; }

    public string? SalesPerson { get; set; }

    public string? EmailAddress { get; set; }

    public string? Phone { get; set; }

    public string? PasswordHash { get; set; } = null!;

    public string? PasswordSalt { get; set; } = null!;

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; } = new List<SalesOrderHeader>();
}
