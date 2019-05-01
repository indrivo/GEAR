using System;
using System.Collections.Generic;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.Models.Forms
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Field : ExtendedModel
    {
        /// <summary>
        /// Html tag
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// List of html attributes
        /// </summary>
        public IEnumerable<Attrs> Attrs { get; set; }
        /// <summary>
        /// Some field configuration
        /// </summary>
        public Config Config { get; set; }
        public Guid ConfigId { get; set; }
        public string FMap { get; set; }
        /// <summary>
        /// Tag content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Builder settings
        /// </summary>
        public Meta Meta { get; set; }
        public Guid MetaId { get; set; }
        /// <summary>
        /// Options for select
        /// </summary>
        public IEnumerable<Option> Options { get; set; }
        /// <summary>
        /// Form Reference
        /// </summary>
        public Guid FormId { get; set; }
        public Form Form { get; set; }
        /// <summary>
        /// Field order
        /// </summary>
        public int Order { get; set; }
        public TableModelFields TableField { get; set; }
        public Guid? TableFieldId { get; set; }
        /// <summary>
        /// Fired field events
        /// </summary>
        public IEnumerable<FormFieldEvent> FieldEvents { get; set; }
    }
}