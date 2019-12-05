using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class UpdateAdditionalStateSettingsViewModel
    {
        public virtual Guid? StateId { get; set; }
        public virtual Dictionary<string, string> Settings { get; set; }
    }
}
