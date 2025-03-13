using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WordBoggle
{
    //Controller for handling endless game mode logic. 
    public class EndlessModeController : MonoBehaviour
    {
        private List<string> _wordsList = new List<string>();
        private Grid _grid;

        public void StartEndlessGame(Grid grid, HashSet<string> words)
        {
            _wordsList = words.ToList();
            _grid = grid;
            
            grid.InitialiseEmptyCells(Constants.EndlessModeGridSizeX, Constants.EndlessModeGridSizeY);
            grid.InitialiseSpawnCells();
            
           var cellsUsed = PlaceExistingWord();
           var remainingCells = new List<Cell>();
           var allCells = grid.GetAllCells();
           foreach (var cell in allCells)
           {
               if (!cellsUsed.Contains(cell))
               {
                   remainingCells.Add(cell);
               }
           }
           
           _grid.FillRemainingCellsRandomly(remainingCells);
        }
        
        private List<Cell> PlaceExistingWord()
        {
            int randomWordIdx = Random.Range(0, _wordsList.Count);
            var randomWord = _wordsList[randomWordIdx];
            return  _grid.PlaceExistingWord(randomWord);
        }
        
        public void RemoveMatchedTiles(List<LetterTile> lettersMatched)
        {
            var remainingCells = new List<Cell>();
            foreach (var letter in lettersMatched)
            {
                var cell = letter.GetCell();
                remainingCells.Add(cell);
                Destroy(letter.gameObject);
                cell.RemoveTile();
            }
            StartCoroutine(PlayDestroyAnimation(remainingCells));
        }

        private IEnumerator PlayDestroyAnimation(List<Cell> remainingCells)
        {
            //Play anim
            yield return new WaitForSeconds(0.2f);
        }
        
    }
}