using System;
using GR.Audit.Abstractions.Enums;

namespace GR.Audit.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrackField : Attribute
    {
        public TrackFieldOption Option;
    }
}
