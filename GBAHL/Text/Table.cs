using System;

namespace GBAHL.Text
{
    internal class Table
    {
        private string[] characters;

        public Table(string[] characters)
        {
            // NOTE: assumes 256 characters are provided
            this.characters = characters ?? throw new ArgumentNullException("characters");
        }

        #region Methods

        public string GetCharacter(byte value)
        {
            return characters[Math.Max(0, Math.Min(value, characters.Length - 1))];
        }

        public int GetByte(string character)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] == character)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        // TODO: Investigate the best way to represent multiple-byte constants.
        //       Maybe as an integer? Note that constants are never more than 4 bytes.

        // TODO: Constants

        public string[] Characters => characters;
    }
}
