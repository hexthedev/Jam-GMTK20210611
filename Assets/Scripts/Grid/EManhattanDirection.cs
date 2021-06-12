using HexCS.Core;

using System.Collections.Generic;

namespace GMTK2021 { 
    public enum EManhattanDirection
    {
        NONE,
        UP, 
        RIGHT, 
        DOWN,
        LEFT
    }

    public static class UTEManhattanDirections
    {
        public static DiscreteVector2 AsDiscreteVector2(EManhattanDirection dir)
        {
            switch (dir)
            {
                case EManhattanDirection.UP: return DiscreteVector2.Up;
                case EManhattanDirection.RIGHT: return DiscreteVector2.Right;
                case EManhattanDirection.DOWN: return DiscreteVector2.Down;
                case EManhattanDirection.LEFT: return DiscreteVector2.Left;
            }
            return default;
        }

        public static DiscreteVector2 AsDiscreteVector2(IEnumerable<EManhattanDirection> dirs)
        {
            DiscreteVector2 vec = default;

            foreach(EManhattanDirection dir in dirs)
            {
                vec += AsDiscreteVector2(dir);
            }

            return vec;
        }
    }
}