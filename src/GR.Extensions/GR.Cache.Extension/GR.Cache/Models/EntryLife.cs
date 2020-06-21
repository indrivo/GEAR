using System;

namespace GR.Cache.Models
{
    internal class EntryLife<T>
    {
        public T Data { get; set; }
        public DateTime DateTime { get; set; }
    }
}
