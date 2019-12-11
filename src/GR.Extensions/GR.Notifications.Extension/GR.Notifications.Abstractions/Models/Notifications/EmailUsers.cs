using System;
using System.Collections.Generic;
using System.Text;
using GR.Core;

namespace GR.Notifications.Abstractions.Models.Notifications
{
    public class EmailUsers : BaseModel
    {
        public Guid EmailId { get; set; }
        public Guid UserEmailFolderId { get; set; }
        public int Type { get; set; }
        public bool IsRead { get; set; }
    }
}
