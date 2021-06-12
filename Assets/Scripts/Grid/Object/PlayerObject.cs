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

        public override bool IsPushable(SoObject pusher, DiscreteVector2 position, DiscreteVector2 direcation, Grid<Tile> grid)
            => false;

        public override bool ReceivesMovement(DiscreteVector2 position, Grid<Tile> grid) => true;
    }
}