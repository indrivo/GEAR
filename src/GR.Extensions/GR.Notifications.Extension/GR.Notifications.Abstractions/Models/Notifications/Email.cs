using System;
using System.Collections.Generic;
using System.Text;
using GR.Core;

namespace GR.Notifications.Abstractions.Models.Notifications
{
    public class Email : BaseModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
