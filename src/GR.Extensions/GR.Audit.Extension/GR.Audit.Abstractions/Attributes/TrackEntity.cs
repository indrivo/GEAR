using System;
using GR.Audit.Abstractions.Enums;

namespace GR.Audit.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TrackEntity : Attribute
    {
        public TrackEntityOption Option;
    }
}
