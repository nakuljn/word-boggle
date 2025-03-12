using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WordBoggle
{
    public enum TileType
    {
        Normal,
        Bonus ,
        Blocker
    }
    
   

    public class LetterTile : MonoBehaviour
    { 
        private static readonly Dictionary<char, int> letterScores = new Dictionary<char, int>
        
        { { 'A', 1 }, { 'B', 3 }, { 'C', 3 }, { 'D', 2 }, { 'E', 1 },
            { 'F', 4 }, { 'G', 2 }, { 'H', 4 }, { 'I', 1 }, { 'J', 8 },
            { 'K', 5 }, { 'L', 1 }, { 'M', 3 }, { 'N', 1 }, { 'O', 1 },
            { 'P', 3 }, { 'Q', 10 }, { 'R', 1 }, { 'S', 1 }, { 'T', 1 },
            { 'U', 1 }, { 'V', 4 }, { 'W', 4 }, { 'X', 8 }, { 'Y', 4 },
            { 'Z', 10 }
        };
        
        [SerializeField] private TextMeshProUGUI letterTxt;
        [SerializeField] private GameObject bonusTile;
        [SerializeField] private GameObject blockerTile;
        
        [SerializeField] private TextMeshProUGUI scoreTxt;
        [SerializeField] private GameObject tileHighlighter;
        public bool CanBeSelected = true;
        private bool _isMoving = false;
        private TileType _tileType = TileType.Normal;
        private Cell  _cell;
        
        private string _letter;
        private GridLayoutGroup _gridLayoutGroup;
        private float _yPosDelta = 215f;

        #region Unity Methods
        
        private void OnEnable()
        {
            bonusTile.SetActive(false);
            blockerTile.SetActive(false);
            _isMoving = false;
            CanBeSelected = true;
        }
        
        private void Update()
        {
            if (_cell == null) return;
            
            if (GameManager.Instance.GameMode == GameMode.Endless)
            {
                var bottomCell = _cell.GetBottomNeighbour();
                if (bottomCell != null && bottomCell.GetTile() == null)
                {
                    var tile = _cell.GetTile();
                    if (tile == null)
                    {
                        return;
                    }
                    if (!_isMoving)
                    {
                        GameManager.Instance.ToggleGridLayoutGroup(false);
                        _isMoving = true;
                    }
                    var endPos = new Vector2(0, _cell.transform.localPosition.y - _yPosDelta);
                    tile.transform.localPosition = Vector2.MoveTowards(transform.localPosition, endPos, Time.deltaTime*700);
                    float distance = Mathf.Abs(tile.transform.localPosition.y - endPos.y);
                    if (distance < 1f)
                    {
                        var currentTile = _cell.GetTile();
                        _cell.RemoveTile();
                        CanBeSelected = true;
                        if (_cell is SpawnCell spawnCell && _cell.GetTile() == null)
                        {
                            spawnCell.GenerateNewTile();
                        }
                        AssignTileToCell(bottomCell, currentTile);
                        _isMoving = false;
                        GameManager.Instance.ToggleGridLayoutGroup(true);
                    }
                    else
                    {
                        CanBeSelected = false;
                    }
                }
            }
        }
        #endregion

        public void Initialise(TileType tileType, string letter, Cell  cell)
        {
            _letter = letter;
            letterTxt.text = _letter;
            _tileType = tileType;
            if (letter.Length > 0)
            {
                var tileScore = GetScore(letter);
                scoreTxt.text = tileScore.ToString();
            }
            if (tileType == TileType.Bonus)
            {
                bonusTile.SetActive(true);
            }

            if (tileType == TileType.Blocker)
            {
                blockerTile.SetActive(true);
            }
            
            _cell = cell;
        }

        public string GetLetterString()
        {
            return _letter;
        }
        
        public void EnableTileHighlight()
        {
            tileHighlighter.SetActive(true);
        }

        public void DisableTileHighlight()
        {
            tileHighlighter.SetActive(false);
        }

        public Cell  GetCell()
        {
            return _cell;
        }

        private void AssignTileToCell(Cell  cell, LetterTile letterTile)
        {
           
            transform.SetParent(cell.transform);
            transform.localPosition = Vector2.zero;
            _cell = cell;
            _cell.SetTile(letterTile);
        }

        public int GetLetterScore()
        {
            if (_tileType == TileType.Normal)
            {
                return GetScore(_letter);
            }
            if (_tileType == TileType.Bonus)
            {
                return GetScore(_letter) * 2;
            }

            return 0;
        }
        
        private static int GetScore(string letterString)
        {
            if (letterString.Length == 0) return 0;
            var letter = letterString[0];
            letter = char.ToUpper(letter); // Ensure uppercase for consistency
            return letterScores.ContainsKey(letter) ? letterScores[letter] : 0;
        }

    }
}
