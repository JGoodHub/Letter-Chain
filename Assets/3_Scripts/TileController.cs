using System.Collections;
using System.Collections.Generic;
using System.Text;
using GoodHub.Core;
using UnityEngine;

public class TileController : Singleton<TileController>
{

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

    private char[] letterBagArray;
    private int letterBagPtr = -1;

    #region Inherited Methods

    private void Start()
    {
        //Generate the starting board using the random list of letters
        for (int y = -(boardSize / 2); y <= boardSize / 2; y++)
        {
            for (int x = -(boardSize / 2); x <= boardSize / 2; x++)
            {
                AddTile(GetNextLetterFromBag(), new Vector2Int(x, y));
            }
        }
    }

    #endregion

    #region Public Methods

    #region Tile Creation and Destruction

    public LetterTile AddTile(char letter, Vector2Int gridPosition)
    {
        Vector2 screenPosition = new Vector2(gridPosition.x * tileSpacing, gridPosition.y * tileSpacing);

        GameObject tileClone = Instantiate(letterTilePrefab, letterTileContainer);
        RectTransform tileRectTransform = (RectTransform)tileClone.transform;
        tileRectTransform.anchoredPosition = screenPosition;

        LetterTile letterTile = tileClone.GetComponent<LetterTile>();
        letterTile.Initialise(letter, gridPosition);

        boardTiles.Add(letterTile);
        return letterTile;
    }

    public void RemoveTiles(LetterTile[] tilesToRemove)
    {
        for (int i = 0; i < tilesToRemove.Length; i++)
        {
            if (boardTiles.Contains(tilesToRemove[i]))
            {
                //Remove the old tile
                boardTiles.Remove(tilesToRemove[i]);
                StartCoroutine(tilesToRemove[i].DestroyCoroutine(0.2f + (i * 0.1f)));

                //Add the new tile
                LetterTile newTile = AddTile(GetNextLetterFromBag(), tilesToRemove[i].GridPosition);
                boardTiles.Add(newTile);
                StartCoroutine(newTile.SpawnCoroutine(0.4f + (i * 0.1f)));
            }
            else
            {
                Debug.LogError("ERROR: Tile not on board");
            }
        }
    }

    #endregion

    #region Connectors Creation and Destruction

    public void AddConnector(LetterTile tileA, LetterTile tileB)
    {
        RectTransform tileARectTransform = (RectTransform)tileA.transform;
        RectTransform tileBRectTransform = (RectTransform)tileB.transform;

        GameObject connectorObject = Instantiate(connectorPrefab, connectorContainer);
        RectTransform connectorRectTransform = (RectTransform)connectorObject.transform;
        connectorRectTransform.anchoredPosition = (tileARectTransform.anchoredPosition + tileBRectTransform.anchoredPosition) / 2;

        Vector2 dragDirection = tileB.GridPosition - tileA.GridPosition;
        float lineAngle = Vector2.Angle(Vector2.up, dragDirection);

        if (lineAngle % 90 == 45)
        {
            if (tileA.GridPosition.x < tileB.GridPosition.x)
            {
                if (lineAngle == 45)
                {
                    connectorRectTransform.Rotate(Vector3.forward, -45);
                }
                else
                {
                    connectorRectTransform.Rotate(Vector3.forward, 45);
                }
            }
            else
            {
                if (lineAngle == 45)
                {
                    connectorRectTransform.Rotate(Vector3.forward, 45);
                }
                else
                {
                    connectorRectTransform.Rotate(Vector3.forward, -45);
                }
            }
        }
        else
        {
            if (lineAngle == 90)
            {
                connectorObject.transform.Rotate(Vector3.forward, 90);
            }
        }

        connectorLines.Add(connectorObject);
    }

    public void ClearAllConnections()
    {
        foreach (GameObject line in connectorLines)
        {
            Destroy(line);
        }
    }

    #endregion

    public bool DoTilesFormChain(List<LetterTile> tileChain)
    {
        for (int i = 0; i < tileChain.Count - 1; i++)
        {
            if (GetAdjacentTiles(tileChain[i]).Contains(tileChain[i + 1]) == false)
            {
                return false;
            }
        }

        return true;
    }

    public HashSet<LetterTile> GetAdjacentTiles(LetterTile tile)
    {
        HashSet<LetterTile> adjacentTiles = new HashSet<LetterTile>();

        //Find the tiles that are located less than one diagonal grid space away (i.e. root of 2 = 1.414)
        foreach (LetterTile otherTile in boardTiles)
        {
            float distanceToOtherTile = Vector2.Distance(tile.GridPosition, otherTile.GridPosition);
            if (distanceToOtherTile > 0.5f && distanceToOtherTile < 1.42f)
            {
                adjacentTiles.Add(otherTile);
            }
        }

        return adjacentTiles;
    }

    public Sprite GetAlphabetSprite(char letter)
    {
        return alphabetSprites[DictionaryManager.Instance.GetAlphabetIndex(letter)];
    }

    private Vector2 GridPositionToAnchoredPosition(Vector2 gridPosition)
    {
        return new Vector2(gridPosition.x * tileSpacing, gridPosition.y * tileSpacing);
    }

    #endregion

    #region Private Methods

    private char GetNextLetterFromBag()
    {
        letterBagPtr++;
        if (letterBagArray == null || letterBagPtr == 100)
        {
            // Generate the letter tiles array
            char currentLetter = 'a';
            letterBagArray = new char[100];
            int bagPtr = 0;

            //                               A  B  C  D  E   F  G  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z
            int[] occurences = new int[26] { 9, 2, 2, 4, 12, 2, 4, 2, 9, 2, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1 };
            int occurPtr = 0;

            while (bagPtr < letterBagArray.Length)
            {
                while (occurences[occurPtr] > 0)
                {
                    letterBagArray[bagPtr] = currentLetter;
                    bagPtr++;

                    occurences[occurPtr]--;
                }

                currentLetter++;
                occurPtr++;
            }

            // Shuffle the array to make a new random order
            for (int i = 0; i < 500; i++)
            {
                int indexA = Random.Range(0, letterBagArray.Length);
                int indexB = Random.Range(0, letterBagArray.Length);

                if (indexA == indexB)
                {
                    i--;
                }
                else
                {
                    char temp = letterBagArray[indexA];
                    letterBagArray[indexA] = letterBagArray[indexB];
                    letterBagArray[indexB] = temp;
                }
            }

            // Reset the pointer
            letterBagPtr = 0;
        }

        return letterBagArray[letterBagPtr];
    }

    #endregion

    #region Coroutines

    public IEnumerator SwapTilesCoroutine(LetterTile tileA, LetterTile tileB)
    {
        if (boardLocked == false)
        {
            boardLocked = true;

            StartCoroutine(tileA.MoveCoroutine(tileB.RectTransform.anchoredPosition, tileB.GridPosition));
            StartCoroutine(tileB.MoveCoroutine(tileA.RectTransform.anchoredPosition, tileA.GridPosition));

            while (tileA.TileMoving)
            {
                yield return null;
            }

            boardLocked = false;
        }
    }

    #endregion

}
