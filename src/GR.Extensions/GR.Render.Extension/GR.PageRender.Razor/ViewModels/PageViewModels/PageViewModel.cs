using GR.PageRender.Abstractions.Models.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.PageRender.Razor.ViewModels.PageViewModels
{
    public class PageViewModel : Page
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        public string TitleTranslateKey { get; set; }

        [Required]
        public new Guid PageTypeId { get; set; }

        [Required]
        public new string Path { get; set; }

        public IEnumerable<PageType> PageTypes { get; set; }
        public IEnumerable<Page> Layouts { get; set; }
        public string Description { get; set; }
    }
}