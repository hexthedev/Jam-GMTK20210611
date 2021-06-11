using UnityEngine;

namespace GMTK2021
{
    [SelectionBase]
    public class TileRenderer : MonoBehaviour
    {
        public SoTileTheme Theme;
        public Tile ManagedTile;

        [SerializeField]
        private MeshRenderer _backgroundRenderer;

        [SerializeField]
        private MeshRenderer _foregroundRenderer;

        [SerializeField]
        private MeshRenderer _objectRenderer;


        public void RenderTile()
        {
            if(ManagedTile == null)
            {
                DisableAllRenderers();
                return;
            }

            RenderFloor(ManagedTile.Floor);
        }

        private void RenderFloor(SOFloorProperties props)
        {
            if(props == null)
            {
                _backgroundRenderer.enabled = false;
                _foregroundRenderer.enabled = false;
                return;
            }

            _backgroundRenderer.enabled = false;
            _foregroundRenderer.material = Theme.GetFloorMat(props.Type);
        }

        private void DisableAllRenderers()
        {
            _backgroundRenderer.enabled = false;
            _foregroundRenderer.enabled = false;
            _objectRenderer.enabled = false;
        }
    }
}