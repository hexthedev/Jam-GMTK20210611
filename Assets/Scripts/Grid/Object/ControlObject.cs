using HexCS.Core;

using System.Linq;

using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(ControlObject), menuName = "GMTK/ControlObject")]
    public class ControlObject : SoObject
    {
        [SerializeField]
        private string _inputAction;

        public override bool IsPlayer => true;

        public override string InputAction => _inputAction;

        public override EManhattanDirection InputDirection => EManhattanDirection.NONE;

        public override bool IsPushable(SoObject pusher, GridElement<Tile> element, DiscreteVector2 direction)
            => IsPushable_IfPushedToEmpty(element, direction);

        public override bool ReceivesMovement(GridElement<Tile> element) => false;







        public override void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element)
        {
            if (_inputAction != input) return;

            element.Grid.GetManhattanNeighbours(element.Cooridnate)
                .Where(n => n.Object != null && n.Object.InputDirection != EManhattanDirection.NONE)
                .Do(d => report.ActivatedDirections.Add(d.Object.InputDirection));
        }

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