using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Groups.Abstractions.ViewModels
{
    public class CreateGroupViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}