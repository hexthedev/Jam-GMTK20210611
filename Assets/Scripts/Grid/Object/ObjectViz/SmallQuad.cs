using UnityEngine;

namespace GMTK2021
{
    public class SmallQuad : MonoBehaviour
    {
        public Vector2 Target;

        public void Slide(float val)
        {
            Vector2 pos = Vector2.Lerp(Vector2.zero, Target, val);
            transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);
        }

        public void PrepareSlide(Vector2 direction)
        {
            Target = Vector2.zero + direction;
        }

        public void StopSlide()
        {
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        }
    }
}