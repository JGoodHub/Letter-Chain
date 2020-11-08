using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OxfordDictionaryService : MonoBehaviour
{

    public delegate string RecievedDefinition();


    IEnumerator Start()
    {
        UnityWebRequest request = new UnityWebRequest();

        request.method = UnityWebRequest.kHttpVerbGET;
        request.SetRequestHeader("app_id", "a116f725");
        request.SetRequestHeader("app_key", "35cad72fb1ecdfbc382ec350411613a8");
        request.url = "https://od-api.oxforddictionaries.com/api/v2/entries/en-gb/example";
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();


        while (request.isDone == false)
            yield return null;

        Debug.Log(request.downloadHandler.text);
    }



}
