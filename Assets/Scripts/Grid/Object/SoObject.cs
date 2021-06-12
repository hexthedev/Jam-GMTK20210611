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
        public abstract bool ReceivesMovement(GridElement<Tile> element);

        /// <summary>
        /// Can the pusher object push this
        /// </summary>
        public abstract bool IsPushable(SoObject pusher, GridElement<Tile> element, DiscreteVector2 direction);





        public abstract void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element);

        public abstract void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element);

        /// <summary>
        /// True if pushed
        /// </summary>
        public abstract bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element);




        protected bool IsPushable_IfPushedToEmpty(GridElement<Tile> element, DiscreteVector2 direction)
        {
            DiscreteVector2 target = element.Cooridnate + direction;
            if (!element.Grid.IsInBounds(target)) return false;
            
            Tile t = element.Grid.Get(target);
            return t.Object == null;
        }

    }
}