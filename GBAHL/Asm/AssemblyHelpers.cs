using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAHL.Asm
{
    public static class AssemblyHelpers
    {
        private static readonly char[] Comments = new[] { '@', ';' };

        public static string RemoveComments(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            var index = str.IndexOfAny(Comments);
            if (index >= 0)
                return str.Remove(index);

            return str;
        }

        public static IEnumerable<string> SplitParameters(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (var i = 0; i < str.Length; i++)
                {
                    var c = str[i];
                    switch (c)
                    {
                        case ',':
                            if (i > 0)
                            {
                                // parameter
                                yield return str.Substring(0, i);
                            }

                            // remove parameter and ','
                            str = str.Substring(i + 1);

                            // reset position
                            i = 0;
                            break;

                        case '{':
                            //if (i != 0)
                            //    throw new Exception("Unexpected '{'.");

                            // the brackets should be well-formed, nesting is not allowed
                            var j = str.IndexOf('}');
                            if (j < 0)
                                throw new Exception("Mismatched brackets, expected '}'.");

                            // grab contents
                            yield return str.Substring(0, j + 1);

                            // remove parameter through '}'
                            str = str.Substring(j + 1);

                            // reset position
                            i = 0;
                            break;
                    }
                }

                if (str.Length > 0)
                {
                    yield return str;
                }
            }
        }
    }
}
