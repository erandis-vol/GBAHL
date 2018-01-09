namespace GBAHL.Text
{
    /// <summary>
    /// Represents a Japanese encoding of characters.
    /// </summary>
    public class JapaneseEncoding : TableEncoding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JapaneseEncoding"/> class.
        /// </summary>
        public JapaneseEncoding() : base(Tables.Japanese)
        { }
    }
}