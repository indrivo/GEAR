using Microsoft.EntityFrameworkCore;
using ST.Entities.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;
using ST.PageRender.Abstractions.Models.PagesACL;
using ST.PageRender.Abstractions.Models.RenderTemplates;
using ST.PageRender.Abstractions.Models.ViewModels;

namespace ST.PageRender.Abstractions
{
    public interface IDynamicPagesContext : IEntityContext
    {
        DbSet<Page> Pages { get; set; }
        DbSet<PageType> PageTypes { get; set; }
        DbSet<PageSettings> PageSettings { get; set; }
        DbSet<Block> Blocks { get; set; }
        DbSet<BlockCategory> BlockCategories { get; set; }
        DbSet<BlockAttribute> BlockAttributes { get; set; }
        DbSet<PageScript> PageScripts { get; set; }
        DbSet<PageStyle> PageStyles { get; set; }
        DbSet<Template> Templates { get; set; }
        DbSet<ViewModel> ViewModels { get; set; }
        DbSet<ViewModelFields> ViewModelFields { get; set; }
        DbSet<RolePagesAcl> RolePagesAcls { get; set; }
        DbSet<ViewModelFieldCode> ViewModelFieldCodesCodes { get; set; }
        DbSet<ViewModelFieldConfiguration> ViewModelFieldConfigurations { get; set; }
    }
}
