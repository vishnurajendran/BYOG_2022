using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMPro.TMP_Text text;

    [SerializeField] UnityEvent onChosen;

    public void OnSelect()
    {
        image.gameObject.SetActive(true);
        text.color = Color.black;
    }

    public void OnDeselect()
    {
        image.gameObject.SetActive(false);
        text.color = Color.white;
    }

    public void OnChosen()
    {
        onChosen?.Invoke();
    }
}
