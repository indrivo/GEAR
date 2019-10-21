using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;
using GR.Entities.Abstractions.Models.Tables;
using GR.PageRender.Abstractions.Models.PagesACL;

namespace GR.PageRender.Abstractions.Models.Pages
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Page : BaseModel
    {
        /// <summary>
        /// Page type reference
        /// </summary>
        public virtual PageType PageType { get; set; }
        public virtual Guid PageTypeId { get; set; }
        /// <summary>
        /// Store page settings
        /// </summary>
        public PageSettings Settings { get; set; }
        public virtual Guid? SettingsId { get; set; }
        /// <summary>
        /// Virtual path on server
        /// </summary>
        public virtual string Path { get; set; }
        /// <summary>
        /// Check if page is system
        /// </summary>
        public virtual bool IsSystem { get; set; }
        /// <summary>
        /// Store if object is layout
        /// </summary>
        public virtual bool IsLayout { get; set; }
        /// <summary>
        /// Layout reference
        /// </summary>
        public virtual Page Layout { get; set; }
        public virtual Guid? LayoutId { get; set; }
        /// <summary>
        /// Page js scripts
        /// </summary>
        public virtual IEnumerable<PageScript> PageScripts { get; set; }

        /// <summary>
        /// Page css files 
        /// </summary>
        public virtual IEnumerable<PageStyle> PageStyles { get; set; }

        /// <summary>
        /// Roles access for pages
        /// </summary>
        public virtual IEnumerable<RolePagesAcl> RolePagesAcls { get; set; }

        /// <summary>
        /// Is enabled to check ACL for user access
        /// </summary>
        public virtual bool IsEnabledAcl { get; set; }
    }

    public interface IPageItem
    {
        Guid PageId { get; set; }
        string Script { get; set; }
        int Order { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class PageScript : BaseModel, IPageItem
    {
        public Guid PageId { get; set; }
        public string Script { get; set; }
        public int Order { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class PageStyle : BaseModel, IPageItem
    {
        public Guid PageId { get; set; }
        public string Script { get; set; }
        public int Order { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class PageSettings : BaseModel
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string TitleTranslateKey { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string HtmlCode { get; set; }
        public string CssCode { get; set; }
        public string JsCode { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class PageType : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class BlockCategory : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Block : BaseModel
    {
        public virtual string Name { get; set; }
        public string Description { get; set; }
        public BlockCategory BlockCategory { get; set; }
        public virtual Guid BlockCategoryId { get; set; }
        public virtual string FaIcon { get; set; }
        public string HtmlCode { get; set; }
        public string CssCode { get; set; }
        public IEnumerable<BlockAttribute> Attributes { get; set; }
        public Guid? TableModelId { get; set; }
        public TableModel TableModel { get; set; }
    }

    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class BlockAttribute : BaseModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public Guid BlockId { get; set; }
    }
}
