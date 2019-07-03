using System.Collections.Generic;
using ST.Identity.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;

namespace ST.PageRender.Abstractions
{
    public interface IPageAclService
    {
        bool HasAccess(Page page, IEnumerable<ApplicationRole> roles);
    }
}
