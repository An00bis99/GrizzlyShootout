using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class BearManager : MonoBehaviour
{
    private List<GameObject> bearsInScene = new List<GameObject>();

    // Update is called once per frame
    private void Update()
    {

    }

    public void ScareBearsInRadius(Vector3 bearPos)
    {
        for (int i = 0; i < bearsInScene.Count; i++)
        {
            GameObject currBear = bearsInScene[i];
            if (Vector3.Distance(currBear.transform.position, bearPos) < 5.0f)
            {
                // Scare the bear if it is within 5 units of distance and isn't the one that got shot
                currBear.GetComponent<TargetScriptV2>().Scare();
            }
        }
    }

    public void Explode(Vector3 bearPos)
    {
        for (int i = 0; i < bearsInScene.Count; i++)
        {
            GameObject currBear = bearsInScene[i];
            if (Vector3.Distance(currBear.transform.position, bearPos) < 5.0f)
            {
                // Kill bear if within explosion radius
                TargetScriptV2 currScript = currBear.GetComponent<TargetScriptV2>();
                currScript.Explode();
            }
        }
    }

    public void AddBear(GameObject currBear)
    {
        bearsInScene.Add(currBear);
    }

    public void RemoveBear(GameObject currBear)
    {
        bearsInScene.Remove(currBear);
    }
}
