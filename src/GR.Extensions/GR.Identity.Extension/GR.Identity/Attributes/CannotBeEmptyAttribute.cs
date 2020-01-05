using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CannotBeEmptyAttribute : RequiredAttribute
    {
        public override bool IsValid(object value) => (value as IEnumerable)?.GetEnumerator().MoveNext() ?? false;
    }
}