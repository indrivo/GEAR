using System.ComponentModel.DataAnnotations.Schema;
using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class FormType : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "char(4)")]
        public string Code { get; set; }
        public bool IsSystem { get; set; }
    }
}