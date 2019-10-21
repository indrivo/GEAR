using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Exceptions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions.Helpers.Enums;

namespace GR.Dashboard.Abstractions.Models
{
    [NotMapped]
    [Author("Lupei Nicolae", 1.1)]
    public abstract class Widget : BaseModel
    {
        /// <summary>
        /// Render service
        /// </summary>
        protected virtual IWidgetRenderer<TWidget> Service<TWidget>() where TWidget : Widget
        {
            if (!IoC.IsServiceRegistered<IWidgetRenderer<TWidget>>()) throw new IoCNotRegisterServiceException();
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
        public virtual bool AllowCache { get; set; } = false;

        /// <summary>
        /// Time to refresh
        /// </summary>
        public virtual TimeSpan CacheRefreshSpan { get; set; }

        /// <summary>
        /// if is allowed allow cache
        /// </summary>
        public virtual DateTime? LastRefreshTime { get; set; }

        /// <summary>
        /// Razor template
        /// </summary>
        [Required]
        public virtual string Template { get; set; }

        /// <summary>
        /// Get or set system security
        /// </summary>
        public virtual bool IsSystem { get; set; }

        /// <summary>
        /// Template type
        /// </summary>
        public virtual WidgetTemplateType WidgetTemplateType { get; set; } = WidgetTemplateType.Html;

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
    }
}
