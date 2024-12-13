using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunRotation : MonoBehaviour
{

    public float xSensitivity = 1f;
    public float ySensitivity = 1f;

    private float xRotate = 0f;
    private float yRotate = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseXValue = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
        float mouseYValue = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

        yRotate += mouseXValue;
        xRotate += mouseYValue;

        // Stop from aiming down more than 10 degrees and aiming up more than 40 degrees
        xRotate = Mathf.Clamp(xRotate, -10f, 25f);
        // Stop from turning further than 50 degrees left and right
        yRotate = Mathf.Clamp(yRotate, -130f, -50f);
        // Stop object from rotating more than it should
        transform.rotation = Quaternion.Euler(transform.rotation.x, yRotate + transform.rotation.y, xRotate + transform.rotation.z);
    }
}
