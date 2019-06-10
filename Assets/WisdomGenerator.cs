using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WisdomGenerator : MonoBehaviour
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
        corpus = Array.ConvertAll(corpus, x => x.ToLower());

        var order = 3;
        Learn(order);

        GenerateAndShow();
    }

    public void GenerateAndShow()
    {
        var wisdomString = "";
        while (string.IsNullOrEmpty(wisdomString))
        {
            wisdomString = ProcessGenerated(InternalGenerate());
        }

        output.text = wisdomString;
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
    }

    List<string> InternalGenerate()
    {
        var wisdom = new List<string>();
        var keyWords = GetRandom(startingWords);
        wisdom.AddRange(keyWords.Split(' '));
        while (true)
        {
            var valueWord = GetRandom(model[keyWords]);

            // If we got a token that is already in sentence -> try decrease its chance and roll again
            if (wisdom.Contains(valueWord))
            {
                var arrCopy = new List<string>(model[keyWords]);
                var occurences = arrCopy.Where(x => x == valueWord).Count();
                var removeCount = occurences > 1 ? occurences / 2 : 0;
                for (var i = 0; i < removeCount; i++) arrCopy.Remove(valueWord);
                valueWord = GetRandom(arrCopy);
            }

            wisdom.Add(valueWord);

            if (valueWord == "." || valueWord == "!") break;

            if (keyWords.IndexOf(' ') != -1) keyWords = keyWords.Substring(keyWords.IndexOf(' ') + 1) + " " + valueWord;
            else keyWords = valueWord;
        }

        return wisdom;
    }

    string ProcessGenerated(List<string> wisdom)
    {
        // Check if too short or too long
        if (wisdom.Count < averageLineSize - 1 || wisdom.Count > averageLineSize + 6)
        {
            InternalGenerate();
            return "";
        }

        // Compose a string
        var wisdomString = string.Join(" ", wisdom).Trim();
        wisdomString = Regex.Replace(wisdomString, @"\s+(?=[.,?!:;])", "");

        // Check if already exists
        if (Array.IndexOf(corpus, wisdomString) >= 0)
        {
            InternalGenerate();
            return "";
        }

        // Format and output
        wisdomString = wisdomString.First().ToString().ToUpper() + wisdomString.Substring(1);
        return wisdomString;
    }

    string GetRandom(List<string> arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Count)];
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
}
