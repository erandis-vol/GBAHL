using System;
using System.Text;

namespace GBAHL.Text.Pokemon
{
    public abstract class PokemonEncoding : TableEncoding
    {
        protected PokemonEncoding(Table table)
            : base(table)
        { }

        protected override string Decode(ByteReader bytes)
        {
            var result = new StringBuilder();

            while (bytes.HasMore)
            {
                result.Append(DecodeCharOrSpecial(bytes));
            }

            return result.ToString();
        }

        protected string DecodeCharOrSpecial(ByteReader bytes)
        {
            var b = bytes.ReadByte();
            if (b == 0xFC)
            {
                return DecodeControl(bytes);
            }
            else if (b == 0xFD)
            {
                return DecodeVariable(bytes);
            }
            else
            {
                return DecodeChar(b);
            }
        }

        protected abstract string DecodeControl(ByteReader bytes);

        protected abstract string DecodeVariable(ByteReader bytes);

        public override byte[] Encode(string str)
        {
            var bytes = new ByteWriter();

            foreach (var ch in Split(str))
            {
                EncodeControlOrChar(ch, bytes);
            }

            return bytes.ToArray();
        }

        protected abstract void EncodeControlOrChar(string ch, ByteWriter bytes);
    }
}
