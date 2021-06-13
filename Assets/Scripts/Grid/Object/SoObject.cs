using HexCS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public abstract class SoObject : ScriptableObject
    {
        [Header("Art")]
        [SerializeField]
        public bool IsSlideAnimated = true;

        [SerializeField]
        public Vector3 Offset;

        [SerializeField]
        public Sprite Sprite;

        [SerializeField]
        public RuntimeAnimatorController Controller;

        [SerializeField]
        public List<string> SpriteSwapAnimNames;

        [SerializeField]
        public List<Sprite> SpriteSwapSprites;


        public event Action<string> OnAnimationTriggered;


        public abstract bool IsPlayer { get; }

        public abstract string InputAction { get; }
         
        public abstract EManhattanDirection InputDirection { get; }

  

        // Are any anim events required
        public abstract void ResolveAnimEvents(GridElement<Tile> element);

        // try to supply input
        public abstract void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element);
        
        // Does this recieve movement
        public abstract bool ReceivesMovement(GridElement<Tile> element);

        // try to move
        public abstract void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element);

        // can I be pushed
        public abstract bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element);

        // Am I activting you
        public abstract bool ResolveIsActivating(GridElement<Tile> element);


        protected bool IsPushable_IfPushedToEmpty(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            DiscreteVector2 target = element.Cooridnate + direction;
            if (!element.Grid.IsInBounds(target)) return false;
            
            Tile t = element.Grid.Get(target);

            if (t.Object == null) return true;
            return t.Object.ResolvePushAttempt(direction, report, element.Grid.AsElement(target));
        }


        public void TriggerAnimation(string anim) => OnAnimationTriggered?.Invoke(anim);
    }
}