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


        public void RenderTile()
        {
            if(ManagedTile == null)
            {
                DisableAllRenderers();
                return;
            }

            _backgroundRenderer.material = Theme.Background;

            RenderFloor(ManagedTile.Floor);
        }

        private void RenderFloor(SOFloorProperties props)
        {
            if(props == null)
            {
                _foregroundRenderer.enabled = false;
                return;
            }

            _foregroundRenderer.enabled = true;
            _foregroundRenderer.material = Theme.GetFloorMat(props);
        }

        public ObjectRenderer MakeObjectRenderer(ObjectRenderer prefab)
        {
            if(ManagedTile.Object != null)
            {
                ObjectRenderer rend = Instantiate(prefab);
                rend.Material = Theme.GetForObject(ManagedTile.Object);
                return rend;
            }

            return null;
        }

        private void DisableAllRenderers()
        {
            _backgroundRenderer.enabled = false;
            _foregroundRenderer.enabled = false;
        }
    }
}