using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashController : MonoBehaviour
{
    [SerializeField] GameObject seq1;
    [SerializeField] GameObject seq2;
    [SerializeField] CanvasGroup cg;
    [SerializeField] TMPro.TMP_Text bootText;
    [SerializeField] Image progressbar;
    [Space(10)]
    [SerializeField, TextArea] string bootSequenceText;
    [SerializeField] List<string> bootSequenceList;

    Coroutine routine;
    void Start()
    {
        bootSequenceText = bootSequenceText.Replace("2015", "2122");
        bootSequenceText = bootSequenceText.Replace("[  OK  ]", "[ <color=green>OK</color> ]");
        bootSequenceText = bootSequenceText.Replace("[FAILED]", "[ <color=red>FAILED</color> ]");
        bootSequenceList = new List<string>();
        bootSequenceList = bootSequenceText.Split('\n').ToList();
        bootSequenceList.RemoveAll(a => string.IsNullOrEmpty(a.Trim()));
        routine = StartCoroutine(StartBootSequence());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(routine != null)
                StopCoroutine(routine);

            SceneManager.LoadScene("Menu");
        }
    }

    //19 seconds before boot image
    //total length 23 seconds
    IEnumerator StartBootSequence()
    {
        float timeStep = 0;
        while(timeStep <= 1)
        {
            timeStep += Time.deltaTime / 1;
            cg.alpha = Mathf.Lerp(1f, 0f, timeStep);
            yield return new WaitForEndOfFrame();
        }

        seq1.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        PrintLines(0, 25);
        yield return new WaitForSeconds(1);
        PrintLines(26, 35);
        yield return new WaitForSeconds(1);
        PrintLines(36, 50);
        yield return new WaitForSeconds(1);
        PrintLines(51, 60);
        yield return new WaitForSeconds(2);
        PrintLines(61, 75);
        yield return new WaitForSeconds(1);
        PrintLines(76, 80);
        yield return new WaitForSeconds(1);
        PrintLines(81, 90);
        yield return new WaitForSeconds(2);
        PrintLines(91, 100);
        yield return new WaitForSeconds(1);
        PrintLines(101, 102);
        yield return new WaitForSeconds(2);
        PrintLines(103, 107);
        yield return new WaitForSeconds(1);
        PrintLines(108, 109);
        yield return new WaitForSeconds(1);
        PrintLines(110, 111);
        yield return new WaitForSeconds(1);
        PrintLines(111, 112);
        yield return new WaitForSeconds(1);
        PrintLines(112, 114);
        yield return new WaitForSeconds(1);
        seq1.gameObject.SetActive(false);
        seq2.gameObject.SetActive(true);

        timeStep = 0;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 4;
            progressbar.fillAmount = Mathf.Lerp(0f, 1f, timeStep);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu");
    }

    private void PrintLines(int indx1, int indx2)
    {
        for(int i = indx1; i <= indx2; i++)
        {
            bootText.text += bootSequenceList[i] + "\n";
        }
    }

}
