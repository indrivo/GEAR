using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Groups.Razor.ViewModels.GroupViewModels
{
    public class CreateGroupViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}