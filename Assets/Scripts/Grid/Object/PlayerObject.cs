using HexCS.Core;

using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(PlayerObject), menuName = "GMTK/PlayerObject")]
    public class PlayerObject : SoObject
    {
        public override bool IsPlayer => true;

        public override string InputAction => null;

        public override EManhattanDirection InputDirection => EManhattanDirection.NONE;

        public override bool ReceivesMovement(GridElement<Tile> element) => true;



        public override void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element)
            => report.CanReceiveInput.Add(element);

        public override void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            DiscreteVector2 targetIndex = element.Cooridnate + direction;
            if (!element.Grid.IsInBounds(targetIndex))
            {
                report.BlockedMove.Add(element);
                return;
            }

            Tile target = element.Grid.Get(targetIndex);

            if(target.Object == null)
            {
                report.FreeMove.Add(element);
                return;
            }

            if (target.Object.ResolvePushAttempt(direction, report, element.Grid.AsElement(targetIndex)))
            {
                report.PusherMove.Add(element);
            }
            else
            {
                report.BlockedMove.Add(element);
            }
        }

        public override bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            return false;
        }
    }
}