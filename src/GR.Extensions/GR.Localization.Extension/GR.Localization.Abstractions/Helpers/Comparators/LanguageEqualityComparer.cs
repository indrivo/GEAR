using System.Collections.Generic;
using GR.Localization.Abstractions.Models;

namespace GR.Localization.Abstractions.Helpers.Comparators
{
    /// <summary>
    /// This equality comparer will ensure that there are 
    /// unique language objects. Because we do not want to have
    /// Identical language identifiers.
    /// </summary>
    public class LanguageEqualityComparer : IEqualityComparer<Language>
    {
        public bool Equals(Language x, Language y) =>
            x.Identifier.Equals(y.Identifier);

        public int GetHashCode(Language obj) =>
            obj.Identifier.GetHashCode();
    }
}
