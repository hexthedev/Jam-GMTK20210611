using HexCS.Core;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    [System.Serializable]
    public class Tile
    {
        public SOFloorProperties Floor;

        public SoObject Object;

        public Tile Copy() => new Tile() { 
            Floor = Floor, 
            Object = Object 
        };
    }
}