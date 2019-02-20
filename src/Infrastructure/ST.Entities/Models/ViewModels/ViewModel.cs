using ST.BaseRepository;
using ST.Entities.Models.Tables;
using System;
using System.Collections.Generic;

namespace ST.Entities.Models.ViewModels
{
    public class ViewModel : ExtendedModel
    {
        public TableModel TableModel { get; set; }
        public Guid TableModelId { get; set; }
        public string Name { get; set; }
        public IEnumerable<ViewModelFields> ViewModelFields { get; set; }
    }

    public class ViewModelFields : ExtendedModel
    {
        public string Name { get; set; }
        public Guid ViewModelId { get; set; }
        public TableModelFields TableModelFields { get; set; }
        public Guid? TableModelFieldsId { get; set; }
        public string Translate { get; set; }
        public string Template { get; set; }
        public int Order { get; set; }
    }
}
