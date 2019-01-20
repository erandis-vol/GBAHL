using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GBAHL
{
    /// <summary>
    /// Represents a 24-bit pointer to a ROM address.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GbPtr : IEquatable<GbPtr>
    {
        /// <summary>
        /// Represents a pointer that has been initialized to zero.
        /// </summary>
        public static GbPtr Zero = new GbPtr();

        private byte _bank;
        private ushort _address;

        /// <summary>
        /// Initializes a new instance of the <see cref="GbPtr"/> struct for the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public GbPtr(byte bank, ushort address)
        {
            _bank = bank;
            _address = address;
        }

        /// <summary>
        /// Returns this 32-bit signed integer as a pointer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GbPtr(ushort value)
        {
            return new GbPtr((byte)(value / 0x4000), (ushort)(value % 0x4000));
        }

        /// <summary>
        /// Returns this 32-bit signed integer as a pointer.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GbPtr(int value)
        {
            return new GbPtr((byte)(value / 0x4000), (ushort)(value % 0x4000));
        }

        /// <summary>
        /// Determines whether this <see cref="GbPtr"/> is equivalent to the specified <see cref="GbPtr"/>.
        /// </summary>
        /// <param name="other">A pointer to compare to.</param>
        /// <returns>
        /// True if <paramref name="other"/> is a <see cref="GbPtr"/>
        /// with the same address as this <see cref="GbPtr"/>; otherwise, false.
        /// </returns>
        public bool Equals(GbPtr other)
        {
            return _address == other._address;
        }

        /// <summary>
        /// Determines whether this <see cref="GbPtr"/> is equivalent to the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a <see cref="GbPtr"/>
        /// with the same address as this <see cref="GbPtr"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is GbPtr) && Equals((GbPtr)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer has code.</returns>
        public override int GetHashCode()
        {
            return _bank.GetHashCode() ^ _address.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="GbPtr"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="GbPtr"/>.</returns>
        public override string ToString()
        {
            return ((_bank * 0x4000) + _address).ToString();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="GbPtr"/> using the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>A string representation of this <see cref="GbPtr"/>.</returns>
        public string ToString(string format)
        {
            return ((_bank * 0x4000) + _address).ToString(format);
        }

        /// <summary>
        /// Gets or sets the bank of this <see cref="GbPtr"/>.
        /// </summary>
        public byte Bank
        {
            get => _bank;
            set
            {
                if (_bank != value)
                {
                    _bank = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the address of this <see cref="GbPtr"/>.
        /// </summary>
        public ushort Address
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
    }
}
