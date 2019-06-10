using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WisdomGeneratorRefactored : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TextAsset sourceText;

    string[] corpus;
    Dictionary<string, List<string>> model = new Dictionary<string, List<string>>();
    List<string> startingWords = new List<string>();
    int averageLineSize;

    void Awake()
    {
        corpus = sourceText.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var order = 3;
        Learn(order);
        Generate();
    }

    string GetRandom(List<string> arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }

    public void Generate()
    {
        var wisdom = new List<string>();

        var keyWords = GetRandom(startingWords);
        wisdom.Add(keyWords);

        while (true)
        {
            // Debug.Log(keyWords);
            var valueWord = GetRandom(model[keyWords]);

            if (wisdom.Contains(valueWord))
            {
                var arrCopy = new List<string>(model[keyWords]);
                var occurences = arrCopy.Where(x => x == valueWord).Count();
                var removeCount = occurences > 1 ? occurences / 2 : 0;
                for (var i = 0; i < removeCount; i++)
                {
                    //Debug.Log("Removing " + word);
                    arrCopy.Remove(valueWord);
                }
                //   Debug.Log("Array had " + occurences + " occurences of word " + word + " and now have only " + arrCopy.Where(x => x == word).Count());

                valueWord = GetRandom(arrCopy);
            }

            wisdom.Add(valueWord);
            if (valueWord == "." || valueWord == "!") break;

            if (keyWords.IndexOf(' ') != -1)
            {
                keyWords = keyWords.Substring(keyWords.IndexOf(' ') + 1) + " " + valueWord;
            }
            else
            {
                keyWords = valueWord;
            }

        }

        if ((wisdom.Count < (averageLineSize - 1)) || (wisdom.Count > (averageLineSize + 5)))
        {
            Generate();
            return;
        }

        var wisdomString = string.Join(" ", wisdom).Trim();
        wisdomString = Regex.Replace(wisdomString, @"\s+(?=[.,?!:;])", "");
        wisdomString = wisdomString.First().ToString().ToUpper() + wisdomString.Substring(1);

        if (Array.IndexOf(corpus, wisdomString) >= 0)
        {
            Generate();
            return;
        }

        output.text = wisdomString;
    }

    string ConcatKey(int pos, int size, string[] arr, StringBuilder sb)
    {
        sb.Clear();
        for (int i = pos; i < pos + size - 1; i++)
        {
            sb.Append(arr[i]);
            sb.Append(" ");
        }
        sb.Append(arr[pos + size - 1]);
        return sb.ToString();
    }

    void Learn(int order)
    {
        var sb = new StringBuilder();
        foreach (var line in corpus)
        {
            var tokens = Regex
                .Split(line, @"(?=[.,!;?: ])|(?<=[.,!;?: ])")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLower())
                .ToArray();

            averageLineSize += tokens.Length;

            if (order >= tokens.Length) continue;

            var startKey = ConcatKey(0, order, tokens, sb);
            startingWords.Add(startKey);

            for (var i = 0; i < tokens.Length - order; i++)
            {
                var key = ConcatKey(i, order, tokens, sb);
                var value = tokens[i + order];

                List<string> tryValue;
                model.TryGetValue(key, out tryValue);
                if (tryValue != null)
                {
                    tryValue.Add(value);
                }
                else
                {
                    model.Add(key, new List<string>() { value });
                }
            }
        }

        averageLineSize /= corpus.Length;
        Debug.Log("Average size: " + averageLineSize);
    }



}
