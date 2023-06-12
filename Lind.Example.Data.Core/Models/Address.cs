using System;
using System.Collections.Generic;

namespace Lind.Example.Data.Models;

public partial class Address : ExampleEntity
{
    public int AddressId { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string StateProvince { get; set; } = null!;

    public string CountryRegion { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    

    public virtual ICollection<CustomerAddress>? CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<SalesOrderHeader>? SalesOrderHeaderBillToAddresses { get; set; } = new List<SalesOrderHeader>();

    public virtual ICollection<SalesOrderHeader>? SalesOrderHeaderShipToAddresses { get; set; } = new List<SalesOrderHeader>();
}
