using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UiInputGate : MonoBehaviour
{
    public Image GateViz;

    public bool SetInputEnabled
    {
        set
        {
            GateViz.color = value ? Color.green : Color.red;
        }
    }
}
