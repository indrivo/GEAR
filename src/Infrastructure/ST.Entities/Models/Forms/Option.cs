using System;
using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class Option : BaseModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
        public Guid FieldId { get; set; }
        public string TypeValue { get; set; }
        public string TypeLabel { get; set; }
        public string ClassName { get; set; }
    }
}