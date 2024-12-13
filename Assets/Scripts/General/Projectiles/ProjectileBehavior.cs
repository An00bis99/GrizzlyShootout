using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject scoreManager;

    public GameObject reticlePrefab;
    private GameObject instantiatedReticle;

    public Transform gunPosition;
    private Vector3 currentTargetPos;

    private bool disabled;
    private Collider myCollision;
    private Vector3 forwardVecNew;
    private const float SPEED_MULTIPLIER = 1.0f;

    private void Start()
    {
        myCollision = GetComponent<Collider>();
        LockOnTarget();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!disabled)
        {
            transform.position += SPEED_MULTIPLIER * forwardVecNew * Time.deltaTime;
            if (myCollision.enabled == false)
            {
                // Shot by bullet, so destroy it
                disabled = true;
                Destroy(instantiatedReticle);
                Destroy(gameObject);
            }
            else if (Vector3.Distance(transform.position, currentTargetPos) < 0.25f)
            {
                // Pretty much hit the player, so trigger the hit and all effects that come with it
                ScoreManager scoreManagerScript = scoreManager.GetComponent<ScoreManager>();
                scoreManagerScript.TriggerHit();
                scoreManagerScript.nextScore -= 20;
                Destroy(instantiatedReticle);
                Destroy(gameObject);
            }
        }
    }

    private void LockOnTarget()
    {
        Vector2 axisOffset = UnityEngine.Random.insideUnitCircle;
        axisOffset.x *= .03f;
        axisOffset.y *= .1f;
        axisOffset.y = Mathf.Clamp(axisOffset.y, 0.3f, 0.7f);
        // A random spot in the player's field of vision is chosen
        // The z offset is so that the projectile gets really close to the screen before registering the hit
        currentTargetPos = new Vector3(gunPosition.position.x + axisOffset.x, gunPosition.position.y + axisOffset.y, gunPosition.position.z - .05f);

        // Rotation calculations
        Vector3 lookVector = Vector3.up;
        forwardVecNew = Vector3.Normalize(currentTargetPos - transform.position);
        transform.rotation = Quaternion.FromToRotation(lookVector, forwardVecNew);

        // Generate reticle
        instantiatedReticle = Instantiate(reticlePrefab, transform.position + (forwardVecNew * 0.2f), Quaternion.FromToRotation(Vector3.forward, forwardVecNew)) as GameObject;
        ReticleScript currReticleScript = instantiatedReticle.GetComponent<ReticleScript>();
        currReticleScript.fwdDirection = forwardVecNew;

    }
}
