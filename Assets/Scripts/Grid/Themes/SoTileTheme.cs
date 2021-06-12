using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(SoTileTheme), menuName = "GMTK/TileTheme")]
    public class SoTileTheme : ScriptableObject
    {
        public Material Background;

        public List<SOFloorProperties> Floors;
        public List<Material> FloorMaterials;

        public List<SoObject> Objects;
        public List<Material> MaterialMap;


        public Material GetForObject(SoObject obj)
        {
            int index = Objects.IndexOf(obj);
            if (index == -1) return null;
            return MaterialMap[index];
        }


        public Material GetFloorMat(SOFloorProperties prop)
        {
            int index = Floors.IndexOf(prop);
            if (index == -1) return null;
            return FloorMaterials[index];
        }
    }
}