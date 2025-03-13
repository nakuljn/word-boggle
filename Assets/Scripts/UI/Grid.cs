using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WordBoggle.Utils;
using Random = UnityEngine.Random;

namespace WordBoggle
{
    /// <summary>
    /// Main class for holding cell data and initialising Cells based on game mode
    /// This can either initialise cells based on level data or constant grid size 
    /// </summary>
    public class Grid : MonoBehaviour
    { 
        #region Fields
        
        [SerializeField] private Cell cellPrefab;
        [SerializeField] private SpawnCell spawnCellPrefab;
        [SerializeField] private Transform cellParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Transform spawnCellsParent;
        private Dictionary<Vector2Int, Cell> _cellsDictionary;
        private Vector2Int _gridSize;

        #endregion
        #region Unity Methods
        private void OnEnable()
        {
            _cellsDictionary = new Dictionary<Vector2Int, Cell>();
        }

        private void OnDestroy()
        {
            ClearAllCells();
        }
        #endregion

        //Initialising grid for levels mode
        
        public void Initialise(LevelData levelData)
        {
            ClearAllCells();
            _gridSize = new Vector2Int(levelData.gridSize.x, levelData.gridSize.y);
            
            int cellCount = 0;
            for (int i = 0; i < _gridSize.x; i++)
            {
                for (int j = 0; j < _gridSize.y; j++)
                {
                    Cell cell = Instantiate(cellPrefab, cellParent);
                    cell.gameObject.name = "Cell " + i + " " + j;
                    cell.Initialise(levelData.gridData[cellCount], new Vector2Int(i, j));
                    cellCount++;
                    var cellCoord = new Vector2Int(i, j);
                    _cellsDictionary[cellCoord] = cell;
                }
            }
            IdentifyNeighbours();
        }

        private void ClearAllCells()
        {
            foreach (var cellMap in _cellsDictionary)
            {
                var cell = cellMap.Value;
                Destroy(cell.gameObject);
            }
            _cellsDictionary.Clear();
        }
        
        //Initialising grid for endless mode
        public void InitialiseEmptyCells(int x, int y)
        {
            _gridSize = new Vector2Int(x, y);
            for (int i = 0; i < _gridSize.x; i++)
            {
                for (int j = 0; j < _gridSize.y; j++)
                {
                    Cell cell = Instantiate(cellPrefab, cellParent);
                    cell.gameObject.name = "Cell " + i + " " + j;
                    cell.gameObject.SetActive(true);
                    var coord = new Vector2Int(i, j);
                    cell.SetCellCoordinates(coord);
                    _cellsDictionary[coord] = cell;
                }
            }
            
            IdentifyNeighbours();
        }

        private void IdentifyNeighbours()
        {
            var cells = _cellsDictionary.Values.ToList();
            var directions = DirectionHelper.Offsets.Keys.ToList();
            foreach (var cell in cells)
            {
                foreach (var dir in directions)
                {
                    var pos = DirectionHelper.GetOffsetFromDirection(dir);
                    var neighbourPos = cell.GetCellCoordinates() + pos;
                    var neighbourCell = GetCell(neighbourPos);
                    if (neighbourCell != null)
                    {
                        cell.AddNeighbour(neighbourCell);
                        if (dir == Direction.Down)
                        {
                            cell.SetBottomNeighbour(neighbourCell);
                        }
                    }
                }
            }
        }

        public void InitialiseSpawnCells()
        {
            for (int j = 0; j < Constants.EndlessModeGridSizeY; j++)
            {
                var spawnCell = Instantiate(spawnCellPrefab, spawnCellsParent.transform);
                spawnCell.name = "Spawn Cell 0 " + j;
                
                var randomLetterIdx = Random.Range(0, 26);
                char randomLetter  = (char)('A' + randomLetterIdx);
                var gridTile = new GridTile {letter = randomLetter.ToString(), tileType = 0};
                var coordinates = new Vector2Int(0, -j)
;               spawnCell.Initialise(gridTile, coordinates);
                IdentifySpawnCellNeighbours(spawnCell);
            }
        }

        private void IdentifySpawnCellNeighbours(SpawnCell spawnCell)
        {
            var coordinates = spawnCell.GetCellCoordinates();
            var y = -coordinates.y;
            var bottomCell = GetCell(0, y);
            spawnCell.AddNeighbour(bottomCell);
            spawnCell.SetBottomNeighbour(bottomCell);

        }
        
        // For endless mode, this method places a valid word on the grid starting from 
        // random position and going into random direction.
        public List<Cell> PlaceExistingWord(string randomWord)
        {
            int randomX = Random.Range(0, Constants.EndlessModeGridSizeX);
            int randomY = Random.Range(0, Constants.EndlessModeGridSizeY);
            
            Vector2Int startCoord = new Vector2Int(randomX, randomY);
            
            var cellsUsed = new List<Cell>();
            foreach (var letter in randomWord)
            {
                var letterString = letter.ToString().ToUpper();;
                var cell = GetCell(startCoord);
                var gridTile = new GridTile {letter = letterString, tileType = 0};
                cell.Initialise(gridTile, startCoord);
                cellsUsed.Add(cell);
                if (cellsUsed.Count == Constants.EndlessModeGridSizeX * Constants.EndlessModeGridSizeY)
                    break;
                
                startCoord = GetNextCoordinate(startCoord, cellsUsed);
            }
            return cellsUsed;
        }
        
        private Vector2Int GetNextCoordinate(Vector2Int currentCoord, List<Cell > cellsCoordUsed)
        {
            var possibleDirections = DirectionHelper.Offsets.Values.ToList();
            possibleDirections = possibleDirections.OrderBy(_ => Random.Range(0, possibleDirections.Count)).ToList();

            foreach (Vector2Int direction in possibleDirections)
            {
                Vector2Int nextCoord = currentCoord + direction;
                var cell = GetCell(nextCoord);
                if (cell != null && !cellsCoordUsed.Contains(cell))
                {
                    return nextCoord;
                }
            }
            return Vector2Int.zero;
        }
        
        public void FillRemainingCellsRandomly(List<Cell> remaniningCells)
        {
            for (int i = 0; i < Constants.EndlessModeGridSizeX; i++)
            {
                for (int j = 0; j < Constants.EndlessModeGridSizeY; j++)
                {
                    var cell = GetCell(new Vector2Int(i, j));
                    if (cell != null && remaniningCells.Contains(cell))
                    {
                        remaniningCells.Add(cell);
                        var randomLetterIdx = Random.Range(0, 26);
                        char randomLetter  = (char)('A' + randomLetterIdx);
                        var gridTile = new GridTile {letter = randomLetter.ToString(), tileType = 0};
                        cell.Initialise(gridTile, new Vector2Int(i, j));
                    }
                }
            }
        }
        
        public void RemoveCells()
        {
            foreach (var cell in _cellsDictionary.Values)
            {
                Destroy(cell.gameObject);
            }
            _cellsDictionary.Clear();
        }

        private Cell GetCell(Vector2Int cellCoord)
        {
            return GetCell(cellCoord.x, cellCoord.y);
        }

        private Cell  GetCell(int x, int y)
        {
            if(x >= 0 && y >= 0 && x < _gridSize.x && y < _gridSize.y)
                return _cellsDictionary[new Vector2Int(x, y)];
            
            return null;
        }

        public List<Cell > GetAllCells()
        {
            return _cellsDictionary.Values.ToList();
        }

        public void ToggleGridLayoutGroup(bool toggle)
        {
            gridLayoutGroup.enabled = toggle;
        }
    }
}
