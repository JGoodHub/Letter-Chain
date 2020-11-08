using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddTimeEffect : MonoBehaviour
{
    public Text timeText;

    public void Initalise(int timeAdded)
    {
        timeText.text = (timeAdded >= 0 ? "+" : "") + timeAdded;
        timeText.color = timeAdded >= 0 ? Color.green : Color.red;

        Destroy(gameObject, 2f);
    }
}
