using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
    public delegate void ActionTaken();

    public event ActionTaken OnConfirmed;
    public event ActionTaken OnCancelled;

    public float closeAfter = 0f;

    [SerializeField] private Text messageText;
    public string Message
    {
        get
        {
            return messageText.text;
        }
        set
        {
            messageText.text = value;
        }
    }

    public void Confirm()
    {
        OnConfirmed?.Invoke();
        Destroy(gameObject, closeAfter);
    }

    public void Cancel()
    {
        OnCancelled?.Invoke();
        Destroy(gameObject, closeAfter);
    }

}
