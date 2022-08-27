using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] AudioClip tapSfx;

    Coroutine routine;
    AudioSource source;

    public void AnimateText(TMPro.TMP_Text text, string str, float durBwChar=0.1f, System.Action onComplete=null)
    {
        if(source == null)
            source = new GameObject("Tap").AddComponent<AudioSource>();

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(TextAnimateRoutine(text, str, durBwChar, onComplete));
    }

    IEnumerator TextAnimateRoutine(TMPro.TMP_Text text, string str, float durBwChar=0.1f, System.Action onComplete=null)
    {
        yield return new WaitForEndOfFrame();

        foreach (var c in str)
        {
            source.PlayOneShot(tapSfx);
            text.text += c;
            yield return new WaitForSeconds(durBwChar);
        }

        yield return new WaitForEndOfFrame();
        onComplete?.Invoke();
    }
}
