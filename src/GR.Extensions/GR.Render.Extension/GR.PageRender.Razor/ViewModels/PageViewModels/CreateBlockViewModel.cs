using GR.PageRender.Abstractions.Models.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.PageRender.Razor.ViewModels.PageViewModels
{
    public class CreateBlockViewModel : Block
    {
        public IEnumerable<BlockCategory> BlockCategories { get; set; }

        [Required]
        public override string Name { get; set; }

        [Required]
        public override Guid BlockCategoryId { get; set; }
    }
}