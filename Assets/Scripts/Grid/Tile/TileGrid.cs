using HexCS.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace GMTK2021
{
    public class TileGrid : Grid<Tile>
    {
        private Stack<ObjectTransaction[]> _history = new Stack<ObjectTransaction[]>();

        public TileGrid(DiscreteVector2 size) : base(size, (i, g) => new Tile()) { }

        public TileGrid(DiscreteVector2 size, Func<DiscreteVector2, Grid<Tile>, Tile> factory) : base(size, factory) { }

        public static TileGrid ConstructFrom(DiscreteVector2 size, Tile[] tiles)
        {
            return new TileGrid(size, Factory);
        
            Tile Factory(DiscreteVector2 coord, Grid<Tile> grid)
            {
                Tile t = tiles[coord.X + size.X * coord.Y];

                if (Application.isPlaying)
                {
                    if (t.Object != null) t.Object = t.Object.Copy();
                }

                if (t.Object != null) t.Object.Init(coord, grid);
                return t;
            }        
        }

        /// <summary>
        /// Resolves an input by action name. Returns true if at least one thing moves
        /// </summary>
        public bool ResolveInput(string input, out ObjectTransaction[] transactions)
        {
            transactions = null;
            if (input == "Undo")
            {
                return Undo(out transactions);
            }

            InputReport inputReport = new InputReport();
            ElementwiseAction((t, e) => { if (t.Object != null) t.Object.ProvideReportAboutInputRole(input, inputReport);});
            if (!inputReport.HasValidInput) return false;

            DiscreteVector2 inputDirection = UTEManhattanDirections.AsDiscreteVector2(inputReport.ActivatedDirections);
            if (inputDirection == default) return false;


            MovementReport moveReport = new MovementReport();
            foreach (GridElement<Tile> el in inputReport.CanReceiveInput)
                Get(el.Cooridnate).Object.ProvideReportAboutMovementCapabilities(inputDirection, moveReport);

            if (moveReport.CanMoveList.Count() == 0) return false;

            transactions = GetTransactions(moveReport, inputDirection);
            PerformObjectTranscations(transactions);
            _history.Push(transactions);
            ElementwiseAction(t =>
            {
                if (t.Object != null) t.Object.Tick();
            });
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

                targetTile.Object.myPosition = transaction.NextIndex;
            }
        }

        private bool Undo(out ObjectTransaction[] inverse)
        {
            inverse = null;
            if (_history.Count == 0) return false;

            ObjectTransaction[] LastMove = _history.Pop();

            inverse = new ObjectTransaction[LastMove.Length];

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
            ElementwiseAction(t =>
            {
                if (t.Object != null) t.Object.BackTick();
            });
            return true;
        }

        public void ReportGameState(GamestateReport report)
        {
            ElementwiseAction(ReportOnGamestate);

            void ReportOnGamestate(Tile t, GridElement<Tile> element)
            {
                if(t.Object != null && t.Floor != null)
                {
                    SOFloorProperties fl = t.Floor;
                    SoObject obj = t.Object;

                    if (fl.PlayerWins && obj.IsPlayer) report.isPlayerOnGoal = true;
                    if (fl.PlayerDies && obj.IsPlayer) report.isPlayerDead = true;
                }
            }
        }



        public class ObjectTransaction
        {
            public SoObject TransactionObject;
            public DiscreteVector2 LastIndex;
            public DiscreteVector2 NextIndex;
        }
    }
}