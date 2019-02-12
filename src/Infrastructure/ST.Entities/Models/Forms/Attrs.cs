using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class Attrs : BaseModel
    {
        public string ClassName { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}