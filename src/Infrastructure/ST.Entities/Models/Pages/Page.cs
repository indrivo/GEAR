using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.Models.Pages
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Page : BaseModel
    {
        public PageType PageType { get; set; }
        public Guid PageTypeId { get; set; }
        public PageSettings Settings { get; set; }
        public Guid? SettingsId { get; set; }
        public string Path { get; set; }
        public bool IsSystem { get; set; }
        public bool IsLayout { get; set; }
        public Page Layout { get; set; }
        public Guid? LayoutId { get; set; }
        public IEnumerable<PageScript> PageScripts { get; set; }
        public IEnumerable<PageStyle> PageStyles { get; set; }
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
