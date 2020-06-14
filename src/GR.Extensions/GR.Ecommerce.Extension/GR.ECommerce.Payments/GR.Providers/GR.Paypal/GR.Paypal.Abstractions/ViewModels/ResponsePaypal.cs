using System;
using GR.Core.Helpers;

namespace GR.Paypal.Abstractions.ViewModels
{
    public class ResponsePaypal : ResultModel<string>
    {
        /// <summary>
        /// Order id
        /// </summary>
        public virtual Guid? OrderId { get; set; }
    }
}
