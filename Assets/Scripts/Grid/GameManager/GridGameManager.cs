using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class GridGameManager : MonoBehaviour
    {
        public SoGameGrid[] Levels;

        public int levelIndex = 0;

        public GameGridRenderer Renderer;

        public Animator Animator;

        public void Start()
        {
            RenderNext();
        }

        public void RenderNext()
        {
            Renderer.Clear();
            Renderer.InitGrid(Levels[levelIndex]);
        }

        public void HandleGameStatus(GamestateReport rep)
        {
            if (rep.isPlayerOnGoal)
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
            Animator.SetTrigger("IN");
        }
    }
}