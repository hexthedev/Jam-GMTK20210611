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

            _backgroundRenderer.material = Theme.Floor_Background;

            RenderFloor(ManagedTile.Floor);
            RenderObject(ManagedTile.Object);
        }

        private void RenderFloor(SOFloorProperties props)
        {
            if(props == null)
            {
                _foregroundRenderer.enabled = false;
                return;
            }

            _foregroundRenderer.material = Theme.GetFloorMat(props.Type);
        }

        private void RenderObject(SoObject props)
        {
            if(props == null)
            {
                _objectRenderer.enabled = false;
                return;
            }

            _objectRenderer.material = Theme.GetForObject(props);
            _objectRenderer.enabled = true;
        }

        private void DisableAllRenderers()
        {
            _backgroundRenderer.enabled = false;
            _foregroundRenderer.enabled = false;
            _objectRenderer.enabled = false;
        }
    }
}