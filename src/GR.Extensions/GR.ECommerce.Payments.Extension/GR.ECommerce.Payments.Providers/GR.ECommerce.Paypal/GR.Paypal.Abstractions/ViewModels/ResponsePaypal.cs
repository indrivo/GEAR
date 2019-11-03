using System;

namespace GR.Paypal.Abstractions.ViewModels
{
    public class ResponsePaypal
    {
        /// <summary>
        /// State 
        /// </summary>
        public virtual bool IsSuccess { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        public virtual Guid? OrderId { get; set; }
    }
}
