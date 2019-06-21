using System;
using System.IO;

namespace GBAHL.Asm
{
    public class AssemblyWriter : IDisposable
    {
        private const string DefaultIndentation = "    ";

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
            WriteComment(comment);
            WriteLine();
        }

        public void WriteLine(AssemblyLine line)
        {
            WriteLine(line, null);
        }

        public void WriteLine(AssemblyLine line, string comment)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            switch (line.Type)
            {
                case AssemblyLineType.None:
                    break;

                case AssemblyLineType.Label:
                    if (IndentLabels)
                    {
                        WriteIndentation();
                    }

                    WriteLabel(line);
                    break;

                case AssemblyLineType.Directive:
                    if (IndentDirectives)
                    {
                        WriteIndentation();
                    }

                    WriteCommand(line);

                    if (line.Parameters.Length > 0)
                    {
                        if (IndentParameters)
                        {
                            WriteIndentation();
                        }
                        else
                        {
                            writer.Write(' ');
                        }

                        WriteParameters(line);
                    }
                    break;

                case AssemblyLineType.Instruction:
                    if (IndentInstructions)
                    {
                        WriteIndentation();
                    }

                    WriteCommand(line);

                    if (line.Parameters.Length > 0)
                    {
                        if (IndentParameters)
                        {
                            WriteIndentation();
                        }
                        else
                        {
                            writer.Write(' ');
                        }

                        WriteParameters(line);
                    }
                    break;

                default:
                    throw new ArgumentException($"Cannot write line of type {line.Type}.", nameof(line));
            }

            if (!string.IsNullOrEmpty(comment))
            {
                //WriteIndentation(); // or space?
                writer.Write(' ');
                WriteComment(comment);
            }

            writer.WriteLine();
        }

        private void WriteComment(string comment)
        {
            writer.Write("@ ");
            writer.Write(comment);
        }
        
        private void WriteIndentation()
        {
            writer.Write(Indentation ?? DefaultIndentation);
        }

        private void WriteLabel(AssemblyLine line)
        {
            writer.Write(line.Value);
            writer.Write(':');
        }

        private void WriteCommand(AssemblyLine line)
        {
            writer.Write(line.Value);
        }

        private void WriteParameters(AssemblyLine line)
        {
            writer.Write(string.Join(", ", line.Parameters));
        }

        public override string ToString()
        {
            if (writer is StringWriter stringWriter)
            {
                return writer.ToString();
            }

            return base.ToString();
        }

        /// <summary>
        /// Gets or sets the string used for indentation.
        /// </summary>
        public string Indentation { get; set; }

        /// <summary>
        /// Gets or sets whether instructions should be indented.
        /// </summary>
        public bool IndentInstructions { get; set; } = true;

        /// <summary>
        /// Gets or sets whether directives should be indented.
        /// </summary>
        public bool IndentDirectives { get; set; } = true;

        /// <summary>
        /// Gets or sets whether parameters should be indented.
        /// </summary>
        public bool IndentParameters { get; set; } = false;

        /// <summary>
        /// Gets or sets whether labels should be indented.
        /// </summary>
        public bool IndentLabels { get; set; } = false;
    }
}
