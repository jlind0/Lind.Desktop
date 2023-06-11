using System;
using System.Collections.Generic;

namespace Lind.Example.Data.Models;

public partial class ProductModelProductDescription : ExampleEntity
{
    public int ProductModelId { get; set; }

    public int ProductDescriptionId { get; set; }

    public string Culture { get; set; } = null!;

    public virtual ProductDescription ProductDescription { get; set; } = null!;

    public virtual ProductModel ProductModel { get; set; } = null!;
}
