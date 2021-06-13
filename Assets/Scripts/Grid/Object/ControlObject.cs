using HexCS.Core;

using System.Linq;

using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(ControlObject), menuName = "GMTK/ControlObject")]
    public class ControlObject : SoObject
    {
        [Header("Input")]
        [SerializeField]
        private string _inputAction;

        public override bool IsPlayer => true;

        public override string InputAction => _inputAction;

        public override EManhattanDirection InputDirection => EManhattanDirection.NONE;





        public override SoObject Copy()
        {
            ControlObject po = CreateInstance<ControlObject>();
            po._inputAction = _inputAction;
            PopulateWithCopy(po);
            return po;
        }

        public override void ProvideReportAboutInputRole(string input, InputReport report)
        {
            if (_inputAction != input) return;

            myGrid.GetManhattanNeighbours(myPosition)
                .Where(n => n.Object != null && n.Object.InputDirection != EManhattanDirection.NONE)
                .Do(d => report.ActivatedDirections.Add(d.Object.InputDirection));
        }


        public override void FireReleventAnimationEvents()
        {
            bool isOneActive = false;

            myGrid.GetManhattanNeighbours(myPosition)
                .Where(n => n.Object != null && n.Object.InputDirection != EManhattanDirection.NONE)
                .Do(d => { d.Object.TriggerAnimation(_inputAction); isOneActive = true; });

            TriggerAnimation( isOneActive ? "ON" : "OFF" );
        }



        public override bool AreYouActivatingMe(GridElement<Tile> element) => true;



        public override bool CanObjectBePushedAndUpdateReport(DiscreteVector2 direction, MovementReport report)
        {
            bool res = IsPushable_IfPushedToEmpty(direction, report);
            if (res) report.PushedMove.Add(myGrid.AsElement(myPosition));
            return res;
        }
    }
}