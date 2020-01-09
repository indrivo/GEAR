using System.Collections.Generic;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.Data
{
    public class SeedEntity
    {
        public List<EntityType> EntityTypes { get; set; } = new List<EntityType>();
        public List<SynchronizeTableViewModel> SynchronizeTableViewModels { get; set; } = new List<SynchronizeTableViewModel>();
        public List<TableFieldGroups> TableFieldGroups { get; set; } = new List<TableFieldGroups>();
    }
}
