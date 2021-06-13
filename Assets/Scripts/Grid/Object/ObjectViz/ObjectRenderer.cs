using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class ObjectRenderer : MonoBehaviour
    {
        private Vector3 _lastSlide;

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

                    Object.OnAnimationTriggered += Object_OnAnimationTriggered;

                }
            }
        }

        private void OnDestroy()
        {
            if(Object != null)
            {
                Object.OnAnimationTriggered -= Object_OnAnimationTriggered;
            }
        }

        private void Object_OnAnimationTriggered(string obj)
        {
            int index = _object.SpriteSwapAnimNames.IndexOf(obj);

            if(index == -1)
            {
                if( _animator != null) _animator.SetTrigger(obj);
            }
            else
            {
                if(_renderer != null)
                {
                    _renderer.sprite = _object.SpriteSwapSprites[index];
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
            _lastSlide = new Vector3(direction.x, direction.y, 0);
            if (!_object.IsSlideAnimated) return;
            _quad.PrepareSlide(_object.Offset, direction);
        }

        public void Slide(float val)
        {
            if (!_object.IsSlideAnimated) return;
            _quad.Slide(val);
        }

        public void StopSlide()
        {
            if (!_object.IsSlideAnimated) return;
            ResetPos();
            _quad.StopSlide();
        }

        public void ResetPos()
        {
            _animator.Play("Idle");
            transform.localPosition = transform.localPosition + _lastSlide;
            _lastSlide = Vector3.zero;
        }
    }
}