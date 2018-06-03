using System;
using System.IO;

namespace GBAHL.Text.Pokemon
{
    public class FireRedEncoding : PokemonEncoding
    {
        private static FireRedEncoding international;
        private static FireRedEncoding japanese;

        /// <summary>
        /// Initializes a new instance of the <see cref="FireRedEncoding"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        private FireRedEncoding(Table table)
            : base(table)
        { }

        /// <summary>
        /// The international FireRed character encoding.
        /// </summary>
        public static FireRedEncoding International
        {
            get => international ?? (international = new FireRedEncoding(new Table(new string[256] {
                " ", "À", "Á", "Â", "Ç", "È", "É", "Ê", "Ë", "Ì", "こ", "Î", "Ï", "Ò", "Ó", "Ô",
                "Œ", "Ù", "Ú", "Û", "Ñ", "ß", "à", "á", "ね", "ç", "è", "é", "ê", "ë", "ì", "ま",
                "î", "ï", "ò", "ó", "ô", "œ", "ù", "ú", "û", "ñ", "º", "ª", "\\h2C", "&", "+", "あ",
                "ぃ", "ぅ", "ぇ", "ぉ", "[lv]", "=", "ょ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ",
                "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
                "っ", "¿", "¡", "[pk]", "[mn]", "[po]", "[ke]", "[bl]", "[oc]", "[k]", "Í", "%", "(", ")", "セ", "ソ",
                "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "â", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "í",
                "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "[u]", "[d]", "[l]", "[r]", "ヲ", "ン", "ァ",
                "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ",
                "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ",
                "ッ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "!", "?", ".", "-", "[.]",
                "[...]", "[\"]", "\"", "[']", "'", "[m]", "[f]", "$", ",", "×", "/", "A", "B", "C", "D", "E",
                "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k",
                "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "[>]",
                ":", "Ä", "Ö", "Ü", "ä", "ö", "ü", "[^]", "[v]", "[<]", "\\l", "\\p", "\\c", "\\v", "\\n", "\\x",
            })));
        }

        /// <summary>
        /// The japanese FireRed character encoding.
        /// </summary>
        public static FireRedEncoding Japanese
        {
            get => japanese ?? (japanese = new FireRedEncoding(new Table(new string[256] {
                " ", "あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ",
                "た", "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま",
                "み", "む", "め", "も", "や", "ゆ", "よ", "ら", "り", "る", "れ", "ろ", "わ", "を", "ん", "ぁ",
                "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ",
                "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
                "っ", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス	", "セ", "ソ",
                "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ",
                "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ヲ", "ン", "ァ",
                "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ",
                "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ",
                "ッ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "！", "？", "。", "ー", "・",
                "[・・]", "『", "』", "「", "」", "[m]", "[f]", "円", ".", "×", "/", "A", "B", "C", "D", "E",
                "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k",
                "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "[>]",
                ":", "Ä", "Ö", "Ü", "ä", "ö", "ü", "[^]", "[v]", "[<]", "\\l", "\\p", "\\c", "\\v", "\\n", "\\x",
            })));
        }

        protected override string DecodeControl(ByteReader bytes)
        {
            try
            {
                switch (bytes.ReadByte())
                {
                    case 0x01: // COLOR
                        return $"\\h01\\h{bytes.ReadByte():X2}";
                    case 0x02: // HIGHLIGHT
                        return $"\\h02\\h{bytes.ReadByte():X2}";
                    case 0x03: // SHADOW
                        return $"\\h03\\h{bytes.ReadByte():X2}";
                    case 0x04: // COLOR and HIGHLIGHT
                        return $"\\h04\\h{bytes.ReadByte():X2}\\h{bytes.ReadByte():X2}";
                    case 0x06: // FONT SIZE
                        return bytes.ReadByte() == 0x00 ? "[small]" : "[normal]";
                    case 0x08: // PAUSE
                        return $"\\h08\\h{bytes.ReadByte():X2}";
                    case 0x09: // WAIT FOR BUTTON
                        return "[wait]";
                    case 0x0C: // SPECIAL CHARACTERS
                        return $"\\h0C\\h{bytes.ReadByte():X2}\\h{bytes.ReadByte():X2}";
                    case 0x0D: // INDENT
                        return $"\\h0D\\h{bytes.ReadByte():X2}";
                    case 0x10: // PLAY SONG
                        return $"\\h04\\h{bytes.ReadByte():X2}\\h{bytes.ReadByte():X2}";
                    case 0x15: // JAPANESE FONT
                        return "[jp]";
                    case 0x16: // INTERNATIONAL FONT
                        return "[intl]";
                    case 0x17: // PAUSE SONG
                        return "[pause_music]";
                    case 0x18: // RESUME SONG
                        return "[resume_music]";
                }
            }
            catch
            { }

            throw new InvalidDataException();
        }

        protected override string DecodeVariable(ByteReader bytes)
        {
            try
            { 
                switch (bytes.ReadByte())
                {
                    case 0x01:
                        return "[player]";
                    case 0x02:
                        return "[buffer1]";
                    case 0x03:
                        return "[buffer2]";
                    case 0x04:
                        return "[buffer3]";
                    case 0x06:
                        return "[rival]";
                }
            }
            catch
            { }

            throw new InvalidDataException();
        }

        protected override void EncodeControlOrChar(string ch, ByteWriter bytes)
        {
            switch (ch.ToLower())
            {
                case "[small]":
                    bytes.Write(0xFC);
                    bytes.Write(0x06);
                    bytes.Write(0x00);
                    break;

                case "[normal]":
                    bytes.Write(0xFC);
                    bytes.Write(0x06);
                    bytes.Write(0x01); // NOTE: any value > 0x00 is valid
                    break;

                case "[wait]":
                    bytes.Write(0xFC);
                    bytes.Write(0x09);
                    break;

                case "[jp]":
                case "[japanese]":
                    bytes.Write(0xFC);
                    bytes.Write(0x15);
                    break;

                case "[intl]":
                case "[international]":
                    bytes.Write(0xFC);
                    bytes.Write(0x16);
                    break;

                case "[pause_music]":
                    bytes.Write(0xFC);
                    bytes.Write(0x17);
                    break;

                case "[resume_music]":
                    bytes.Write(0xFC);
                    bytes.Write(0x18);
                    break;

                case "[player]":
                    bytes.Write(0xFD);
                    bytes.Write(0x01);
                    break;

                case "[buffer1]":
                    bytes.Write(0xFD);
                    bytes.Write(0x02);
                    break;

                case "[buffer2]":
                    bytes.Write(0xFD);
                    bytes.Write(0x03);
                    break;

                case "[buffer3]":
                    bytes.Write(0xFD);
                    bytes.Write(0x04);
                    break;

                case "[rival]":
                    bytes.Write(0xFD);
                    bytes.Write(0x06);
                    break;

                default:
                    bytes.Write(EncodeChar(ch));
                    break;
            }
        }
    }
}
