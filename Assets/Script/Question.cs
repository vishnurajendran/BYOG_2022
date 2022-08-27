using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Question 
{
    public List<string> phrases;
    public List<string> expectedKeyWords;
    public List<string> typeAResponses;
    public List<string> typeBResponses;
    public List<string> typeCResponses;
    public List<string> typeDResponses;
}