using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodHub.Core;
using Random = UnityEngine.Random;

public class DictionaryManager : Singleton<DictionaryManager>
{

    [Header("Words and Scores")]
    [SerializeField] private TextAsset dictionaryFile;
    [SerializeField] private int[] letterScores = new int[26];

    private Dictionary<string, string> dict;

    #region Inherited Methods

    private void Start()
    {
        dict = ReadDictionaryFileToMemory(dictionaryFile);
    }

    #endregion

    #region Public Methods

    public bool ContainsWord(string word)
    {
        return dict.ContainsKey(word.ToLower());
    }

    public int GetWordScore(string word)
    {
        int wordScore = 0;
        foreach (char letter in word.ToLower())
        {
            wordScore += letterScores[GetAlphabetIndex(letter)];
        }

        return wordScore;
    }

    public string GetWordDefinition(string word)
    {
        if (ContainsWord(word))
        {
            return dict[word.ToLower()];
        }
        else
        {
            return "???";
        }
    }

    public int GetAlphabetIndex(char letter)
    {
        byte letterByte = (byte)letter;
        if (letterByte >= 97 && letterByte <= 122)
        {
            return letterByte - 97;
        }
        else
        {
            Debug.LogError($"Error: Attempting to convert non alphabetic letter {(byte)letter}");
            return 0;
        }
    }

    #endregion

    #region Private Methods

    private Dictionary<string, string> ReadDictionaryFileToMemory(TextAsset textFile)
    {
        string[] definitions = textFile.text.Split('\n');
        dict = new Dictionary<string, string>(definitions.Length / 2);

        for (int i = 0; i < definitions.Length - 1; i += 2)
        {
            string word = definitions[i].ToLower().Trim().Trim('\n', '\r');
            string def = definitions[i + 1].ToLower().Trim().Trim('\n', '\r');

            if (dict.ContainsKey(word) == false)
            {
                dict.Add(word, def);
            }
        }

        return dict;
    }

    #endregion

}

