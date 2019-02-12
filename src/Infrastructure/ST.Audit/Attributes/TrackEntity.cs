using System;
using ST.Audit.Enums;

namespace ST.Audit.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TrackEntity : Attribute
    {
        public TrackEntityOption Option;
    }
}
