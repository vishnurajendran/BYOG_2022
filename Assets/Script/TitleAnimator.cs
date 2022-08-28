using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimator : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text titleText;
    [SerializeField] List<string> titles;
    [SerializeField] float defTime = 0.1f;

    [SerializeField, TextArea] string text;

    Coroutine animator;

    private void Start()
    {
        RickRoll();
    }

    public void RickRoll()
    {
        Animate(text);
    }

    public void Animate(string str, System.Action onComplete=null,bool rickRoll=true)
    {
        if (animator != null)
            StopCoroutine(animator);

        titleText.text = "";
        animator = StartCoroutine(AnimateAsTitle(str, () => {
            if (rickRoll)
                RickRoll();

            onComplete?.Invoke();
        }));
    }

    private IEnumerator AnimateAsTitle(string text, System.Action onComplete=null)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        var split = text.Split("\n");
        titles = new List<string>();
        foreach (var s in split)
        {
            var s1 = s.Trim();
            if (!string.IsNullOrEmpty(s1))
                titles.Add(s1);

        }
        if (text.Length <= 0)
            yield break;

        titleText.text = "";
        int indx = 0;
        int left = titles.Count;
        while (left > 0)
        {
            string title = titles[indx];
            foreach (var c in title)
            {
                titleText.text += c;
                yield return new WaitForSeconds(defTime);
            }

            yield return new WaitForSeconds(Random.Range(1, 3));

            while (titleText.text.Length > 0)
            {
                titleText.text = titleText.text.Substring(0, titleText.text.Length - 1);
                yield return new WaitForSeconds(defTime);
            }

            yield return new WaitForSeconds(Random.Range(1, 2));
            indx = (indx + 1) % titles.Count;

            left-=1;
        }

        onComplete?.Invoke();
    }

    public void StopAnim()
    {
        titleText.gameObject.SetActive(false);
    }

}