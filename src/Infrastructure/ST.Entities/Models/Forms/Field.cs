using System;
using System.Collections.Generic;
using ST.BaseRepository;
using ST.Entities.Models.Tables;

namespace ST.Entities.Models.Forms
{
    public class Field : BaseModel
    {
        public string Tag { get; set; }
        public Attrs Attrs { get; set; }
        public Guid AttrsId { get; set; }
        public Config Config { get; set; }
        public Guid ConfigId { get; set; }
        public string FMap { get; set; }
        public string Content { get; set; }
        public Meta Meta { get; set; }
        public Guid MetaId { get; set; }
        public IEnumerable<Option> Options { get; set; }
        public Guid FormId { get; set; }
        public int Order { get; set; }
        public TableModelFields TableField { get; set; }
        public Guid? TableFieldId { get; set; }
    }
}