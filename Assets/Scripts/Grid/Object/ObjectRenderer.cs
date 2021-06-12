using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2021
{
    public class ObjectRenderer : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer rend;
        
        public Material Material
        {
            get => rend.material;
            set => rend.material = value;
        }

        public Vector2 Origin;
        public Vector2 Target;
        public bool IsSliding;

        public void Slide(float val)
        {
            if (IsSliding)
            {
                Vector2 lerp = Vector2.Lerp(Origin, Target, val);
                SetOrigin(lerp);
            }
        }

        public void SetOrigin(Vector2 coord)
        {
            transform.localPosition = new Vector3(coord.x, coord.y, transform.localPosition.z);
        }




    }
}