using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Pepino.Retrievers.Additional
{
    /// <summary>
    /// Retrieves a <see cref="Color"/> from a string representation.
    /// </summary>
    /// <seealso cref="RetrieverBase" />
    /// <remarks>
    /// String must be:
    /// * a name of a color - in such case <see cref="Color"/>
    ///     with <see cref="Color.IsNamedColor"/> set to <c>true</c> is returned.
    /// * a 3, 4, 6, or 8 hexadecimla chars - in such case 3 or 4 chars 
    ///     are duplicated and new string is used to read RGB values, 
    ///     with Alpha channel value, if string is of 8 characters.
    /// </remarks>
    public class ColorRetriever : IRetriever
    {
        private readonly Regex rxHex = new Regex(
            @"^[0-9a-f]+$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);


        private ColorRetriever() { }

        private static Lazy<ColorRetriever> lazy
            => new Lazy<ColorRetriever>(() => new ColorRetriever());


        public static ColorRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;

            bool isHex = stringRepresentation != null
                && (stringRepresentation.StartsWith("#:", StringComparison.Ordinal)
                    || stringRepresentation.StartsWith('#'));

            if (isHex)
            {
                string v = stringRepresentation;
                if (v.StartsWith("#:", StringComparison.Ordinal))
                {
                    v = v[2..];
                }
                else if (v.StartsWith('#'))
                {
                    v = v[1..];
                }

                var l = v.Length;
                if (l < 3 || l == 5 || l == 7 || l > 8
                    || !rxHex.IsMatch(v))
                {
                    result = default(Color);
                    errorDescription = "Bad string length or not a hex number.";
                    return false;
                }

                if (l == 3 || l == 4)
                {
                    var sb = new StringBuilder(l * 2);
                    // duplicate every symbol
                    var chars = v.ToCharArray();
                    foreach (var c in chars)
                    {
                        _ = sb.Append(c, 2);
                    }
                    v = sb.ToString();
                    sb = null;
                    l *= 2;
                }

                int a;
                int r;
                int g;
                int b;
                if (l == 8)
                {
                    a = int.Parse(v[0..1], NumberStyles.HexNumber);
                    r = int.Parse(v[2..3], NumberStyles.HexNumber);
                    g = int.Parse(v[4..5], NumberStyles.HexNumber);
                    b = int.Parse(v[6..7], NumberStyles.HexNumber);
                }
                else
                {
                    a = 255;
                    r = int.Parse(v[0..1], NumberStyles.HexNumber);
                    g = int.Parse(v[2..3], NumberStyles.HexNumber);
                    b = int.Parse(v[4..5], NumberStyles.HexNumber);
                }

                errorDescription = null;
                result = Color.FromArgb(a, r, g, b);
            }
            else if (stringRepresentation is null)
            {
                errorDescription = "Cannot retrieve a Color from NULL string.";
                result = default;
                rv = false;
            }
            else
            {
                errorDescription = null;
                result = Color.FromName(stringRepresentation);
            }

            return rv;
        }
    }
}
