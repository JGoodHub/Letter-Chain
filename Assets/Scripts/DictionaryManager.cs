using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodHub.Core;
using Random = UnityEngine.Random;

public class DictionaryManager : Singleton<DictionaryManager> {

    [Header("Words and Scores")]
    [SerializeField] private TextAsset dictionaryFile;
    [SerializeField] private int[] letterScores = new int[26];

    private Dictionary<string, string> dict;

    private static List<char> vowels = new List<char>(new char[] { 'a', 'e', 'i', 'o', 'u' });

    #region Inherited Methods

    private void Start() {
        dict = ReadInteralDictionaryFileToMemory(dictionaryFile);
    }

    #endregion

    #region Public Methods

    public bool ContainsWord(string word) {
        return dict.ContainsKey(word.ToLower());
    }

    public int GetWordScore(string word) {
        int wordScore = 0;
        foreach (char letter in word.ToLower()) {
            wordScore += letterScores[GetAlphabetIndex(letter)];
        }

        return wordScore;
    }

    public string GetWordDefinition(string word) {
        if (ContainsWord(word)) {
            return dict[word.ToLower()];
        } else {
            return "???";
        }
    }

    public int GetAlphabetIndex(char letter) {
        byte letterByte = (byte)letter;
        if (letterByte >= 97 && letterByte <= 122) {
            return letterByte - 97;
        } else {
            Debug.LogError("ERROR: Attempting to convert non alphabetic letter" + (byte)letter);
            return 0;
        }
    }


    public char[] GetRandomLetterArray(int size, int minVowelCount) {
        char[] letterArray = new char[size];
        int vowelCount = 0;

        //Randomly generate a list of letters
        for (int n = 0; n < size; n++) {
            char letter = GetRandomLetter();
            vowelCount = vowels.Contains(letter) ? vowelCount + 1 : vowelCount;
            letterArray[n] = letter;
        }

        //Randomly insert vowels until the minimum limit is reached
        while (vowelCount < minVowelCount) {
            int index = Random.Range(0, letterArray.Length);
            if (vowels.Contains(letterArray[index]) == false) {
                char vowel = GetRandomVowel();
                vowelCount++;
                letterArray[index] = vowel;
            }
        }

        return letterArray;
    }

    #endregion

    #region Private Methods

    private Dictionary<string, string> ReadInteralDictionaryFileToMemory(TextAsset textFile) {
        string[] definitions = textFile.text.Split('\n');
        dict = new Dictionary<string, string>(definitions.Length / 2);

        for (int i = 0; i < definitions.Length - 1; i += 2) {
            string word = definitions[i].ToLower().Trim().Trim('\n', '\t', '\r');
            string def = definitions[i + 1].ToLower().Trim().Trim('\n', '\t', '\r');

            if (dict.ContainsKey(word) == false) {
                dict.Add(word, def);
            } else {
                Debug.Log(i + 1);
                Debug.Log(word);
                Debug.Log(def);
            }
        }

        return dict;
    }

    private char GetRandomLetter() {
        char letter = (char)(97 + Random.Range(0, 26));
        return letter;
    }

    private char GetRandomVowel() {
        int vowelPtr = Random.Range(0, vowels.Count);
        return vowels[vowelPtr];
    }

    #endregion

}

