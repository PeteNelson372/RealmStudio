/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using System.Text.RegularExpressions;

namespace RealmStudio
{
    public static class StringExtensions
    {
        // from: https://stackoverflow.com/questions/6724840/how-can-i-truncate-my-strings-with-a-if-they-are-too-long

        public static string Truncate(this string value, int maxLength, bool addEllipsis = false)
        {
            // Check for valid string before attempting to truncate
            if (string.IsNullOrEmpty(value)) return value;

            // Proceed with truncating
            string? result;
            if (value.Length > maxLength)
            {
                result = value[..maxLength];
                if (addEllipsis) result += "...";
            }
            else
            {
                result = value;
            }

            return result;
        }

        public static string FindBestMatch(string stringToCompare, IEnumerable<string> strs)
        {
            HashSet<string> strCompareHash = [.. SplitCamelCase(stringToCompare).Split(' ')];

            int maxIntersectCount = 0;
            string bestMatch = string.Empty;

            foreach (string str in strs)
            {
                HashSet<string> strHash = [.. SplitCamelCase(str).Split(" ")];

                int intersectCount = strCompareHash.Intersect(strHash).Count();

                if (intersectCount > maxIntersectCount)
                {
                    maxIntersectCount = intersectCount;
                    bestMatch = str;
                }
            }

            return bestMatch;
        }

        public static string SplitCamelCase(string str)
        {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        }
    }
}
