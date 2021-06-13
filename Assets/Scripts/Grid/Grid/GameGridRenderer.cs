using HexCS.Core;

using HexUN.Engine.Utilities;
using HexUN.Events;

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
    [ExecuteAlways]
    public class GameGridRenderer : MonoBehaviour
    {
        private const float cSlideSpeed = 0.25f;

        [Header("Visual Options")]
        public SoTileTheme Theme;

        [SerializeField]
        TileRenderer _tilePrefab;

        [SerializeField]
        ObjectRenderer _objectPrefab;

        [SerializeField]
        private Transform _fullParent;
        [SerializeField]
        private Transform _tileParent;
        [SerializeField]
        private Transform _objectParent;


        [Header("Initalization")]
        public bool autoInitalize = false;

        [Header("Running Instance")]
        public SoGameGrid SerializedGrid;


        [Header("Events")]
        public GamestateReportReliableEvent _onGameState;
        public BooleanReliableEvent _onInputState;
        public VoidReliableEvent _onAnimIn;
        public VoidReliableEvent _onAnimOut;


        // Note: asusmes 1x1 prefabs
        private bool _inputState = false;
        private DiscreteVector2 _size;
        private TileGrid _dataGrid;
        private Grid<TileRenderer> _tileRenderGrid;
        private Dictionary<DiscreteVector2, ObjectRenderer> _objectRenderGrid = new Dictionary<DiscreteVector2, ObjectRenderer>();

        public bool InputState
        {
            get => _inputState;
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
                InputState = true;
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
            ClearParents();

            _size = new DiscreteVector2(serializedGrid.Width, serializedGrid.Height);

            _dataGrid = TileGrid.ConstructFrom(_size, Application.isPlaying ? serializedGrid.CopyTiles() : serializedGrid.TileGrid);
            _tileRenderGrid = new Grid<TileRenderer>(_size, SpawnTileRenderer);

            _objectRenderGrid.Clear();
            _dataGrid.ElementwiseAction(
                (Tile t, GridElement<Tile> i) =>
                {
                    if (t.Object == null) return;

                    ObjectRenderer obr = Instantiate(_objectPrefab, _objectParent);

                    if (obr != null)
                    {
                        obr.Object = t.Object;
                        _objectRenderGrid.Add(i.Cooridnate, obr);
                        obr.transform.localPosition = new UnityEngine.Vector3(i.Cooridnate.X, i.Cooridnate.Y, obr.transform.localPosition.z) + t.Object.Offset;
                    }
                }
            );

            _fullParent.transform.localPosition = new Vector3(-_dataGrid.Size.X / 2 + 0.5f, -_dataGrid.Size.Y / 2 + 0.5f, 0);
            RenderTick();
        }

        public void Clear()
        {
            ClearParents();

            _dataGrid = null;
            _tileRenderGrid = null;
            _objectRenderGrid.Clear();
        }

        private TileRenderer SpawnTileRenderer(DiscreteVector2 coord, Grid<TileRenderer> g)
        {
            TileRenderer renderer = Instantiate(_tilePrefab, _tileParent);
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
                DiscreteVector2 dir = obt.NextIndex - obt.LastIndex;

                if (dir == DiscreteVector2.Down) obt.TransactionObject.TriggerAnimation("Move_Down");
                if (dir == DiscreteVector2.Left) obt.TransactionObject.TriggerAnimation("Move_Left");
                if (dir == DiscreteVector2.Right) obt.TransactionObject.TriggerAnimation("Move_Right");
                if (dir == DiscreteVector2.Up) obt.TransactionObject.TriggerAnimation("Move_Up");

                ObjectRenderer rend = _objectRenderGrid[obt.LastIndex];

                DiscreteVector2 direction = obt.NextIndex - obt.LastIndex;
                rend.PrepareSlide(new UnityEngine.Vector2(direction.X, direction.Y));
                rends.Add(rend);
                _objectRenderGrid.Remove(obt.LastIndex);
                targ.Add(obt.NextIndex);
            }

            float time = 0;

            while (time < cSlideSpeed)
            {
                foreach (ObjectRenderer rend in rends) rend.Slide(time / cSlideSpeed);
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
                _objectRenderGrid.Add(targ[i], rends[i]);
            }

            PerformStatusReport();

            InputState = true;
        }

        private void RenderTick()
        {
            _dataGrid.ElementwiseAction(DoAnimUpdate);
            void DoAnimUpdate(Tile t, GridElement<Tile> ge)
            {
                if (t.Object != null) t.Object.FireReleventAnimationEvents();
            }

            _tileRenderGrid.ElementwiseAction(DoRender);
            void DoRender(TileRenderer rend) => rend.RenderTile();
        }
        #endregion



        public void AnimIn()
        {
            if (!autoInitalize)
            {
                _onAnimIn.Invoke();
            }
        }

        public void AnimOut()
        {
            if (!autoInitalize)
            {
                _onAnimOut.Invoke();
            }
        }






        public void ReceiveInput(CallbackContext Context)
        {
            if (InputState == false) return;

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

        private void ClearParents()
        {
            UTGameObject.DestroyAllChildren_EditorSafe(_tileParent.gameObject);
            UTGameObject.DestroyAllChildren_EditorSafe(_objectParent.gameObject);
        }

        private void PerformStatusReport()
        {
            GamestateReport report = new GamestateReport();
            _dataGrid.ReportGameState(report);
            _onGameState.Invoke(report);
        }
    }
}