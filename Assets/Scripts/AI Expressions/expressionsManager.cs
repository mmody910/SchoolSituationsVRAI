using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public class expressionsManager : MonoBehaviour
{
    public TextAsset jsonFile;
    string prompt = "Classify the emotion that a listener would have to the in response to the following sentences.";
    [HideInInspector] public expressions expressions;
    [HideInInspector] public SkinnedMeshRenderer skinnedMeshRenderer;
    [HideInInspector] public Mesh skinnedMesh;
    [HideInInspector] public List<string> expressionsNamesList;
    private bool expressionPlaying = false;
    private void Awake()
    {
        expressions = JsonUtility.FromJson<expressions>(jsonFile.text);
        skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;

        foreach (expression exp in expressions.expressionsList)
        {
            if (exp.expressionName != "EyesLookUp" || exp.expressionName != "EyesLookDown" || exp.expressionName != "EyesLookRight" || exp.expressionName != "EyesLookLeft")
            {
                expressionsNamesList.Add(exp.expressionName);
            }
        }
    }

    void Start()
    {
        //react("I'm happy for you!");
    }

    public async Task react(string sentence)
    {
        // Cancel if an expression is currently running
        if(expressionPlaying) return;

        //  print(sentence);
        string expressionName = "";
        string expressionCombinedString = string.Join(" , ", expressionsNamesList.ToArray());

        // creating the prompt to output expression
        prompt = "Valid emotions are: " + expressionCombinedString + "\n";
        prompt += "So I went to the store and this crazy person was just running through the aisles. Emotion: Surprise. \n"
                + "I'm not so happy today, its been difficult lately. Emotion: Sadness. \n"
                + "So we are getting married! Emotion: HappySurprise. \n";

        prompt += "\n" + sentence;
        prompt += "\n Emotion:";

        //  Debug.Log(prompt);

        openAiExpressionData openAIexpression = new openAiExpressionData();
        openAIexpression.prompt = prompt;

        openAIexpression.user = PlayerPrefs.GetString("UserId");
        string expressionJson = JsonUtility.ToJson(openAIexpression);

        // to generate expression 
        using (UnityWebRequest www = UnityWebRequest.Put("https://api.openai.com/v1/engines/text-curie-001/completions", expressionJson))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + LoadKeys.OPEN_AI_API_KEY.ToString());

            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //   print(www.downloadHandler.text);
                openAiExpressionResponse openAIExpresionresponse = JsonUtility.FromJson<openAiExpressionResponse>(www.downloadHandler.text);
                // print(openAIExpresionresponse.choices.Length);
                if (openAIExpresionresponse.choices.Length > 0)
                {
                    expressionName = openAIExpresionresponse.choices[0].text;
                    if (expressionName.Contains(":"))
                    {
                        string[] res = Regex.Split(expressionName, "Emotion:");

                        if (res.Length == 2)
                        {
                            expressionName = res[1];
                        }
                        res[0] = res[0].Replace("\n", "");
                    }
                }

                expressionName = expressionName.Replace("\n", "");
                expressionName = expressionName.Replace(".", "");
                expressionName = expressionName.Replace(" ", "");
                //     print(expressionName);
            }
        }
        StartCoroutine(playExpression(expressionName));
    }

    public IEnumerator playExpression(string expressionName, bool sync = false)
    {
        int expressionIndex = getExpressionIndex(expressionName);

        if (expressionIndex != -1 && expressionPlaying != true)
        {
            // Detect if an expression started playing except synced eye expressions
            if(sync == false)
            {
                expressionPlaying = true;
            }

            expression exp = expressions.expressionsList[expressionIndex];
            float randomPlaySpeed = Random.Range(exp.minSpeed, exp.maxSpeed);
            float randomFreezeTime = Random.Range(exp.minFreezeTime, exp.maxFreezeTime);
            int randomValue = Random.Range(exp.minValue, exp.maxValue);

            // Play the expression by changing all the blendshape values for this expression
            foreach (blendShape bs in expressions.expressionsList[expressionIndex].blendShapes)
            {
                // don't sync all blendshape values (is true for eyes)
                if (sync == false)
                {
                    randomValue = Random.Range(bs.minValue, bs.maxValue);
                }
                StartCoroutine(changeBlendShape(bs.blendShapeName, randomValue, randomPlaySpeed));
            }

            yield return new WaitForSeconds(randomPlaySpeed + randomFreezeTime);

            // Change all blendshape values back to zero
            foreach (blendShape bs in expressions.expressionsList[expressionIndex].blendShapes)
            {
                StartCoroutine(changeBlendShape(bs.blendShapeName, 0f, randomPlaySpeed));
            }
            yield return new WaitForSeconds(randomPlaySpeed);

            // Return the expressionPlaying Bool to false when the animation ends, 
            // unless it's an eye expression it won't pass through this condition cause eyes are synced
            if(sync == false)
            {
                expressionPlaying = false;
            }
        }
    }

    int getExpressionIndex(string expressionName)
    {
        // Get Index of the expression we want to play from the expressionsList
        int expressionIndex = -1;
        for (int i = 0; i < expressions.expressionsList.Count; i++)
        {
            if (expressionName.Equals(expressions.expressionsList[i].expressionName))
            {
                expressionIndex = i;
                break;
            }
        }

        // If Index is -1 then expressionName is not found in the expressionsList
        if (expressionIndex == -1)
        {
            Debug.LogWarning(expressionName + " is not found in expressionsList!");
        }

        return expressionIndex;
    }
    IEnumerator changeBlendShape(string blendShapeName, float value, float blendTimeInSeconds)
    {
        // Index of given blendshape name
        int blendShapeIndex = skinnedMesh.GetBlendShapeIndex(blendShapeName);
        // Current blendshape value
        int currentValue = (int)skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
        // Calculating blend time in seconds
        float secondsToWait = blendTimeInSeconds / (Mathf.Abs(value - currentValue) / 5);

        while (currentValue++ < value)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, currentValue);
            if (currentValue % 5 == 0)
                yield return new WaitForSeconds(secondsToWait);
        }
        while (currentValue-- > value)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, currentValue);
            if (currentValue % 5 == 0)
                yield return new WaitForSeconds(secondsToWait);
        }
    }
}

[System.Serializable]
public class expressions
{
    public List<expression> expressionsList;
}

[System.Serializable]
public class expression
{
    public string expressionName;
    public List<blendShape> blendShapes;
    public int minValue = 0, maxValue = 100;
    public float minSpeed = 0.1f, maxSpeed = 0.5f;
    public float minFreezeTime = 0.2f, maxFreezeTime = 3f;
}

[System.Serializable]
public class blendShape
{
    public string blendShapeName;
    public int minValue = 0;
    public int maxValue = 100;
}