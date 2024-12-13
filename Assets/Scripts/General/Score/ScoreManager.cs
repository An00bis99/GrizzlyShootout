using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEditor;

public class ScoreManager : MonoBehaviour

{
    public int nextScore = 0;
    public bool doublePoints = false;
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameEndText;
    public TextMeshProUGUI goalScoreText;
    public UnityEngine.UI.Image gameOverOverlay;
    public UnityEngine.UI.Image hitOverlay;
    public UnityEngine.UI.Image pauseOverlay;
    public UnityEngine.UI.Image hintOverlay;
    public UnityEngine.UI.Image pointValues;
    public UnityEngine.UI.Image pauseButton;
    public UnityEngine.UI.Button unpauseDetection;
    public UnityEngine.UI.Image exitButton;
    public UnityEngine.UI.Button exitDetection;
    public UnityEngine.UI.Image hintScreenOverlay;
    public UnityEngine.UI.Image hintSkipButton;
    public UnityEngine.UI.Button hintSkipDetection;

    //public TextMeshProUGUI[] bearTallyText;
    public int[] bearTallyNum = new int[6];

    public UnityEngine.UI.Image leafTransitionLeft;
    public UnityEngine.UI.Image leafTransitionRight;

    public AudioClip[] audioClips;
    private AudioSource audioSource;

    private Transform leftLeafTransform;
    private Transform rightLeafTransform;

    public int currLevel;
    private int score = 0;
    private int scoreGoalLevel1 = 50;
    private int scoreGoalLevel2 = 75;
    private int scoreGoalLevel3 = 85;
    private float level1Timer = 25.0f;
    private float level2Timer = 50.0f;
    private float level3Timer = 60.0f;
    private float currTime;
    private int nextTime = 49;
    private float gameStartTime = 5.0f;
    private bool hintSkipped = true;

    private bool gameDone = false;
    private float gameOverDuration = 2.5f;
    private float gameOverAlpha = 0.0f;

    private bool triggerHit = false;
    private bool reverseOverlay = false;
    private const float hitDuration = 0.75f;
    private float hitOverlayAlpha = 0.0f;

    private bool pointBoardFadeOut = false;
    private float pointBoardAlpha = 1.0f;
    private bool gameStarting = false;

    public bool leafTransitionIn = false;
    public bool leafTransitionOut = true;
    private const float TRANSITION_TIME = 0.75f;
    private float transitionTimeElapsed = 0.0f;
    private float TRANSITION_DISTANCE; // Moves leaves to cover whole screen

    public bool isPaused = false;
    private const float PAUSE_ALPHA_AMOUNT = 0.2f;

    private bool gameWon = false;
    private bool lowTimeColorChange = false;

    public GameObject ScoreTallyObj;
    private ScoreTallyScript scoreTally;
    private bool mScoreAnimationFinished = false;
    private bool mScoreAnimStarted = false;
    private bool mSoundPlayed = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreTally = ScoreTallyObj.GetComponent<ScoreTallyScript>();

        hintSkipDetection.enabled = false;

        audioSource = GetComponent<AudioSource>();
        float displayX = GetComponent<Canvas>().renderingDisplaySize.x;
        TRANSITION_DISTANCE = (displayX / 2.0f) + (displayX * 0.25f);
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        leftLeafTransform = leafTransitionLeft.transform;
        rightLeafTransform = leafTransitionRight.transform;

