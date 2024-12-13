using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using USCG.Core.Telemetry;

public class MetricManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    static bool metricsStarted = false;
    private bool mainInstance;

    public TelemetryManager managerScript;

    private MetricId _shotsFired = default;
    public int shotsFiredPool = 0;

    private MetricId _shotsHit = default;
    public int shotsHitPool = 0;

    private MetricId _powerUpInterval = default;
    public bool powerUpSpawned = false;

    private MetricId _powerUpsHit = default;
    public int powerUpsHitPool = 0;

    private MetricId _rareBearsSpawned = default;
    public int rareBearsSpawnedPool = 0;

    private MetricId _rareBearsHit = default;
    public int rareBearsHitPool = 0;

    void Start()
    {
        if (!metricsStarted)
        {
            _shotsFired = managerScript.CreateAccumulatedMetric("shotsFired");
            _shotsHit = managerScript.CreateAccumulatedMetric("shotsHit");
            _powerUpInterval = managerScript.CreateSampledMetric<float>("powerUpInterval");
            _powerUpsHit = managerScript.CreateAccumulatedMetric("powerUpsHit");
            _rareBearsSpawned = managerScript.CreateAccumulatedMetric("rareBearsSpawned");
            _rareBearsHit = managerScript.CreateAccumulatedMetric("rareBearsHit");
            mainInstance = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (mainInstance)
        {
            if (shotsFiredPool != 0)
            {
                managerScript.AccumulateMetric(_shotsFired, shotsFiredPool);
                shotsFiredPool = 0;
            }

            if (shotsHitPool != 0)
            {
                managerScript.AccumulateMetric(_shotsHit, shotsHitPool);
                shotsHitPool = 0;
            }

            if (powerUpSpawned)
            {
                managerScript.AddMetricSample<float>(_powerUpInterval, Time.timeSinceLevelLoad);
                powerUpSpawned = false;
            }

            if (powerUpsHitPool != 0)
            {
                managerScript.AccumulateMetric(_powerUpsHit, powerUpsHitPool);
                powerUpsHitPool = 0;
            }

            if (rareBearsSpawnedPool != 0)
            {
                managerScript.AccumulateMetric(_rareBearsSpawned, rareBearsSpawnedPool);
                rareBearsSpawnedPool = 0;
            }

            if (rareBearsHitPool != 0)
            {
                managerScript.AccumulateMetric(_rareBearsHit, rareBearsHitPool);
                rareBearsHitPool = 0;
            }
        }
    }
}
