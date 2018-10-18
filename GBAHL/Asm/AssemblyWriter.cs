using System;
using System.IO;

namespace GBAHL.Asm
{
    public class AssemblyWriter : IDisposable
    {
        public const char DefaultCommentChar = ';';

        private TextWriter writer;

        public AssemblyWriter()
        {
            writer = new StringWriter();
        }

        public AssemblyWriter(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException();

            writer = new StreamWriter(stream);
        }

        ~AssemblyWriter()
        {
            Dispose();
        }

        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }

        public void WriteLine()
        {
            writer.WriteLine();
        }

        public void WriteLine(string comment)
        {
            WriteLine(comment, DefaultCommentChar);
        }

        public void WriteLine(string comment, char commentChar)
        {
            writer.WriteLine(commentChar + " " + comment);
        }

        public void WriteLine(AssemblyLine line)
        {
            WriteLine(line, null, DefaultCommentChar);
        }

        public void WriteLine(AssemblyLine line, string comment)
        {
            WriteLine(line, comment, DefaultCommentChar);
        }

        public void WriteLine(AssemblyLine line, string comment, char commentChar)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            switch (line.Type)
            {
                case AssemblyLineType.None:
                    break;

                case AssemblyLineType.Label:
                    writer.Write(line.Value + ":");
                    break;

                case AssemblyLineType.Directive:
                case AssemblyLineType.Instruction:
                    writer.Write(line.Value);

                    if (line.Parameters.Length > 0)
                    {
                        writer.Write(' ');
                        writer.Write(string.Join(", ", line.Parameters));
                    }
                    break;

                default:
                    throw new ArgumentException($"Cannot write line of type {line.Type}.", nameof(line));
            }

            if (!string.IsNullOrEmpty(comment))
            {
                writer.Write(" " + commentChar + " " + comment);
            }

            writer.WriteLine();
        }

        public override string ToString()
        {
            if (writer is StringWriter stringWriter)
            {
                return writer.ToString();
            }

            return base.ToString();
        }
    }
}
