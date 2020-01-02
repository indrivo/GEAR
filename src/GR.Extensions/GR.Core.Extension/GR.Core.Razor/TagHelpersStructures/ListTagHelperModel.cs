using System.Collections.Generic;
using GR.Core.Razor.Enums;
using GR.Core.Razor.TagHelpers.Drawers;
using GR.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;

namespace GR.Core.Razor.TagHelpersStructures
{
    public class ListTagHelperModel : TagHelperStructureBaseModel
    {
        #region General Data
        public virtual string ListIdentifier { get; set; }

        /// <summary>
        /// Jquery table dom
        /// </summary>
        public virtual string Dom { get; set; } = $"<\"table_render_{{Identifier}}\" <\"CustomizeColumns\">lBfr<\"table-responsive\"t>ip >";

        /// <summary>
        /// Style attributes
        /// </summary>
        public virtual ICollection<InlineStyleAttribute> StyleAttributes { get; set; } =
            new List<InlineStyleAttribute>();

        /// <summary>
        /// Get list identifier
        /// </summary>
        public virtual string Identifier =>
            string.IsNullOrEmpty(ListIdentifier)
                ? Title.ToLower().Replace(" ", string.Empty) : ListIdentifier;

        /// <summary>
        /// Provide the title of list
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Provide the sub title of page
        /// </summary>
        public string SubTitle { get; set; } = string.Empty;
        /// <summary>
        /// Documentation
        /// </summary>
        public virtual string Documentation { get; set; }
        #endregion

        #region Api Configuration
        public virtual ListApiConfigurationViewModel Api { get; set; }
        #endregion

        #region HeadList
        public ICollection<UrlTagHelperViewModel> HeadButtons { get; set; } = new List<UrlTagHelperViewModel>();
        #endregion

        #region Columns
        /// <summary>
        /// List of rendered columns
        /// </summary>
        public ICollection<ListRenderColumn> RenderColumns { get; set; } = new List<ListRenderColumn>();
        #endregion

        #region List Actions
        /// <summary>
        /// Has list actions
        /// </summary>
        public bool HasActions { get; set; }

        public ICollection<ListActionViewModel> ListActions { get; set; } = new List<ListActionViewModel>();

        /// <summary>
        /// Action drawer mode
        /// </summary>
        public BaseListActionDrawer ListActionDrawer { get; set; } = new BaseListActionDrawer();
        #endregion
    }

    public class ListRenderColumn
    {
        public ListRenderColumn(string columnName, string apiIdentifier)
        {
            ColumnName = columnName;
            ApiIdentifier = apiIdentifier;
        }

        /// <summary>
        /// Has custom template
        /// </summary>
        public virtual bool HasTemplate { get; set; }

        /// <summary>
        /// Column name
        /// </summary>
        public string ColumnName { get; set; }
        public string ApiIdentifier { get; set; }
        public string Template { get; set; }
        public ICollection<InlineStyleAttribute> StyleAttributes { get; set; } = new List<InlineStyleAttribute>();

        /// <summary>
        /// Attributes
        /// </summary>
        public virtual ICollection<HtmlAttribute> HtmlAttributes { get; set; } = new List<HtmlAttribute>();

        /// <summary>
        /// Use a system template
        /// </summary>
        public virtual RenderCellBodySystemTemplate BodySystemTemplate { get; set; } = RenderCellBodySystemTemplate.Undefined;
    }
}
