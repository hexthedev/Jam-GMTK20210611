using UnityEngine;

namespace GMTK2021
{
    public class SmallQuad : MonoBehaviour
    {
        public Vector3 Target;

        public void Slide(float val)
        {
            Vector3 pos = Vector3.Lerp(Vector3.zero, Target, val);
            transform.localPosition = pos;
        }

        public void PrepareSlide(Vector2 offset, Vector2 direction)
        {
            Target = direction;
        }

        public void StopSlide()
        {
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        }
    }
}