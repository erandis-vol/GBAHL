using System;

namespace GBAHL.Asm
{
    /// <summary>
    /// Represents a single line of assembly code.
    /// </summary>
    public class AssemblyLine
    {
        private static readonly string[] noParameters = new string[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLine"/> class.
        /// </summary>
        private AssemblyLine(AssemblyLineType type, string value, string[] parameters, int number)
        {
            Type = type;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Number = number;
        }

        /// <summary>
        /// Creates a new empty line.
        /// </summary>
        /// <param name="number">The line number.</param>
        /// <returns></returns>
        public static AssemblyLine Empty(int number) =>
            new AssemblyLine(
                AssemblyLineType.None,
                string.Empty,
                noParameters,
                number
            );

        /// <summary>
        /// Creates a new label line.
        /// </summary>
        /// <param name="value">The label value.</param>
        /// <param name="number">The line number.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public static AssemblyLine Label(string value, int number) =>
            new AssemblyLine(
                AssemblyLineType.Label,
                value,
                noParameters,
                number
            );

        /// <summary>
        /// Creates a new directive line with no parameters.
        /// </summary>
        /// <param name="value">The directive value.</param>
        /// <param name="number">The line number.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public static AssemblyLine Directive(string value, int number) =>
            new AssemblyLine(
                AssemblyLineType.Directive,
                value,
                noParameters,
                number
            );

        /// <summary>
        /// Creates a new directive line.
        /// </summary>
        /// <param name="value">The directive value.</param>
        /// <param name="parameters">The directive parameters.</param>
        /// <param name="number">The line number.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="parameters"/> is <c>null</c>.
        /// </exception>
        public static AssemblyLine Directive(string value, string[] parameters, int number) =>
            new AssemblyLine(
                AssemblyLineType.Directive,
                value,
                parameters,
                number
            );

        /// <summary>
        /// Creates a new instruction line with no parameters.
        /// </summary>
        /// <param name="value">The isntruction value.</param>
        /// <param name="number">The line number.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public static AssemblyLine Instruction(string value, int number) =>
            new AssemblyLine(
                AssemblyLineType.Instruction,
                value,
                noParameters,
                number
            );

        /// <summary>
        /// Creates a new instruction line.
        /// </summary>
        /// <param name="value">The instruction value.</param>
        /// <param name="parameters">The directive parameters.</param>
        /// <param name="number">The line number.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="parameters"/> is <c>null</c>.
        /// </exception>
        public static AssemblyLine Instruction(string value, string[] parameters, int number) =>
            new AssemblyLine(
                AssemblyLineType.Instruction,
                value,
                parameters,
                number
            );

#if DEBUG

        /// <summary>
        /// Returns a string representation of the line.
        /// </summary>
        /// <returns>A string representation of the line.</returns>
        public override string ToString()
        {
            if (Parameters != null && Parameters.Length > 0)
            {
                return $"{{Type={Type}, Value={Value}, Parameters={string.Join(",", Parameters)}, Number={Number}}}";
            }
            else
            {
                return $"{{Type={Type}, Value={Value}, Number={Number}}}";
            }
        }

#endif

        /// <summary>
        /// Gets the type.
        /// </summary>
        public AssemblyLineType Type { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public string[] Parameters { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int Number { get; }
    }
}
