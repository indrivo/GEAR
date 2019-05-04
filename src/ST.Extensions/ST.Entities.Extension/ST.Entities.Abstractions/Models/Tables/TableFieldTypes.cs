using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Audit.Attributes;
using ST.Audit.Enums;

namespace ST.Entities.Abstractions.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableFieldTypes
    {
        /// <summary>
        /// TableFieldType Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// TableFieldType Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// TableFieldType Name
        /// </summary>
        public string DataType { get; set; }

        [Column(TypeName = "char(2)")]
        public string Code { get; set; }

        /// <summary>
        /// Lists of TableFieldConfigs for TableFieldType
        /// </summary>
        public ICollection<TableFieldConfigs> TableFieldConfigs { get; set; }

        /// <summary>
        /// TableFieldType
        /// </summary>
        public TableFieldGroups TableFieldGroups { get; set; }

        /// <summary>
        /// TableFieldType id
        /// </summary>
        public Guid TableFieldGroupsId { get; set; }
    }
}