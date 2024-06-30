using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringMatching : MonoBehaviour
{
    public static double similarityPercentage(string a, string b)
    {
        a = a.ToLower();
        b = b.ToLower();

        double equivalency = 0;
        double minLength = (a.Length > b.Length) ? b.Length : a.Length;
        double maxLength = (a.Length < b.Length) ? b.Length : a.Length;
        for (int i = 0; i < minLength; i++)
        {
            if (a[i] == b[i])
            {
                equivalency++;
            }
        }

        double weight = equivalency / maxLength;
        return weight;
    }

    public static string getSimilarFromList(string s, List<string> stringList)
    {
        int mostSimilarIndex=0;
        double mostSimilarPercentage=0.0;

        for (int i=0; i< stringList.Count; i++)
        {
            double ithSimilarPercentage = similarityPercentage(s, stringList[i]);
            if(ithSimilarPercentage > mostSimilarPercentage)
            {
                mostSimilarPercentage = ithSimilarPercentage;
                mostSimilarIndex = i;
            }
        }

        return stringList[mostSimilarIndex];
    }
}
