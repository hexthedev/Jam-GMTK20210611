using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class UiLevelCanvas : MonoBehaviour
    {
        public UiVictory UiVictory;

        public void HandleGameState(GamestateReport rep)
        {
            UiVictory.IsOver = rep.isPlayerDead || rep.isPlayerOnGoal;
            UiVictory.IsWon = rep.isPlayerOnGoal;
        }
    }
}