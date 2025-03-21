using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace WordBoggle
{
    
    /// <summary>
    /// Central manager for receiving touch input to the grid.
    /// Stores the tiles from where user clicks and drags to make a word
    /// Send these tiles to the grid manager for processing 
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Fields

        
        public event Action OnSelectionStart = null;
        public event Action<List<LetterTile>> OnSelectionComplete = null;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private EventSystem eventSystem;
        
        public bool BlockInput = false;
        private readonly string _tileLayerName = "Tile";
        private List<LetterTile> _selectedTiles = new List<LetterTile>(); 
        private bool _isDragging = false;

        
        #endregion
        
        #region Unity Methods

        void Update()
        {
            if (BlockInput) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                OnBeginTouch(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                OnDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnEndTouch(Input.mousePosition);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnBeginTouch(touch.position);
                        break;
                    case TouchPhase.Moved:
                        OnDrag(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnEndTouch(touch.position);
                        break;
                }
            }
        }
        
        #endregion

        private void OnBeginTouch(Vector2 position)
        {
            var objTouched = GetObjectUnderTouch(position);
            _selectedTiles.Clear();
            if (objTouched != null && LayerMask.LayerToName(objTouched.layer) == _tileLayerName)
            {
                _isDragging = true;
                LetterTile tile = objTouched.GetComponentInParent<LetterTile>();
                if (tile != null && tile.CanBeSelected)
                {
                     OnSelectionStart?.Invoke();
                     _selectedTiles.Add(tile);
                     tile.EnableTileHighlight();
                }

            }
        }

        private void OnDrag(Vector2 position)
        {
            if (_isDragging)
            {
                GameObject objTouched = GetObjectUnderTouch(position);
              
                if (objTouched != null && LayerMask.LayerToName(objTouched.layer) == _tileLayerName)
                {
                    LetterTile tile = objTouched.GetComponentInParent<LetterTile>();
                    if (tile !=null && !_selectedTiles.Contains(tile) && tile.CanBeSelected)
                    {
                        var lastCell = _selectedTiles.LastOrDefault()?.GetCell();
                        var currCell = tile.GetCell();
                        if (lastCell == null || lastCell.IsNeighbour(currCell))
                        {
                            tile.EnableTileHighlight();
                            _selectedTiles.Add(tile);
                        }
                    }
                }
            }
        }

        private void OnEndTouch(Vector2 position)
        {
            if (_selectedTiles.Count > 0)
            {
                _selectedTiles.ForEach(tile => tile.DisableTileHighlight());
            }

            if (_isDragging)
            {
                OnSelectionComplete?.Invoke(_selectedTiles);
            }
           
            _isDragging = false;
        }
        
        private GameObject GetObjectUnderTouch(Vector2 position)
        {
            PointerEventData eventData = new PointerEventData(eventSystem)
            {
                position = position
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            return results.Count > 0 ? results[0].gameObject : null;
        }
        
       
    }
}
