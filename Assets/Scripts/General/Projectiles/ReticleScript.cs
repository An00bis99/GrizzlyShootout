using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ReticleScript : MonoBehaviour
{
    public Sprite[] animArray;
    private SpriteRenderer mSpriteRenderer;
    private int currIdx = 0;
    private int animArraySize;
    private const float FRAME_TIME = 0.05f;
    private float currFrameTime = 0.0f;

    public Vector3 fwdDirection;

    // Start is called before the first frame update
    void Start()
    {
        animArraySize = animArray.Length;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        currFrameTime += Time.deltaTime;
        if (currFrameTime >= FRAME_TIME)
        {
            SetNextSprite();
        }
        // Movement update
        transform.position += fwdDirection * Time.deltaTime;

    }

    private void SetNextSprite()
    {
        mSpriteRenderer.sprite = animArray[currIdx];
        currIdx++;
        if (currIdx >= animArraySize)
        {
            currIdx = 0;
        }

        currFrameTime = 0.0f;
    }
}
