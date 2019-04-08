using System;
using System.Collections.Generic;
using System.Text;
using ST.Identity.Services.Abstractions;

namespace ST.Configuration.Models
{
    public class StCollection<T> : List<T>, ICacheModel
    {

    }
}
