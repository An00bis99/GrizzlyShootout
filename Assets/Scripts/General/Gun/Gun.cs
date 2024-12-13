using UnityEngine;
using UnityEngine.SceneManagement;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public GameObject trajectoryPrefab;
    public LineRenderer laserSight;
    public AudioSource audioSource;
    public AudioClip shot;
    public bool isGun = true;
    public bool isShotgun = false;
    public bool isSniper = false;

    public GameObject MetricManager;
    private MetricManagerScript metricManagerScript;

    public float bulletSpeed = 300;
    public float bulletCooldown = 0.0f;

    private const float MAX_COOLDOWN = 0.3f;
    private float startTime = 5.0f;
    private bool canShoot = false;

    private void Start()
    {
        MetricManager = GameObject.Find("MetricManager");
        metricManagerScript = MetricManager.GetComponent<MetricManagerScript>();
        laserSight.SetPosition(0, bulletSpawnPoint.position);
        laserSight.SetPosition(1, transform.TransformDirection(Vector3.right) + new Vector3(0, 0, 10.0f));
        laserSight.enabled = false;

    }
    void Update()
    {
        if (!canShoot)
        {
            startTime -= Time.unscaledDeltaTime;
            if (startTime <= 0.0f)
            {
                canShoot = true;
            }
        }
        else
        {
            if (isSniper)
            {
                laserSight.SetPosition(0, bulletSpawnPoint.position);
                laserSight.SetPosition(1, transform.TransformDirection(Vector3.right) * 10.0f + transform.position);

            }
            if (Input.GetMouseButtonDown(0) && bulletCooldown <= 0.0f && Time.timeScale != 0.0f)
            {
                audioSource.clip = shot;
                audioSource.Play();

                Shoot();
                bulletCooldown = MAX_COOLDOWN;
            }

            /*
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            */
            bulletCooldown -= Time.deltaTime;
        }

    }

    private void Shoot()
    {
        metricManagerScript.shotsFiredPool++;
        if (isGun)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation) as GameObject;
            bullet.GetComponent<Bullet>().metricScript = metricManagerScript;
            bullet.GetComponent<Bullet>().Initialize(0);
        }
        else if (isShotgun)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere;
                randomOffset.x *= .5f;
                randomOffset.y *= .1f;
                randomOffset.z = 0.0f;
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position + randomOffset, bulletSpawnPoint.rotation) as GameObject;
                bullet.GetComponent<Bullet>().metricScript = metricManagerScript;
                bullet.GetComponent<Bullet>().Initialize(1);
            }
        }
        else if (isSniper)
        {
            // Make sure line renderer is enabled before this happens
            RaycastHit hit;
            if (Physics.Raycast(bulletSpawnPoint.position, bulletSpawnPoint.transform.TransformDirection(Vector3.forward), out hit, 15.0f))
            {
                if (hit.collider.tag != "Bullet" && hit.collider.tag != "NonDestructable")
                {
                    hit.collider.gameObject.GetComponent<Collider>().enabled = false;
                    metricManagerScript.shotsHitPool++;
                }
            }
        }

    }
}