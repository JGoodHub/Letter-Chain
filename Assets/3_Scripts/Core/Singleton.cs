using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodHub.Core
{

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"Error: No singleton of {typeof(T)} found, creating one instead");

                        //Create a new gameobject to store the singleton and get it up to speed
                        GameObject singletonObject = new GameObject($"{typeof(T)}_SINGLETON");
                        _instance = singletonObject.AddComponent<T>();
                        _instance.BroadcastMessage("Awake");
                        _instance.BroadcastMessage("Start");

                    }
                    else if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debug.LogError($"Error: More than one singleton of {typeof(T)} found, this is not how a SINGLEton works");

                        return _instance;
                    }
                    else
                    {
                        return _instance;
                    }
                }

                return _instance;
            }
        }

        [Header("Singleton Settings")]
        [SerializeField] private bool dontDestroy;

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = Instance;

                _instance.gameObject.name = $"{typeof(T)}_SINGLETON";

                //Debug.Log($"Singleton {typeof(T)} Instance Set");

                if (dontDestroy)
                {
                    _instance.transform.parent = null;
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            else
            {
                //If the instance is already set then this version of the singleton should be destroyed to maintain the pattern
                Destroy(gameObject);
            }
        }
    }
}