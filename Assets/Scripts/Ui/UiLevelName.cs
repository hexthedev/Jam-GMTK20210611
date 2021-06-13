using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class UiLevelName : MonoBehaviour
{
    public TMP_Text txt;

    public void SetLevelName(string name)
    {
        txt.text = name;
    }
}
