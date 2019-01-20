using System.Text;

namespace GBAHL.Utils
{
    internal static class StringHelper
    {
        public static string ToString(byte[] bytes, bool breakOnNull)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                if (b != 0)
                {
                    sb.Append((char)b);
                }
                else if (breakOnNull)
                {
                    break;
                }
            }

            return sb.ToString();
        }

        public static string StripNull(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                if (c != '\0')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
