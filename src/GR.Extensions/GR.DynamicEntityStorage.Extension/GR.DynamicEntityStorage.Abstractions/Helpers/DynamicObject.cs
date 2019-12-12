using System;

namespace GR.DynamicEntityStorage.Abstractions.Helpers
{
    public class DynamicObject
    {
        public IDynamicService Service { get; set; }
        public Type Type { get; set; }
    }
}
