﻿using System;
using System.Collections.Generic;
using ST.Core.Attributes.Documentation;
using ST.Dashboard.Abstractions.Helpers.Enums;
using ST.Dashboard.Abstractions.Models.RowWidgets;

namespace ST.Dashboard.Abstractions.Models.WidgetTypes
{
    [Author("Lupei Nicolae", 1.1)]
    public class ReportWidget : Widget
    {
        /// <summary>
        /// Report reference
        /// </summary>
        public virtual Guid ReportId { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Widget template type
        /// </summary>
        public override WidgetTemplateType WidgetTemplateType { get; set; } = WidgetTemplateType.Razor;

        /// <summary>
        /// Row reference
        /// </summary>
        public virtual ICollection<RowReportWidget> ReportWidgets { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Render
        /// </summary>
        /// <returns></returns>
        public override string Render()
        {
            return Service<ReportWidget>().Render(this);
        }
    }
}