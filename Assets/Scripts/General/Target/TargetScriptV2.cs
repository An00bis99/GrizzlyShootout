using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro.Examples;
using UnityEngine;

public class TargetScriptV2 : MonoBehaviour
{
    public GameObject pointPopUpPrefab;
    public GameObject bearStandPrefab;
    public GameObject bearStandStickPrefab;
    public GameObject powerUpStandPrefab;
    public GameObject PowerUpController;
    public GameObject projectilePrefab;

    public GameObject bearManager;
    public BearManager bearManagerScript;

    public GameObject pointManager;
    public MetricManagerScript metricManagerScript;

    public Transform gunTransform;

    public int colorPick;
    public int direction = 1;
    public AudioSource audioPlayer;
    public AudioClip[] randomClips;

    private bool isDisabled = false;
    private Renderer targetRenderer;
    private Transform myTransform;
    private bool started = false;
    private float moveSpeed;
    private GameObject bearStandInitialized;
    private GameObject bearStandStickInitialized;
    private GameObject powerUpInitialized;
    private bool isPowerup = false;
    private bool isBear = false;
    private bool isPink = false;
    private bool isBomb = false;
    private bool isScared = false;
    private bool exploded = false;
    public bool canThrowProjectile = false;
    private float projectileCountdown;
    private int BEAR_COUNT = 6;



    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        targetRenderer = GetComponent<Renderer>();
        targetRenderer.enabled = !targetRenderer.enabled;
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.volume = .2f;
        // TODO Make this countdown unique for every bear
        projectileCountdown = UnityEngine.Random.Range(1.0f, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            // Stop the other movement code from running before this is initialized properly
            Initialize();
            started = true;
        }
        else if (!GetComponent<Collider>().enabled && !isDisabled)
        {
            // This code is reached when the raycast bullet says that this target is hit
            if (isBear)
            {
                if (colorPick == 1)
                {
                    isPink = true;
                }
                if (colorPick == 3)
                {
                    isBomb = true;
                }
                if (!exploded)
                {
                    // Only play hit audio clip if hit by bullet, not explosion
                    PlayAudioClip();
                }

                if ((!isPink && exploded) || (!exploded))
                {
                    GameObject pointPopup;
                    pointPopup = Instantiate(pointPopUpPrefab, myTransform.position, myTransform.rotation) as GameObject;
                    if (colorPick > 2)
                    {
                        pointPopup.GetComponent<PointScript>().spriteIndex = colorPick - 1;
                    }
                    else
                    {
                        pointPopup.GetComponent<PointScript>().spriteIndex = colorPick;
                    }

                }

                if (colorPick == 0)
                {
                    // Red
                    pointManager.GetComponent<ScoreManager>().nextScore += 1;
                }
                else if (colorPick == 1 && !exploded)
                {
                    // Fuschia
                    pointManager.GetComponent<ScoreManager>().nextScore += -5;
                }
                else if (colorPick == 2)
                {
                    // Blue
                    pointManager.GetComponent<ScoreManager>().nextScore += 5;
                }
                else if (colorPick == 3)
                {
                    // Bomb
                    pointManager.GetComponent<ScoreManager>().nextScore += 5;

                }
                else if (colorPick == 4)
                {
                    // Green
                    pointManager.GetComponent<ScoreManager>().nextScore += 10;
                    metricManagerScript.rareBearsHitPool++;
                }
                else if (colorPick == 5)
                {
                    // Black
                    pointManager.GetComponent<ScoreManager>().nextScore += 20;
                    metricManagerScript.rareBearsHitPool++;
                }
                pointManager.GetComponent<ScoreManager>().bearTallyNum[colorPick] += 1;
                bearManagerScript.RemoveBear(gameObject);
                if (isBomb)
                {
                    bearManagerScript.Explode(transform.position);
                }
                else
                {
                    // Scare the other bears in radius
                    bearManagerScript.ScareBearsInRadius(transform.position);
                }

                // Tell the bear stand and stick that they do their falling animation and destroy themselves.
                bearStandInitialized.GetComponent<BearStandScript>().isShot = true;
            }
            else if (isPowerup)
            {
                if (PowerUpController.GetComponent<PowerUpControlScript>().powerUpActive == false)
                {
                    PowerUpController.GetComponent<PowerUpControlScript>().currPowerUp = colorPick - BEAR_COUNT;
                }
                else
                {
                    PowerUpController.GetComponent<PowerUpControlScript>().currentPowerUpTimer = 10.0f;
                    PowerUpController.GetComponent<PowerUpControlScript>().currPowerUp = colorPick - BEAR_COUNT;
                }
                PlayAudioClip();
                powerUpInitialized.GetComponent<PowerUpScript>().isShot = true;
            }
            bearStandStickInitialized.GetComponent<BearStandStickScript>().isShot = true;
            Destroy(gameObject, 1f);
            isDisabled = true;
        }
        else if (!isDisabled)
        {
            // Horizontal movement
            if (canThrowProjectile)
            {
                projectileCountdown -= Time.deltaTime;
                if (projectileCountdown <= 0.0f)
                {
                    GameObject spawnedProjectile = Instantiate(projectilePrefab, myTransform.position, projectilePrefab.transform.rotation) as GameObject;
                    spawnedProjectile.GetComponent<ProjectileBehavior>().scoreManager = pointManager;
                    spawnedProjectile.GetComponent<ProjectileBehavior>().gunPosition = gunTransform;
                    canThrowProjectile = false;
                }
            }
            myTransform.position = new Vector3(myTransform.position.x + (moveSpeed * Time.deltaTime), myTransform.position.y, myTransform.position.z);
            if ((myTransform.position.x > 10 || myTransform.position.x < -10) && !isDisabled)
            {
                // Destroys the target once it gets off screen
                if (isBear)
                {
                    bearManagerScript.RemoveBear(gameObject);
                    Destroy(bearStandInitialized, 0.5f);

                }
                else if (isPowerup)
                {
                    Destroy(powerUpInitialized, 0.5f);
                }
                Destroy(bearStandStickInitialized, 0.5f);
                Destroy(gameObject);
            }
        }

    }

    private void Initialize()
    {
        if (colorPick == 0)
        {
            // Red selected, meaning 1 point
            moveSpeed = 4.0f * direction;
            isBear = true;
            //targetRenderer.material.SetColor("_Color", Color.red);

        }
        else if (colorPick == 1)
        {
            // Pink so -5 points
            moveSpeed = UnityEngine.Random.Range(3.0f, 6.0f) * direction;
            isBear = true;
            //Color fuschiaColor = new Color(153f, 15f, 68f, 1f);
            //targetRenderer.material.SetColor("_Color", fuschiaColor);
        }
        else if (colorPick == 2 || colorPick == 3)
        {
            // Blue or Bomb selected, so 5 points
            moveSpeed = UnityEngine.Random.Range(5.5f, 7.5f) * direction;
            isBear = true;
            //targetRenderer.material.SetColor("_Color", Color.blue);
        }
        else if (colorPick == 4)
        {
            // Green selected, so 10 points
            moveSpeed = UnityEngine.Random.Range(8.0f, 10.0f) * direction;
            isBear = true;
            //targetRenderer.material.SetColor("_Color", Color.green);
        }
        else if (colorPick == 5)
        {
            // Black selected, so 20 points
            moveSpeed = UnityEngine.Random.Range(10.0f, 15f) * direction;
            isBear = true;
            //targetRenderer.material.SetColor("_Color", Color.black);
        }
        else if (colorPick == 6)
        {
            // DoublePoints PowerUp
            moveSpeed = UnityEngine.Random.Range(10.0f, 15f) * direction;
            isPowerup = true;
        }
        else if (colorPick == 7)
        {
            // Shotgun powerup
            moveSpeed = UnityEngine.Random.Range(10.0f, 15f) * direction;
            isPowerup = true;
        }
        else if (colorPick == 8)
        {
            // Sniper
            moveSpeed = UnityEngine.Random.Range(10.0f, 15f) * direction;
            isPowerup = true;
        }

        // Bear sprite is instanitated and given parameter it needs to function
        if (isBear)
        {
            bearStandInitialized = Instantiate(bearStandPrefab, myTransform.position, myTransform.rotation) as GameObject;
            bearManagerScript.AddBear(gameObject);

            // Setting variables in their respective scripts before their first frame update
            int spriteIndex = colorPick;
            // Change sprite number to match the current level
            if (!canThrowProjectile)
            {
                if (spriteIndex < 3)
                {
                    // Do not do any calculation if bomb selected because it
                    // doesn't have any alternate sprites

                    /* Uncomment when level 3 sprites are put in
                    if (canThrowProjectile)
                    {
                        spriteIndex += BEAR_COUNT * 2;
                    }
                    else
                    {
                        spriteIndex += BEAR_COUNT;
                    }
                    */
                    spriteIndex += BEAR_COUNT;

                }
                else if (spriteIndex > 3)
                {
                    // Account for bears after the bomb bear
                    spriteIndex += BEAR_COUNT - 1;
                }
            }
            else
            {
                float choice = UnityEngine.Random.Range(0.0f, 100.0f);
                if (choice <= 60.0f)
                {
                    canThrowProjectile = false;
                }

                if (spriteIndex < 3)
                {
                    // Do not do any calculation if bomb selected because it
                    // doesn't have any alternate sprites

                    /* Uncomment when level 3 sprites are put in
                    if (canThrowProjectile)
                    {
                        spriteIndex += BEAR_COUNT * 2;
                    }
                    else
                    {
                        spriteIndex += BEAR_COUNT;
                    }
                    */
                    spriteIndex += BEAR_COUNT;

                }
                else if (spriteIndex > 3)
                {
                    // Account for bears after the bomb bear
                    spriteIndex += BEAR_COUNT - 1;
                }

                if (spriteIndex != 3)
                {
                    if (canThrowProjectile)
                    {

                        spriteIndex += (BEAR_COUNT - 1) * 2;
                    }
                    else
                    {
                        spriteIndex += BEAR_COUNT - 1;
                    }
                }

            }


            bearStandInitialized.GetComponent<BearStandScript>().spriteIndex = spriteIndex;
            bearStandInitialized.GetComponent<BearStandScript>().moveSpeed = moveSpeed;
        }
        else if (isPowerup)
        {
            powerUpInitialized = Instantiate(powerUpStandPrefab, myTransform.position, myTransform.rotation) as GameObject;
            powerUpInitialized.GetComponent<PowerUpScript>().spriteIndex = colorPick - BEAR_COUNT;
            powerUpInitialized.GetComponent<PowerUpScript>().moveSpeed = moveSpeed;
        }

        // Bear stand stick is instantiated and given proper parameters
        Vector3 stickOffest = new Vector3(0f, -0.2f, .3f);
        bearStandStickInitialized = Instantiate(bearStandStickPrefab, myTransform.position + stickOffest, myTransform.rotation) as GameObject;
        bearStandStickInitialized.GetComponent<BearStandStickScript>().moveSpeed = moveSpeed;
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        // This shouldn't be called because the bullet uses raycasting, but left here just in case
        // it somehow actually collides with the target instead of using the raycast.
        if (collision.gameObject.tag == "Bullet")
        {
            if (isBear)
            {
                GameObject pointPopup = Instantiate(pointPopUpPrefab, myTransform.position, myTransform.rotation) as GameObject;
                pointPopup.GetComponent<PointScript>().spriteIndex = colorPick;
                bearStandInitialized.GetComponent<BearStandScript>().isShot = true;

            }
            else if (isPowerup)
            {
                powerUpInitialized.GetComponent<PowerUpScript>().isShot = true;
            }
            bearStandStickInitialized.GetComponent<BearStandStickScript>().isShot = true;
            Destroy(collision.gameObject);
            Destroy(gameObject, 1f);
            isDisabled = true;

        }
    }
    */

    private void PlayAudioClip()
    {
        // Plays a randomized audio clip from the given array
        int clipIndex = 0;
        if (isPink)
        {
            // Point loss sound effect is third to last
            clipIndex = randomClips.Length - 3;
        }
        else if (isBomb)
        {
            // Explosion clip is second to last
            clipIndex = randomClips.Length - 2;
        }
        else if (isPowerup)
        {
            // PowerUp Clip is last in the list
            audioPlayer.volume = 1.0f;
            audioPlayer.pitch = 1.5f;
            clipIndex = randomClips.Length - 1;
        }
        else
        {
            // Only pick sound effects besides the point loss, explosion, and powerUp sounds
            clipIndex = Random.Range(0, randomClips.Length - 3);
        }
        audioPlayer.clip = randomClips[clipIndex];
        audioPlayer.Play();
        audioPlayer.pitch = 1.0f;
        audioPlayer.volume = 0.2f;
    }

    public void Scare()
    {
        // Called by outside to change the bear sprite to its scared counterpart
        if (!isScared)
        {
            isScared = true;
            // Change direction and increase speed to show they're scared
            moveSpeed *= -1.2f;
            // Propogate scaredness to other parts (stand and stick)
            bearStandInitialized.GetComponent<BearStandScript>().ScareStand();
            bearStandStickInitialized.GetComponent<BearStandStickScript>().ScareStick();
        }
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            GetComponent<Collider>().enabled = false;
        }
    }

}
