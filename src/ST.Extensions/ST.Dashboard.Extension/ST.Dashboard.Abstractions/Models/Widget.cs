using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Core;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions.Helpers.Enums;

namespace ST.Dashboard.Abstractions.Models
{
    [NotMapped]
    public abstract class Widget : BaseModel, IWidgetUISettings
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
        public bool AllowCache { get; set; } = false;

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

        public virtual string Width { get; set; }
        public virtual string Height { get; set; }
        public virtual string BackGroundColor { get; set; }
        public virtual int BorderRadius { get; set; }
        public virtual string BorderStyle { get; set; }
        public string ClassAttribute { get; set; }
    }
}
