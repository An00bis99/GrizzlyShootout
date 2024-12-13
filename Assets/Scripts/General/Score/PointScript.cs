using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScript : MonoBehaviour
{
    public Sprite[] spriteArray;
    public int spriteIndex = -1;
    private float alpha = 1f;
    private bool spriteLoaded = false;
    private SpriteRenderer spriteRenderer;
    private Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteIndex != -1)
        {
            if (!spriteLoaded)
            {
                spriteLoaded = true;
                spriteRenderer.sprite = spriteArray[spriteIndex];


            }
            alpha -= Time.deltaTime;
            float rComp = spriteRenderer.color.r;
            float gComp = spriteRenderer.color.g;
            float bComp = spriteRenderer.color.b;
            spriteRenderer.color = new Color(rComp, gComp, bComp, alpha);
            myTransform.position += new Vector3(0f, .5f, 0f) * Time.deltaTime;

        }
        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }

}
