using HexCS.Core;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = "GameGrid", menuName = "GMTK/GameGrid")]
    public class SoGameGrid : ScriptableObject
    {
        [SerializeField]
        private Tile _defaultTile;

        [SerializeField]
        private Tile[] _tileGrid;

        public int Width;
        public int Height;

        public Tile[] TileGrid { 
            get {
                if(_tileGrid.Length != Width*Height)
                {
                    _tileGrid = UTArray.ConstructArray(
                        Width * Height,
                        _defaultTile.Copy
                    );
                }

                return _tileGrid;
            }
        }

    }
}