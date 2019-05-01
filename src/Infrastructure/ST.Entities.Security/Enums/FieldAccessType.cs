using System.ComponentModel.DataAnnotations;

namespace ST.Entities.Security.Enums
{
    public enum FieldAccessType
    {
        FullControl,
        Read,
        Update,
        Delete,
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
        [Display(Name = "Write")]
        Write,
        [Display(Name = "Owner")]
        Owner
    }
}
