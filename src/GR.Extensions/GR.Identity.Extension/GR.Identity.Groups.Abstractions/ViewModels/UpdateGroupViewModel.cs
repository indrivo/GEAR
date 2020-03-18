using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Groups.Abstractions.ViewModels
{
    public sealed class UpdateGroupViewModel
    {
        public Guid Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}
