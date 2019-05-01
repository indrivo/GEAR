using System;
using System.Collections.Generic;
using System.Text;
using ST.Core;

namespace ST.Notifications.Abstractions.Models.Notifications
{
    public class EmailUsers : ExtendedModel
    {
        public Guid EmailId { get; set; }
        public Guid UserEmailFolderId { get; set; }
        public int Type { get; set; }
        public bool IsRead { get; set; }
    }
}
