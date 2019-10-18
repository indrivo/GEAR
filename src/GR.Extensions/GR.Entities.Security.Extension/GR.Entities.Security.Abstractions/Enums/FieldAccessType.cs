using System.ComponentModel.DataAnnotations;

namespace GR.Entities.Security.Abstractions.Enums
{
    public enum FieldAccessType
    {
        FullControl,
        Read,
        Update,
        Delete,
        Restore,
        DeletePermanent,
        Owner,
        InlineEdit,
        Disable
    }

    public enum EntityAccessType
    {
        [Display(Name = "Full Control")]
        FullControl,
        [Display(Name = "Read")]
        Read,
        [Display(Name = "Update")]
        Update,
        [Display(Name = "Delete")]
        Delete,
        [Display(Name = "Delete Permanent")]
        DeletePermanent,
        [Display(Name = "Restore")]
        Restore,
        [Display(Name = "Write")]
        Write
    }
}
