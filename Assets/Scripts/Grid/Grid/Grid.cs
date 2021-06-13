using HexCS.Core;

using System;
using System.Collections.Generic;
using System.Linq;


namespace GMTK2021
{
    /// <summary>
    /// Manages a grid and provides basic grid manupulation api. Layed out in x y format. So index is x + y * width, starting bottom left.
    /// </summary>
    public class Grid<T>
    {
        private Func<DiscreteVector2, Grid<T>, T> _defaulElementFactory;

        /// <summary>
        /// Flat array of all elements
        /// </summary>
        public T[] Array { get; private set; }

        /// <summary>
        /// Size of the array
        /// </summary>
        public DiscreteVector2 Size { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Grid(DiscreteVector2 size, Func<DiscreteVector2, Grid<T>, T> defaultElementFactory)
        {
            Size = size;
            _defaulElementFactory = defaultElementFactory;
            Array = UTArray.ConstructArray(Size.Combinations, (i) => _defaulElementFactory(GetSplitIndex(i), this));
        }

        /// <summary>
        /// Do something with each element
        /// </summary>
        public void ElementwiseAction(Action<T> action)
        {
            foreach (T tile in Array) action(tile);
        }

        /// <summary>
        /// Do something on each element with it's coordinate and a grid reference
        /// </summary>
        public void ElementwiseAction(Action<T, GridElement<T>> action)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                action(Array[i], AsElement(GetSplitIndex(i)));
            }
        }

        /// <summary>
        /// Returns coordinates of elements where condition is met
        /// </summary>
        public DiscreteVector2[] ElementsWhere(Predicate<T> condition)
        {
            List<DiscreteVector2> tiles = new List<DiscreteVector2>();

            for(int i = 0; i<Array.Length; i++)
            {
                if (condition(Array[i])) tiles.Add(GetSplitIndex(i));
            }

            return tiles.ToArray();
        }

        /// <summary>
        /// Returns all neighbours based on manhattan movement
        /// </summary>
        public T[] GetManhattanNeighbours(DiscreteVector2 target)
        {
            DiscreteVector2[] candidates = new DiscreteVector2[]
            {
                target + DiscreteVector2.Left,
                target + DiscreteVector2.Right,
                target + DiscreteVector2.Up,
                target + DiscreteVector2.Down
            };

            IEnumerable<DiscreteVector2> vecs = candidates.Where(c => IsInBounds(c));

            List<T> mans = new List<T>();
            foreach (DiscreteVector2 vec in vecs) mans.Add(Get(vec));

            return mans.ToArray();
        }

        /// <summary>
        /// Is the cooridnate inbounds
        /// </summary>
        public bool IsInBounds(DiscreteVector2 coord)
        {
            return !(coord.X < 0 || coord.Y < 0 || coord.X >= Size.X || coord.Y >= Size.Y);
        }

        public GridElement<T> AsElement(DiscreteVector2 coordinate) => new GridElement<T>() { Cooridnate = coordinate, Grid = this };

        /// <summary>
        /// Get an element by coordinate
        /// </summary>
        public T Get(DiscreteVector2 coord) => Array[GetFlatIndex(coord)];

        /// <summary>
        /// Set an element by cooridnate
        /// </summary>
        public void Set(DiscreteVector2 coord, T value) => Array[GetFlatIndex(coord)] = value;

        /// <summary>
        /// Convert from coordinate to flat index
        /// </summary>
        public int GetFlatIndex(DiscreteVector2 coord) => coord.X + Size.X * coord.Y;

        /// <summary>
        /// Convert from flat index to cooridnate
        /// </summary>
        public DiscreteVector2 GetSplitIndex(int flat) => new DiscreteVector2( flat%Size.Y, flat/Size.Y);
    }
}