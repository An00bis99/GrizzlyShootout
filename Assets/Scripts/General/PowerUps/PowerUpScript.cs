using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    public bool isShot = false;
    public Sprite[] spriteArray;
    public int spriteIndex = -1;
    public float moveSpeed;

    private float rotationTime = 0.0f;
    private float hitRotation = 180.0f;
    private Transform myTransform;
    private bool spriteLoaded = false;
    private SpriteRenderer spriteRenderer;
    private bool isStopped = false;
    private Vector3 belowPivotPoint;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
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
}
