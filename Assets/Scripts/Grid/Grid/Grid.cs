using HexCS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
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

        public T Get(DiscreteVector2 coord) => Array[GetFlatIndex(coord)];
        public void Set(DiscreteVector2 coord, T value) => Array[GetFlatIndex(coord)] = value;
        public int GetFlatIndex(DiscreteVector2 coord) => coord.X + Size.X * coord.Y;
        public DiscreteVector2 GetSplitIndex(int flat) => new DiscreteVector2( flat%Size.Y, flat/Size.Y);
    }
}