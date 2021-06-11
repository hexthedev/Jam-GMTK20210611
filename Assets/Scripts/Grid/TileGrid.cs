using HexCS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class TileGrid : Grid<Tile>
    {
        public TileGrid(DiscreteVector2 size) : base(size, i => new Tile()) { }

        public TileGrid(DiscreteVector2 size, Func<DiscreteVector2, Tile> factory) : base(size, factory) { }

        public static TileGrid ConstructFrom(DiscreteVector2 size, Tile[] tiles)
        {
            return new TileGrid(size, Factory);
        
            Tile Factory(DiscreteVector2 coord)
            {
                return tiles[coord.X + size.X * coord.Y];
            }        
        }
    }
}