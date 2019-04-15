﻿using ST.Audit.Attributes;
using ST.Audit.Enums;
using System;

namespace ST.Entities.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableFieldConfigValues
    {
        /// <summary>
        /// TableModelField
        /// </summary>
        public TableModelFields TableModelField { get; set; }

        /// <summary>
        /// TableModelField Id
        /// </summary>
        public Guid TableModelFieldId { get; set; }

        /// <summary>
        /// TableFieldConfig
        /// </summary>
        public TableFieldConfigs TableFieldConfig { get; set; }

        /// <summary>
        /// TableFieldConfig Id
        /// </summary>
        public Guid TableFieldConfigId { get; set; }

        /// <summary>
        /// TableFieldConfigValue Value
        /// </summary>
        public string Value { get; set; }
    }
}