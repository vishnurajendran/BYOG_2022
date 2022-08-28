using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndController : MonoBehaviour
{
    const string ENDING_TYPE = "ENDING_TYPE";
    const string ENDING_BAD_SUMMARY = "ENDING_BAD_SUMMARY";
    const string ENDING_GOOD_DIALOGUE = "ENDNING_GOOD_DIALOGUE";

    [SerializeField] Transform analysingPanel;
    [SerializeField] TMPro.TMP_Text analyseText;
    [SerializeField] Image progress;

    [SerializeField] Transform goodEndigPanel;
    [SerializeField] TMPro.TMP_Text goodEndingText;

    [SerializeField] Transform badEndingPanel;
    [SerializeField] GameObject continuePrompt;
    [SerializeField] TMPro.TMP_Text badEndingText;
    [SerializeField, TextArea] string defText;

    [SerializeField] CanvasGroup trueEnding;
    [SerializeField] TitleAnimator animator;

    int endingType = -1;
    bool activateContinue = false;
    bool moreTextAvail = false;

    List<string> lines;
    int currLine = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAnalysing());
    }

    IEnumerator LoadAnalysing()
    {
        float timeStep = 0;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 3;
            progress.fillAmount = Mathf.Lerp(0, 1, timeStep);
            analyseText.text = $"Analyising ({(int)(progress.fillAmount * 100)}%)";
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        analysingPanel.gameObject.SetActive(false);
        if(PlayerPrefs.GetInt(ENDING_TYPE) == 0)
        {
            endingType = 0;
            ShowBadEnding();
        }
        else
        {
            endingType = 1;
            ShowGoodEnding();
        }
    }

    void ShowBadEnding()
    {
        activateContinue = true;
        goodEndigPanel.gameObject.SetActive(false);
        badEndingPanel.gameObject.SetActive(true);

        string str = PlayerPrefs.HasKey(ENDING_BAD_SUMMARY) ? PlayerPrefs.GetString(ENDING_BAD_SUMMARY):defText;
        badEndingText.text = string.Format(badEndingText.text, str);
        StartCoroutine(ShowContinue());
    }

    IEnumerator ShowContinue()
    {
        yield return new WaitForSeconds(3);
        activateContinue = true;
        continuePrompt.SetActive(true);
    }

    void ShowGoodEnding()
    {
        activateContinue = false;
        goodEndigPanel.gameObject.SetActive(true);
        badEndingPanel.gameObject.SetActive(false);
        lines = PlayerPrefs.GetString(ENDING_GOOD_DIALOGUE).Split('\n').ToList();
        NextSet();
    }

    void NextSet()
    {
        if(currLine <= lines.Count-1)
        {
            goodEndingText.text = lines[currLine++];
            moreTextAvail = true;
        }
        else
        {
            moreTextAvail = false;
            LoadTrueEnding();
        }
    }

    void LoadTrueEnding()
    {
        activateContinue = false;
        moreTextAvail = false;
        StartCoroutine(TrueEndingRoutine());
    }

    IEnumerator TrueEndingRoutine()
    {
        float timeStep = 0;
        while(timeStep <= 1)
        {
            timeStep += Time.deltaTime / 1;
            trueEnding.alpha = Mathf.Lerp(0, 1, timeStep);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        animator.Animate("Simulation Over\nThank you for playing :)", rickRoll: false, onComplete: () => {
            SceneManager.LoadScene("Menu");
        });

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (activateContinue)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                if (endingType == 1)
                {
                    if (moreTextAvail)
                    {
                        NextSet();
                    }
                }
            }
        }
    }
}
