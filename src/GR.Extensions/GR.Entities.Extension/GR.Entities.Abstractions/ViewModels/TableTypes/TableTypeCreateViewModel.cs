using System.ComponentModel.DataAnnotations;

namespace GR.Entities.Abstractions.ViewModels.TableTypes
{
    public class EntityTypeCreateViewModel
    {
        /// <summary>
        ///     Name of TableModel
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        ///     MachienName of Schema
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string MachineName { get; set; }

        /// <summary>
        ///     Add description for TableModel
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}