using System;
using ST.Audit.Enums;

namespace ST.Audit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrackField : Attribute
    {
        public TrackFieldOption Option;
    }
}
