using HexCS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace GMTK2021
{
    /// <summary>
    /// Grid management
    /// </summary>
    public class Grid<T>
    {
        // NOTE: Layed out in x y format. So index is x + y * width, starting bottom left.

        public Func<DiscreteVector2, T> DefaulElementCreator { get; private set; }

        public T[] Array { get; private set; }

        public DiscreteVector2 Size { get; private set; }

        public Grid(DiscreteVector2 size, Func<DiscreteVector2, T> defaultElementCreator)
        {
            Size = size;
            DefaulElementCreator = defaultElementCreator;
            Array = UTArray.ConstructArray(Size.Combinations, (i) => DefaulElementCreator(GetSplitIndex(i)));
        }

        public void PerformOnTiles(Action<T> action)
        {
            foreach (T tile in Array) action(tile);
        }

        public void PerformOnTiles(Action<T, DiscreteVector2, Grid<T>> action)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                action(Array[i], GetSplitIndex(i), this);
            }
        }

        public DiscreteVector2[] GetTilesWhere(Predicate<T> condition)
        {
            List<DiscreteVector2> tiles = new List<DiscreteVector2>();

            for(int i = 0; i<Array.Length; i++)
            {
                if (condition(Array[i])) tiles.Add(GetSplitIndex(i));
            }

            return tiles.ToArray();
        }

        public T[] GetManhattan(DiscreteVector2 target)
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

        public bool IsInBounds(DiscreteVector2 coord)
        {
            return !(coord.X < 0 || coord.Y < 0 || coord.X >= Size.X || coord.Y >= Size.Y);
        }

        public T Get(DiscreteVector2 coord) => Array[GetFlatIndex(coord)];
        public void Set(DiscreteVector2 coord, T value) => Array[GetFlatIndex(coord)] = value;
        public int GetFlatIndex(DiscreteVector2 coord) => coord.X + Size.X * coord.Y;
        public DiscreteVector2 GetSplitIndex(int flat) => new DiscreteVector2( flat%Size.Y, flat/Size.Y);
    }
}