using System;
using System.Collections.Generic;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;

namespace GR.Entities.Abstractions.Models.Tables
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
        public ICollection<TableFieldType> TableFieldTypes { get; set; }
    }
}