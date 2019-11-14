using System.Collections.Generic;

namespace GR.Entities.Abstractions.ViewModels.Table
{
    public class SynchronizeTableViewModel
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsStaticFromEntityFramework { get; set; }
        public bool IsCommon { get; set; }

        public IEnumerable<CreateTableFieldViewModel> Fields { get; set; }
    }
}