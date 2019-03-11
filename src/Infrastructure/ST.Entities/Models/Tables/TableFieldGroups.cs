using ST.Audit.Attributes;
using ST.Audit.Enums;
using System;
using System.Collections.Generic;

namespace ST.Entities.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableFieldGroups
    {
        /// <summary>
        /// TableFieldType Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// TableFieldType Name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// TableFieldType Name
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Lists of TableFieldConfigs for TableFieldType
        /// </summary>
        public ICollection<TableFieldTypes> TableFieldTypes { get; set; }
    }
}