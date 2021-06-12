using HexCS.Core;

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

        public override bool IsPushable(SoObject pusher, GridElement<Tile> element, DiscreteVector2 direction)
            => IsPushable_IfPushedToEmpty(element, direction);

        public override bool ReceivesMovement(GridElement<Tile> element) => false;

        public override void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element) { return; }

        public override void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            return;
        }

        public override bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            bool res = IsPushable_IfPushedToEmpty(element, direction);
            if (res) report.PushedMove.Add(element);
            return res;
        }
    }
}