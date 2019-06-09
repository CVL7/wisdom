using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WisdomGenerator : MonoBehaviour
{

    public TextMeshProUGUI output;
    public TextAsset sourceText;

    string[] textArray;

    Dictionary<String, List<String>> dict = new Dictionary<string, List<string>>();
    List<String> startingWords = new List<String>();

    string GetRandom(List<string> arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }

    public void Generate()
    {
        var wisdom = "";
        var word = GetRandom(startingWords);
        while (true)
        {

            if (!(word.Length == 1 && Char.IsPunctuation(word[0]))) wisdom += " ";
            wisdom += word;
            word = GetRandom(dict[word]);
            if (word == ".") break;
        }
        wisdom = wisdom.Trim();
        wisdom = wisdom.First().ToString().ToUpper() + wisdom.Substring(1);
        wisdom += ".";

        if (wisdom.Length < 16) { Generate(); }
        else
        {
            output.text = wisdom;
        }
    }

    void Awake()
    {
        //textArray = sourceText.text.Split(new char[] { ' ', '.', ',', '!' }, StringSplitOptions.RemoveEmptyEntries);
        textArray = Regex.Split(sourceText.text, @"(?=[\\.,!;?: ])|(?<=[\\.,!;?: ])");
        textArray = textArray.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        for (var i = 0; i < textArray.Length - 1; i++)
        {
            var element = textArray[i].Trim().ToLower();
            var nextElement = textArray[i + 1].Trim().ToLower();

            if (char.IsUpper(textArray[i].Trim()[0])) startingWords.Add(element);

            List<string> value;
            dict.TryGetValue(element, out value);
            if (value != null)
            {
                value.Add(nextElement);
            }
            else
            {
                dict.Add(element, new List<string>() { nextElement });
            }
        }

        Generate();


        /*
                foreach (KeyValuePair<string, List<string>> kvp in dict)
                {
                    Debug.Log(kvp.Key + " " + kvp.Value.Count);
                }

                Debug.Log("Starting word:");
                foreach (var word in startingWords)
                {
                    Debug.Log(word);
                }
                */

    }

}
