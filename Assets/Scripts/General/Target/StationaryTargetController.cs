using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryTargetController : MonoBehaviour
{
    public GameObject pointManager;
    public GameObject powerUpController;
    public GameObject targetPrefab;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public int spawnerType;

    public GameObject metricManager;
    private MetricManagerScript metricManagerScript;

    public GameObject bearManager;
    private BearManager bearManagerScript;

    private int colorPick;
    private ArrayList colorList;
    private Transform myTransform;
    private float myTimer;
    private float powerUpCooldown;
    private float pityPowerUp;
    private const int POWER_UP_CHANCE = 2;

    void Start()
    {
        myTimer = 0.0f;
        powerUpCooldown = 0.0f;
        pityPowerUp = 0.0f;

        myTransform = GetComponent<Transform>();

        metricManager = GameObject.Find("MetricManager");
        metricManagerScript = metricManager.GetComponent<MetricManagerScript>();

        bearManagerScript = bearManager.GetComponent<BearManager>();

        colorList = new ArrayList();
        colorList.Add("red");
        colorList.Add("fuschia");
        colorList.Add("blue");
        colorList.Add("green");
        colorList.Add("black");
        colorList.Add("doublePoints");
        colorList.Add("shotgun");
        colorList.Add("sniper");
    }

    // Update is called once per frame
    void Update()
    {
        if (myTimer <= 0f)
        {
            if (spawnerType == 0)
            {
                // Spawn more frequently
                myTimer = UnityEngine.Random.Range(0.5f, 2f);
            }
            else if (spawnerType == 1)
            {
                // Spawn less frequently
                myTimer = UnityEngine.Random.Range(1.0f, 2.5f);
            }
            else
            {
                // Spawn very rarely
                myTimer = UnityEngine.Random.Range(2.0f, 3.0f);
            }
            PickColor();
            SpawnTarget();
        }
        myTimer -= Time.deltaTime;
        powerUpCooldown -= Time.deltaTime;
        pityPowerUp += Time.deltaTime;
    }

    private void PickColor()
    {
        if ((UnityEngine.Random.Range(0, 100) <= POWER_UP_CHANCE || pityPowerUp >= 25.0f) && powerUpCooldown <= 0.0f)
        {
            // Pick a powerup and set cooldown
            pityPowerUp = 0.0f;
            powerUpCooldown = 10.0f;
            colorPick = UnityEngine.Random.Range(5, 8);
            metricManagerScript.powerUpSpawned = true;

        }
        else if (spawnerType == 0)
        {

            // Front spawner so only spawn low-point targets Red and Fuschia
            colorPick = UnityEngine.Random.Range(0, 2);
        }
        else if (spawnerType == 1)
        {
            // Mid Spawner so spawn Fuschia, Blue, and occasionally green
            bool greenPossible = false;
            if (UnityEngine.Random.Range(0, 100) <= 10)
            {
                greenPossible = true;
            }
            if (greenPossible)
            {
                colorPick = UnityEngine.Random.Range(1, 4);
            }
            else
            {
                colorPick = UnityEngine.Random.Range(1, 3);
            }
        }
        else if (spawnerType == 2)
        {
            // Back Spawner so spawn green and occasionally black
            bool blackPossible = false;
            if (UnityEngine.Random.Range(0, 100) <= 5)
            {
                blackPossible = true;
            }
            int choice = 0;
            if (blackPossible)
            {
                choice = UnityEngine.Random.Range(0, 2);
            }
            else
            {
                choice = UnityEngine.Random.Range(0, 1);
            }

            if (choice == 0)
            {
                // Green
                colorPick = 3;

            }
            else if (choice == 1)
            {
                // Black
                colorPick = 4;
            }
        }

        // Metric updating
        if (colorPick == 3 || colorPick == 4)
        {
            metricManagerScript.rareBearsSpawnedPool++;
        }
    }

    private void SpawnTarget()
    {
        GameObject target = Instantiate(targetPrefab, myTransform.position + PickSpawnPoint(), Quaternion.Euler(Vector3.zero)) as GameObject;
        StationaryTargetScript currTargetScript = target.GetComponent<StationaryTargetScript>();
        currTargetScript.PowerUpController = powerUpController;

        currTargetScript.metricManagerScript = metricManagerScript;
        currTargetScript.pointManager = pointManager;
        currTargetScript.bearManagerScript = bearManagerScript;

        currTargetScript.colorPick = colorPick;
    }

    private Vector3 PickSpawnPoint()
    {
        // Set random target spawn within a specific range from the spawner and taking into account the color
        float xPos = UnityEngine.Random.Range(-7.0f, 7.0f);
        return new Vector3(xPos, 0, 0);
    }
}
