using System;
using System.Collections.Generic;

namespace GR.Identity.Models.NotificationsViewModels
{
    public class NotificationMessageViewModel
    {
        //
        // Summary:
        //     Stores subject of notification
        public string Subject { get; set; }

        //
        // Summary:
        //     Stores message
        public string Message { get; set; }

        //
        //     Stores Id of the Object
        public Guid Id { get; set; }

        //
        // Summary:
        //     Stores Id of the User that created the object
        public string Author { get; set; }

        //
        // Summary:
        //     Stores the time when object was created
        public DateTime? Created { get; set; }

        //
        // Summary:
        //     Stores the Id of the User that modified the object. Nullable
        public string ModifiedBy { get; set; }

        //
        // Summary:
        //     Stores the time when object was modified. Nullable
        public DateTime? Changed { get; set; }

        //
        // Summary:
        //     Stores state of the Object. True if object is deleted and false otherwise
        public bool? IsDeleted { get; set; }

        //
        // Summary:
        //     Stores recipients of notification
        public ICollection<NotificationMessageViewModel> Recipients { get; set; }
    }
}