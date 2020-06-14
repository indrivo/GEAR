using System;
using System.Threading;

namespace GR.EmailTwoFactorAuth.Models
{
    public class CheckStateModel
    {
        public AutoResetEvent Event { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public int Counter { get; set; } = 0;
    }
}