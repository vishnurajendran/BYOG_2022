using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    const int leftOffset = -765;

    const string otherDudeLog = "{0}: {1}\n";
    const string myDudeLog = "{0}: {1}\n";

    [SerializeField] KeyFaker keyFaker;
    [SerializeField] HumanLikenessMeter liknessMeter;
    [SerializeField] TextAnimator textAnimator;
    [SerializeField] TMPro.TMP_Text otherText;
    [SerializeField] TMPro.TMP_InputField inputField;

    [SerializeField] HorizontalLayoutGroup layoutGroup;
    [SerializeField] TMPro.TMP_Text log;

    bool showLog;
    

    public System.Action<string> OnUserSubmittedAnswer;

    bool keyboardActive;

    Coroutine logOpenRoutine;

    private void Test()
    {
        AskQuestion("Is the world green? but I know that you are dumb :P");
    }

    // Start is called before the first frame update
    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
    }

    public void SetLikenessMeter(float perc)
    {
        liknessMeter.SetPerc(perc);
    }

    public void AskQuestion(string question)
    {
        
        otherText.text = "";
        textAnimator.AnimateText(otherText, question,0.05f, onComplete:ActivateInputField);
        AddToLog(string.Format(otherDudeLog, Application.platform, question));
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
        inputField.DeactivateInputField();
        keyFaker.enabled = false;
        keyboardActive = false;
        OnUserSubmittedAnswer?.Invoke(text);
        inputField.text = "";
        AddToLog(string.Format(myDudeLog, Application.platform, text));
        SetLikenessMeter(Random.Range(0, 1f));
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
