using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintAnimation : MonoBehaviour
{
    public Sprite[] spriteArray;
    private UnityEngine.UI.Image myImage;
    private int currIndex = 0;
    private int maxIndex;
    private const float IMAGE_TIME = 1.25f;
    private float currTimer;

    // Start is called before the first frame update
    void Start()
    {
        maxIndex = spriteArray.Length - 1;
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currTimer < IMAGE_TIME)
        {
            currTimer += Time.unscaledDeltaTime;
        }
        else
        {
            currTimer = 0.0f;
            ChangeSprite();
        }
    }

    private void ChangeSprite()
    {
        if (currIndex == maxIndex)
        {
            currIndex = 0;
        }
        else
        {
            currIndex++;
        }

        myImage.sprite = spriteArray[currIndex];
    }
}
