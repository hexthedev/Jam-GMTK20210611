using UnityEngine;

namespace GMTK2021
{
    public class AnimTriggerRelay : MonoBehaviour
    {
        [SerializeField]
        ObjectRenderer _rend;

        public void AnimEnd()
        {
            _rend.ResetPos();
        }
    }
}