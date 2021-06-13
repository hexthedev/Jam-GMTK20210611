using HexCS.Core;

using System.Linq;

using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(DirectionObject), menuName = "GMTK/DirectionObject")]
    public class DirectionObject : SoObject
    {
        [SerializeField]
        public EManhattanDirection direction;

        public override bool IsPlayer => true;

        public override string InputAction => null;

        public override EManhattanDirection InputDirection => direction;

        public override bool ReceivesMovement(GridElement<Tile> element) => false;

        public override void ResolveAnimEvents(GridElement<Tile> element)
        {
            bool isOneActive = false;

            element.Grid.GetManhattanNeighbours(element.Cooridnate)
                .Where(c => c.Object != null && !string.IsNullOrEmpty(c.Object.InputAction))
                .Do(DoThing);


            TriggerAnimation(isOneActive ? "ON" : "OFF");
            
            void DoThing(Tile t)
            {
                if(t.Object.ResolveIsActivating(element)) isOneActive = true;
            }
        }

        public override void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element) { return; }

        public override bool ResolveIsActivating(GridElement<Tile> element) => false;

        public override void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            return;
        }

        public override bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            bool res = IsPushable_IfPushedToEmpty(direction, report, element);
            if (res) report.PushedMove.Add(element);
            return res;
        }
    }
}