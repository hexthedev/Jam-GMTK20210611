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

        public float slideSpeed = 0.5f;

        private bool _inputState = true;

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
            if (_inputState == false) return;

            if (Context.performed)
            {
                if (_dataGrid.ResolveInput(Context.action.name, out TileGrid.ObjectTransaction[] trans))
                {
                    _inputState = false;
                    _onInputState.Invoke(_inputState);

                    GamestateReport report = new GamestateReport();
                    _dataGrid.ReportGameState(report);
                    _onGameState.Invoke(report);

                    RenderTick();
                    StartCoroutine(PerformAnimations(trans));
                }
            }
        }



        private IEnumerator PerformAnimations(TileGrid.ObjectTransaction[] trans)
        {
            List<ObjectRenderer> rends = new List<ObjectRenderer>();
            List<DiscreteVector2> targ = new List<DiscreteVector2>();

            foreach(TileGrid.ObjectTransaction obt in trans)
            {
                ObjectRenderer rend = _objectRends[obt.LastIndex];
                rend.Origin = new UnityEngine.Vector2(obt.LastIndex.X, obt.LastIndex.Y);
                rend.Target = new UnityEngine.Vector2(obt.NextIndex.X, obt.NextIndex.Y);
                rend.IsSliding = true;
                rends.Add(rend);
                _objectRends.Remove(obt.LastIndex);
                targ.Add(obt.NextIndex);
            }

            float time = 0;

            while(time < slideSpeed)
            {
                foreach (ObjectRenderer rend in rends) rend.Slide(time/slideSpeed);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            foreach (ObjectRenderer rend in rends)
            {
                rend.Slide(1);
                rend.IsSliding = false;
            }

            for(int i = 0; i<rends.Count; i++)
            {
                _objectRends.Add(targ[i], rends[i]);
            }

            _inputState = true;
            _onInputState.Invoke(true);
        }





        [ContextMenu("ReloadSerializedGrid")]
        private void InitGrid()
        {
            if (this == null) return;
            UTGameObject.DestroyAllChildren_EditorSafe(gameObject);

            _size = new DiscreteVector2(SerializedGrid.Width, SerializedGrid.Height);

            _dataGrid = TileGrid.ConstructFrom(_size, Application.isPlaying ? SerializedGrid.CopyTiles() : SerializedGrid.TileGrid);



            _renderGrid = new Grid<TileRenderer>(_size, SpawnRenderer);

            _objectRends.Clear();
            _renderGrid.ElementwiseAction(
                (TileRenderer tr, GridElement<TileRenderer> i) =>
                {
                    ObjectRenderer obr = tr.MakeObjectRenderer(_objectPrefab);
                    if (obr != null) _objectRends.Add(i.Cooridnate, obr);
                }
            );

            foreach(KeyValuePair<DiscreteVector2, ObjectRenderer> or in _objectRends)
            {
                or.Value.transform.SetParent(transform, true);
                or.Value.SetOrigin(new UnityEngine.Vector2(or.Key.X, or.Key.Y));
            }

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