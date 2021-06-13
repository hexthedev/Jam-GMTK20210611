using HexCS.Core;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public abstract class SoObject : ScriptableObject
    {
        [Header("Art")]
        [SerializeField]
        public Sprite Sprite;

        [SerializeField]
        public RuntimeAnimatorController Controller;

        public abstract bool IsPlayer { get; }

        public abstract string InputAction { get; }
         
        public abstract EManhattanDirection InputDirection { get; }

        /// <summary>
        /// Based on the grid, can this object recieve movement
        /// </summary>
        public abstract bool ReceivesMovement(GridElement<Tile> element);





        public abstract void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element);

        public abstract void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element);

        /// <summary>
        /// True if pushed
        /// </summary>
        public abstract bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element);




        protected bool IsPushable_IfPushedToEmpty(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            DiscreteVector2 target = element.Cooridnate + direction;
            if (!element.Grid.IsInBounds(target)) return false;
            
            Tile t = element.Grid.Get(target);

            if (t.Object == null) return true;
            return t.Object.ResolvePushAttempt(direction, report, element.Grid.AsElement(target));
        }

    }
}