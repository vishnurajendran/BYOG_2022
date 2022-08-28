using System.Collections.Generic;
using System;
using AI;
using UnityEngine;

[Serializable]
public class Question
{
    public List<string> phrases;
    public List<WeightedResponse> expectedKeyWords;
    public List<WeightedResponse> unexpectedKeyWords;

    public int questionThreadA;
    public int questionThreadB;
    public int questionThreadC;
    public int questionThreadD;

    [HideInInspector]public Oxford oxford;

    private void ToLowerCase()
    {
        if (expectedKeyWords != null)
        {
            foreach (var weightSet in expectedKeyWords)
            {
                if (weightSet.forThisWeight != null)
                {
                    for (var i = 0; i < weightSet.forThisWeight.Count; i++)
                    {
                        weightSet.forThisWeight[i] = weightSet.forThisWeight[i].ToLower();
                    }
                }
            }
        }

        if (unexpectedKeyWords != null)
        {
            foreach (var weightSet in unexpectedKeyWords)
            {
                if (weightSet.forThisWeight != null)
                {
                    for (var i = 0; i < weightSet.forThisWeight.Count; i++)
                    {
                        weightSet.forThisWeight[i] = weightSet.forThisWeight[i].ToLower();
                    }
                }
            }
        }
    }
    private void OnValidate()
    {
        ToLowerCase();
    }
    public void Init(Oxford op)
    {
        oxford = op;
        List<string> temp = new();
        foreach (var weightedResponse in expectedKeyWords)
        {
            foreach (var word in weightedResponse.forThisWeight)
            {
                var syns = op.GetSynonyms(word);
                foreach (var possibleSyn in syns)
                {
                    if (!weightedResponse.forThisWeight.Contains(possibleSyn))
                    {
                        temp.Add(possibleSyn);
                    }
                }
            }
            weightedResponse.forThisWeight.AddRange(temp);
        }

        temp = new();
        foreach (var weightedResponse in unexpectedKeyWords)
        {
            foreach (var word in weightedResponse.forThisWeight)
            {
                var syns = op.GetSynonyms(word);
                foreach (var possibleSyn in syns)
                {
                    if (!weightedResponse.forThisWeight.Contains(possibleSyn))
                    {
                        temp.Add(possibleSyn);
                    }
                }
            }
            weightedResponse.forThisWeight.AddRange(temp);
        }
        
        ToLowerCase();
    }
    public bool DoesItExistInExpected(string wordToCheck, out float weight, out bool isGibberish)
    {
        //direct check
        foreach (var matchingSet in expectedKeyWords)
        {
            foreach (var matchingWord in matchingSet.forThisWeight)
            {
                if (Extensions.CalculateSimilarity(matchingWord,wordToCheck))
                {
                    weight = matchingSet.Weight;
                    isGibberish = false;
                    return true;
                }
            }
        }

        //direct check
        foreach (var matchingSet in unexpectedKeyWords)
        {
            foreach (var matchingWord in matchingSet.forThisWeight)
            {
                if (Extensions.CalculateSimilarity(matchingWord,wordToCheck))
                {
                    weight = -1 * matchingSet.Weight;
                    isGibberish = false;
                    return true;
                }
            }
        }
        
        //indirect check of synonyms in expected
        var synonyms = oxford.GetSynonyms(wordToCheck);
        foreach (var wordToCheckKaSynonym in synonyms)
        {
            foreach (var matchingSet in expectedKeyWords)
            {
                foreach (var matchingWord in matchingSet.forThisWeight)
                {
                    if (Extensions.CalculateSimilarity(matchingWord,wordToCheckKaSynonym))
                    {
                        weight = matchingSet.Weight;
                        isGibberish = false;
                        return true;
                    }
                }
            }
        }

        
        //indirect check of synonyms in unexpected
        foreach (var wordToCheckKaSynonym in synonyms)
        {
            foreach (var matchingSet in unexpectedKeyWords)
            {
                foreach (var matchingWord in matchingSet.forThisWeight)
                {
                    if (Extensions.CalculateSimilarity(matchingWord,wordToCheck))
                    {
                        weight = -1 * matchingSet.Weight;
                        isGibberish = false;
                        return true;
                    }
                }
            }
        }
        
        
        //gibberish
        weight = -0.1f;
        isGibberish = true; 
        return false;
    }
}
[Serializable]
public class WeightedResponse
{
    public float Weight;
    public List<string> forThisWeight;
}