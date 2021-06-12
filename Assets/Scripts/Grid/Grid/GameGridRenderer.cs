using HexCS.Core;

using HexUN.Engine.Utilities;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.InputSystem.InputAction;

namespace GMTK2021
{
    [System.Serializable]
    public class GameStateEvent : UnityEvent<GamestateReport> { }

    [ExecuteAlways]
    public class GameGridRenderer : MonoBehaviour
    {
        public SoTileTheme Theme;
        public SoGameGrid SerializedGrid;

        public GameStateEvent _onGameState;

        // Note: asusmes 1x1 prefabs
        private DiscreteVector2 _size;

        private TileGrid _dataGrid;
        private Grid<TileRenderer> _renderGrid;

        [SerializeField]
        TileRenderer _tilePrefab;


        private void OnEnable()
        {
            InitGrid();
#if UNITY_EDITOR
            EditorSceneManager.sceneSaved += t => InitGrid();
            EditorSceneManager.sceneSaved += t => SaveGrid();
#endif
        }

        private void OnDisable()
        {
            SaveGrid();
#if UNITY_EDITOR
            EditorSceneManager.sceneSaved -= t => SaveGrid();
            EditorSceneManager.sceneSaved -= t => InitGrid();
#endif
        }

        public void ReceiveInput(CallbackContext Context)
        {
            if (Context.performed)
            {
                if (_dataGrid.ResolveInput(Context.action.name))
                {
                    GamestateReport report = new GamestateReport();
                    _dataGrid.ReportGameState(report);
                    _onGameState.Invoke(report);

                    RenderTick();
                }
            }
        }


        [ContextMenu("ReloadSerializedGrid")]
        private void InitGrid()
        {
            if (this == null) return;
            UTGameObject.DestroyAllChildren_EditorSafe(gameObject);

            _size = new DiscreteVector2(SerializedGrid.Width, SerializedGrid.Height);

            _dataGrid = TileGrid.ConstructFrom(_size, Application.isPlaying ? SerializedGrid.CopyTiles() : SerializedGrid.TileGrid);

            _renderGrid = new Grid<TileRenderer>(_size, SpawnRenderer);
            RenderTick();
        }

        [ContextMenu("SaveGrid")]
        private void SaveGrid()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(SerializedGrid);
#endif
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