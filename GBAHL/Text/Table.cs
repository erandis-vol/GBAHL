using System;

namespace GBAHL.Text
{
    /// <summary>
    /// Represents a table of characters.
    /// </summary>
    public class Table
    {
        public Table(string[] characters)
        {
            if (characters == null)
                throw new ArgumentNullException(nameof(characters));

            if (characters.Length != 256)
                throw new ArgumentException("Table must contain 256 characters.", nameof(characters));

            Characters = characters ?? throw new ArgumentNullException("characters");
        }

        #region Methods

        public string GetCharacter(byte value) => Characters[value] ?? string.Empty;

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

        #endregion

        // TODO: Investigate the best way to represent multiple-byte constants.
        //       Maybe as an integer? Note that constants are never more than 4 bytes.

        public string[] Characters { get; }
    }
}
