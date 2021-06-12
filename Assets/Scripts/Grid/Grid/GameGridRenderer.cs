using HexCS.Core;

using HexUN.Engine.Utilities;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static UnityEngine.InputSystem.InputAction;

namespace GMTK2021
{
    public class GameGridRenderer : MonoBehaviour
    {
        public SoTileTheme Theme;
        public SoGameGrid SerializedGrid;

        // Note: asusmes 1x1 prefabs
        private DiscreteVector2 _size;

        private TileGrid _dataGrid;
        private Grid<TileRenderer> _renderGrid;

        [SerializeField]
        TileRenderer _tilePrefab;


        private void Start()
        {
            InitGrid();
        }

        public void ReceiveInput(CallbackContext Context)
        {
            if (Context.performed)
            {
                if(_dataGrid.ResolveInput(Context.action.name)) RenderTick();
            }
        }


        [ContextMenu("ReloadSerializedGrid")]
        private void InitGrid()
        {
            UTGameObject.DestroyAllChildren_EditorSafe(gameObject);

            _size = new DiscreteVector2(SerializedGrid.Width, SerializedGrid.Height);

            _dataGrid = TileGrid.ConstructFrom(_size, Application.isPlaying ? SerializedGrid.CopyTiles() : SerializedGrid.TileGrid);

            _renderGrid = new Grid<TileRenderer>(_size, SpawnRenderer);
            RenderTick();
        }

        private void RenderTick()
        {
            _renderGrid.ElementwiseAction(DoRender);
            void DoRender(TileRenderer rend) => rend.RenderTile();
        }


        private TileRenderer SpawnRenderer(DiscreteVector2 coord)
        {
            TileRenderer renderer = Instantiate(_tilePrefab, transform);
            renderer.ManagedTile = _dataGrid.Get(coord);
            renderer.Theme = Theme;
            renderer.transform.localPosition = new Vector3(coord.X, coord.Y);
            return renderer;
        }
    }
}