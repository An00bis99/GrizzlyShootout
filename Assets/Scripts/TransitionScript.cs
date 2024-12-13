using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class TransitionScript : MonoBehaviour
{

    public int transitionNum;

    public bool leafTransitionIn = false;
    public bool leafTransitionOut = true;

    public Image backgroundImage;
    public Image leafTransitionLeft;
    public Image leafTransitionRight;
    public Image nextButtonImage;
    public Button nextButton;

    private bool canInteract = false;
    private bool startPressed = false;

    private Transform leftLeafTransform;
    private Transform rightLeafTransform;


    private const float TRANSITION_TIME = 0.75f;
    private float transitionTimeElapsed = 0.0f;
    private float TRANSITION_DISTANCE;

    private const float CUTSCENE_TIME = 2.5f;
    private const float NEXT_FADE_TIME = 1.0f;
    private float visibleButtonCounter = 0.0f;
    private float nextButtonAlpha = 0.0f;
    private bool buttonFade = false;
    private bool buttonFadeDone = false;

    // Level 1 to 2 transition only
    public Sprite[] backgroundArray;
    private int spriteArraySize = 0;
    private const float SCENE_TIME = 1.5f;
    private float sceneTimeCounter = 0.0f;
    private int currCutsceneImage = 1;
    private bool cutscenePlaying = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        nextButtonImage.color = new Color(nextButtonImage.color.r, nextButtonImage.color.g, nextButtonImage.color.b, 0);
        nextButton.enabled = false;
        float displayX = GetComponent<Canvas>().renderingDisplaySize.x;
        TRANSITION_DISTANCE = (displayX / 2.0f) + (displayX * 0.25f);
        leftLeafTransform = leafTransitionLeft.transform;
        rightLeafTransform = leafTransitionRight.transform;
        if (backgroundArray != null)
        {
            spriteArraySize = backgroundArray.Length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!buttonFade)
        {
            // Countup until next button starts fading in
            visibleButtonCounter += Time.deltaTime;
            if (visibleButtonCounter >= CUTSCENE_TIME)
            {
                buttonFade = true;
                visibleButtonCounter = 0.0f;
            }

        }
        else if (buttonFade && !buttonFadeDone)
        {
            nextButtonAlpha += Time.deltaTime;
            if (nextButtonAlpha < NEXT_FADE_TIME)
            {
                nextButtonImage.color = new Color(nextButtonImage.color.r, nextButtonImage.color.g, nextButtonImage.color.b, nextButtonAlpha);
            }
            else
            {
                nextButtonImage.color = new Color(nextButtonImage.color.r, nextButtonImage.color.g, nextButtonImage.color.b, 1.0f);
                buttonFadeDone = true;
                canInteract = true;
                nextButton.enabled = true;
            }

        }

        if (leafTransitionOut)
        {
            // Do transition stuff
            transitionTimeElapsed += Time.unscaledDeltaTime;
            MoveLeaves();
            if (transitionTimeElapsed >= TRANSITION_TIME)
            {
                // Transition Done
                leafTransitionOut = false;
                transitionTimeElapsed = 0.0f;
                cutscenePlaying = true;
            }
        }

        // Do leaf transition to go to next level
        if (leafTransitionIn)
        {
            transitionTimeElapsed += Time.deltaTime;
            MoveLeaves();
            if (transitionTimeElapsed >= TRANSITION_TIME)
            {
                // Transition Done
                leafTransitionIn = false;
                canInteract = true;
                LoadNextLevel();
            }
        }

        /*
        if (cutscenePlaying)
        {
            sceneTimeCounter += Time.deltaTime;
            if (sceneTimeCounter >= SCENE_TIME)
            {
                // Load next image and reset timer
                sceneTimeCounter = 0.0f;
                LoadNextCutsceneImage();
            }
        }
        */

    }

    public void NextScene()
    {
        // Loads next scene if at end of sprite array
        // Else loads next sprite
        if (canInteract)
        {
            canInteract = false;
            startPressed = true;
            if (currCutsceneImage < spriteArraySize)
            {
                // Reset button fade animation
                buttonFade = false;
                visibleButtonCounter = 0.0f;
                buttonFadeDone = false;
                nextButtonImage.color = new Color(nextButtonImage.color.r, nextButtonImage.color.g, nextButtonImage.color.b, 0.0f);
                nextButtonAlpha = 0.0f;
                nextButton.enabled = false;
                LoadNextCutsceneImage();
            }
            else
            {
                leafTransitionIn = true;
            }
        }
    }

    private void MoveLeaves()
    {
        Vector3 leftLeafPos = leftLeafTransform.position;
        Vector3 rightLeafPos = rightLeafTransform.position;

        if (leafTransitionIn)
        {
            leftLeafPos.x += TRANSITION_DISTANCE * (1.0f / TRANSITION_TIME) * Time.unscaledDeltaTime;
            rightLeafPos.x += -TRANSITION_DISTANCE * (1.0f / TRANSITION_TIME) * Time.unscaledDeltaTime;
        }
        else
        {
            leftLeafPos.x += -TRANSITION_DISTANCE * (1.0f / TRANSITION_TIME) * Time.unscaledDeltaTime;
            rightLeafPos.x += TRANSITION_DISTANCE * (1.0f / TRANSITION_TIME) * Time.unscaledDeltaTime;
        }

        leftLeafTransform.position = leftLeafPos;
        rightLeafTransform.position = rightLeafPos;
    }

    private void LoadNextLevel()
    {
        switch (transitionNum)
        {
            case 0:
                SceneManager.LoadScene("Level 1");
                break;
            case 1:
                SceneManager.LoadScene("Level 2");
                break;
            case 2:
                SceneManager.LoadScene("Level 3");
                break;
            case 3:
                // Uncomment this to replace the current call when credits are implemented
                SceneManager.LoadScene("Credits");
                break;
            case 4:
                SceneManager.LoadScene("Main Menu");
                break;
        }
    }

    private void LoadNextCutsceneImage()
    {
        if (currCutsceneImage < spriteArraySize - 1)
        {
            currCutsceneImage++;
            backgroundImage.sprite = backgroundArray[currCutsceneImage];
        }
        else
        {
            backgroundImage.sprite = backgroundArray[currCutsceneImage];
            currCutsceneImage++;
            buttonFade = true;
            cutscenePlaying = false;
        }
    }
}
