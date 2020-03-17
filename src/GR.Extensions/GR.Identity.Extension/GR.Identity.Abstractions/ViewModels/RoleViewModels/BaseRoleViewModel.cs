using System;

namespace GR.Identity.Abstractions.ViewModels.RoleViewModels
{
    public class BaseRoleViewModel
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
    }
}