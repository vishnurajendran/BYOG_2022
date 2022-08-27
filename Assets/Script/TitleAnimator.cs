using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimator : MonoBehaviour
{
    [SerializeField] AudioClip sfx;
    [SerializeField] TMPro.TMP_Text titleText;
    [SerializeField] List<string> titles;
    [SerializeField] float defTime = 0.1f;

    [SerializeField, TextArea] string text;
    AudioSource source;

    private IEnumerator Start()
    {
        source = new GameObject("Tap").AddComponent<AudioSource>();
        var split = text.Split("\n");
        titles = new List<string>();
        foreach (var s in split)
        {
            var s1 = s.Trim();
            if (!string.IsNullOrEmpty(s1))
                titles.Add(s1);

        }

        titleText.text = "";
        int indx = 0;
        while (true)
        {
            string title = titles[indx];
            foreach(var c in title)
            {
                source.PlayOneShot(sfx);
                titleText.text += c;
                yield return new WaitForSeconds(defTime);
            }

            yield return new WaitForSeconds(Random.Range(1, 3));

            while(titleText.text.Length > 0)
            {
                source.PlayOneShot(sfx);
                titleText.text = titleText.text.Substring(0 , titleText.text.Length - 1);
                yield return new WaitForSeconds(defTime);
            }

            yield return new WaitForSeconds(Random.Range(1, 2));
            indx = (indx + 1) % titles.Count;
        }
    }

}