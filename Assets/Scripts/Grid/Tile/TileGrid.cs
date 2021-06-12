using HexCS.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GMTK2021
{
    public class TileGrid : Grid<Tile>
    {
        private Stack<ObjectTransaction[]> _history = new Stack<ObjectTransaction[]>();


        public TileGrid(DiscreteVector2 size) : base(size, i => new Tile()) { }

        public TileGrid(DiscreteVector2 size, Func<DiscreteVector2, Tile> factory) : base(size, factory) { }

        public static TileGrid ConstructFrom(DiscreteVector2 size, Tile[] tiles)
        {
            return new TileGrid(size, Factory);
        
            Tile Factory(DiscreteVector2 coord)
            {
                return tiles[coord.X + size.X * coord.Y];
            }        
        }


        private bool Undo()
        {
            if (_history.Count == 0) return false;

            ObjectTransaction[] LastMove = _history.Pop();

            ObjectTransaction[] inverse = new ObjectTransaction[LastMove.Length];

            for(int i = 0; i<LastMove.Length; i++)
            {
                inverse[i] = new ObjectTransaction()
                {
                    TransactionObject = LastMove[i].TransactionObject,
                    LastIndex = LastMove[i].NextIndex,
                    NextIndex = LastMove[i].LastIndex
                };
            }

            PerformObjectTranscations(inverse);
            return true;
        }

        /// <summary>
        /// Resolves an input by action name. Returns true if at least one thing moves
        /// </summary>
        public bool ResolveInput(string input)
        {
            if (input == "Undo") return Undo();

            DiscreteVector2 inputDirection = ResolveDirection(input);
            if (inputDirection == default) return false;

            DiscreteVector2[] movementCandidates = ResolveAllMovementCandidates();
            DiscreteVector2[] movementWillApplyTo = ResolveMovementConsequences(movementCandidates, inputDirection);
            
            ObjectTransaction[] transactions = GetTransactions(movementWillApplyTo, inputDirection);
            PerformObjectTranscations(transactions);
            _history.Push(transactions);
            return true;
        }

        /// <summary>
        /// Analyzes the grid for the direction associated with the input
        /// </summary>
        public DiscreteVector2 ResolveDirection(string input)
        {
            DiscreteVector2[] inputTileIndicies = GetTilesWhere(t => t.Object != null && t.Object.InputAction == input);

            Tile[] tiles = inputTileIndicies
                .Select(t => GetManhattan(t))
                .UniqueMerge()
                .ToArray();

            EManhattanDirection[] directions = tiles
                .Select(t => t.Object == null ? EManhattanDirection.NONE : t.Object.InputDirection)
                .Where(s => s != EManhattanDirection.NONE)
                .ToArray();

            return UTEManhattanDirections.AsDiscreteVector2(directions);
        }

        public DiscreteVector2[] ResolveAllMovementCandidates()
        {
            List<DiscreteVector2> _movables = new List<DiscreteVector2>();
            PerformOnTiles(AddToListIfMoveable);
            return _movables.ToArray();

            void AddToListIfMoveable(Tile t, DiscreteVector2 index, Grid<Tile> grid)
            {
                if(t.Object != null)
                {
                    if (t.Object.ReceivesMovement(index, grid))
                    {
                        _movables.Add(index);
                    }
                }
            }
        }

        /// <summary>
        /// If movement candidates attempt to move, do other things move too. Returns index to all
        /// things that should move
        /// </summary>
        public DiscreteVector2[] ResolveMovementConsequences(DiscreteVector2[] movementCandidates, DiscreteVector2 inputDirection)
        {
            List<DiscreteVector2> movers = new List<DiscreteVector2>();

            foreach(DiscreteVector2 cand in movementCandidates)
            {
                DiscreteVector2 targetIndex = cand + inputDirection;
                if (!IsInBounds(targetIndex)) continue;

                SoObject obj = Get(cand).Object;
                Tile target = Get(targetIndex);
                
                // is the target tile empty
                if(target.Object == null)
                {
                    movers.Add(cand);
                    continue;
                }

                // if not empty, can target be pushed
                if (target.Object.IsPushable(obj, targetIndex, inputDirection, this))
                {
                    movers.Add(cand);
                    movers.Add(targetIndex);
                    continue;
                }
            }

            return movers.ToArray();
        }

        private ObjectTransaction[] GetTransactions(DiscreteVector2[] toMoveObjects, DiscreteVector2 direction)
        {
            List<ObjectTransaction> transactions = new List<ObjectTransaction>();

            foreach(DiscreteVector2 objIndex in toMoveObjects)
            {
                transactions.Add( new ObjectTransaction()
                {
                    TransactionObject = Get(objIndex).Object,
                    LastIndex = objIndex,
                    NextIndex = objIndex + direction
                } );
            }

            return transactions.ToArray();
        }




        private void PerformObjectTranscations(ObjectTransaction[] transactions)
        {
            foreach(ObjectTransaction transaction in transactions)
            {
                Tile initTile = Get(transaction.LastIndex);
                Tile targetTile = Get(transaction.NextIndex);

                targetTile.Object = transaction.TransactionObject;
                
                if(!transactions.QueryContains( s => s.NextIndex == transaction.LastIndex))
                {
                    initTile.Object = null;
                }
            }
        }




        private class ObjectTransaction
        {
            public SoObject TransactionObject;
            public DiscreteVector2 LastIndex;
            public DiscreteVector2 NextIndex;
        }
    }
}