using HexCS.Core;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.InputSystem.InputAction;

namespace GMTK2021
{
    public class GridGameManager : MonoBehaviour
    {
        public SoGameGrid[] Levels;

        public int levelIndex = 0;

        public GameGridRenderer Renderer;

        public Animator Animator;

        public UnityEvent _onGameWin;

        public Camera cam;

        public bool isGameWon = false;

        public Color[] bgColors;

        public void Start()
        {
            RenderNext();
        }

        public void RenderNext()
        {
            if(levelIndex == Levels.Length)
            {
                _onGameWin.Invoke();
                isGameWon = true;
            }
            else
            {
                Renderer.Clear();
                Renderer.InitGrid(Levels[levelIndex]);
            }
        }

        public void HandleGameStatus(GamestateReport rep)
        {
            if (rep.isPlayerOnGoal)
            {
                levelIndex++;
                Animator.SetTrigger("OUT");
            }
        }

        public void SkipToNext(CallbackContext c)
        {
            if (c.performed)
            {
                levelIndex++;
                Animator.SetTrigger("OUT");
            }
        }

        public void AnimIn()
        {
            Renderer.InputState = true;
        }

        public void AnimOut()
        {
            Renderer.InputState = false;
            RenderNext();

            if (!isGameWon)
            {
                cam.backgroundColor = bgColors.RandomElement();
                Animator.SetTrigger("IN");
            }
        }

        public void DoSlideIn()
        {
            StartCoroutine(SlideIn());
        }

        public IEnumerator SlideIn()
        {
            float timer = 0;

            while(timer < 1)
            {
                timer += Time.deltaTime;

                transform.localPosition = new Vector3(
                    15 - 15 * timer, 0, 0
                );

                yield return new WaitForEndOfFrame();
            }

            transform.localPosition = Vector3.zero;
        }
    }
}