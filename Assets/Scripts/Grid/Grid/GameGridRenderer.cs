using HexCS.Core;

using HexUN.Engine.Utilities;

using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

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

#if UNITY_EDITOR
        [ContextMenu("ReloadSerializedGrid")]
        private void EditorInitGrid()
        {
            UTGameObject.DestroyAllChildren_EditorSafe(gameObject);

            _size = new DiscreteVector2(SerializedGrid.Width, SerializedGrid.Height);
            _dataGrid = TileGrid.ConstructFrom(_size, SerializedGrid.TileGrid);

            _renderGrid = new Grid<TileRenderer>(_size, SpawnRenderer);
            RenderTick();

            TileRenderer SpawnRenderer(DiscreteVector2 coord)
            {
                TileRenderer renderer = Instantiate(_tilePrefab, transform);
                renderer.ManagedTile = _dataGrid.Get(coord);
                renderer.Theme = Theme;
                renderer.transform.localPosition = new Vector3(coord.X, coord.Y);
                return renderer;
            }
        }
#endif


        private void Start()
        {
            UTGameObject.DestroyAllChildren_EditorSafe(gameObject);
            InitGrid();
        }

        private void InitGrid()
        {
            //Size = new DiscreteVector2(_columns, _rows);

            //_dataGrid = new TileGrid(Size);
            //_renderGrid = new Grid<TileRenderer>(Size,SpawnRenderer);

            //TileRenderer SpawnRenderer(DiscreteVector2 coord)
            //{
            //    TileRenderer renderer = Instantiate(_tilePrefab, transform);
            //    renderer.ManagedTile = _dataGrid.Get(coord);
            //    renderer.transform.localPosition = new Vector3(coord.X, coord.Y);
            //    return renderer;
            //}
        }

        private void RenderTick()
        {
            _renderGrid.PerformOnTiles(DoRender);
            void DoRender(TileRenderer rend) => rend.RenderTile();
        }

    }
}