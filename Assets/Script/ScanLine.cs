using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanLine : MonoBehaviour
{
    Material material;
    [SerializeField] float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset = new Vector2(0, material.mainTextureOffset.y + speed*Time.deltaTime);
    }
}
