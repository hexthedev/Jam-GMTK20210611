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
        private EManhattanDirection _startDirection;

        bool _isStarted = false;

        private EManhattanDirection _currentDir;


        public override bool IsPlayer => true;

        public override string InputAction => _inputAction;

        public override EManhattanDirection InputDirection => EManhattanDirection.NONE;

        public override bool ReceivesMovement(GridElement<Tile> element) => true;



        public override void ResolveAnimEvents(GridElement<Tile> element)
        {
            DiscreteVector2 activeDir = UTEManhattanDirections.AsDiscreteVector2(_currentDir);
            DiscreteVector2 target = element.Cooridnate + activeDir;

            if (!element.Grid.IsInBounds(target))
            {
                TriggerAnimation("OFF");
                return;
            }

            Tile t = element.Grid.Get(element.Cooridnate + activeDir);

            if(t.Object == null || t.Object.InputDirection == EManhattanDirection.NONE)
            {
                TriggerAnimation("OFF");
                return;
            }

            t.Object.TriggerAnimation(InputAction);
            TriggerAnimation("ON");
        }

        public override void ResolveInputRecieved(string input, InputReport report, GridElement<Tile> element)
        {
            if (!_isStarted)
            {
                _currentDir = _startDirection;
                _isStarted = true;
            }

            switch (_currentDir)
            {
                case EManhattanDirection.DOWN: _currentDir = EManhattanDirection.LEFT; break;
                case EManhattanDirection.LEFT: _currentDir = EManhattanDirection.UP; break;
                case EManhattanDirection.UP: _currentDir = EManhattanDirection.RIGHT; break;
                case EManhattanDirection.RIGHT: _currentDir = EManhattanDirection.DOWN; break;
            }

            if (_inputAction != input) return;
            
            DiscreteVector2 activeDir = new DiscreteVector2
            (
                _currentDir == EManhattanDirection.DOWN || _currentDir == EManhattanDirection.UP ? 0 : _currentDir == EManhattanDirection.LEFT ? -1 : 1,
                _currentDir == EManhattanDirection.LEFT || _currentDir == EManhattanDirection.RIGHT ? 0 : _currentDir == EManhattanDirection.DOWN ? -1 : 1
            );

            DiscreteVector2 target = element.Cooridnate + activeDir;

            if (!element.Grid.IsInBounds(target))
            {
                return;
            }

            Tile t = element.Grid.Get(element.Cooridnate + activeDir);

            if (t.Object == null || t.Object.InputDirection == EManhattanDirection.NONE)
            {
                return;
            }

            report.ActivatedDirections.Add(t.Object.InputDirection);
        }
        

        public override void ResolveMovementRecieved(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {


            return;
        }

        public override bool ResolvePushAttempt(DiscreteVector2 direction, MovementReport report, GridElement<Tile> element)
        {
            bool res = IsPushable_IfPushedToEmpty(direction, report, element);
            if (res) report.PushedMove.Add(element);
            return res;
        }

        public override bool ResolveIsActivating(GridElement<Tile> element)
        {
            DiscreteVector2 idx = element.Cooridnate - UTEManhattanDirections.AsDiscreteVector2(_currentDir);

            if (!element.Grid.IsInBounds(idx)) return false;
            Tile t = element.Grid.Get(idx);
            return t.Object == this;
        }
    }
}