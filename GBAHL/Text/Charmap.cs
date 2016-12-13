using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GBAHL.Text
{
    /* Experimental, revisit later. */

    internal class Charmap
    {
        private Dictionary<char, byte[]> characters = new Dictionary<char, byte[]>();
        private byte[][] escapes = new byte[128][];
        private Dictionary<string, byte[]> constants = new Dictionary<string, byte[]>();

        public Charmap(string filename)
        {
            var reader = new CharmapReader(filename);
            while (true)
            {
                // Read left hand side
                var left = reader.ReadLeft();
                if (left.Type == LhsType.None)
                    break;

                // Read equal sign
                reader.ExpectEquals();

                // Read right side
                var right = reader.ReadRight();

                // Create definition
                switch (left.Type)
                {
                    case LhsType.Character:
                        characters[left.Character] = right;
                        break;
                    case LhsType.Escape:
                        escapes[left.Character] = right;
                        break;
                    case LhsType.Constant:
                        constants[left.Constant] = right;
                        break;
                }

                // Read rest of line
                reader.ExpectEmptyRestOfLine();
            }
        }

        public byte[] GetCharacter(char c)
        {
            return characters.ContainsKey(c) ? characters[c] : null;
        }

        public byte[] GetConstant(string s)
        {
            return constants.ContainsKey(s) ? constants[s] : null;
        }

        public byte[] GetEscape(char c)
        {
            return c < 128 ? escapes[c] : null;
        }
    }

    internal static class EncodingQ
    {
        public const byte EndOfString = 0xFF;

        public static byte[] GetBytes(string s, Charmap charmap)
        {
            // don't parse an empty string
            if (string.IsNullOrEmpty(s))
                return new byte[0];

            // BAD
            if (charmap == null)
                throw new ArgumentNullException("charmap");

            // list to hold output
            var result = new List<byte>();

            int i = 0;
            while (i < s.Length)
            {
                if (s[i] == '[')
                {
                    i++; // eat [

                    // gather constant
                    var sb = new StringBuilder();
                    while (i < s.Length && s[i] != ']')
                    {
                        sb.Append(s[i++]);
                    }

                    // ensure complete constant
                    if (i < s.Length && s[i++] != ']')
                        throw new Exception("Malformed constant!");

                    // check for mapping
                    var seq = charmap.GetConstant(sb.ToString());
                    if (seq == null)
                        throw new Exception("Invalid constant!");

                    // add to result
                    result.AddRange(seq);
                }
                else
                {
                    // check for an escape sequence
                    bool isEscape = s[i] == '\\';
                    if (isEscape)
                    {
                        // eat \
                        i++;

                        // escape sequence MUST allow a follow-up character
                        if (i >= s.Length)
                            throw new Exception("Invalid escape sequence!");

                        // that should be all
                    }

                    // get next character
                    char c = s[i++];

                    // get sequence
                    var seq = isEscape ? charmap.GetEscape(c) : charmap.GetCharacter(c);

                    // check if there was a mapping
                    if (seq == null)
                    {
                        if (isEscape)
                            throw new Exception("Invalid escape sequence!");
                        else
                            throw new Exception("Invalid character!");
                    }

                    // add to result
                    result.AddRange(seq);
                }
            }

            return result.ToArray();
        }

        public static string GetString(byte[] buffer, Charmap charmap)
        {
            if (buffer == null || buffer.Length == 0)
                return string.Empty;

            if (charmap == null)
                throw new ArgumentNullException("charmap");

            var sb = new StringBuilder();

            int i = 0;
            while (i < buffer.Length)
            {
                i++;
            }

            return sb.ToString();
        }
    }

    internal enum LhsType
    {
        Character, Escape, Constant, None
    }

    internal struct Lhs
    {
        public LhsType Type;
        public string Constant;
        public char Character;
    }

    internal class CharmapReader
    {
        private readonly char[] whitespace = { ' ', '\t', '\r' };

        private char[] buffer;
        private int pos = 0;
        private int line = 1;

        public CharmapReader(string filename)
        {
            // Read buffer from file
            buffer = File.ReadAllText(filename).ToCharArray();

            // Remove comments
            RemoveComments();
        }

        private char Read()
        {
            return pos < buffer.Length ? buffer[pos++] : '\0';
        }

        private char Peek()
        {
            return pos < buffer.Length ? buffer[pos] : '\0';
        }

        private Exception Error(string message)
        {
            return new Exception($"{line}. {message}");
        }

        public Lhs ReadLeft()
        {
            Lhs lhs = new Lhs();

            // Skip whitespace at the start of the line
            while (true)
            {
                SkipWhite();

                if (Peek() == '\n')
                {
                    pos++;
                    line++;
                }
                else break;
            }

            if (Peek() == '\0')
            {
                // End of file
                lhs.Type = LhsType.None;
            }
            else if (Peek() == '\'')
            {
                // Character/Escape
                pos++;

                // Check for an escape sequence
                bool isEscape = Peek() == '\\';
                if (isEscape)
                    pos++;

                // Get character
                lhs.Character = Read();

                // Check for closing '
                if (Read() != '\'')
                    throw Error("Invalid character literal!");

                // Interpret
                if (isEscape)
                {
                    if (lhs.Character >= 0x80)
                        throw Error("Non-ASCII escape characters are not supported!");

                    switch (lhs.Character)
                    {
                        case '\'':
                        case '\\':
                            lhs.Type = LhsType.Character;
                            break;

                        default:
                            lhs.Type = LhsType.Escape;
                            break;
                    }
                }
                else
                {
                    lhs.Type = LhsType.Character;
                }
            }
            else if (char.IsLetter(buffer[pos]))
            {
                // Constant
                var sb = new StringBuilder();

                while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
                {
                    sb.Append(Read());
                }

                lhs.Constant = sb.ToString();
                lhs.Type = LhsType.Constant;
            }
            else
            {
                throw Error("Junk at start of line.");
            }

            return lhs;
        }

        public byte[] ReadRight()
        {
            var sequence = new List<byte>();

            // Skip whitespace
            SkipWhite();

            // Read two hex digits at a time
            while (pos < buffer.Length - 1 && (IsHexDigit(buffer[pos]) && IsHexDigit(buffer[pos + 1])))
            {
                // Get digits
                int digit1 = ConvertHexDigit(Read());
                int digit2 = ConvertHexDigit(Read());

                // Add byte value to sequence
                sequence.Add((byte)((digit1 << 4) + digit2));

                // Skip more whitespace
                SkipWhite();
            }

            return sequence.ToArray();
        }

        public void ExpectEquals()
        {
            SkipWhite();

            if (Peek() != '=')
                throw Error($"Expected equal sign, got {Peek()}.");

            pos++;
        }

        public void ExpectEmptyRestOfLine()
        {
            SkipWhite();

            if (Peek() == '\n')
            {
                pos++;
                line++;
            }
            else
            {
                throw Error("Junk at end of line.");
            }
        }

        private void RemoveComments()
        {
            int i = 0;
            bool inString = false;

            while (i < buffer.Length)
            {
                if (inString)
                {
                    if (buffer[i] == '\\' && (i < buffer.Length - 1 && buffer[i + 1] == '\''))
                        i += 2;
                    else
                    {
                        if (buffer[i] == '\'')
                            inString = false;
                        i++;
                    }
                }
                else if (buffer[i] == '@')
                {
                    //buffer[i++] = ' ';
                    while (i < buffer.Length && buffer[i] != '\n')
                        buffer[i++] = ' ';
                }
                else
                {
                    if (buffer[i] == '\'')
                        inString = true;
                    i++;
                }
            }
        }

        private void SkipWhite()
        {
            while (whitespace.Contains(Peek()))
            {
                pos++;
            }
        }

        private bool IsHexDigit(char c)
        {
            return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9');
        }

        private int ConvertHexDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return c - 'a';
            if (c >= 'A' && c <= 'F')
                return c - 'A';

            throw Error($"{c} is not a hexadecimal digit!");
        }
    }
}
