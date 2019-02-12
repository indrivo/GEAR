using System;
using System.ComponentModel.DataAnnotations;

namespace ST.Entities.ViewModels.TableTypes
{
    public class EntityTypeUpdateViewModel
    {
        /// <summary>
        ///     Id of Schema
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Name of Schema
        /// </summary>
        
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        ///     MachienName of Schema
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string MachineName { get; set; }

        /// <summary>
        ///     Add description for Schema
        /// </summary>
        public string Description { get; set; }
    }
}