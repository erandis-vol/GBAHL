using System;
using System.Collections.Generic;
using System.IO;

namespace GBAHL.Text
{
    // Helpers for encoding

    internal static class Common
    {
        public static IEnumerable<string> Split(char[] chars, int index, int count)
        {
            return Split(new string(chars, index, count));
        }

        public static IEnumerable<string> Split(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (c == '\\')
                {
                    if (i >= str.Length - 1)
                    {
                        throw new InvalidDataException("Malformed string.");
                    }

                    yield return "\\" + str[++i];
                    continue;
                }

                if (c == '[')
                {
                    var start = i;

                    // scan until we reach ']'
                    while (i < str.Length && str[i] != ']')
                    {
                        i++;
                    }

                    // ensure the string is well-formed
                    if (i >= str.Length)
                    {
                        throw new InvalidDataException("Malformed string.");
                    }

                    yield return str.Substring(start, start - i);
                    continue;
                }

                yield return new string(c, 1);
            }
        }
    }
}
