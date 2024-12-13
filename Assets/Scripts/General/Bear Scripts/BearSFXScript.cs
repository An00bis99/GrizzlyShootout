using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSFXScript : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audioPlayer;
    public AudioClip[] randomClips;
    public bool pinkHit = false;
    public bool isInitialized;

    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isInitialized)
        {
            isInitialized = false;
            PlayAudioClip();
        }
    }

    private void PlayAudioClip()
    {
        int clipIndex;
        if (pinkHit)
        {
            // Pick last sound (the wrong sound)
            clipIndex = randomClips.Length - 1;
        }
        else
        {
            clipIndex = Random.Range(0, randomClips.Length - 1);
        }
        audioPlayer.clip = randomClips[clipIndex];
        audioPlayer.Play();
        Destroy(gameObject, 2f);
    }
}