        scoreText.text = "Score: " + score.ToString();
        string currLevelGoal;
        if (currLevel == 0)
        {
            currLevelGoal = scoreGoalLevel1.ToString();
            currTime = level1Timer;
            hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 0.0f);
        }
        else if (currLevel == 1)
        {
            currLevelGoal = scoreGoalLevel2.ToString();
            currTime = level2Timer;
            hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 0.0f);
        }
        else
        {
            currLevelGoal = scoreGoalLevel3.ToString();
            currTime = level3Timer;
            hintSkipped = false;
            hintScreenOverlay.color = new Color(hintScreenOverlay.color.r, hintScreenOverlay.color.g, hintScreenOverlay.color.b, 1.0f);
            hintSkipButton.color = new Color(hintSkipButton.color.r, hintSkipButton.color.g, hintSkipButton.color.b, 1.0f);
            hintSkipDetection.enabled = true;

        }
        goalScoreText.text = " Goal: " + currLevelGoal;
        timerText.text = currTime.ToString();
        gameEndText.color = new Color(gameEndText.color.r, gameEndText.color.g, gameEndText.color.b, 0);
        gameOverOverlay.color = new Color(gameOverOverlay.color.r, gameOverOverlay.color.g, gameOverOverlay.color.b, 0);
        hitOverlay.color = new Color(hitOverlay.color.r, hitOverlay.color.g, hitOverlay.color.b, 0);
        pauseOverlay.color = new Color(pauseOverlay.color.r, pauseOverlay.color.g, pauseOverlay.color.b, 0);
        pauseButton.color = new Color(pauseButton.color.r, pauseButton.color.g, pauseButton.color.b, 0);
        unpauseDetection.enabled = false;
        exitButton.color = new Color(exitButton.color.r, exitButton.color.g, exitButton.color.b, 0);
        exitDetection.enabled = false;
        Time.timeScale = 0.0f;

    }

    private void Update()
    {
        if (leafTransitionOut)
        {
            transitionTimeElapsed += Time.unscaledDeltaTime;
            MoveLeaves();
            if (transitionTimeElapsed >= TRANSITION_TIME)
            {
                // Transition Done
                leafTransitionOut = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Confined;
                UnityEngine.Cursor.visible = true;
            }
        }
        if (hintSkipped)
        {
            // Pause Game
            gameStartTime -= Time.unscaledDeltaTime;

            if (Input.GetKeyDown(KeyCode.Escape) && gameStartTime <= 0.0f && pointBoardFadeOut == false)
            {
                if (!isPaused && !gameDone)
                {
                    isPaused = true;
                    pauseOverlay.color = new Color(pauseOverlay.color.r, pauseOverlay.color.g, pauseOverlay.color.b, PAUSE_ALPHA_AMOUNT);
                    pauseButton.color = new Color(pauseButton.color.r, pauseButton.color.g, pauseButton.color.b, 1.0f);
                    hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 1.0f);
                    exitButton.color = new Color(exitButton.color.r, exitButton.color.g, exitButton.color.b, 1.0f);
                    unpauseDetection.enabled = true;
                    exitDetection.enabled = true;
                    UnityEngine.Cursor.visible = true;

                    Time.timeScale = 0.0f;
                }
            }

            /*
            if (gameStarting)
            {
                // Skip Level (Uncomment for debugging)
                CheckSkipLevel();
            }
            */

            if (gameStartTime <= 0.0f && !gameStarting)
            {
                pointBoardFadeOut = true;
                gameStarting = true;
            }

            if (pointBoardFadeOut)
            {
                pointBoardAlpha -= 0.5f * Time.unscaledDeltaTime;
                if (pointBoardAlpha > 0.0f)
                {
                    pointValues.color = new Color(pointValues.color.r, pointValues.color.g, pointValues.color.b, pointBoardAlpha);
                    if (currLevel == 2)
                    {
                        hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, pointBoardAlpha);
                    }
                }
                else
                {
                    pointBoardAlpha = 0.0f;
                    pointValues.color = new Color(pointValues.color.r, pointValues.color.g, pointValues.color.b, pointBoardAlpha);
                    if (currLevel == 2)
                    {
                        hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, pointBoardAlpha);
                    }
                    Unpause();
                    pointBoardFadeOut = false;
                }
            }
            currTime -= Time.deltaTime;

            // Hit overlay animation
            if (triggerHit)
            {
                if (!reverseOverlay)
                {
                    hitOverlayAlpha += Time.unscaledDeltaTime;
                    if (hitOverlayAlpha < hitDuration)
                    {
                        hitOverlay.color = new Color(hitOverlay.color.r, hitOverlay.color.g, hitOverlay.color.b, hitOverlayAlpha);
                    }
                    else
                    {
                        hitOverlayAlpha = hitDuration;
                        hitOverlay.color = new Color(hitOverlay.color.r, hitOverlay.color.g, hitOverlay.color.b, hitOverlayAlpha);
                        reverseOverlay = true;
                    }
                }
                else
                {
                    // Fade the animation out again
                    hitOverlayAlpha -= Time.unscaledDeltaTime;
                    if (hitOverlayAlpha > 0.0f)
                    {
                        hitOverlay.color = new Color(hitOverlay.color.r, hitOverlay.color.g, hitOverlay.color.b, hitOverlayAlpha);
                    }
                    else
                    {
                        hitOverlayAlpha = 0.0f;
                        hitOverlay.color = new Color(hitOverlay.color.r, hitOverlay.color.g, hitOverlay.color.b, hitOverlayAlpha);
                        reverseOverlay = false;
                        triggerHit = false;
                    }
                }

            }

            // Done State
            if (gameDone)
            {
                if (!mScoreAnimStarted)
                {
                    mScoreAnimStarted = true;
                    scoreTally.mBearTallyNum = bearTallyNum;
                    scoreTally.mGameEnded = true;
                    scoreTally.mGameWon = gameWon;
                }

                if (!mScoreAnimationFinished)
                {
                    mScoreAnimationFinished = scoreTally.mAnimationEnded;
                }
                else
                {
                    if (!mSoundPlayed)
                    {
                        if (gameWon)
                        {
                            audioSource.clip = audioClips[0];
                        }
                        else
                        {
                            audioSource.clip = audioClips[1];
                        }
                        audioSource.volume = 0.4f;
                        audioSource.Play();
                        mSoundPlayed = true;
                    }
                    gameOverDuration -= Time.unscaledDeltaTime;
                }
                /*
                gameOverAlpha += Time.unscaledDeltaTime;
                if (gameOverAlpha <= 1.0f)
                {
                    gameEndText.color = new Color(gameEndText.color.r, gameEndText.color.g, gameEndText.color.b, gameOverAlpha);
                    gameOverOverlay.color = new Color(gameOverOverlay.color.r, gameOverOverlay.color.g, gameOverOverlay.color.b, gameOverAlpha);
                }
                */
            }

            // Score calculation
            if (doublePoints)
            {
                score += nextScore * 2;
            }
            else
            {
                score += nextScore;
            }

            nextScore = 0;
            scoreText.text = "Score: " + score.ToString();

            // Timer calculation
            if (currTime <= nextTime)
            {
                // Set the timer to the one that the float value is closest to
                timerText.text = nextTime.ToString();
                nextTime -= 1;
            }

            if (currTime <= 10.0f && !lowTimeColorChange)
            {
                // LowTime
                timerText.color = new Color(255.0f, 0.0f, 0.0f);
                lowTimeColorChange = true;
            }

            if (currTime <= 0.0f)
            {
                // Game Over when timer reaches 0 if goal not met
                timerText.text = "0";
                if ((score >= scoreGoalLevel1 && currLevel == 0) || (score >= scoreGoalLevel2 && currLevel == 1) || (score >= scoreGoalLevel3 && currLevel == 2))
                {
                    gameEndText.text = "LEVEL WON!\n\nSCORE GOAL MET";
                    gameWon = true;
                }
                if (!gameDone)
                {
                    GameOver();
                }

            }

            if (gameOverDuration <= 0.75f && !leafTransitionIn)
            {
                // Do leaf transition in
                leafTransitionIn = true;
            }
            if (gameOverDuration <= 0.75f)
            {
                MoveLeaves();
            }

            // Game Level Changed
            if (gameOverDuration <= 0.0f)
            {
                Unpause();
                if (gameWon)
                {
                    if (currLevel == 0)
                    {
                        SceneManager.LoadScene("Transition LvL1 to LvL2");
                    }
                    else if (currLevel == 1)
                    {
                        SceneManager.LoadScene("Transition LvL2 to Lvl3");
                    }
                    else if (currLevel == 2)
                    {
                        // TODO Change this to load the end scene
                        SceneManager.LoadScene("Transition LvL3 to Credits");
                    }
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0; // Pause Game
        /*
        if (gameWon)
        {
            audioSource.clip = audioClips[0];
        }
        else
        {
            audioSource.clip = audioClips[1];
        }
        audioSource.volume = 0.4f;
        audioSource.Play();
        */
        gameDone = true;

    }

    private void Unpause()
    {
        // Simply a utility function (kinda useless at the moment)
        Time.timeScale = 1; // Unpause
    }

    // Update score
    public void AddPoint()
    {
        score += 1;
        scoreText.text = "Score: " + score.ToString();
    }

    public void TriggerHit()
    {
        triggerHit = true;
        reverseOverlay = false;
        hitOverlayAlpha = 0.0f;
        int choice = UnityEngine.Random.Range(2, 4);
        audioSource.clip = audioClips[choice];
        audioSource.volume = 0.3f;
        audioSource.Play();
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

    public void UnpausePressed()
    {
        // This is called when the unpause button is pressed within the pause menu
        isPaused = false;
        pauseOverlay.color = new Color(pauseOverlay.color.r, pauseOverlay.color.g, pauseOverlay.color.b, 0.0f);
        pauseButton.color = new Color(pauseButton.color.r, pauseButton.color.g, pauseButton.color.b, 0);
        exitButton.color = new Color(exitButton.color.r, exitButton.color.g, exitButton.color.b, 0);
        hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 0.0f);
        unpauseDetection.enabled = false;
        exitDetection.enabled = false;
        Time.timeScale = 1.0f;
        UnityEngine.Cursor.visible = false;
    }

    public void ExitPressed()
    {
        Time.timeScale = 1.0f;
        UnityEngine.Cursor.visible = false;
        Application.Quit();
    }

    public void MainMenuPressed()
    {
        Time.timeScale = 1.0f;
        UnityEngine.Cursor.visible = false;
        SceneManager.LoadScene("Main Menu");
    }

    private void CheckSkipLevel()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.M))
        {
            int sceneNum = SceneManager.GetActiveScene().buildIndex;
            sceneNum++;
            /*
            if (sceneNum == 3)
            {
                sceneNum = 0;
            }
            else
            {
                sceneNum++;
            }
            */
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(sceneNum);
        }
    }

    public void SkipHint()
    {
        hintScreenOverlay.color = new Color(hintScreenOverlay.color.r, hintScreenOverlay.color.g, hintScreenOverlay.color.b, 0.0f);
        hintSkipButton.color = new Color(hintSkipButton.color.r, hintSkipButton.color.g, hintSkipButton.color.b, 0.0f);
        hintSkipped = true;
        UnityEngine.Cursor.visible = false;
        hintSkipDetection.enabled = false;
    }
}
