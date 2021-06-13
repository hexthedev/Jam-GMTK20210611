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






        public override SoObject Copy()
        {
            DirectionObject po = CreateInstance<DirectionObject>();
            PopulateWithCopy(po);
            po.direction = direction;
            return po;
        }

        public override bool CanObjectBePushedAndUpdateReport(DiscreteVector2 direction, MovementReport report)
        {
            bool res = IsPushable_IfPushedToEmpty(direction, report);
            if (res) report.PushedMove.Add(myGrid.AsElement(myPosition));
            return res;
        }






        public override void FireReleventAnimationEvents()
        {
            bool isOneActive = false;

            myGrid.GetManhattanNeighbours(myPosition)
                .Where(c => c.Object != null && !string.IsNullOrEmpty(c.Object.InputAction))
                .Do(DoThing);


            TriggerAnimation(isOneActive ? "ON" : "OFF");
            
            void DoThing(Tile t)
            {
                if(t.Object.AreYouActivatingMe(myGrid.AsElement(myPosition))) isOneActive = true;
            }
        }

        public override bool AreYouActivatingMe(GridElement<Tile> element) => false;


    }
}