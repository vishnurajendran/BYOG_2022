using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    const string otherDudeLog = "{0}:{1}";
    const string myDudeLog = "{0}:{1}";

    [SerializeField] KeyFaker keyFaker;
    [SerializeField] HumanLikenessMeter liknessMeter;
    [SerializeField] TextAnimator textAnimator;
    [SerializeField] TMPro.TMP_Text otherText;
    [SerializeField] TMPro.TMP_InputField inputField;

    string log;

    public System.Action<string> OnUserSubmittedAnswer;

    bool keyboardActive;

    private void Test()
    {
        AskQuestion("Is the world green? but I know that you are dumb :P");
    }

    // Start is called before the first frame update
    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
        Test();
    }

    public void SetLikenessMeter(float perc)
    {
        liknessMeter.SetPerc(perc);
    }

    public void AskQuestion(string question)
    {
        otherText.text = "";
        textAnimator.AnimateText(otherText, question,0.05f, onComplete:ActivateInputField);
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

        SetLikenessMeter(Random.Range(0, 1f));
        Test();
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
    }
}
