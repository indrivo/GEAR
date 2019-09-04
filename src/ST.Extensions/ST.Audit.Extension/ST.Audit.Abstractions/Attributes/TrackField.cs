using System;
using ST.Audit.Abstractions.Enums;

namespace ST.Audit.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrackField : Attribute
    {
        public TrackFieldOption Option;
    }
}
