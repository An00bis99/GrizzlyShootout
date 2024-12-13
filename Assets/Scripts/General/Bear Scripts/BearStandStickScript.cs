using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BearStandStickScript : MonoBehaviour
{

    public float moveSpeed = 0.0f;
    public bool isShot = false;
    private bool isScared = false;

    private float hitRotation = 180.0f;
    private float rotationTime = 0.0f;
    private Transform myTransform;
    private bool isStopped = false;
    private Vector3 belowPivotPoint;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShot)
        {
            // Just move from one side of the screen to the other while not shot
            myTransform.position = new Vector3(myTransform.position.x + (moveSpeed * Time.deltaTime), myTransform.position.y, myTransform.position.z);
        }
        else
        {
            if (!isStopped)
            {
                isStopped = true;
                belowPivotPoint = new Vector3(myTransform.position.x, myTransform.position.y - 1.8f, myTransform.position.z - 0.2f);

            }
            // Do the rotation animation by 90 degrees when target is hit
            if (rotationTime > 0.5f)
            {
                Destroy(gameObject);
            }
            else
            {
                // Rotate around a pivot point a little bit below the normal pivot, along the x-axis
                myTransform.RotateAround(belowPivotPoint, Vector3.right, (hitRotation * Time.deltaTime));
                rotationTime += Time.deltaTime;
            }
        }
    }

    public void ScareStick()
    {
        if (!isScared)
        {
            isScared = true;
            moveSpeed *= -1.2f;
        }
    }
}
