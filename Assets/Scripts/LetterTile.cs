using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterTile : MonoBehaviour {

    //-----VARIABLES-----

    public char Letter { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public bool IsSelected { get; private set; }

    [Header("Image Effects")]
    [SerializeField] private Image letterImage;
    [SerializeField] private Image glowImage;

    [Header("Movement Parameters")]
    [SerializeField] private float tileMoveSpeed;
    public bool TileMoving { get; private set; }

    //Cached references
    public RectTransform RectTransform { get; private set; }

    #region Inherited Methods

    public override string ToString() {
        return "Tile " + Letter + ", " + base.ToString();
    }

    #endregion

    #region Public Methods

    public void Initialise(char _tileLetter, Vector2Int _gridPosition) {
        Letter = char.ToLower(_tileLetter);
        GridPosition = _gridPosition;
        RectTransform = (RectTransform)transform;

        letterImage.sprite = TileController.Instance.GetAlphabetSprite(_tileLetter);
        SetSelected(false);
    }

    public void SetSelected(bool state) {
        if (state == true) {
            glowImage.enabled = true;
            IsSelected = true;
        } else {
            glowImage.enabled = false;
            IsSelected = false;
        }
    }

    #endregion

    #region Event Handlers
    
    public void OnTileDown() {
        InteractionController.Instance.StartWordChain(this);
    }

    public void OnTileEnter() {
        InteractionController.Instance.AddToWordChain(this);
    }

    public void OnTileUp () {
        InteractionController.Instance.FinishWordChain();
    }

    public void OnTileClicked() {
        //SetSelected(!IsSelected);
        InteractionController.Instance.TileClicked(this);
    }

    #endregion

    #region Coroutines

    public IEnumerator SpawnCoroutine (float delay) {
        RectTransform.localScale = Vector3.zero;
        yield return new WaitForSeconds(delay);

        while (RectTransform.localScale != Vector3.one) {
            RectTransform.localScale = Vector3.MoveTowards(RectTransform.localScale, Vector3.one, 8f * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator MoveCoroutine(Vector2 targetAnchoredPosition, Vector2Int targetGridPosition) {
        TileMoving = true;

        yield return null;

        while (RectTransform.anchoredPosition.Equals(targetAnchoredPosition) == false) {
            RectTransform.anchoredPosition = Vector2.MoveTowards(RectTransform.anchoredPosition, targetAnchoredPosition, tileMoveSpeed * Time.deltaTime);
            yield return null;
        }

        GridPosition = targetGridPosition;

        TileMoving = false;
    }

    public IEnumerator DestroyCoroutine(float delay) {
        yield return new WaitForSeconds(delay);

        while (RectTransform.localScale != Vector3.zero) {
            RectTransform.localScale = Vector3.MoveTowards(RectTransform.localScale, Vector3.zero, 8f * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion

}
