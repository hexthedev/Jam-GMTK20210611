using HexCS.Core;

using System.Linq;

using UnityEngine;

namespace GMTK2021
{
    [CreateAssetMenu(fileName = nameof(GateControlObject), menuName = "GMTK/GateControlObject")]
    public class GateControlObject : SoObject
    {
        [Header("Input")]
        [SerializeField]
        private string _inputAction;

        [SerializeField]
        private EManhattanDirection[] _directionLoop;


        public override bool IsPlayer => true;

        public override string InputAction => _inputAction;

        public override EManhattanDirection InputDirection => EManhattanDirection.NONE;


        private int index = 0;





        public override SoObject Copy()
        {
            GateControlObject po = CreateInstance<GateControlObject>();
            PopulateWithCopy(po);
            po._inputAction = _inputAction;
            po._directionLoop = _directionLoop;
            return po;
        }

        public override void Tick()
        {
            index++;
            index = index % _directionLoop.Length;
        }

        public override void BackTick()
        {
            index--;
            if (index < 0) index = _directionLoop.Length - 1;
        }

        public override void ProvideReportAboutInputRole(string input, InputReport report)
        {
            if (_inputAction != input) return;

            DiscreteVector2 target = myPosition + UTEManhattanDirections.AsDiscreteVector2(_directionLoop[index]);

            if (!myGrid.IsInBounds(target))
            {
                return;
            }

            Tile t = myGrid.Get(target);

            if (t.Object == null || t.Object.InputDirection == EManhattanDirection.NONE)
            {
                return;
            }

            report.ActivatedDirections.Add(t.Object.InputDirection);
        }

        public override bool CanObjectBePushedAndUpdateReport(DiscreteVector2 direction, MovementReport report)
        {
            bool res = IsPushable_IfPushedToEmpty(direction, report);
            if (res) report.PushedMove.Add(myGrid.AsElement(myPosition));
            return res;
        }

        public override void FireReleventAnimationEvents()
        {
            DiscreteVector2 activeDir = UTEManhattanDirections.AsDiscreteVector2(_directionLoop[index]);
            DiscreteVector2 target = myPosition + activeDir;

            if (!myGrid.IsInBounds(target))
            {
                TriggerAnimation("OFF");
                return;
            }

            Tile t = myGrid.Get(myPosition + activeDir);

            if(t.Object == null || t.Object.InputDirection == EManhattanDirection.NONE)
            {
                TriggerAnimation("OFF");
                return;
            }

            t.Object.TriggerAnimation(InputAction);
            TriggerAnimation("ON");
        }


        public override bool AreYouActivatingMe(GridElement<Tile> me)
        {
            DiscreteVector2 activating = myPosition + UTEManhattanDirections.AsDiscreteVector2(_directionLoop[index]);
            bool isAct = me.Cooridnate == activating;
            return isAct;
        }
    }
}