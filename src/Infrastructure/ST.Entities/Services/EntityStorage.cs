using System;
using System.Collections.Generic;
using System.Text;
using ST.Entities.Data;

namespace ST.Entities.Services
{
    public static class EntityStorage
    {
        public static EntitiesDbContext EntityContext { get; set; }
        public static IList<Entity> DynamicEntities { get; set; } = new List<Entity>();
    }

    public class Entity
    {
        public string Name { get; set; }

        public Type Type => Instance.Object.GetType();

        public string Description { get; set; }

        public DynamicObject Instance => new ObjectService(Name).Resolve(EntityStorage.EntityContext);
    }
}
