using System;
using System.Collections.Generic;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.Models.ViewModels
{
    public class ViewModel : BaseModel
    {
        public TableModel TableModel { get; set; }
        public Guid TableModelId { get; set; }
        public string Name { get; set; }
        public IEnumerable<ViewModelFields> ViewModelFields { get; set; }
    }

    public class ViewModelFields : BaseModel
    {
        public string Name { get; set; }
        public Guid ViewModelId { get; set; }
        public TableModelField TableModelField { get; set; }
        public Guid? TableModelFieldsId { get; set; }
        public string Translate { get; set; }
        public string Template { get; set; }
        public int Order { get; set; }
    }
}
