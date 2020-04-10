using System;

namespace GR.Processes.Abstractions.ViewModels
{
    public class UpdateProcessDiagramViewModel
    {
        public string Diagram { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}