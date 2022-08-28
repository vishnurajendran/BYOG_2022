using System.Collections.Generic;
using System;
using AI;

[Serializable]
public class Question
{
    public List<string> phrases;
    public string flawlessAnswer;
    public List<WeightedResponse> expectedKeyWords;
    public List<WeightedResponse> unexpectedKeyWords;

    public int questionThreadA;
    public int questionThreadB;
    public int questionThreadC;
    public int questionThreadD;

    public Oxford oxford;
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
public class WeightedResponse
{
    public float Weight;
    public List<string> forThisWeight;
}