﻿using System;
namespace MyCRM.Core
{
    public interface IEntity
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}
