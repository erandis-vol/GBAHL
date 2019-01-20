using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GBAHL
{
    /// <summary>
    /// Represents a 32-bit pointer to a ROM address.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GbaPtr : IEquatable<GbaPtr>
    {
        /// <summary>
        /// Represents a pointer that has been initialized to zero.
        /// </summary>
        public static GbaPtr Zero = new GbaPtr();

        /// <summary>
        /// Represents a pointer that has an invalid value.
        /// </summary>
        public static GbaPtr Invalid = new GbaPtr(-1);

        /// <summary>
        /// The maximum size of a single ROM. This allows for banks 08 through 0D.
        /// </summary>
        private const int MaximumRomSize = 0x5FFFFFF;

        /// <summary>
        /// The address of the pointer.
        /// </summary>
        private int _address;

        /// <summary>
        /// Initializes a new instance of the <see cref="GbaPtr"/> struct for the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public GbaPtr(int address)
        {
            _address = address;
        }

        /// <summary>
        /// Returns this 32-bit signed integer as a pointer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GbaPtr(int value)
        {
            return Unsafe.As<int, GbaPtr>(ref value);
        }

        /// <summary>
        /// Returns this <see cref="GbaPtr"/> as a 32-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(GbaPtr value)
        {
            return Unsafe.As<GbaPtr, int>(ref value);
        }

        /// <summary>
        /// Determines whether this <see cref="GbaPtr"/> is equivalent to the specified <see cref="GbaPtr"/>.
        /// </summary>
        /// <param name="other">A pointer to compare to.</param>
        /// <returns>
        /// True if <paramref name="other"/> is a <see cref="GbaPtr"/>
        /// with the same address as this <see cref="GbaPtr"/>; otherwise, false.
        /// </returns>
        public bool Equals(GbaPtr other)
        {
            return _address == other._address;
        }

        /// <summary>
        /// Determines whether this <see cref="GbaPtr"/> is equivalent to the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a <see cref="GbaPtr"/>
        /// with the same address as this <see cref="GbaPtr"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is GbaPtr) && Equals((GbaPtr)obj);
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
        /// Returns a string representation of this <see cref="GbaPtr"/>.
        /// </summary>
        /// <returns>a string representation of this <see cref="GbaPtr"/>.</returns>
        public override string ToString()
        {
            return $"0x{_address:X6}";
        }

        /// <summary>
        /// Gets the bank of this <see cref="GbaPtr"/>.
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
        /// Gets or sets the address of this <see cref="GbaPtr"/>.
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
        /// Determines whether this <see cref="GbaPtr"/> is valid.
        /// </summary>
        public bool IsValid => _address >= 0 && _address <= MaximumRomSize;
    }
}
