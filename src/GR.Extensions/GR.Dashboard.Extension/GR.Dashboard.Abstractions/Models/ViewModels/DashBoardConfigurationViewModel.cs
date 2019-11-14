using System;
using System.Collections.Generic;

namespace GR.Dashboard.Abstractions.Models.ViewModels
{
    public sealed class DashBoardConfigurationViewModel
    {
        /// <summary>
        /// Dashboard identifier
        /// </summary>
        public Guid? DashboardId { get; set; }

        /// <summary>
        /// Store row configuration
        /// </summary>
        public ICollection<DashboardRowViewModel> Rows { get; set; } = new List<DashboardRowViewModel>();
    }

    public sealed class DashboardRowViewModel
    {
        /// <summary>
        /// Row order
        /// </summary>
        public int Order { get; set; } = 1;

        /// <summary>
        /// Row id
        /// </summary>
        public Guid? RowId { get; set; }

        /// <summary>
        /// Widgets
        /// </summary>
        public ICollection<RowWidgetViewModel> Widgets { get; set; } = new List<RowWidgetViewModel>();
    }

    public sealed class RowWidgetViewModel
    {
        /// <summary>
        /// Widget order
        /// </summary>
        public int Order { get; set; } = 1;

        /// <summary>
        /// Widget id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Widget group id
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Widget name
        /// </summary>
        public string Name { get; set; }
    }
}
