using System.Collections.Generic;
using UnityEngine;

namespace WordBoggle.Utils
{
    public enum Direction
    {
        Right,      
        Left,       
        Down,       
        Up,         
        DownRight,  
        DownLeft,   
        UpRight,    
        UpLeft      
    }

    public static class DirectionHelper
    {
        public static readonly Dictionary<Direction, Vector2Int> Offsets = new Dictionary<Direction, Vector2Int>
        {
            { Direction.Right, new Vector2Int(0, 1) },
            { Direction.Left, new Vector2Int(0, -1) },
            { Direction.Down, new Vector2Int(1, 0) },
            { Direction.Up, new Vector2Int(-1, 0) },
            { Direction.DownRight, new Vector2Int(1, 1) },
            { Direction.DownLeft, new Vector2Int(1, -1) },
            { Direction.UpRight, new Vector2Int(-1, 1) },
            { Direction.UpLeft, new Vector2Int(-1, -1) }
        };


        public static Vector2Int GetOffsetFromDirection(Direction direction)
        {
            if (Offsets.ContainsKey(direction))
            {
                return Offsets[direction];
            }

            return Vector2Int.zero;
        }
    }

}