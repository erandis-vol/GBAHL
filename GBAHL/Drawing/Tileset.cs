using System;
using System.Collections.Generic;
using System.Linq;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents a collection of drawable tiles.
    /// </summary>
    public class Tileset
    {
        private Tile[] tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class with the specified length.
        /// </summary>
        /// <param name="length">The length of the tileset.</param>
        public Tileset(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            tiles = new Tile[length];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class for the specified tiles.
        /// </summary>
        /// <param name="tiles"></param>
        public Tileset(IEnumerable<Tile> tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            this.tiles = tiles.ToArray();
        }

        /// <summary>
        /// Gets the specified tile.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <returns></returns>
        public ref Tile this[int index]
        {
            get => ref tiles[index];
        }

        /// <summary>
        /// Determines all values that create an image with no extra tiles.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetPerfectColumns()
        {
            for (int i = 1; i <= tiles.Length; i++)
            {
                if (tiles.Length % i == 0)
                    yield return i;
            }
        }

        /// <summary>
        /// Converts the tileset to an array of tiles.
        /// </summary>
        /// <returns></returns>
        public Tile[] ToArray()
        {
            return (Tile[])tiles.Clone();
        }

        /// <summary>
        /// Gets the number of tiles.
        /// </summary>
        public int Length => tiles.Length;
    }
}
