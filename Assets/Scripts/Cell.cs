
using System.Collections.Generic;
using UnityEngine;

namespace WordBoggle
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private LetterTile letterPrefab;
        
        private List<Cell> _neighbours;
        private Vector2Int _cellCoordinates;
        private LetterTile _letterTile;
        private Cell _bottomNeighbour;
        
        private void OnEnable()
        {
            _neighbours = new List<Cell>();
        }
        public void Initialise(GridTile gridTile, Vector2Int cellCoordinates)
        {
            var letterTileObj = Instantiate(letterPrefab, transform);
            letterTileObj.name = "Letter " + gridTile.letter;
            letterTileObj.gameObject.SetActive(true);
            _letterTile = letterTileObj;
            TileType tileType = GetTileType(gridTile.tileType);
            _letterTile.Initialise(tileType, gridTile.letter, this);
            SetCellCoordinates(cellCoordinates);
        }

        public void SetCellCoordinates(Vector2Int cellCoordinates)
        {
            _cellCoordinates = cellCoordinates;
        }
        
        public void SetBottomNeighbour(Cell neighbourCell)
        {
            _bottomNeighbour = neighbourCell;
        }

        public Cell GetBottomNeighbour()
        {
            return _bottomNeighbour;
        }
     
        
        public LetterTile GetTile()
        {
            return _letterTile;
        }

        public void SetTile(LetterTile tile)
        {
            _letterTile = tile;
        }

        public void RemoveTile()
        {
            _letterTile = null;
        }
        public Vector2Int GetCellCoordinates()
        {
            return _cellCoordinates;
        }
        
        public void AddNeighbour(Cell cell)
        {
            _neighbours.Add(cell);
        }

        public bool IsNeighbour(Cell cell)
        {
            return _neighbours.Contains(cell);
        }

        private TileType GetTileType(int tileNumber)
        {
            if (tileNumber == 0 || tileNumber == 1)
                return TileType.Normal;
            if (tileNumber == 2)
                return TileType.Bonus;
            if (tileNumber >= 3 && tileNumber <= 6)
                return TileType.Blocker;

            return TileType.Normal;
        }
     
    }
}
