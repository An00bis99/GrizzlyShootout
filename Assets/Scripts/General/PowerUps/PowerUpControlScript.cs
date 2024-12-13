using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpControlScript : MonoBehaviour
{
    // Start is called before the first frame update
    private const float POWER_UP_TIME = 10.0f;
    public GameObject playerGun;
    public GameObject pointManager;
    public GameObject powerUpUI;
    public float currentPowerUpTimer;
    public int currPowerUp;
    public bool powerUpActive;
    private ScoreManager scoreManager;
    private Gun gunScript;
    private PowerUpUIScript powerUI;

    public GameObject metricManager;
    private MetricManagerScript metricManagerScript;

    void Start()
    {
        currentPowerUpTimer = 0.0f;
        currPowerUp = -1;
        powerUpActive = false;
        scoreManager = pointManager.GetComponent<ScoreManager>();
        gunScript = playerGun.GetComponent<Gun>();
        powerUI = powerUpUI.GetComponent<PowerUpUIScript>();

        metricManager = GameObject.Find("MetricManager");
        metricManagerScript = metricManager.GetComponent<MetricManagerScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (currentPowerUpTimer <= 0.0f && currPowerUp != -1 && !powerUpActive)
        {
            // Set timer
            powerUpActive = true;
            currentPowerUpTimer = POWER_UP_TIME;
            metricManagerScript.powerUpsHitPool++;


        }
        else if (currentPowerUpTimer <= 0.0f && powerUpActive)
        {
            powerUpActive = false;
            scoreManager.doublePoints = false;
            gunScript.isShotgun = false;
            gunScript.isSniper = false;
            gunScript.isGun = true;
            gunScript.laserSight.enabled = false;
            /*
            if (currPowerUp == 0)
            {
                scoreManager.doublePoints = false;
            }
            else if (currPowerUp == 1)
            {
                gunScript.isShotgun = false;
                gunScript.isGun = true;
            }
            else if (currPowerUp == 2)
            {
                gunScript.isSniper = false;
                gunScript.isGun = true;
                gunScript.laserSight.enabled = false;
            }
            */
            currPowerUp = -1;
            powerUI.spriteIndex = -1;
        }

        if (currPowerUp == 0)
        {
            // Double Points
            scoreManager.doublePoints = true;
            powerUI.spriteIndex = 0;

        }
        else if (currPowerUp == 1)
        {
            // Shotgun
            gunScript.isShotgun = true;
            gunScript.isGun = false;
            gunScript.isSniper = false;
            gunScript.laserSight.enabled = false;
            powerUI.spriteIndex = 1;

        }
        else if (currPowerUp == 2)
        {
            // Sniper
            gunScript.isSniper = true;
            gunScript.laserSight.enabled = true;
            gunScript.isGun = false;
            gunScript.isShotgun = false;
            powerUI.spriteIndex = 2;
        }
        currentPowerUpTimer -= Time.deltaTime;
    }
}
