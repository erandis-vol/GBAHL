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
        public static Ptr Zero = new Ptr();

        /// <summary>
        /// Represents a pointer that has an invalid value.
        /// </summary>
        public static Ptr Invalid = new Ptr(-1);

        /// <summary>
        /// The maximum size of a single ROM. This allows for banks 08 through 0D.
        /// </summary>
        private const int MaximumRomSize = 0x5FFFFFF;

        /// <summary>
        /// The address of the pointer.
        /// </summary>
        private int _address;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ptr"/> struct for the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public Ptr(int address)
        {
            _address = address;
        }

        /// <summary>
        /// Returns this 32-bit signed integer as a pointer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Ptr(int value)
        {
            return Unsafe.As<int, Ptr>(ref value);
        }

        /// <summary>
        /// Returns this <see cref="Ptr"/> as a 32-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Ptr value)
        {
            return Unsafe.As<Ptr, int>(ref value);
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
            return _address == other._address;
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
            return _address.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Ptr"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="Ptr"/>.</returns>
        public override string ToString()
        {
            return _address.ToString();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Ptr"/> using the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>A string representation of this <see cref="Ptr"/>.</returns>
        public string ToString(string format)
        {
            return _address.ToString(format);
        }

        /// <summary>
        /// Gets the bank of this <see cref="Ptr"/>.
        /// </summary>
        public byte Bank
        {
            get
            {
                if (IsValid)
                {
                    return (byte)(0x08 + (_address >> 24));
                }

                return 0x00;
            }
        }

        /// <summary>
        /// Gets or sets the address of this <see cref="Ptr"/>.
        /// </summary>
        public int Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                }
            }
        }

        /// <summary>
        /// Determines whether this <see cref="Ptr"/> is valid.
        /// </summary>
        public bool IsValid => _address >= 0 && _address <= MaximumRomSize;
    }
}
