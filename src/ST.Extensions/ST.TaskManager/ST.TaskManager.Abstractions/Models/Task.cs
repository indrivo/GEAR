using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.TaskManager.Abstractions.Models
{
    public class Task : BaseModel
    {
        /// <summary>
        /// Task name
        /// </summary>
        [Required]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
    }
}
