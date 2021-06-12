using HexCS.Core;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public abstract class SoObject : ScriptableObject
    {
        public abstract bool IsPlayer { get; }

        public abstract string InputAction { get; }
         
        public abstract EManhattanDirection InputDirection { get; }

        /// <summary>
        /// Based on the grid, can this object recieve movement
        /// </summary>
        public abstract bool ReceivesMovement(DiscreteVector2 position, Grid<Tile> grid);

        /// <summary>
        /// Can the pusher object push this
        /// </summary>
        public abstract bool IsPushable(SoObject pusher, DiscreteVector2 position, DiscreteVector2 direcation, Grid<Tile> grid);





        protected bool IsPushable_IfPushedToEmpty(DiscreteVector2 position, DiscreteVector2 direction, Grid<Tile> grid)
        {
            DiscreteVector2 target = position + direction;
            if (!grid.IsInBounds(target)) return false;
            
            Tile t = grid.Get(target);
            return t.Object == null;
        }

    }
}