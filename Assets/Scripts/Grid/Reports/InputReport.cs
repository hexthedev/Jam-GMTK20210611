using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    /// <summary>
    /// OnInput, all grid tiles can provide info to the input report
    /// </summary>
    public class InputReport
    {
        public List<GridElement<Tile>> CanReceiveInput = new List<GridElement<Tile>>();
        public List<EManhattanDirection> ActivatedDirections = new List<EManhattanDirection>();

        public bool HasValidInput => CanReceiveInput.Count > 0 && ActivatedDirections.Count > 0;
    }
}