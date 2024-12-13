using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USCG.Core.Telemetry;

public class Bullet : MonoBehaviour
{
    public enum GunType
    {
        Pistol = 0,
        Shotgun = 1,
        Sniper = 2
    }

    public int gunType = 0;

    public MetricManagerScript metricScript;

    private float life = 3;
    private float speed = 100f;
    private float gravForce = 10;
    private Vector3 startPos;
    private Vector3 startForward;
    private bool deltaChecked = false;
    private bool isInitialized = false;
    private float startTime = -1;
    private float currTime = 0;
    private float nextTime = 0;

    public void Initialize(int gunNum)
    {
        startPos = transform.position;
        startForward = transform.forward;
        isInitialized = true;
        gunType = gunNum;
    }

    private void FixedUpdate()
    {
        if (!isInitialized)
        {
            return;
        }
        if (startTime < 0)
        {
            startTime = Time.time;
        }

        RaycastHit hit;
        if (!deltaChecked)
        {
            currTime = Time.time - startTime;
        }
        else
        {
            currTime = nextTime;
        }
        nextTime = currTime + Time.fixedDeltaTime;

        Vector3 currPoint;
        Vector3 nextPoint;

        if (gunType == (int)(GunType.Pistol) || gunType == (int)(GunType.Shotgun))
        {
            // Do the regular arc
            currPoint = FindArcPoint(currTime);
            nextPoint = FindArcPoint(nextTime);
        }
        else
        {
            // Do a straight shot like hitscan
            currPoint = transform.position;
            nextPoint = currPoint + (startForward * speed * 2 * Time.fixedDeltaTime);
        }

        if (RaycastCheck(currPoint, nextPoint, out hit))
        {
            if (hit.collider.gameObject.tag != "Bullet" && hit.collider.gameObject.tag != "NonDestructable")
            {
                hit.collider.gameObject.GetComponent<Collider>().enabled = false;
                metricScript.shotsHitPool++;

            }
            Destroy(gameObject);
        }

        if (gunType == (int)(GunType.Sniper))
        {
            transform.position = nextPoint;
        }
    }

    private void Update()
    {
        if (!isInitialized || startTime < 0)
        {
            return;
        }
        if (gunType == (int)(GunType.Pistol) || gunType == (int)(GunType.Shotgun))
        {
            float currentTime = Time.time - startTime;
            Vector3 currPoint = FindArcPoint(currentTime);
            transform.position = currPoint;
        }
    }

    private Vector3 FindArcPoint(float time)
    {
        Vector3 point = startPos + (startForward * speed * time);
        Vector3 gravVec = Vector3.down * gravForce * time * time;
        return point + gravVec;
    }

    private bool RaycastCheck(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
    }

    void Awake()
    {
        Destroy(gameObject, life);
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "NonDestructable")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

    }
    */
}
