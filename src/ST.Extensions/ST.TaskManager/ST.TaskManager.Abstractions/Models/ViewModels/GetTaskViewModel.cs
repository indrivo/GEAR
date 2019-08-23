using System;
using System.Collections.Generic;
using System.Text;
using ST.TaskManager.Abstractions.Enums;

namespace ST.TaskManager.Abstractions.Models.ViewModels
{
    public class GetTaskViewModel : CreateTaskViewModel
    {
        public Guid Id { get; set; }
    }
}
