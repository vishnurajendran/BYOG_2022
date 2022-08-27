using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    const string PREF_KEY = "Intro";
    
    [SerializeField] AudioClip sfx;
    [SerializeField] Transform buttonParent;
    [SerializeField] TitleAnimator titleAnimator;

    [SerializeField, TextArea(minLines: 3,maxLines:15)] string credits;
    [SerializeField, TextArea(minLines: 3, maxLines: 15)] string howToPlay;
    [SerializeField, TextArea(minLines: 3, maxLines: 15)] string startingText;
    [SerializeField] bool cleanPrefs=false;
    List<Option> options;
    int indx = 0;

    AudioSource source;
    private void Start()
    {
        source = new GameObject("MenuSFX").AddComponent<AudioSource>();
        options = new List<Option>();
        foreach (Transform child in buttonParent)
        {
            options.Add(child.GetComponent<Option>());
        }
    }

    private void Update()
    {
        if(cleanPrefs)
        {
            cleanPrefs = false;
            if(PlayerPrefs.HasKey(PREF_KEY))
                PlayerPrefs.DeleteKey(PREF_KEY);
        }

        if (Input.GetButtonDown("Vertical"))
        {
            float vert = Input.GetAxis("Vertical");
            int delta = 0;
            if (vert < 0)
                delta = 1;
            else if (vert > 0)
                delta = -1;

            int currIndx = indx;
            indx = Mathf.Clamp(indx + delta, 0, options.Count-1);
            if(currIndx != indx)
            {
                source.PlayOneShot(sfx);
                options[currIndx].OnDeselect();
                options[indx].OnSelect();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            source.PlayOneShot(sfx);
            options[indx].OnChosen();
        }
    }

    public void ShowCredits()
    {
        titleAnimator.Animate(credits);
    }

    public void ShowHowToPlay()
    {
        titleAnimator.Animate(howToPlay);
    }

    public void Play()
    {
        buttonParent.gameObject.SetActive(false);
        if (PlayerPrefs.HasKey(PREF_KEY))
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            titleAnimator.Animate(startingText, rickRoll: false, onComplete: () =>
            {
                PlayerPrefs.SetInt(PREF_KEY, 1);
                SceneManager.LoadScene("Game");
            });
        }
        
    }

    public void Exit()
    {
        Application.Quit();
    }
}
