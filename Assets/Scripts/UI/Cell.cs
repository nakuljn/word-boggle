
using System.Collections.Generic;
using UnityEngine;

namespace WordBoggle
{
    /// <summary>
    /// Holder for containing tiles. Grid consists of cells and cells holds each tile.
    /// Cell here referes to grid cells.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private LetterTile letterPrefab;
        
        private List<Cell> _neighbours;
        private Vector2Int _cellCoordinates;
        private LetterTile _letterTile;
        private Cell _bottomNeighbour;
        
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            _neighbours = new List<Cell>();
        }
        #endregion

        public void Initialise(GridTile gridTile, Vector2Int cellCoordinates)
        {
            var letterTileObj = Instantiate(letterPrefab, transform);
            letterTileObj.name = "Letter " + gridTile.letter;
            letterTileObj.gameObject.SetActive(true);
            _letterTile = letterTileObj;
            _letterTile.Initialise(gridTile, this);
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

      
    }
}
