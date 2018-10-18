using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GBAHL.Asm
{
    /// <summary>
    /// Reprsents a reader that can read assembly lines.
    /// </summary>
    public class AssemblyReader : IDisposable
    {
        /// <summary>The EOF value.</summary>
        private const int EOF = -1;

        /// <summary>The label character.</summary>
        private const char Label = ':';

        /// <summary>The whitespace characters.</summary>
        private static readonly char[] WhiteSpace = { ' ', '\t' };

        private TextReader reader;
        private int currentLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyReader"/> class for the specified string.
        /// </summary>
        /// <param name="s">The string to be read.</param>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        public AssemblyReader(string s)
        {
            reader = new StringReader(s);
            currentLine = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyReader"/> for the specified stream.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
        public AssemblyReader(Stream stream)
        {
            reader = new StreamReader(stream);
            currentLine = 1;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="AssemblyReader"/>.
        /// </summary>
        public void Dispose()
        {
            reader?.Dispose();
            reader = null;
        }

        /// <summary>
        /// Reads the next line from the stream.
        /// </summary>
        /// <returns></returns>
        public AssemblyLine ReadLine()
        {
            if (reader == null)
                return null;

            // Read the next line from the stream
            var line = reader.ReadLine();
            if (line == null)
                return null;

            // Trim trailing whitespace
            line = line.Trim();

            // Trim the comment
            line = AssemblyHelpers.RemoveComments(line);

            // Handle empty lines
            if (line.Length == 0)
                return AssemblyLine.Empty(currentLine++);

            // Handle labels
            var labelIndex = line.IndexOf(Label);
            if (labelIndex >= 0)
            {
                return AssemblyLine.Label(
                    line.Substring(0, labelIndex),
                    currentLine++
                );
            }

            // Handle directives
            var parameterIndex = line.IndexOfAny(WhiteSpace);
            if (parameterIndex >= 0)
            {
                var value = line.Substring(0, parameterIndex);
                var parameters = AssemblyHelpers.SplitParameters(line.Substring(parameterIndex)).ToArray();

                if (value.Length > 0 && value[0] == '.')
                {
                    return AssemblyLine.Directive(
                        value,
                        parameters,
                        currentLine++
                    );
                }
                else
                {
                    return AssemblyLine.Instruction(
                        value,
                        parameters,
                        currentLine++
                    );
                }
            }
            else
            {
                return (line.Length > 0 && line[0] == '.') ?  AssemblyLine.Directive(line, currentLine++)
                    : AssemblyLine.Instruction(line, currentLine++);
            }
        }

        /// <summary>
        /// Reads all lines until the end of the stream.
        /// </summary>
        /// <returns></returns>
        public AssemblyLine[] ReadToEnd()
        {
            var lines = new List<AssemblyLine>();

            while (HasMore)
            {
                var line = ReadLine();

                if (line == null)
                    break;

                lines.Add(line);
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Determines whether the reader has more lines to read.
        /// </summary>
        public bool HasMore
        {
            get
            {
                if (reader == null)
                    return false;

                return reader.Peek() != EOF;
            }
        }
    }
}
