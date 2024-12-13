using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryTargetScript : MonoBehaviour
{
    public GameObject pointPopUpPrefab;
    public GameObject bearStandPrefab;
    public GameObject bearStandStickPrefab;
    public GameObject powerUpStandPrefab;
    public GameObject PowerUpController;

    public GameObject bearManager;
    public BearManager bearManagerScript;

    public GameObject pointManager;
    public MetricManagerScript metricManagerScript;

    public int colorPick;
    public AudioSource audioPlayer;
    public AudioClip[] randomClips;

    private float currLifetime;

    private bool isDisabled = false;
    private Renderer targetRenderer;
    private Transform myTransform;
    private bool started = false;
    private GameObject bearStandInitialized;
    private GameObject bearStandStickInitialized;
    private GameObject powerUpInitialized;
    private bool isPowerup = false;
    private bool isBear = false;
    private bool isPink = false;
    private bool lifeExpired = false;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        targetRenderer = GetComponent<Renderer>();
        targetRenderer.enabled = !targetRenderer.enabled;
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.volume = .2f;
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
                if (!lifeExpired)
                {
                    // Only do pointPopup when the target is not dead from being on screen for too long
                    PlayAudioClip();
                    GameObject pointPopup = Instantiate(pointPopUpPrefab, myTransform.position, myTransform.rotation) as GameObject;
                    pointPopup.GetComponent<PointScript>().spriteIndex = colorPick;
                    if (colorPick == 0)
                    {
                        // Red
                        pointManager.GetComponent<ScoreManager>().nextScore += 1;
                        pointManager.GetComponent<ScoreManager>().bearTallyNum[colorPick] += 1;

                    }
                    else if (colorPick == 1)
                    {
                        // Fuschia
                        pointManager.GetComponent<ScoreManager>().nextScore += -5;
                        pointManager.GetComponent<ScoreManager>().bearTallyNum[colorPick] += 1;
                    }
                    else if (colorPick == 2)
                    {
                        // Blue
                        pointManager.GetComponent<ScoreManager>().nextScore += 5;
                        pointManager.GetComponent<ScoreManager>().bearTallyNum[colorPick] += 1;
                    }
                    else if (colorPick == 3)
                    {
                        // Green
                        pointManager.GetComponent<ScoreManager>().nextScore += 10;
                        metricManagerScript.rareBearsHitPool++;
                        pointManager.GetComponent<ScoreManager>().bearTallyNum[colorPick + 1] += 1;
                    }
                    else if (colorPick == 4)
                    {
                        // Black
                        pointManager.GetComponent<ScoreManager>().nextScore += 20;
                        metricManagerScript.rareBearsHitPool++;
                        pointManager.GetComponent<ScoreManager>().bearTallyNum[colorPick + 1] += 1;
                    }
                    bearManagerScript.RemoveBear(gameObject);
                }
                else
                {
                    bearManagerScript.RemoveBear(gameObject);
                }
                // Tell the bear stand and stick that they do their falling animation and destroy themselves.
                bearStandInitialized.GetComponent<BearStandScript>().isShot = true;

            }
            else if (isPowerup)
            {
                if (!lifeExpired)
                {
                    if (PowerUpController.GetComponent<PowerUpControlScript>().powerUpActive == false)
                    {
                        PowerUpController.GetComponent<PowerUpControlScript>().currPowerUp = colorPick - 5;
                    }
                    else
                    {
                        PowerUpController.GetComponent<PowerUpControlScript>().currentPowerUpTimer = 10.0f;
                        PowerUpController.GetComponent<PowerUpControlScript>().currPowerUp = colorPick - 5;
                    }
                    PlayAudioClip();
                }
                powerUpInitialized.GetComponent<PowerUpScript>().isShot = true;
            }
            bearStandStickInitialized.GetComponent<BearStandStickScript>().isShot = true;
            Destroy(gameObject, 1f);
            isDisabled = true;
        }
        else if (!isDisabled)
        {
            // Lifetime countdown
            currLifetime -= Time.deltaTime;

            if (currLifetime <= 0 && !isDisabled)
            {
                // Destroys the target its timer expires
                if (isBear)
                {
                    bearManagerScript.RemoveBear(gameObject);
                }
                lifeExpired = true;
                GetComponent<Collider>().enabled = false;
            }
        }

    }

    private void Initialize()
    {
        if (colorPick == 0)
        {
            // Red selected, meaning 1 point
            currLifetime = 4.0f;
            isBear = true;

        }
        else if (colorPick == 1)
        {
            // Pink so -5 points
            currLifetime = 3.0f;
            isBear = true;
        }
        else if (colorPick == 2)
        {
            // Blue selected, so 5 points
            currLifetime = 3.0f;
            isBear = true;
        }
        else if (colorPick == 3)
        {
            // Green selected, so 10 points
            currLifetime = 2.0f;
            isBear = true;
        }
        else if (colorPick == 4)
        {
            // Black selected, so 20 points
            currLifetime = 1.75f;
            isBear = true;
        }
        else if (colorPick >= 5)
        {
            // Is a powerUp
            currLifetime = 2.25f;
            isPowerup = true;
        }
        // Bear sprite is instanitated and given parameter it needs to function
        /*
        int spriteIndex = colorPick;
        if (colorPick > 2)
        {
            // Skip the bomb sprite since it is unused on the stationary target
            spriteIndex++;
        }
        */
        if (isBear)
        {
            bearStandInitialized = Instantiate(bearStandPrefab, myTransform.position, myTransform.rotation) as GameObject;
            bearManagerScript.AddBear(gameObject);

            // Setting variables in their respective scripts before their first frame update
            int spriteIndex = colorPick;
            if (colorPick > 2)
            {
                spriteIndex++;
            }
            bearStandInitialized.GetComponent<BearStandScript>().spriteIndex = spriteIndex;
        }
        else if (isPowerup)
        {
            powerUpInitialized = Instantiate(powerUpStandPrefab, myTransform.position, myTransform.rotation) as GameObject;
            powerUpInitialized.GetComponent<PowerUpScript>().spriteIndex = colorPick - 5;
        }

        // Bear stand stick is instantiated and given proper parameters
        Vector3 stickOffest = new Vector3(0f, -0.2f, .3f);
        bearStandStickInitialized = Instantiate(bearStandStickPrefab, myTransform.position + stickOffest, myTransform.rotation) as GameObject;
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
            clipIndex = randomClips.Length - 3;
        }
        else if (isPowerup)
        {
            audioPlayer.volume = 1.0f;
            audioPlayer.pitch = 1.5f;
            clipIndex = randomClips.Length - 1;
        }
        else
        {
            // Only pick sound effects besides the point loss and powerup
            clipIndex = Random.Range(0, randomClips.Length - 3);
        }
        audioPlayer.clip = randomClips[clipIndex];
        audioPlayer.Play();
        audioPlayer.pitch = 1.0f;
        audioPlayer.volume = 0.2f;
    }
}
