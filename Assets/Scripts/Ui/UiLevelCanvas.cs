using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class UiLevelCanvas : MonoBehaviour
    {
        public UiVictory UiVictory;
        public UiInputGate UiInputGate;

        public RectTransform MainMenu;

        public void HandleGameState(GamestateReport rep)
        {
            UiVictory.IsOver = rep.isPlayerDead || rep.isPlayerOnGoal;
            UiVictory.IsWon = rep.isPlayerOnGoal;
        }

        public void HandleInputState(bool state)
        {
            UiInputGate.SetInputEnabled = state;
        }

        


        public void SlideOut()
        {
            StartCoroutine(SlideBetween(0, -1920));
        }

        public void SlideIn()
        {
            StartCoroutine(SlideBetween(-1920, 0));
        }


        IEnumerator SlideBetween(int origin, int dest)
        {
            float timer = 0;

            while(timer < 1)
            {
                timer += Time.deltaTime;

                MainMenu.localPosition = Vector3.Lerp(
                    new Vector3(origin, MainMenu.localPosition.y, MainMenu.localPosition.z),
                    new Vector3(dest, MainMenu.localPosition.y, MainMenu.localPosition.z),
                    timer
                );

                yield return new WaitForEndOfFrame();
            }
        }

    }
}