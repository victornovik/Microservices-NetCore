﻿using Play.Common;

namespace Play.Inventory.Service.Entitites;

public class CatalogEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}