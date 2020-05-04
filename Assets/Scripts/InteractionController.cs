using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodHub.Core;

public class InteractionController : Singleton<InteractionController> {

    private LetterTile selectedTile = null;

    private List<LetterTile> tileChain = new List<LetterTile>();
    private bool chainStarted = false;

    private bool isTouching = false;

    #region Inherited Methods
    
    private void Update() {
        if (isTouching && Input.touchCount == 0) {
            isTouching = false;

            OnTouchUp();
        } else if (Input.touchCount > 0) {
            isTouching = true;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0)) {
            OnTouchUp();
        }
#endif
    }

    #endregion

    #region Public Methods

    public void TileClicked(LetterTile tileClicked) {
        if (selectedTile == null) {
            selectedTile = tileClicked;
            selectedTile.SetSelected(true);
        } else if (selectedTile == tileClicked) {
            selectedTile = null;
            tileClicked.SetSelected(false);
        } else {
            if (TileController.Instance.GetAdjacentTiles(selectedTile).Contains(tileClicked)) {
                //Swap the two adjacent tiles
                StartCoroutine(TileController.Instance.SwapTilesCoroutine(selectedTile, tileClicked));

                selectedTile.SetSelected(false);
                tileClicked.SetSelected(false);
                selectedTile = null;
            } else {
                //Deselect the old tile and select the newly clicked tile
                selectedTile.SetSelected(false);
                selectedTile = tileClicked;
                selectedTile.SetSelected(true);
            }
        }
    }

    public void StartWordChain(LetterTile startingTile) {
        tileChain = new List<LetterTile>();
        tileChain.Add(startingTile);
        startingTile.SetSelected(true);
        chainStarted = true;
    }

    public void AddToWordChain(LetterTile tile) {
        if (chainStarted) {
            if (tileChain.Contains(tile) == false) {
                // && TileController.Instance.GetAdjacentTiles(tileChain[tileChain.Count - 1]).Contains(tile)
                TileController.Instance.AddConnector(tileChain[tileChain.Count - 1], tile);
                tile.SetSelected(true);
                tileChain.Add(tile);
            }
        }
    }

    public void FinishWordChain() {
        if (chainStarted && tileChain.Count > 1) {
            string attemptedWord = "";
            for (int i = 0; i < tileChain.Count; i++) {
                attemptedWord += tileChain[i].Letter;
            }

            if (TileController.Instance.DoTilesFormChain(tileChain) && DictionaryManager.Instance.ContainsWord(attemptedWord)) {
                ScoreManager.Instance.IncreaseScore(DictionaryManager.Instance.GetWordScore(attemptedWord));

                TileController.Instance.RemoveTiles(tileChain.ToArray());

                UIManager.Instance.DisplayWord(attemptedWord + " (+" + DictionaryManager.Instance.GetWordScore(attemptedWord) + ")");
                UIManager.Instance.DisplayDefinition(DictionaryManager.Instance.GetWordDefinition(attemptedWord));
            } else {
                ScoreManager.Instance.DecreaseScore(1);

                UIManager.Instance.DisplayWord(attemptedWord + " (-1)");
                UIManager.Instance.DisplayDefinition(attemptedWord + " is not a word");
                
                foreach (LetterTile tile in tileChain) {
                    tile.SetSelected(false);
                }
            }

            TileController.Instance.ClearAllConnections();
        } else {
            DeselectTileChain();
        }
        
        chainStarted = false;
    }

    #endregion

    #region Private Methods

    private void OnTouchUp() {
        FinishWordChain();
    }

    private void DeselectTileChain() {
        foreach (LetterTile tile in tileChain) {
            tile.SetSelected(false);
        }

        tileChain.Clear();
    }

    #endregion
}

