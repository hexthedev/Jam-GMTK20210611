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

        public DiscreteVector2 myPosition;
        public Grid<Tile> myGrid;




        /*
         * 1) Copy to protect serialized data
         * 2) Initialize to provide inital position
         * 3) A tick is provided to represent a turn has been performed
         * 4) A back tick is provided to indicate that an undo ha been performed
         * 
         * 5) INPUT: Provide a report about who you are in input scheme
         * 
         * 6) MOVEMENT: Check Can be moved by input
         * 7)         : Provide a report about movement capabilities
         * 8)         : OnPush check push reaction
         * 
         * 9) ANIMATION : 
         */

        public abstract SoObject Copy();

        public virtual void Init(DiscreteVector2 coord, Grid<Tile> grid)
        {
            myPosition = coord;
            myGrid = grid;
        }

        public virtual void Tick(){}

        public virtual void BackTick(){}




        // try to supply input
        public virtual void ProvideReportAboutInputRole(string input, InputReport report) { }



        // Does this recieve movement
        public virtual bool CanBeMovedByInput() => false;

        // try to move
        public virtual void ProvideReportAboutMovementCapabilities(DiscreteVector2 direction, MovementReport report) { }

        // can I be pushed
        public virtual bool CanObjectBePushedAndUpdateReport(DiscreteVector2 direction, MovementReport report) { return false; }



        // Am I activting you
        public virtual bool AreYouActivatingMe(GridElement<Tile> me) => false;

        // Are any anim events required
        public virtual void FireReleventAnimationEvents() { }



















        public void TriggerAnimation(string anim) => OnAnimationTriggered?.Invoke(anim);









        /*
         * PROTECTED HELPER METHODS
         */

        protected bool IsPushable_IfPushedToEmpty(DiscreteVector2 direction, MovementReport report)
        {
            DiscreteVector2 target = myPosition + direction;
            if (!myGrid.IsInBounds(target)) return false;

            Tile t = myGrid.Get(target);

            if (t.Object == null) return true;
            return t.Object.CanObjectBePushedAndUpdateReport(direction, report);
        }

        protected void PopulateWithCopy(SoObject obj)
        {
            obj.IsSlideAnimated = IsSlideAnimated;
            obj.Offset = Offset;
            obj.Sprite = Sprite;
            obj.Controller = Controller;
            obj.SpriteSwapAnimNames = SpriteSwapAnimNames;
            obj.SpriteSwapSprites = SpriteSwapSprites;
        }




    }
}