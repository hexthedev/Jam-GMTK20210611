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

        /// <summary>
        /// Resolves an input by action name. Returns true if at least one thing moves
        /// </summary>
        public bool ResolveInput(string input)
        {
            if (input == "Undo") return Undo();

            InputReport inputReport = new InputReport();
            ElementwiseAction((t, e) => { if (t.Object != null) t.Object.ResolveInputRecieved(input, inputReport, e);});
            if (!inputReport.HasValidInput) return false;

            DiscreteVector2 inputDirection = UTEManhattanDirections.AsDiscreteVector2(inputReport.ActivatedDirections);
            if (inputDirection == default) return false;


            MovementReport moveReport = new MovementReport();
            foreach (GridElement<Tile> el in inputReport.CanReceiveInput)
                Get(el.Cooridnate).Object.ResolveMovementRecieved(inputDirection, moveReport, el);


            ObjectTransaction[] transactions = GetTransactions(moveReport, inputDirection);
            PerformObjectTranscations(transactions);
            _history.Push(transactions);
            return true;
        }


        private ObjectTransaction[] GetTransactions(MovementReport moveReport, DiscreteVector2 direction)
        {
            List<ObjectTransaction> transactions = new List<ObjectTransaction>();

            foreach(GridElement<Tile> element in moveReport.CanMoveList)
            {
                transactions.Add( new ObjectTransaction()
                {
                    TransactionObject = Get(element.Cooridnate).Object,
                    LastIndex = element.Cooridnate,
                    NextIndex = element.Cooridnate + direction
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

        private bool Undo()
        {
            if (_history.Count == 0) return false;

            ObjectTransaction[] LastMove = _history.Pop();

            ObjectTransaction[] inverse = new ObjectTransaction[LastMove.Length];

            for (int i = 0; i < LastMove.Length; i++)
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



        private class ObjectTransaction
        {
            public SoObject TransactionObject;
            public DiscreteVector2 LastIndex;
            public DiscreteVector2 NextIndex;
        }
    }
}