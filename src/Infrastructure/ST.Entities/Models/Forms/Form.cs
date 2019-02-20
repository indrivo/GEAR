using System;
using System.Collections.Generic;
using ST.Entities.Models.Tables;

namespace ST.Entities.Models.Forms
{
    public class Form : ExtendedModel
    {
        /// <summary>
        /// Form name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Form description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Reference to entity
        /// </summary>
        public TableModel Table { get; set; }
        public Guid TableId { get; set; }
        /// <summary>
        /// Form type
        /// </summary>
        public FormType Type { get; set; }
        public Guid TypeId { get; set; }
        /// <summary>
        /// Form settings
        /// </summary>
        public Settings Settings { get; set; }
        public Guid SettingsId { get; set; }
        /// <summary>
        /// Form stages [Designer mode]
        /// </summary>
        public IEnumerable<Stage> Stages { get; set; }
        /// <summary>
        /// Form rows [Designer mode]
        /// </summary>
        public IEnumerable<Row> Rows { get; set; }
        /// <summary>
        /// Form columns [Designer mode]
        /// </summary>
        public IEnumerable<Column> Columns { get; set; }
        /// <summary>
        /// Form fields [Designer mode]
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }
        /// <summary>
        /// Url to redirect after submit form
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// Url to call on submit form
        /// </summary>
        public string PostUrl { get; set; }
    }
}