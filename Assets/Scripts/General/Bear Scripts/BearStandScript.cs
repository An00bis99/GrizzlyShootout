using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BearStandScript : MonoBehaviour
{
    public bool isShot = false;
    public Sprite[] spriteArray;
    public int spriteIndex = -1;
    public float moveSpeed;

    private bool isScared = false;

    private float rotationTime = 0.0f;
    private float hitRotation = 180.0f;
    private Transform myTransform;

    private bool spriteLoaded = false;
    private SpriteRenderer spriteRenderer;
    private bool isStopped = false;
    private Vector3 belowPivotPoint;

    private const int BEAR_TYPE_AMOUNT = 6;

    void Start()
    {
        myTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isShot)
        {
            // Sprite render and movement code
            if (spriteIndex != -1)
            {
                if (!spriteLoaded)
                {
                    spriteLoaded = true;
                    spriteRenderer.sprite = spriteArray[spriteIndex];
                }
                myTransform.position = new Vector3(myTransform.position.x + (moveSpeed * Time.deltaTime), myTransform.position.y, myTransform.position.z);
            }
        }
        else
        {
            // This is the rotation code when target is hit
            if (!isStopped)
            {
                isStopped = true;
                belowPivotPoint = new Vector3(myTransform.position.x, myTransform.position.y - 2.0f, myTransform.position.z);

            }
            if (rotationTime > 0.5f)
            {
                Destroy(gameObject);
            }
            else
            {
                myTransform.RotateAround(belowPivotPoint, Vector3.right, (hitRotation * Time.deltaTime));
                rotationTime += Time.deltaTime;
            }

        }
    }

    public void ScareStand()
    {
        // Called by outside to change the bear sprite to its scared counterpart
        if (!isScared)
        {
            isScared = true;
            // Change direction and increase speed to show they're scared
            moveSpeed *= -1.2f;
            // Uncomment next line if all scared sprites have been put into the spriteArray
            // so the correct sprite can be displayed
            //spriteIndex += BEAR_TYPE_AMOUNT;
        }

    }

}
