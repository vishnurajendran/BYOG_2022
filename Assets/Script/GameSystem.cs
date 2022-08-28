using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public List<Question> questionList;
    public List<string> failSafeResponseList;
    const int leftOffset = -765;

    public Oxford noxford;
    
    const string otherDudeLog = "{0}: {1}\n";
    const string myDudeLog = "{0}: {1}\n";

    [SerializeField] KeyFaker keyFaker;
    [SerializeField] HumanLikenessMeter liknessMeter;
    [SerializeField] TextAnimator textAnimator;
    [SerializeField] TMPro.TMP_Text otherText;
    [SerializeField] TMPro.TMP_InputField inputField;

    [SerializeField] HorizontalLayoutGroup layoutGroup;
    [SerializeField] TMPro.TMP_Text log;

    [SerializeField] GameObject continuePrompt;

    bool showLog;
    

    public System.Action<string> OnUserSubmittedAnswer;

    [SerializeField] bool keyboardActive;
    private int questionID;
    private int phraseID = -1;
    private Question currQuestion;

    private int currentThreadId;

    Coroutine logOpenRoutine;
    
    public void NextPhrase()
    {
        phraseID++;
        if (phraseID == currQuestion.phrases.Count)
        {
            NextQuestion();
        }
        
        ShowPhrase(currQuestion.phrases[phraseID]);
    }

    void NextQuestion() 
    {
        if (currentThreadId == 0)
        {
            questionID = currQuestion.questionThreadA;
        }
        else if (currentThreadId == 1)
        { 
            questionID = currQuestion.questionThreadB;
        }
        else if (currentThreadId == 2)
        { 
            questionID = currQuestion.questionThreadC;
        }
        else if (currentThreadId == 3)
        { 
            questionID = currQuestion.questionThreadD;
        }

        if (questionID < 0)
        { 
            //Game over
        }

        currQuestion = questionList[questionID];
        phraseID = 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        noxford.Init();
        questionList.ForEach(x => x.oxford = noxford);
        currQuestion = questionList[questionID];
        inputField.onSubmit.AddListener(OnSubmit);
        NextPhrase();
    }

    public void SetLikenessMeter(float perc)
    {
        liknessMeter.SetPerc(perc);
    }

    private void HideContinuePrompt()
    {
        continuePrompt.SetActive(false);
    }

    private void ShowContinuePrompt()
    {
        continuePrompt.SetActive(true);
    }

    void ShowPhrase(string phrase)
    {
        HideContinuePrompt();
        otherText.text = "";
        textAnimator.AnimateText(otherText, phrase,0.05f, phraseID == currQuestion.phrases.Count - 1 ? ActivateInputField : ShowContinuePrompt);
        AddToLog(string.Format(otherDudeLog, Application.platform, phrase));
    }

    private void ActivateInputField()
    {
        keyFaker.enabled = true;
        keyboardActive = true;
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }

    private void OnSubmit(string text)
    {
        var responeType = noxford.GetSimilarityIndex(text, currQuestion);
        Debug.Log($"Response type = {responeType}");
        currentThreadId = (int)responeType;
        inputField.DeactivateInputField();
        inputField.ReleaseSelection();
        if (!keyboardActive)
            return;
        keyFaker.enabled = false;
        keyboardActive = false;
        OnUserSubmittedAnswer?.Invoke(text);
        inputField.text = "";
        AddToLog(string.Format(myDudeLog, Application.platform, text));
        SetLikenessMeter(Random.Range(0, 1f));
        NextPhrase();
    }

    private void AddToLog(string str)
    {
        log.text += str + "\n";
        LayoutRebuilder.MarkLayoutForRebuild(log.GetComponent<RectTransform>());
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            if (keyboardActive)
                inputField.ActivateInputField();
        }

        if(!keyboardActive && inputField.IsActive())
        {
            inputField.DeactivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Opening Log");
            showLog = !showLog;
            if (logOpenRoutine != null)
                StopCoroutine(logOpenRoutine);

            logOpenRoutine = StartCoroutine(LerpLogPanel(showLog));
        }

        if (Input.GetKeyDown(KeyCode.E) && !keyboardActive)
        {
            NextPhrase();
        }
    }

    private IEnumerator LerpLogPanel(bool show)
    {
        float timeStep = 0;
        int leftPadding = layoutGroup.padding.left;
        float newVal = show ? leftOffset : 0;
        
        while(timeStep <= 1)
        {
            timeStep += Time.deltaTime / 0.15f;
            layoutGroup.padding.left = (int)Mathf.Lerp(leftPadding, newVal, timeStep);
            LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.GetComponent<RectTransform>());    
            yield return new WaitForEndOfFrame();
        }
    }
}
