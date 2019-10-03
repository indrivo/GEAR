﻿using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.Dashboard.Abstractions.Models
{
    public class WidgetGroup : BaseModel
    {
        /// <summary>
        /// Group name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public virtual int Order { get; set; } = 1;

        /// <summary>
        /// Is system
        /// </summary>
        public virtual bool IsSystem { get; set; }
    }
}
