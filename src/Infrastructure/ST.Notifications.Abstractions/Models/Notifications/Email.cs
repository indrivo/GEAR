using System;
using System.Collections.Generic;
using System.Text;
using ST.Shared;

namespace ST.Notifications.Abstractions.Models.Notifications
{
    public class Email : ExtendedModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
