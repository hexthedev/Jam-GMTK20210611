using System.Collections.Generic;

namespace GMTK2021
{
    /// <summary>
    /// When trying to apply movement, reports what must moves, what is pushed and what is blocked
    /// </summary>
    public class MovementReport
    {
        public List<GridElement<Tile>> FreeMove = new List<GridElement<Tile>>();
        public List<GridElement<Tile>> PusherMove = new List<GridElement<Tile>>();
        public List<GridElement<Tile>> PushedMove = new List<GridElement<Tile>>();
        public List<GridElement<Tile>> BlockedMove = new List<GridElement<Tile>>();

        public List<EManhattanDirection> ActivatedDirs = new List<EManhattanDirection>();
        public IEnumerable<GridElement<Tile>> CanMoveList
        {
            get
            {
                List<GridElement<Tile>> tiles = new List<GridElement<Tile>>();

                tiles.AddRange(FreeMove);
                tiles.AddRange(PushedMove);
                tiles.AddRange(PusherMove);

                return tiles;
            }
        }
    }
}