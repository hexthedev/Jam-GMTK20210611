using HexCS.Core;

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

        public override bool IsPushable(SoObject pusher, DiscreteVector2 position, DiscreteVector2 direction, Grid<Tile> grid)
            => IsPushable_IfPushedToEmpty(position, direction, grid);

        public override bool ReceivesMovement(DiscreteVector2 position, Grid<Tile> grid) => false;
    }
}