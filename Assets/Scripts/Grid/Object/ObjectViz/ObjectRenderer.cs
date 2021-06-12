using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class ObjectRenderer : MonoBehaviour
    {
        public bool smallQuadMode = true;

        [SerializeField]
        private MeshRenderer rend;

        [SerializeField]
        private SmallQuad smallQuad;

        [SerializeField]
        private MeshRenderer smallQuadRenderer;

        public Material Material
        {
            get => rend.material;
            set => rend.material = value;
        }

        public void SetMode(bool smallQuad)
        {
            rend.enabled = !smallQuad;
            this.smallQuad.enabled = smallQuad;
        }

        public void SetArt(Material mat)
        {
            if (smallQuadMode)
            {
                smallQuadRenderer.material = mat;
            }
        }

        public void PrepareSlide(Vector2 direction)
        {
            if (smallQuadMode) smallQuad.PrepareSlide(direction);
        }

        public void Slide(float val)
        {
            if (smallQuadMode) smallQuad.Slide(val);
        }

        public void StopSlide()
        {
            if (smallQuadMode)
            {
                transform.localPosition = transform.localPosition + smallQuad.transform.localPosition;
                smallQuad.StopSlide();
            }
        }
    }
}