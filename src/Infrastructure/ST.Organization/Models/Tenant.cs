using ST.BaseRepository;
using System.ComponentModel.DataAnnotations;

namespace ST.Organization.Models
{
    public class Tenant : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string SiteWeb { get; set; }
        public string Adress { get; set; }
    }
}
