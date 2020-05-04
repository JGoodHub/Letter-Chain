using System.Collections;
using System.Collections.Generic;
using System.Text;
using GoodHub.Core;
using UnityEngine;

public class TileController : Singleton<TileController> {

    [Header("Alphabet Sprites")]
    public Sprite[] alphabetSprites = new Sprite[26];

    [Header("UI Elements")]
    public GameObject letterTilePrefab;
    public GameObject connectorPrefab;

    [Header("Spacing and Parenting")]
    public RectTransform letterTileContainer;
    public RectTransform connectorContainer;

    private const float tileSpacing = 200f;
    private const int boardSize = 5;
    
    //Object collections
    private HashSet<LetterTile> boardTiles = new HashSet<LetterTile>();
    private List<GameObject> connectorLines = new List<GameObject>();

    //State variables
    private bool boardLocked = false;
    [HideInInspector] public int minimumVowels = 5;

    private char[] tileBagArray;
    private int tileBagPtr = 0;

    #region Inherited Methods

    private void Start() {
        minimumVowels = GameManager.Instance.GetVowelMinimum();
        tileBagArray = DictionaryManager.Instance.GetRandomLetterArray(boardSize * boardSize, minimumVowels);

        RegenerateTileBoard();
    }

    #endregion

    #region Public Methods

    #region Tile Creation and Destruction

    public void RegenerateTileBoard() {
        foreach (LetterTile tile in boardTiles) {
            Destroy(tile.gameObject);
        }

        char[] letterArray = DictionaryManager.Instance.GetRandomLetterArray(boardSize * boardSize, minimumVowels);

        //Generate the starting board using the random list of letters
        int letterIndex = 0;
        for (int y = -(boardSize / 2); y <= boardSize / 2; y++) {
            for (int x = -(boardSize / 2); x <= boardSize / 2; x++) {
                AddTile(letterArray[letterIndex], new Vector2Int(x, y));
                letterIndex++;
            }
        }
    }

    public LetterTile AddTile(char letter, Vector2Int gridPosition) {
        Vector2 screenPosition = new Vector2(gridPosition.x * tileSpacing, gridPosition.y * tileSpacing);

        GameObject tileClone = Instantiate(letterTilePrefab, letterTileContainer);
        RectTransform tileRectTransform = (RectTransform)tileClone.transform;
        tileRectTransform.anchoredPosition = screenPosition;

        LetterTile letterTile = tileClone.GetComponent<LetterTile>();
        letterTile.Initialise(letter, gridPosition);

        boardTiles.Add(letterTile);
        return letterTile;
    }

    public void RemoveTiles(LetterTile[] tilesToRemove) {
        for (int i = 0; i < tilesToRemove.Length; i++) {
            if (boardTiles.Contains(tilesToRemove[i])) {
                //Remove the old tile
                boardTiles.Remove(tilesToRemove[i]);
                StartCoroutine(tilesToRemove[i].DestroyCoroutine(0.2f + (i * 0.1f)));

                //Add the new tile
                LetterTile newTile = AddTile(GetLetterFromBag(), tilesToRemove[i].GridPosition);
                boardTiles.Add(newTile);
                StartCoroutine(newTile.SpawnCoroutine(0.4f + (i * 0.1f)));
            } else {
                Debug.LogError("ERROR: Tile not on board");
            }
        }
    }

    #endregion
    
    #region Connectors Creation and Destruction

    public void AddConnector(LetterTile tileA, LetterTile tileB) {
        RectTransform tileARectTransform = (RectTransform)tileA.transform;
        RectTransform tileBRectTransform = (RectTransform)tileB.transform;

        GameObject connectorObject = Instantiate(connectorPrefab, connectorContainer);
        RectTransform connectorRectTransform = (RectTransform)connectorObject.transform;
        connectorRectTransform.anchoredPosition = (tileARectTransform.anchoredPosition + tileBRectTransform.anchoredPosition) / 2;

        Vector2 dragDirection = tileB.GridPosition - tileA.GridPosition;
        float lineAngle = Vector2.Angle(Vector2.up, dragDirection);
        
        if (lineAngle % 90 == 45) {
            if (tileA.GridPosition.x < tileB.GridPosition.x) {
                if (lineAngle == 45) {
                    connectorRectTransform.Rotate(Vector3.forward, -45);
                } else {
                    connectorRectTransform.Rotate(Vector3.forward, 45);
                }
            } else {
                if (lineAngle == 45) {
                    connectorRectTransform.Rotate(Vector3.forward, 45);
                } else {
                    connectorRectTransform.Rotate(Vector3.forward, -45);
                }
            }
        } else {
            if (lineAngle == 90) {
                connectorObject.transform.Rotate(Vector3.forward, 90);
            }
        }

        connectorLines.Add(connectorObject);
    }

    public void ClearAllConnections() {
        foreach (GameObject line in connectorLines) {
            Destroy(line);
        }
    }

    #endregion

    public bool DoTilesFormChain(List<LetterTile> tileChain) {
        for (int i = 0; i < tileChain.Count - 1; i++) {
            if (GetAdjacentTiles(tileChain[i]).Contains(tileChain[i + 1]) == false) {
                return false;
            }
        }

        return true;
    }
    
    public HashSet<LetterTile> GetAdjacentTiles(LetterTile tile) {
        HashSet<LetterTile> adjacentTiles = new HashSet<LetterTile>();

        //Find the tiles that are located less than one diagonal grid space away (i.e. root of 2 = 1.414)
        foreach (LetterTile otherTile in boardTiles) {
            float distanceToOtherTile = Vector2.Distance(tile.GridPosition, otherTile.GridPosition);
            if (distanceToOtherTile > 0.5f && distanceToOtherTile < 1.42f) {
                adjacentTiles.Add(otherTile);
            }
        }

        return adjacentTiles;
    }

    public Sprite GetAlphabetSprite(char letter) {
        return alphabetSprites[DictionaryManager.Instance.GetAlphabetIndex(letter)];
    }

    private Vector2 GridPositionToAnchoredPosition(Vector2 gridPosition) {
        return new Vector2(gridPosition.x * tileSpacing, gridPosition.y * tileSpacing);
    }

    #endregion

    #region Private Methods

    private char GetLetterFromBag () {
        char letter = tileBagArray[tileBagPtr];

        tileBagPtr = (tileBagPtr + 1) % tileBagArray.Length;
        if (tileBagPtr == 0) {
            tileBagArray = DictionaryManager.Instance.GetRandomLetterArray(boardSize * boardSize, minimumVowels);
        }

        return letter;
    }

    #endregion

    #region Coroutines

    public IEnumerator SwapTilesCoroutine(LetterTile tileA, LetterTile tileB) {
        if (boardLocked == false) {
            boardLocked = true;

            StartCoroutine(tileA.MoveCoroutine(tileB.RectTransform.anchoredPosition, tileB.GridPosition));
            StartCoroutine(tileB.MoveCoroutine(tileA.RectTransform.anchoredPosition, tileA.GridPosition));

            while (tileA.TileMoving) {
                yield return null;
            }

            boardLocked = false;
        }
    }

    #endregion
    
}
