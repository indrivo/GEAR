using Microsoft.EntityFrameworkCore;
using GR.Entities.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;
using GR.PageRender.Abstractions.Models.PagesACL;
using GR.PageRender.Abstractions.Models.RenderTemplates;
using GR.PageRender.Abstractions.Models.ViewModels;

namespace GR.PageRender.Abstractions
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
