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

        public override bool IsPushable(SoObject pusher, DiscreteVector2 position, DiscreteVector2 direction, Grid<Tile> grid)
            => IsPushable_IfPushedToEmpty(position, direction, grid);

        public override bool ReceivesMovement(DiscreteVector2 position, Grid<Tile> grid) => false;
    }
}