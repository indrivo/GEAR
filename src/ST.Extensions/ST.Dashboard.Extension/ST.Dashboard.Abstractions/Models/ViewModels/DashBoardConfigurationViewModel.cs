using System;
using System.Collections.Generic;

namespace ST.Dashboard.Abstractions.Models.ViewModels
{
    public class DashBoardConfigurationViewModel
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

    public class DashboardRowViewModel
    {
        /// <summary>
        /// Row order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Row id
        /// </summary>
        public Guid? RowId { get; set; }
    }
}
