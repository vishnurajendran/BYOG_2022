using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanLikenessMeter : MonoBehaviour
{
    [SerializeField] Image meter;
    [SerializeField] TMPro.TMP_Text percValue;

    [SerializeField] Color safe;
    [SerializeField] Color fuck;

    Coroutine routine;
    public void SetPerc(float perc)
    {
        if (routine != null)
            StopCoroutine(routine);
        percValue.text = $"Radio Strength: {(int)(perc * 100f)}%";
        routine = StartCoroutine(PercSetterRoutine(perc));
    }

    IEnumerator PercSetterRoutine(float perc)
    {
        float timeStep = 0;
        float startValue = meter.fillAmount;
        while(timeStep <= 1)
        {
            timeStep += Time.deltaTime / 0.15f;
            meter.fillAmount = Mathf.Lerp(startValue, perc, timeStep);
            meter.color = Color.Lerp(fuck,safe, meter.fillAmount);
            yield return new WaitForEndOfFrame();
        }
    }
}
