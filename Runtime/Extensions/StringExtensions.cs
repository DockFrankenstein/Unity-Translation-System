using System.Collections.Generic;
using System.Linq;
using System;

namespace Translations
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitWithSplits(this IEnumerable<string> array, string split, StringSplitOptions splitOptions = StringSplitOptions.None) =>
            array.SelectMany(x => x.Split(split, splitOptions).InsertBetween(split))
            .Where(x => !string.IsNullOrEmpty(x));
    }
}