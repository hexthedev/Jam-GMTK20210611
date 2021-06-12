using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class UiVictory : MonoBehaviour
{
    public TMP_Text _text;

    private bool _isOver = false;
    public bool IsOver
    {
        get => _isOver;
        set
        {
            if (_isOver != value)
            {
                _isOver = value;
                Render();
            }
        }
    }

    private bool _isWon = false;
    public bool IsWon
    {
        get => _isWon;
        set
        {
            if(_isWon != value)
            {
                _isWon = value;
                Render();
            }
        }
    }

    public void Render()
    {
        if(!_isOver)
        {
            _text.text = string.Empty;
            return;
        }

        if (_isWon) _text.text = $"You have won";
        else _text.text = _text.text = $"You have lost";
    }
}
