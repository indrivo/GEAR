using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions.Helpers.Enums;

namespace ST.Dashboard.Abstractions.Models
{
    public class Widget : BaseModel
    {
        /// <summary>
        /// Render service
        /// </summary>
        protected virtual IWidgetRenderer<TWidget> Service<TWidget>() where TWidget : Widget
        {
            return IoC.Resolve<IWidgetRenderer<TWidget>>();
        }

        /// <summary>-
        /// Widget name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Widget description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Allow cache
        /// </summary>
        public bool AllowCache { get; set; }

        /// <summary>
        /// Time to refresh
        /// </summary>
        public TimeSpan CacheRefreshSpan { get; set; }

        /// <summary>
        /// if is allowed allow cache
        /// </summary>
        public DateTime? LastRefreshTime { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// Razor template
        /// </summary>
        [Required]
        public string Template { get; set; }

        /// <summary>
        /// Template type
        /// </summary>
        public WidgetTemplateType WidgetTemplateType { get; set; } = WidgetTemplateType.Html;

        /// <summary>
        /// Row reference
        /// </summary>
        public virtual Row Row { get; set; }
        public virtual Guid RowId { get; set; }

        /// <summary>
        /// Reference to group 
        /// </summary>
        public virtual WidgetGroup WidgetGroup { get; set; }
        public virtual Guid WidgetGroupId { get; set; }

        #region Style
        /// <summary>
        /// Css class
        /// </summary>
        public virtual string ClassAttribute { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Render html dom
        /// </summary>
        /// <returns></returns>
        public virtual string Render()
        {
            return Service<Widget>().Render(this);
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Render();

        #endregion
    }
}
