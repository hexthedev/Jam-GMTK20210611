using HexCS.Core;

using HexUN.Engine.Utilities;

using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using static UnityEngine.InputSystem.InputAction;

namespace GMTK2021
{
    [System.Serializable]
    public class GameStateEvent : UnityEvent<GamestateReport> { }

    [System.Serializable]
    public class InputStateEvent : UnityEvent<bool> { }

    [ExecuteAlways]
    public class GameGridRenderer : MonoBehaviour
    {
        public bool autoInitalize = false;

        public SoTileTheme Theme;
        public SoGameGrid SerializedGrid;

        public GameStateEvent _onGameState;
        public InputStateEvent _onInputState;

        // Note: asusmes 1x1 prefabs
        private DiscreteVector2 _size;

        private TileGrid _dataGrid;
        private Grid<TileRenderer> _renderGrid;
        private Dictionary<DiscreteVector2, ObjectRenderer> _objectRends = new Dictionary<DiscreteVector2, ObjectRenderer>();

        [SerializeField]
        TileRenderer _tilePrefab;

        [SerializeField]
        ObjectRenderer _objectPrefab;

        [SerializeField]
        private Transform parent;

        public float slideSpeed = 0.5f;

        private bool _inputState = false;
        public bool InputState
        {
            get => InputState;
            set 
            {
                if(_inputState != value)
                {
                    _inputState = value;
                    _onInputState.Invoke(_inputState);
                }
            }
        }



        #region Init, Save, Load

        private void OnEnable()
        {
            if(!Application.isPlaying || autoInitalize)
            {
                InitGrid(SerializedGrid);
            }
            else
            {
                Clear();
            }
#if UNITY_EDITOR
            EditorSceneManager.sceneSaved += EditorInitalizeGrid;
            EditorSceneManager.sceneSaved += EditorSaveGrid;
#endif
        }

        private void OnDisable()
        {
            SaveGrid(SerializedGrid);
#if UNITY_EDITOR
            EditorSceneManager.sceneSaved -= EditorSaveGrid;
            EditorSceneManager.sceneSaved -= EditorInitalizeGrid;
#endif
        }


#if UNITY_EDITOR
        [ContextMenu("ReloadSerializedGrid")]
        private void EditorInitalizeGrid(Scene s) => InitGrid(SerializedGrid);

        [ContextMenu("SaveGrid")]
        private void EditorSaveGrid(Scene s) => SaveGrid(SerializedGrid);
#endif
        public void SaveGrid(SoGameGrid serializedGrid)
        {
            EditorUtility.SetDirty(serializedGrid);
        }

        public void InitGrid(SoGameGrid serializedGrid)
        {
            if (this == null) return;
            UTGameObject.DestroyAllChildren_EditorSafe(parent.gameObject);

            _size = new DiscreteVector2(serializedGrid.Width, serializedGrid.Height);

            _dataGrid = TileGrid.ConstructFrom(_size, Application.isPlaying ? serializedGrid.CopyTiles() : serializedGrid.TileGrid);

            _renderGrid = new Grid<TileRenderer>(_size, SpawnRenderer);

            _objectRends.Clear();
            _renderGrid.ElementwiseAction(
                (TileRenderer tr, GridElement<TileRenderer> i) =>
                {
                    ObjectRenderer obr = tr.MakeObjectRenderer(_objectPrefab);

                    if (obr != null)
                    {
                        _objectRends.Add(i.Cooridnate, obr);
                        obr.SetMode(true);
                    }
                }
            );

            foreach (KeyValuePair<DiscreteVector2, ObjectRenderer> or in _objectRends)
            {
                or.Value.transform.SetParent(parent, true);
                or.Value.transform.localPosition = new UnityEngine.Vector3(or.Key.X, or.Key.Y, or.Value.transform.localPosition.z);
            }

            parent.transform.localPosition = new Vector3(-_dataGrid.Size.X / 2 + 0.5f, -_dataGrid.Size.Y / 2 + 0.5f, 0);
            RenderTick();
        }

        public void Clear()
        {
            UTGameObject.DestroyAllChildren(parent.gameObject);

            _dataGrid = null;
            _renderGrid = null;
            _objectRends.Clear();
        }

        private TileRenderer SpawnRenderer(DiscreteVector2 coord)
        {
            TileRenderer renderer = Instantiate(_tilePrefab, parent);
            renderer.ManagedTile = _dataGrid.Get(coord);
            renderer.Theme = Theme;
            renderer.transform.localPosition = new Vector3(coord.X, coord.Y);
            return renderer;
        }
        #endregion



        #region Rendering and Animations
        private IEnumerator PerformAnimations(TileGrid.ObjectTransaction[] trans)
        {
            List<ObjectRenderer> rends = new List<ObjectRenderer>();
            List<DiscreteVector2> targ = new List<DiscreteVector2>();

            foreach (TileGrid.ObjectTransaction obt in trans)
            {
                ObjectRenderer rend = _objectRends[obt.LastIndex];

                DiscreteVector2 direction = obt.NextIndex - obt.LastIndex;
                rend.PrepareSlide(new UnityEngine.Vector2(direction.X, direction.Y));
                rends.Add(rend);
                _objectRends.Remove(obt.LastIndex);
                targ.Add(obt.NextIndex);
            }

            float time = 0;

            while (time < slideSpeed)
            {
                foreach (ObjectRenderer rend in rends) rend.Slide(time / slideSpeed);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            foreach (ObjectRenderer rend in rends)
            {
                rend.Slide(1);
                rend.StopSlide();
            }

            for (int i = 0; i < rends.Count; i++)
            {
                _objectRends.Add(targ[i], rends[i]);
            }

            PerformStatusReport();

            InputState = true;
        }

        private void RenderTick()
        {
            _renderGrid.ElementwiseAction(DoRender);
            void DoRender(TileRenderer rend) => rend.RenderTile();
        }
        #endregion









        public void ReceiveInput(CallbackContext Context)
        {
            if (_inputState == false) return;

            if (Context.performed)
            {
                if (_dataGrid.ResolveInput(Context.action.name, out TileGrid.ObjectTransaction[] trans))
                {
                    InputState = false;

                    RenderTick();
                    StartCoroutine(PerformAnimations(trans));
                }
            }
        }

        private void PerformStatusReport()
        {
            GamestateReport report = new GamestateReport();
            _dataGrid.ReportGameState(report);
            _onGameState.Invoke(report);
        }









    }
}