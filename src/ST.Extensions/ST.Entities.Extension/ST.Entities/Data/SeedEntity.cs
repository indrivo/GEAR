using System.Collections.Generic;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.ViewModels.Table;

namespace ST.Entities.Data
{
    public class SeedEntity
    {
        public List<EntityType> EntityTypes { get; set; }
        public List<SynchronizeTableViewModel> SynchronizeTableViewModels { get; set; }
        public List<TableFieldGroups> TableFieldGroups { get; set; }
    }
}
