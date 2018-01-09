using System;
using System.Globalization;
using System.Windows.Forms;

namespace GBAHL.Windows.Forms
{
    /// <summary>
    /// Represents a TextBox that accepts decimal integer values.
    /// </summary>
    public class DecimalBox : TextBox
    {
        public int MaximumValue { get; set; } = int.MaxValue - 1;
        public int MinimumValue { get; set; } = 0;

        public int Value
        {
            get
            {
                if (TextLength > 0)
                {
                    int i;
                    if (int.TryParse(Text, out i))
                        return i;
                }
                return 0;
            }
            set
            {
                Text = value.ToString();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;

            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (Value < MinimumValue)
                Value = MinimumValue;
            if (Value > MaximumValue)
                Value = MaximumValue;
        }
    }

    /// <summary>
    /// Represents a TextBox that accents hexadecimal integer values.
    /// </summary>
    public class HexBox : TextBox
    {
        public int MaximumValue { get; set; } = int.MaxValue - 1;
        public int MinimumValue { get; set; } = 0;

        public int Value
        {
            get
            {
                if (TextLength > 0)
                {
                    int i;
                    if (int.TryParse(Text, NumberStyles.HexNumber, null, out i))
                        return i;
                }
                return 0;
            }
            set
            {
                Text = value.ToString("X");
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                !(e.KeyChar >= 'a' && e.KeyChar <= 'f') &&
                !(e.KeyChar >= 'A' && e.KeyChar <= 'F'))
                e.Handled = true;

            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (Value < MinimumValue)
                Value = MinimumValue;
            if (Value > MaximumValue)
                Value = MaximumValue;
        }
    }
}
