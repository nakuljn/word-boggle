using UnityEngine;

namespace WordBoggle
{
    /// <summary>
    /// Extension of Cell which is present in spawn grid.
    /// Generates a new letter tile, when tile in spawn grid cascades
    /// </summary>
    public class SpawnCell : Cell
    {
        public void GenerateNewTile()
        {
            var randomLetterIdx = Random.Range(0, 26);
            char randomLetter  = (char)('A' + randomLetterIdx);
            var gridTile = new GridTile {letter = randomLetter.ToString(), tileType = 0};
            Initialise(gridTile, GetCellCoordinates());
        }
    }
}