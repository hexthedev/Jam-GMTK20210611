using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(SoTileTheme), menuName = "GMTK/TileTheme")]
    public class SoTileTheme : ScriptableObject
    {
        public Material Floor_Base;
        public Material Floor_Wall;
        public Material Floor_Pit;
    
    
        public Material GetFloorMat(EFloorType type)
        {
            switch (type)
            {
                case EFloorType.Base: return Floor_Base;
                case EFloorType.Pit: return Floor_Pit;
                case EFloorType.Wall: return Floor_Wall;
            }

            return null;
        }
    }
}