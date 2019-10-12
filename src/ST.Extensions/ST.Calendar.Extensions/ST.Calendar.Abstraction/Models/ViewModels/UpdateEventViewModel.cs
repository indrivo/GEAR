using System;

namespace ST.Calendar.Abstractions.Models.ViewModels
{
    public sealed class UpdateEventViewModel : BaseEventViewModel
    {
        /// <summary>
        /// Event id
        /// </summary>
        public Guid Id { get; set; }
    }
}
