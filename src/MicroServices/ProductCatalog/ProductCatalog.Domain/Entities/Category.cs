﻿using Shared.Domain.Entities;

namespace ProductCatalog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

}