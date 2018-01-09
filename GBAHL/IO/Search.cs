using System;
using System.IO;

namespace GBAHL.IO
{
    public static class Search
    {
        public const int NotFound = -1;

        /// <summary>
        /// Searches the stream for the specified values.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="values">The values to find.</param>
        /// <param name="startAt">The offset to start searching from.</param>
        public static int Find(this Stream stream,
            byte[] values, int startAt = 0)
        {
            // Validate parameters
            if (stream.Length < values.Length + startAt || startAt < 0)
                throw new ArgumentOutOfRangeException("startAt");

            // Empty value search, never found
            if (values == null || values.Length == 0)
                return NotFound;

            // Single value search
            if (values.Length == 1)
                return Find(stream, values[0], startAt);

            // Get contents of stream
            var buffer = stream.ToArray();

            // Linear search for values
            for (int i = startAt; i < buffer.Length; i++)
            {
                if (buffer[i] == values[0] && buffer[i + 1] == values[1])
                {
                    var match = true;

                    for (int j = 2; j < values.Length; j++)
                    {
                        if (buffer[i + j] != values[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match) return i;
                }
            }

            // Search failed
            return NotFound;
        }

        /// <summary>
        /// Searches the stream for the specified value.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="startAt">The offset to start searching from.</param>
        /// <returns></returns>
        public static int Find(this Stream stream,
            byte value, int startAt = 0)
        {
            if (startAt >= stream.Length - 1 || startAt < 0)
                throw new ArgumentOutOfRangeException("startAt");

            // Get contents of stream
            var buffer = stream.ToArray();

            // Linear search for value
            for (int i = startAt; i < buffer.Length; i++)
            {
                if (buffer[i] == value)
                    return i;
            }

            // Search failed
            return NotFound;
        }

        /// <summary>
        /// Searches the stream for the specified repeated value.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="count">The number of times the value should repeat.</param>
        /// <param name="startAt">The offset to start searching from.</param>
        /// <returns></returns>
        public static int Find(this Stream stream,
            byte value, int count, int startAt = 0)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            if (count == 1)
                return Find(stream, value, startAt);

            var values = new byte[count];
            for (int i = 0; i < count; i++)
                values[i] = value;

            return Find(stream, values, startAt);
        }
    }
}
