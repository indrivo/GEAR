using System.Collections.Generic;
using ST.CORE.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;

namespace ST.CORE.Razor.TagHelpersStructures
{
    public class ListTagHelperModel : TagHelperStructureBaseModel
    {
        #region General Data
        public virtual string ListIdentifier { get; set; }

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
        public string SubTitle { get; set; }
        /// <summary>
        /// Documentation
        /// </summary>
        public virtual string Documentation { get; set; }
        #endregion

        #region Api Configuration
        public virtual ListApiConfigurationViewModel Api { get; set; }
        #endregion

        #region HeadList
        public ICollection<UrlTagHelperViewModel> HeadButtons { get; set; }
        public virtual ICollection<string> Columns { get; set; }
        #endregion

        #region Render Columns
        /// <summary>
        /// List of rendered columns
        /// </summary>
        public ICollection<ListRenderColumn> RenderColumns { get; set; }
        #endregion

        #region List Actions
        /// <summary>
        /// Has list actions
        /// </summary>
        public bool HasActions { get; set; }

        public ICollection<ListActionViewModel> ListActions { get; set; }
        #endregion
    }

    public class ListRenderColumn
    {
        public ListRenderColumn(string columnName, string apiIdentifier)
        {
            ColumnName = columnName;
            ApiIdentifier = apiIdentifier;
        }

        public bool HasTemplate { get; set; }
        public string ColumnName { get; set; }
        public string ApiIdentifier { get; set; }
        public string Template { get; set; }
    }
}
