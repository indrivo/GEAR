using System.Collections.Generic;
using ST.Entities.Data;
using ST.Entities.ViewModels.Table;

namespace ST.Files.Providers
{
    public class FileConfig
    {
        public EntitiesDbContext DbContext { get; set; }
        public string WebRootPath { get; set; }
        public Dictionary<string, TableConfigViewModel> Tables { get; set; }
    }
}