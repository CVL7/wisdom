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

    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
    List<string> startingWords = new List<string>();
    int averageLineSize;

    string GetRandom(List<string> arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }

    public void Generate()
    {
        var wisdom = new List<string>();
        var word = GetRandom(startingWords);
        wisdom.Add(word);

        while (true)
        {
            var previousWord = word;
            word = GetRandom(dict[word]);

            if (wisdom.Contains(word))
            {
                var arrCopy = new List<string>(dict[previousWord]);
                var occurences = arrCopy.Where(x => x == word).Count();
                var removeCount = occurences > 1 ? occurences / 2 : 0;
                for (var i = 0; i < removeCount; i++)
                {
                    Debug.Log("Removing " + word);
                    arrCopy.Remove(word);
                }
                Debug.Log("Array had " + occurences + " occurences of word " + word + " and now have only " + arrCopy.Where(x => x == word).Count());

                word = GetRandom(arrCopy);
            }

            wisdom.Add(word);

            if (word == "." || word == "!")
            {
                break;
            }

            word = previousWord.Split(' ')[1] + " " + word;
        }

        if ((wisdom.Count < (averageLineSize - 1)) || (wisdom.Count > (averageLineSize + 5)))
        {
            Debug.Log("Too long: " + wisdom.Count + " REGENERATING");
            // Generate();
            // return;
        }

        var wisdomString = "";
        foreach (var part in wisdom)
        {
            var love = part.Length == 1 && Char.IsPunctuation(part[0]);
            if (!love || part == "—") wisdomString += " ";
            wisdomString += part;
        }
        wisdomString = wisdomString.Trim();
        wisdomString = wisdomString.First().ToString().ToUpper() + wisdomString.Substring(1);

        output.text = wisdomString;
    }

    void Learn2()
    {
        var lines = sourceText.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            var textArray = Regex.Split(line, @"(?=[.,!;?: ])|(?<=[.,!;?: ])");
            textArray = textArray.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            averageLineSize += textArray.Length;

            if (char.IsUpper(textArray[0].Trim()[0])) startingWords.Add(textArray[0].Trim().ToLower() + " " + textArray[1].Trim().ToLower());

            for (var i = 0; i < textArray.Length - 2; i++)
            {
                var first = textArray[i].Trim().ToLower();
                var second = textArray[i + 1].Trim().ToLower();
                var key = first + " " + second;
                var third = textArray[i + 2].Trim().ToLower();

                List<string> value;
                dict.TryGetValue(key, out value);
                if (value != null)
                {
                    //Debug.Log(third);
                    value.Add(third);
                }
                else
                {
                    dict.Add(key, new List<string>() { third });
                    //Debug.Log(key);
                }
            }
        }


        foreach (KeyValuePair<string, List<string>> kvp in dict)
        {
            Debug.Log(kvp.Key + " " + kvp.Value.Count);
        }

        averageLineSize /= lines.Length;
        Debug.Log("Average size: " + averageLineSize);
    }

    void Learn()
    {
        var lines = sourceText.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            var textArray = Regex.Split(line, @"(?=[.,!;?: ])|(?<=[.,!;?: ])");
            textArray = textArray.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            averageLineSize += textArray.Length;

            if (char.IsUpper(textArray[0].Trim()[0])) startingWords.Add(textArray[0].Trim().ToLower());

            for (var i = 0; i < textArray.Length - 1; i++)
            {
                var element = textArray[i].Trim().ToLower();
                var nextElement = textArray[i + 1].Trim().ToLower();

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
        }

        averageLineSize /= lines.Length;
        Debug.Log("Average size: " + averageLineSize);
    }

    void Awake()
    {
        Learn2();
        Generate();
        /* 
        var textArray = Regex.Split(sourceText.text, @"(?=[.,!;?: ])|(?<=[.,!;?: ])");
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
        }*/
    }

}
