using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace AI
{
    [Serializable]
    public class SynonymList
    {
        public List<string> synonyms;
    }

    public enum ResponeType
    {
        Flawless,
        Flawed,
        NegativeResponse,
        Gibberish
    }
    
    [CreateAssetMenu(fileName = "Noxford", menuName = "BYOG/Noxford", order = 0)]
    public class Oxford : ScriptableObject
    {
        public string[] allWordsForNo;
        public string[] allWordsForYes;
        public string[] allWordsForPossibility;
        public string[] articles;
        public string[] adverbs;

        private Dictionary<string, List<string>> synonymDictionary;

        public void Init()
        {
            synonymDictionary = new Dictionary<string, List<string>>();
            var db = Resources.FindObjectsOfTypeAll<TextAsset>();
            foreach (var textAsset in db)
            {
                var word = textAsset.name;
                var text = textAsset.text;

                var synonyms = JsonConvert.DeserializeObject<SynonymList>(text);
                synonymDictionary.Add(word, synonyms.synonyms);
            }
        }

        public string[] GetSynonyms(string word)
        {
            if (synonymDictionary.ContainsKey(word))
            {
                return synonymDictionary[word].ToArray();
            }

            var keys = synonymDictionary.Keys.ToList();
            foreach (var key in keys)
            {
                if (Extensions.CalculateSimilarity(key, word))
                {
                    return synonymDictionary[key].ToArray();
                }
            }

            return new string[] { };
        }
        
        public ResponeType GetSimilarityIndex(string receivedAnswer, Question expectedPara)
        {
            if (receivedAnswer == expectedPara.flawlessAnswer)
            {
                return ResponeType.Flawless;
            }

            var recWords = receivedAnswer.Split(" ").ToList();

            float similarityScore = 0;
            foreach (var wordInAns in recWords)
            {
                if (expectedPara.DoesItExistInExpected(wordInAns, out float weightToAdd, out bool isGibberish))
                {
                    similarityScore += weightToAdd;
                }
            }

            switch (similarityScore)
            {
                case >= 0.8f:
                    return ResponeType.Flawless;
                case >= 0.5f:
                    return ResponeType.Flawed;
                case >= 0:
                    return ResponeType.Gibberish;
                default:
                    return ResponeType.NegativeResponse;
            }
        }

        public bool GetWhetherSentenceIsNegativeOrPositive(string toCheckSentence)
        {
            var splitWords = toCheckSentence.Split(" ");
            bool isNegativeSentence = false;
            foreach (var word in splitWords)
            {
                foreach (var wordForNo in allWordsForNo)
                {
                    if (Extensions.CalculateSimilarity(word, wordForNo))
                    {
                        isNegativeSentence = !isNegativeSentence;
                        break;
                    }
                }
            }
            return isNegativeSentence;
        }
    }


    public static class Extensions
    {
        // public static string[] GetSentencesFromParagraph(this string str, out List<string> splitWords)
        // {
        //     var sentences = str.Split(".");
        //     splitWords = new List<string>();
        //     foreach (var sentence in sentences)
        //     {
        //         splitWords.AddRange(sentence.Split(" "));
        //     }
        //     return sentences;
        // }
        
        // public static string RemoveSpecialCharacters(this string str) 
        // {
        //     StringBuilder sb = new StringBuilder();
        //     foreach (char c in str) {
        //         if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') 
        //         {
        //             sb.Append(c);
        //         }
        //     }
        //     return sb.ToString();
        // }
        //
        private static int LevenshteinDistance(string source, string target)
        {
            // degenerate cases
            if (source == target) return 0;
            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[target.Length + 1];
            int[] v1 = new int[target.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < source.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < target.Length; j++)
                {
                    var cost = (source[i] == target[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[target.Length];
        }

        //TODO :: Remove calls for this function later
        public static bool CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return false;
            if ((source.Length == 0) || (target.Length == 0)) return false;
            if (source == target) return true;

            int stepsToSame = LevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length))) > 0.8f;
        }
    }
}