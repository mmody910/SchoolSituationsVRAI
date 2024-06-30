using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Converters;

using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;


public class Survey : MonoBehaviour
{
    saveDataForSessionController dataController;

    public GameObject instruction;
    List<KeyValuePair<string, string>> questions = new List<KeyValuePair<string, string>>();
    [SerializeField]
    List<KeyValuePair<string, string>> questionsEng = new List<KeyValuePair<string, string>>();
    // List<string> questionsEng = new List<string>();
    [SerializeField]
    List<KeyValuePair<string, string>> questionsNor = new List<KeyValuePair<string, string>>();
    //List<string> questionsNor = new List<string>();
    Dictionary<string, int> survey;
    public TextMeshPro QuestionRenderer;
    public GameObject EndSurveyText;
    [SerializeField]
    private int currentQuestion;
    private bool Production = false; //Switch between production and staging
    private void Start()
    {
        AllQuestions p = Newtonsoft.Json.JsonConvert.DeserializeObject<AllQuestions>(PlayerPrefs.GetString("YatekQuestions"));


        dataController = GameObject.FindGameObjectWithTag("sessionController").GetComponent<saveDataForSessionController>();
        setQuestions(p);

        string language = PlayerPrefs.GetString("YatekLang");
        // language = "nb-NO";
        if (language == "en-US")
        {
            questions = questionsEng;
        }
        else
        {
            questions = questionsNor;
        }
        survey = new Dictionary<string, int>();
        currentQuestion = 0;
        QuestionRenderer.text = questions[currentQuestion].Value;

        foreach (Questions q in p.questions)
        {
            survey.Add(q.uid, 0);
        }



        // print(serializedPerson) ;
    }
    void setQuestions(AllQuestions p)
    {
        foreach (Questions q in p.questions)
        {
            questionsEng.Add(new KeyValuePair<string, string>(q.uid, q.question.en));
            questionsNor.Add(new KeyValuePair<string, string>(q.uid, q.question.nb));
        }
        // questionsEng = p.question.en;
        // questionsNor = p.question.nb;
    }

    public void RecordResponse(int value)
    {
        if (Done())
        {
            StartCoroutine(EndSurvey());
            return;
        }
        survey[questions[currentQuestion].Key] = value;
        currentQuestion++;
        if (!Done())
            QuestionRenderer.text = questions[currentQuestion].Value;
        else
        {
            StartCoroutine(EndSurvey());
        }
    }

    public bool Done()
    {
        return currentQuestion >= questions.Count;
    }
    public IEnumerator EndSurvey()
    {

        var serializedPerson = Newtonsoft.Json.JsonConvert.SerializeObject(survey);
        print("survey : " + serializedPerson);
        getData(survey);
        instruction.SetActive(false);
        EndSurveyText.SetActive(true);
        QuestionRenderer.enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        //load home scene
    }





    private IEnumerator getToken(long timestamp, int practiceTime, string scenarioId, Dictionary<string, int> rate)
    {

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        Task<string> t = user.TokenAsync(true);

        while (!t.IsCompleted)
            yield return new WaitForEndOfFrame();
        Debug.Log(" FirebaseID is " + t.Result);

        addRecord(timestamp, practiceTime, scenarioId, rate, t.Result);



    }
    public async Task addRecord(long timestamp, int practiceTime, string scenarioId, Dictionary<string, int> rate, string token)
    {

        patientRecord newRecord = new patientRecord();
        newRecord.rates = rate;
        newRecord.practiceTime = practiceTime;
        newRecord.scenarioId = scenarioId;
        newRecord.timestamp = timestamp;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newRecord);

        string apiUrl;

        if (Production) apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/updatePatientData";
        else apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/updatePatientData";

        using (UnityWebRequest www = UnityWebRequest.Put(apiUrl, json))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + token);

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
                // check if it fails
                SceneManager.LoadScene(0);
            }

        }



    }
    public long calculateTimeInMilliSecond()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
    async void getData(Dictionary<string, int> rates)
    {
        //  ft = dataController.calculateTimeInMilliSecond();
        int praTime = (int)(calculateTimeInMilliSecond() - dataController.st);
        praTime /= 1000;

        StartCoroutine(getToken(dataController.st, praTime, dataController.scenarioId, rates));



    }

}