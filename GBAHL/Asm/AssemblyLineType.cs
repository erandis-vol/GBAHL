namespace GBAHL.Asm
{
    /// <summary>
    /// Represents the type of an <see cref="AssemblyLine"/>.
    /// </summary>
    public enum AssemblyLineType
    {
        /// <summary>
        /// The line is empty.
        /// </summary>
        None,

        /// <summary>
        /// The line represents a label.
        /// </summary>
        Label,

        /// <summary>
        /// The line represents a directive.
        /// </summary>
        Directive,

        /// <summary>
        /// The line represents an instruction.
        /// </summary>
        Instruction,
    }
}
