using System;

namespace GBAHL.Text
{
    /// <summary>
    /// Represents a table of characters.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="characters">The character table.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="characters"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="characters"/> does not have a length of 256.
        /// </exception>
        public Table(string[] characters)
        {
            if (characters == null)
                throw new ArgumentNullException(nameof(characters));

            if (characters.Length != 256)
                throw new ArgumentException("Table must contain 256 characters.", nameof(characters));

            Characters = characters ?? throw new ArgumentNullException("characters");
        }

        /// <summary>
        /// Decodes the specified value.
        /// </summary>
        /// <param name="value">The value to be decoded.</param>
        /// <returns>A string representing the decoded character.</returns>
        public string GetCharacter(byte value) => Characters[value] ?? string.Empty;

        /// <summary>
        /// Encodes the specified character.
        /// </summary>
        /// <param name="character">The character to be encoded.</param>
        /// <returns>The value of the character if found in the table; otherwise, -1.</returns>
        public int GetByte(string character)
        {
            for (int i = 0; i < Characters.Length; i++)
            {
                if (Characters[i] == character)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the character table.
        /// </summary>
        public string[] Characters { get; }
    }
}
