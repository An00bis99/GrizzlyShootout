using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreTallyScript : MonoBehaviour
{
    // This class is meant to display at the end of a level and shows how the player performed
    // including if they beat the level or not

    // Stores the text boxes of every bear in order
    public TextMeshProUGUI[] mTallyText;

    public ParticleSystem mParticleSystem; // To display effects where score pops up

    public bool mGameEnded = false; // Score Manager sets this to tell this script to start displaying info
    public bool mGameWon = false;
    public bool mAnimationEnded = false; // Score Manager reads this to know when to display next button and let player interact

    public int[] mBearTallyNum = new int[6];

    public AudioClip mSoundClip;
    private AudioSource mAudio;

    private const float TEXT_ANIMATION_TIME = 0.75f;

    private const float OVERLAY_FADE_TIME = 0.5f;
    private float mOverlayAlpha = 0.0f;

    private float mTextAnimTimer = 0.0f;
    private int mCurrIdx = 0;
    private bool mShotSoundPlayed = false;
    private Vector3 mAnchorPoint = Vector3.zero;

    private const float COMPLETION_ANIMATION_TIME = 1.0f;
    private float mCompletionTimer = 0.0f;
    private bool mCompletionAnimationStarted = false;

    public UnityEngine.UI.Image mOverlay;
    private Color mOverlayColor;

    public UnityEngine.UI.Image mWinImage;
    public UnityEngine.UI.Image mLoseImage;

    void Start()
    {
        mParticleSystem = GetComponent<ParticleSystem>();
        mAudio = GetComponent<AudioSource>();
        mAudio.clip = mSoundClip;
        mAudio.volume = 0.3f;
        mOverlayColor = mOverlay.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (mGameEnded && !mCompletionAnimationStarted)
        {
            if (mOverlayAlpha < 1.0f)
            {
                DoOverlayFade();
            }
            else
            {
                DoTextAnimation();
            }
        }

        if (mCompletionAnimationStarted && !mAnimationEnded)
        {
            CountdownCompletion();
        }
    }

    private void DisplayWinText()
    {
        Color currColor = mWinImage.color;
        mWinImage.color = new Color(currColor.r, currColor.g, currColor.b, 1.0f);
    }

    private void DisplayLoseText()
    {
        Color currColor = mLoseImage.color;
        mLoseImage.color = new Color(currColor.r, currColor.g, currColor.b, 1.0f);
    }

    private void DoTextAnimation()
    {
        if (mTextAnimTimer < TEXT_ANIMATION_TIME)
        {
            if (!mShotSoundPlayed)
            {
                // Play shot sound and do inital animation
                mAudio.Play();
                mShotSoundPlayed = true;
                // Change text to match bear tally
                mTallyText[mCurrIdx].text = "x" + mBearTallyNum[mCurrIdx];
                mTallyText[mCurrIdx].color = new Color(mTallyText[mCurrIdx].color.r, mTallyText[mCurrIdx].color.g, mTallyText[mCurrIdx].color.b, 1.0f);

                // Setup for text shake
                RectTransform rt = mTallyText[mCurrIdx].rectTransform;
                mAnchorPoint = rt.anchoredPosition;
                // Call particle system script to display particles at this location
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.startColor = Color.black;
                emitParams.startSize = 100.0f;
                emitParams.position = mAnchorPoint;
                emitParams.startLifetime = 0.75f;

                mParticleSystem.Emit(emitParams, 5);
            }
            DoTextShake();
            mTextAnimTimer += Time.unscaledDeltaTime * (1.0f / TEXT_ANIMATION_TIME);
        }
        else
        {
            mTextAnimTimer = 0.0f;

            RectTransform rt = mTallyText[mCurrIdx].rectTransform;
            rt.anchoredPosition = mAnchorPoint;

            mShotSoundPlayed = false;
            mCurrIdx++;
            if (mCurrIdx == mTallyText.Length)
            {
                // Done with text animations, display completion text
                // Start playing drumroll
                mCompletionAnimationStarted = true;
            }
        }
    }

    private void DoOverlayFade()
    {
        mOverlayAlpha += Time.unscaledDeltaTime * (1 / OVERLAY_FADE_TIME);

        if (mOverlayAlpha > 1.0f)
        {
            // Cap the overlay alpha
            mOverlayAlpha = 1.0f;
        }
        mOverlayColor = new Color(mOverlayColor.r, mOverlayColor.g, mOverlayColor.b, mOverlayAlpha);
        mOverlay.color = mOverlayColor;
    }

    private void DoTextShake()
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 10.0f;
        randomPos.z = 0.0f;
        mTallyText[mCurrIdx].rectTransform.anchoredPosition = mAnchorPoint + randomPos;
    }

    private void CountdownCompletion()
    {
        if (mCompletionTimer < COMPLETION_ANIMATION_TIME)
        {
            mCompletionTimer += Time.unscaledDeltaTime * (1.0f / COMPLETION_ANIMATION_TIME);
        }
        else
        {
            mAnimationEnded = true;
            if (mGameWon)
            {
                DisplayWinText();
            }
            else
            {
                DisplayLoseText();
            }
        }
    }
}
