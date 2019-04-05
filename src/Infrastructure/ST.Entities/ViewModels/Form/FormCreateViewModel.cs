using System;
using System.Collections.Generic;
using ST.Entities.Models.Forms;
using ST.Entities.Models.Tables;

namespace ST.Entities.ViewModels.Form
{
    public class FormCreateViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TableModel Table { get; set; }
        public Guid TableId { get; set; }
        public Models.Forms.Settings Settings { get; set; }
        public Guid SettingsId { get; set; }
        public IEnumerable<Stage> Stages { get; set; }
        public IEnumerable<Row> Rows { get; set; }
        public IEnumerable<Column> Columns { get; set; }
        public IEnumerable<Field> Fields { get; set; }
    }
}