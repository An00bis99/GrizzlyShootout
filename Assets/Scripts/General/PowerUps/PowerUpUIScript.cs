using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIScript : MonoBehaviour
{
    public Sprite[] spriteArray;
    public int spriteIndex = -1;
    private Image currImage;
    // Start is called before the first frame update
    void Start()
    {
        currImage = GetComponent<Image>();
        currImage.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteIndex != -1)
        {
            currImage.color = Color.white;
            currImage.sprite = spriteArray[spriteIndex];
        }
        else
        {
            currImage.color = Color.clear;
        }
    }
}
