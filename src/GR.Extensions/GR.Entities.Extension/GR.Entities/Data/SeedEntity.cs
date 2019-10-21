using System.Collections.Generic;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.Data
{
    public class SeedEntity
    {
        public List<EntityType> EntityTypes { get; set; }
        public List<SynchronizeTableViewModel> SynchronizeTableViewModels { get; set; }
        public List<TableFieldGroups> TableFieldGroups { get; set; }
    }
}
