using System;
using System.Collections.Generic;

namespace Lind.Example.Data.Models;

public partial class ProductDescription : ExampleEntity
{
    public int ProductDescriptionId { get; set; }

    public string Description { get; set; } = null!;


    public virtual ICollection<ProductModelProductDescription> ProductModelProductDescriptions { get; set; } = new List<ProductModelProductDescription>();
}
