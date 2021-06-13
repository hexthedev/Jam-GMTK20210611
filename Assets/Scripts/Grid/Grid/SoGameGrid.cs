using HexCS.Core;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = "GameGrid", menuName = "GMTK/GameGrid")]
    public class SoGameGrid : ScriptableObject
    {
        public string name;
        public GameObject bgprefab;

        public int Width;
        public int Height;

        [SerializeField]
        private Tile _defaultTile;

        [SerializeField]
        private Tile[] _tileGrid;

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

        public Tile[] CopyTiles()
        {
            Tile[] tiles = new Tile[_tileGrid.Length];

            for(int i = 0; i<tiles.Length; i++)
            {
                tiles[i] = _tileGrid[i].Copy();
            }

            return tiles;
        }

    }
}