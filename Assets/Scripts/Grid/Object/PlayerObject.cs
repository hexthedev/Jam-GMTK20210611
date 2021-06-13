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



        public override SoObject Copy()
        {
            PlayerObject po = CreateInstance<PlayerObject>();
            PopulateWithCopy(po);
            return po;
        }

        public override void ProvideReportAboutInputRole(string input, InputReport report)
            => report.CanReceiveInput.Add( myGrid.AsElement(myPosition) );

        public override bool CanBeMovedByInput() => true;

        public override void ProvideReportAboutMovementCapabilities(DiscreteVector2 direction, MovementReport report)
        {
            DiscreteVector2 targetIndex = myPosition + direction;
            if (!myGrid.IsInBounds(targetIndex))
            {
                report.BlockedMove.Add(myGrid.AsElement(myPosition));
                return;
            }

            Tile target = myGrid.Get(targetIndex);

            if (target.Object == null)
            {
                report.FreeMove.Add(myGrid.AsElement(myPosition));
                return;
            }

            if (target.Object.CanObjectBePushedAndUpdateReport(direction, report))
            {
                report.PusherMove.Add(myGrid.AsElement(myPosition));
            }
            else
            {
                report.BlockedMove.Add(myGrid.AsElement(myPosition));
            }
        }
    }
}