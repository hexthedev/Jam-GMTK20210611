using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class ObjectRenderer : MonoBehaviour
    {
        private SoObject _object;
        public SoObject Object
        {
            get => _object;
            set
            {
                if(_object != value)
                {
                    _object = value;
                    _renderer.sprite = _object.Sprite;
                    _animator.runtimeAnimatorController = _object.Controller;
                }
            }
        }

        [Header("Small")]

        [SerializeField]
        private SpriteRenderer _renderer;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private SmallQuad _quad;

        public void PrepareSlide(Vector2 direction)
        {
            _quad.PrepareSlide(direction);
        }

        public void Slide(float val)
        {
            _quad.Slide(val);
        }

        public void StopSlide()
        {
            transform.localPosition = transform.localPosition + _quad.transform.localPosition;
            _quad.StopSlide();
        }
    }
}