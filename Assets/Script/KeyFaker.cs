using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyFaker : MonoBehaviour
{
    [SerializeField] AudioClip keyClip;

    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = new GameObject("KeyFakeSource").AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            source.PlayOneShot(keyClip);
        }
    }
}
