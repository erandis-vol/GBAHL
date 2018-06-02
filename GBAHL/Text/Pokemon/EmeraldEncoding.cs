namespace GBAHL.Text.Pokemon
{
    public class EmeraldEncoding : PokemonEncoding
    {
        private EmeraldEncoding(Table table)
            : base(table)
        { }

        protected override string DecodeControl(ByteReader bytes)
        {
            throw new System.NotImplementedException();
        }

        protected override string DecodeVariable(ByteReader bytes)
        {
            throw new System.NotImplementedException();
        }

        protected override void EncodeControlOrChar(string ch, ByteWriter bytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
