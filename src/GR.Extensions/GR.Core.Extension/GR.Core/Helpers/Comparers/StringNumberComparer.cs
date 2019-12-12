using System;
using System.Collections.Generic;

namespace GR.Core.Helpers.Comparers
{
    public class StringNumberComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int compareResult;
            int xIndex = 0, yIndex = 0;
            int xIndexLast = 0, yIndexLast = 0;
            var xLength = x?.Length;
            var yLength = y?.Length;

            do
            {
                var xHasNextNumber = TryGetNextNumber(x, ref xIndex, out var xNumber);
                var yHasNextNumber = TryGetNextNumber(y, ref yIndex, out var yNumber);

                if (!(xHasNextNumber && yHasNextNumber))
                {
                    // At least one the strings has either no more number or contains non-numeric chars
                    // In this case do a string comparison of that last part
                    return string.Compare(x?.Substring(xIndexLast), y?.Substring(yIndexLast), StringComparison.Ordinal);
                }

                xIndexLast = xIndex;
                yIndexLast = yIndex;

                compareResult = xNumber.CompareTo(yNumber);
            }
            while (compareResult == 0
                   && xIndex < xLength
                   && yIndex < yLength);

            return compareResult;
        }

        private static bool TryGetNextNumber(string text, ref int startIndex, out int number)
        {
            number = 0;

            var pos = text.IndexOf('.', startIndex);
            if (pos < 0) pos = text.Length;

            if (!int.TryParse(text.Substring(startIndex, pos - startIndex), out number))
                return false;

            startIndex = pos + 1;

            return true;
        }
    }
}
