using UnityEngine;

namespace GMTK2021
{
    [SelectionBase]
    public class TileRenderer : MonoBehaviour
    {
        public SoTileTheme Theme;
        public Tile ManagedTile;

        [SerializeField]
        private SpriteRenderer _tileRenderer;

        [SerializeField]
        private Animator _animator;

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
                _tileRenderer.enabled = false;
                return;
            }

            _tileRenderer.sprite = props.Sprite;
            _animator.runtimeAnimatorController = props.AnimatorController;
        }

        private void DisableAllRenderers()
        {
            _tileRenderer.enabled = false;
        }
    }
}