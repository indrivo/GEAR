using System;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core.Abstractions;

namespace ST.Entities.Abstractions.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableFieldConfigs : IBase<Guid>
    {
        /// <summary>
        /// TableFieldConfig Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// TableFieldConfig Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// TableFieldConfig Name
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// TableFieldConfig Type
        /// </summary>
        public string Type { get; set; }


        [Column(TypeName = "char(4)")]
        public string Code { get; set; }

        /// <summary>
        /// TableFieldType
        /// </summary>
        public TableFieldType TableFieldType { get; set; }

        /// <summary>
        /// TableFieldType id
        /// </summary>
        public Guid TableFieldTypeId { get; set; }
    }
}
