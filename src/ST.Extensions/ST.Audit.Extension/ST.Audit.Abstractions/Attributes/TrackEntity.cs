using System;
using ST.Audit.Abstractions.Enums;

namespace ST.Audit.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TrackEntity : Attribute
    {
        public TrackEntityOption Option;
    }
}
