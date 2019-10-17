using System;
using System.ComponentModel.DataAnnotations;

namespace ST.Calendar.Abstractions.Models.ViewModels
{
    public sealed class UpdateEventViewModel : BaseEventViewModel
    {
        /// <summary>
        /// Event id
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}
