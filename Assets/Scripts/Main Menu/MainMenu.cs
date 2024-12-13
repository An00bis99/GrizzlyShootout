using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Camera mainCam;

    public Image blackOverlay;
    public Image leafTransitionLeft;
    public Image leafTransitionRight;

    private Transform leftLeafTransform;
    private Transform rightLeafTransform;

    private bool idleMenu = true;
    public bool canInteract = false;

    private bool settingsPressed = false;
    private bool inSettings = false;

    public bool startPressed = false;
    public bool exitPressed = false;

    public bool backPressed = false;

    public bool blackFadingOut = true; // Initial black fade out when starting application
    public bool blackFadingIn = false;
    private const float FADE_TIME = 2.0f;
    private float blackOverlayAlpha = 1.0f;

    public bool leafTransitionIn = false;
    public bool leafTransitionOut = false;
    private const float TRANSITION_TIME = 0.75f;
    private float transitionTimeElapsed = 0.0f;
    private const float TRANSITION_DISTANCE_OLD = 1338.0f; // Moves leaves to cover whole screen
    private float TRANSITION_DISTANCE;

    void Start()
    {
        float displayX = GetComponent<Canvas>().renderingDisplaySize.x;
        TRANSITION_DISTANCE = (displayX / 2.0f) + (displayX * 0.25f);
        Cursor.lockState = CursorLockMode.Locked;
        leftLeafTransform = leafTransitionLeft.transform;
        rightLeafTransform = leafTransitionRight.transform;

    }

    // Update is called once per frame
    void Update()
    {
        // Do black fade in transition when exiting game
        if (blackFadingIn)
        {
            blackOverlayAlpha += (1.0f / FADE_TIME) * Time.deltaTime;

            // Increase opacity to a max of 1.0
            if (blackOverlayAlpha < 1.0f)
            {
                blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, blackOverlayAlpha);

            }
            else
            {
                blackOverlayAlpha = 1.0f;
                blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, blackOverlayAlpha);
                // Done fading, so exit out
                blackFadingIn = false;
                Application.Quit();
            }
        }

        // Do black fade out upon entering this scene, also keep cursor locked while transition is happening
        if (blackFadingOut)
        {
            blackOverlayAlpha -= (1.0f / FADE_TIME) * Time.deltaTime;

            if (blackOverlayAlpha > 0.0f)
            {
                blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, blackOverlayAlpha);

            }
            else
            {
                blackOverlayAlpha = 0.0f;
                blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, blackOverlayAlpha);
                // Done fading, so let the player interact;
                canInteract = true;
                Cursor.lockState = CursorLockMode.None;
                blackFadingOut = false;
            }
        }

        // Do leaf transition in
        if (leafTransitionIn)
        {
            transitionTimeElapsed += Time.deltaTime;
            MoveLeaves();
            if (transitionTimeElapsed >= TRANSITION_TIME)
            {
                // Transition Done
                leafTransitionIn = false;
                canInteract = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("Transition Main to LvL1");
            }
        }
    }

    public void StartButtonPress()
    {
        // Tell the UI to do the leaf transition in the overlay and lock the user
        // Only execute if user is allowed to click on things
        if (canInteract)
        {
            canInteract = false;
            leafTransitionIn = true;
            startPressed = true;
        }
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    private void MoveLeaves()
    {
        Vector3 leftLeafPos = leftLeafTransform.position;
        Vector3 rightLeafPos = rightLeafTransform.position;

        leftLeafPos.x += TRANSITION_DISTANCE * (1.0f / TRANSITION_TIME) * Time.deltaTime;
        rightLeafPos.x += -TRANSITION_DISTANCE * (1.0f / TRANSITION_TIME) * Time.deltaTime;

        leftLeafTransform.position = leftLeafPos;
        rightLeafTransform.position = rightLeafPos;
    }
}
