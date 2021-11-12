using System;
namespace MyCRM.Core
{
    public abstract class BaseEntity<TEntity> : IEntity
    {
        protected BaseEntity(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
