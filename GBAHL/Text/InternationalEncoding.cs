namespace GBAHL.Text
{
    /// <summary>
    /// Represents an international encoding of characters.
    /// </summary>
    public class InternationalEncoding : TableEncoding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternationalEncoding"/> class.
        /// </summary>
        public InternationalEncoding()
            : base(Tables.International)
        { }
    }
}
