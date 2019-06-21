using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GBAHL
{
    /// <summary>
    /// Represents a 32-bit pointer to a ROM address.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ptr : IEquatable<Ptr>
    {
        /// <summary>
        /// Represents a pointer that has been initialized to zero.
        /// </summary>
        public static readonly Ptr Zero;

        /// <summary>
        /// Represents a pointer that has an invalid value.
        /// </summary>
        public static readonly Ptr Invalid = new Ptr(-1);

        /// <summary>
        /// The maximum size of a single ROM. This allows for banks 08 through 0D.
        /// </summary>
        private const int MaximumRomSize = 0x5FFFFFF;

        /// <summary>
        /// The valuie of the pointer.
        /// </summary>
        private int value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ptr"/> struct for the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public Ptr(int address)
        {
            if (address >= 0)
            {
                value = address;
            }
            else
            {
                value = -1;
            }
        }

        #region Methods

        /// <summary>
        /// Converts a string representation of a pointer to its equivalent value.
        /// </summary>
        /// <param name="s">A string containing the pointer to convert.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Ptr result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = Zero;
                return true;
            }

            if (s.StartsWith("0x") || s.StartsWith("0X") || s.StartsWith("&h") || s.StartsWith("&H"))
            {
                try
                {
                    result = (Ptr)Convert.ToInt32(s.Substring(2), 16);
                    return true;
                }
                catch { }
            }
            else if (s.StartsWith("0b") || s.StartsWith("0B"))
            {
                try
                {
                    result = (Ptr)Convert.ToInt32(s.Substring(2), 2);
                    return true;
                }
                catch { }
            }
            else if (s.StartsWith("0o") || s.StartsWith("0O"))
            {
                try
                {
                    result = (Ptr)Convert.ToInt32(s.Substring(2), 8);
                    return true;
                }
                catch { }
            }
            else
            {
                try
                {
                    result = (Ptr)Convert.ToInt32(s, 10);
                    return true;
                }
                catch { }
            }

            result = Invalid;
            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="Ptr"/> is equivalent to the specified <see cref="Ptr"/>.
        /// </summary>
        /// <param name="other">A pointer to compare to.</param>
        /// <returns>
        /// True if <paramref name="other"/> is a <see cref="Ptr"/>
        /// with the same address as this <see cref="Ptr"/>; otherwise, false.
        /// </returns>
        public bool Equals(Ptr other)
        {
            return value == other.value;
        }

        /// <summary>
        /// Determines whether this <see cref="Ptr"/> is equivalent to the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a <see cref="Ptr"/>
        /// with the same address as this <see cref="Ptr"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is Ptr) && Equals((Ptr)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer has code.</returns>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Ptr"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="Ptr"/>.</returns>
        public override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Ptr"/> using the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>A string representation of this <see cref="Ptr"/>.</returns>
        public string ToString(string format)
        {
            return value.ToString(format);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Returns this 32-bit signed integer as a pointer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Ptr(int value)
        {
            return Unsafe.As<int, Ptr>(ref value);
        }

        /// <summary>
        /// Returns this <see cref="Ptr"/> as a 32-bit signed integer.
        /// </summary>
        /// <param name="ptr"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(Ptr ptr)
        {
            return ptr.value;
        }

        public static bool operator true(Ptr ptr) => ptr.IsValid;

        public static bool operator false(Ptr ptr) => !ptr.IsValid;

        public static bool operator ==(Ptr p1, Ptr p2) => p1.Equals(p2);

        public static bool operator !=(Ptr p1, Ptr p2) => !(p1 == p2);

        public static bool operator >=(Ptr p1, Ptr p2) => p1.Address >= p2.Address;

        public static bool operator <=(Ptr p1, Ptr p2) => p1.Address <= p2.Address;

        public static bool operator >(Ptr p1, Ptr p2) => p1.Address > p2.Address;

        public static bool operator <(Ptr p1, Ptr p2) => p1.Address < p2.Address;

        public static Ptr operator +(Ptr p1, Ptr p2)
        {
            p1.value += p2.value;
            return p1;
        }

        public static Ptr operator +(Ptr ptr, int value)
        {
            ptr.value += value;
            return ptr;
        }

        public static Ptr operator -(Ptr ptr, int value)
        {
            ptr.value -= value;
            return ptr;
        }

        public static Ptr operator -(Ptr p1, Ptr p2)
        {
            p1.value -= p2.value;
            return p1;
        }

        public static Ptr operator *(Ptr p1, Ptr p2)
        {
            p1.value *= p2.value;
            return p1;
        }

        public static Ptr operator *(Ptr ptr, int value)
        {
            ptr.value *= value;
            return ptr;
        }

        public static Ptr operator /(Ptr p1, Ptr p2)
        {
            p1.value /= p2.value;
            return p1;
        }

        public static Ptr operator /(Ptr ptr, int value)
        {
            ptr.value /= value;
            return ptr;
        }

        public static Ptr operator %(Ptr p1, Ptr p2)
        {
            p1.value %= p2.value;
            return p1;
        }

        public static Ptr operator %(Ptr ptr, int value)
        {
            ptr.value %= value;
            return ptr;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bank of this pointer.
        /// </summary>
        public byte Bank
        {
            get
            {
                if (IsValid)
                {
                    return (byte)(0x08 + (value >> 24));
                }

                return 0x00;
            }
        }

        /// <summary>
        /// Gets or sets the address of this pointer.
        /// </summary>
        public int Address
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// Determines whether this pointer is zero.
        /// </summary>
        public bool IsZero => value == 0;

        /// <summary>
        /// Determines whether this pointer is valid.
        /// </summary>
        public bool IsValid => value >= 0 && value <= MaximumRomSize;

        #endregion
    }
}
