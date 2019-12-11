using System.Collections.Generic;
using GR.Identity.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;

namespace GR.PageRender.Abstractions
{
    public interface IPageAclService
    {
        bool HasAccess(Page page, IEnumerable<GearRole> roles);
    }
}
