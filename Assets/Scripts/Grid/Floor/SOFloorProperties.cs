using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(SOFloorProperties), menuName = "GMTK/FloorBehaviour")]
    public class SOFloorProperties : ScriptableObject, IFloor
    {
        public EFloorType Type;

        public bool BlockUp = false;
        public bool BlockRight = false;
        public bool BlockDown = false;
        public bool BlockLeft = false;

        public bool PlayerDies = false;
        public bool PlayerWins = false;

        public Sprite Sprite;
        public RuntimeAnimatorController AnimatorController;
    }
}