using System.Collections;
using System.Collections.Generic;
using System.Text;

using TMPro;

using UnityEngine;

namespace GMTK2021 {
    public class UiLastInput : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        public void SetLastInput(EManhattanDirection[] dirs, bool failed)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach(EManhattanDirection dir in dirs)
            {
                sb.Append(dir.ToString());
                sb.Append(" ");
            }

            if (failed) sb.Append("FAILED");

            _text.text = sb.ToString();
        }
    }
}