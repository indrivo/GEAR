using System;
// ReSharper disable InconsistentNaming

namespace GR.Dashboard.Abstractions.Constants
{
    public struct WidgetType
    {
        /// <summary>
        /// Custom widget
        /// </summary>
        public static Guid CUSTOM = Guid.Parse("c67f5e2f-507d-4ed2-aab3-c107384b1937");

        /// <summary>
        /// Report type
        /// </summary>
        public static Guid REPORT = Guid.Parse("05490637-ba30-4bbb-9165-2cbeba51995a");

        /// <summary>
        /// Chart type
        /// </summary>
        public static Guid CHARTS = Guid.Parse("aced3174-744a-48e4-82b9-de80d8d76114");
    }
}
