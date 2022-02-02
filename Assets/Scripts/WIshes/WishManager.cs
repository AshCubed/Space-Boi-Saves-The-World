using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class WishManager : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;
    [SerializeField] private Timer timer;
    [SerializeField] private List<WishTube> wishTubes;
    [SerializeField] private Slider sldrStability;
    [SerializeField] private float waitTillDestabilization;
    private int currentScore;
    private float currentStablity;
    private int totalWishesGranted;
    private int correctWishesGranted;
    private int nuclearWishesGranted;
    private int superWishesGranted;

    [Header("Random Wish Generation")]
    [SerializeField] private string stringSeed = "seed string";
    [SerializeField] private bool useStringSeed;
    [SerializeField] private int seed;
    [SerializeField] private bool randomizeSeed;


    [Header("Wishes to Shoot")]
    [SerializeField] private Transform planetHitPos;
    [SerializeField] private GameObject wishBad;
    [SerializeField] private GameObject wishBasic;
    [SerializeField] private GameObject wishGood;
    [SerializeField] private GameObject wishKaiju;

    [Header("EndGame UI")]
    [SerializeField] CanvasGroup canvasGroupFade;
    [SerializeField] Animator animGameOverUi;
    [SerializeField] string trigger;
    [SerializeField] GameObject pnlEndScreen;
    [SerializeField] TextMeshProUGUI tmpSubtitle;
    [SerializeField] TextMeshProUGUI tmpGameReport;
    [SerializeField] Button btnContinue;

    // Start is called before the first frame update
    void Start()
    {
        if (useStringSeed)
        {
            seed = stringSeed.GetHashCode();
        }

        if (randomizeSeed)
        {
            seed = Random.Range(0, 99999);
        }
        Random.InitState(seed);

        canvasGroupFade.gameObject.SetActive(true);
        canvasGroupFade.alpha = 1;
        LeanTween.alphaCanvas(canvasGroupFade, 0, 2f).setOnComplete(() => { canvasGroupFade.gameObject.SetActive(false); });

        mainManager.onGameOverCallBack += OnGameOver;
        mainManager.onGamePauseCallBack += OnGamePause;
        currentScore = 0;
        currentStablity = 60;

        totalWishesGranted = 0;
        correctWishesGranted = 0;
        nuclearWishesGranted = 0;
        superWishesGranted = 0;
        foreach (var item in wishTubes)
        {
            LeanTween.delayedCall(2f, () => item.AssignWish(new Wish(Random.Range(0, 4))));
        }
        LeanTween.delayedCall(waitTillDestabilization, () => StartCoroutine(ManipulateStability()));
    }

    private void OnDestroy()
    {
        mainManager.onGameOverCallBack -= OnGameOver;
        mainManager.onGamePauseCallBack -= OnGamePause;
    }

    private Wish GenerateNewWish()
    {
        Wish newWish = new Wish(Random.Range(0, 4));
        switch (newWish.GetWishEmotionType())
        {
            case Ingredient.EmotionType.Negative:
                AudioManager.instance.PlaySounds("Generate Bad");
                break;
            case Ingredient.EmotionType.Positive:
                AudioManager.instance.PlaySounds("Generate Good");
                break;
            case Ingredient.EmotionType.Common:
                break;
            default:
                break;
        }
        return newWish;
    }

    public void GrantWish(WishTube currentTube, Wish currentWish)
    {
        int tempScore = 0;
        int tempStabilityChange = 0;

        StopAllCoroutines();
        //Check if max ingredients has been reached to allow to grant wish
        if (currentWish.HasMaxIngredientsReached())
        {
            //Check if both ingredients are of the same emotion type
            if (currentWish._givenIngredients[0].emotion == currentWish._givenIngredients[1].emotion)
            {
                //Check if those two common are Kaijus
                //Kaiju the tubes
                if (currentWish._givenIngredients.TrueForAll(x=> x.isKaiju == true))
                {
                    Debug.Log("Kaiju Time");
                    KaijuAllWishTubes();
                    SpawnWishObjects(Ingredient.EmotionType.Common, true);
                    nuclearWishesGranted++;
                }
                //If both are just the same ingredients, stability does not move, no score gained
                else
                {
                    Debug.Log("Duplicate Ingredients");
                    ScoreAndStabilityChange(0, false);
                    SpawnWishObjects(Ingredient.EmotionType.Common);
                }
            }
            //Check if one is correct emotion, and one is common 
            //(Kaiju empowerd ingredient modifier is handled in ScoreAndStabilityChange())
            else
            {
                //If correct ingredient and binding common ingredient
                if (currentWish._givenIngredients.Find(x => x.emotion == currentWish.GetWishEmotion()) 
                    && currentWish._givenIngredients.Find(x => x.emotion == Ingredient.Emotion.Common))
                {
                    Debug.Log("Correct ingredient, has binding agent");
                    switch (currentWish.GetWishEmotion())
                    {
                        case Ingredient.Emotion.Angry:
                            ScoreAndStabilityChange(-5);
                            SpawnWishObjects(Ingredient.EmotionType.Negative);
                            break;
                        case Ingredient.Emotion.Sad:
                            ScoreAndStabilityChange(-5);
                            SpawnWishObjects(Ingredient.EmotionType.Negative);
                            break;
                        case Ingredient.Emotion.Happy:
                            ScoreAndStabilityChange(5);
                            SpawnWishObjects(Ingredient.EmotionType.Positive);
                            break;
                        case Ingredient.Emotion.Inspired:
                            ScoreAndStabilityChange(5);
                            SpawnWishObjects(Ingredient.EmotionType.Positive);
                            break;
                        case Ingredient.Emotion.Common:
                            break;
                        default:
                            break;
                    }
                    correctWishesGranted++;
                }
                //if not correct ingredient and binding common ingredient
                else if (!currentWish._givenIngredients.Find(x => x.emotion == currentWish.GetWishEmotion()) 
                    && currentWish._givenIngredients.Find(x => x.emotion == Ingredient.Emotion.Common))
                {
                    Debug.Log("Incorrect ingredient, has binding agent");
                    Ingredient ingredient = currentWish._givenIngredients.Find(x => x.emotion != currentWish.GetWishEmotion() && x.emotion != Ingredient.Emotion.Common);

                    switch (ingredient.emotion)
                    {
                        case Ingredient.Emotion.Angry:
                            ScoreAndStabilityChange(-5, false);
                            SpawnWishObjects(Ingredient.EmotionType.Negative);
                            break;
                        case Ingredient.Emotion.Sad:
                            ScoreAndStabilityChange(-5, false);
                            SpawnWishObjects(Ingredient.EmotionType.Negative);
                            break;
                        case Ingredient.Emotion.Happy:
                            ScoreAndStabilityChange(5, false);
                            SpawnWishObjects(Ingredient.EmotionType.Positive);
                            break;
                        case Ingredient.Emotion.Inspired:
                            ScoreAndStabilityChange(5, false);
                            SpawnWishObjects(Ingredient.EmotionType.Positive);
                            break;
                        case Ingredient.Emotion.Common:
                            break;
                        default:
                            break;
                    }
                }
            }

            currentStablity += tempStabilityChange;
            sldrStability.value = Mathf.RoundToInt(currentStablity);
            currentScore += tempScore;
            if (!HasPlayerFailed())
            {
                currentTube.DestroyWish();
                LeanTween.delayedCall(2f, () => currentTube.AssignWish(GenerateNewWish()));
                Debug.Log("Wish Granted", currentTube.gameObject);
                totalWishesGranted++;
            }

        }
        else
        {
            Debug.Log("NEEDS MORE INGREDIENTS");
        }
        StartCoroutine(WaitToDestabilize());

        void ScoreAndStabilityChange(int stabilityMod, bool shouldGainScore = true)
        {
            if (shouldGainScore) tempScore = ScoreBasedOnStability();
            if (currentWish._givenIngredients.Find(x => x.isKaiju))
            {
                tempStabilityChange = stabilityMod * 4;
                superWishesGranted++;
            }
            else
            {
                tempStabilityChange = stabilityMod;
            }

            int ScoreBasedOnStability()
            {
                if (currentStablity > 0 && currentStablity < 10)
                {
                    return 5;
                }
                else if (currentStablity > 10 && currentStablity < 20)
                {
                    return 10;
                }
                else if (currentStablity > 20 && currentStablity < 30)
                {
                    return 20;
                }
                else if (currentStablity > 30 && currentStablity < 40)
                {
                    return 50;
                }
                else if (currentStablity > 40 && currentStablity < 60)
                {
                    return 100;
                }
                else if (currentStablity > 60 && currentStablity < 70)
                {
                    return 50;
                }
                else if (currentStablity > 70 && currentStablity < 80)
                {
                    return 20;
                }
                else if (currentStablity > 80 && currentStablity < 90)
                {
                    return 10;
                }
                else if (currentStablity > 90 && currentStablity < 100)
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
        }
        
        void KaijuAllWishTubes()
        {
            foreach (var item in wishTubes)
            {
                item.DestroyWish();
            }
            foreach (var item in wishTubes)
            {
                LeanTween.delayedCall(2f, () => item.AssignWish(new Wish(Random.Range(0, 4))));
            }
        }

        void SpawnWishObjects(Ingredient.EmotionType emotionType, bool isKaiju = false)
        {
            if (!isKaiju)
            {
                Debug.Log("Spawned wish");
                switch (emotionType)
                {
                    case Ingredient.EmotionType.Negative:
                        WishMoveTo newGO = Instantiate(wishBad, currentTube.GetWishLaunchPos().position, Quaternion.identity)
                            .GetComponent<WishMoveTo>();
                        newGO.SetMoveToPos(planetHitPos);
                        break;
                    case Ingredient.EmotionType.Positive:
                        WishMoveTo newGO1 = Instantiate(wishGood, currentTube.GetWishLaunchPos().position, Quaternion.identity)
                            .GetComponent<WishMoveTo>();
                        newGO1.SetMoveToPos(planetHitPos);
                        break;
                    case Ingredient.EmotionType.Common:
                        WishMoveTo newGO2 = Instantiate(wishBasic, currentTube.GetWishLaunchPos().position, Quaternion.identity)
                            .GetComponent<WishMoveTo>();
                        newGO2.SetMoveToPos(planetHitPos);
                        break;
                    default:
                        break;
                }
                AudioManager.instance.PlaySounds("Launch Wish");
            }
            else
            {
                WishMoveTo newGO = Instantiate(wishKaiju, currentTube.GetWishLaunchPos().position, Quaternion.identity)
                    .GetComponent<WishMoveTo>();
                newGO.SetMoveToPos(planetHitPos);
                AudioManager.instance.PlaySounds("Launch Wish");
            }
        }
    }

    IEnumerator WaitToDestabilize()
    {
        yield return new WaitForSeconds(waitTillDestabilization);
        StartCoroutine(ManipulateStability());
    }

    IEnumerator ManipulateStability()
    {
        if (!HasPlayerFailed())
        {
            if (currentStablity > 50)
            {
                currentStablity += .5f;
            }
            else
            {
                currentStablity -= .5f;
            }
            sldrStability.value = Mathf.RoundToInt(currentStablity);
            yield return new WaitForSeconds(waitTillDestabilization);
            StartCoroutine(ManipulateStability());
        }
    }

    private bool HasPlayerFailed()
    {
        if (currentStablity >= 100 || currentStablity <= 0)
        {
            mainManager.onGameOverCallBack.Invoke(MainManager.GameOverState.STABILITY);
            Debug.Log("You had one job! YOU FAILURE");
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnGameOver(MainManager.GameOverState gameOverState)
    {
        FindObjectOfType<CursorController>().ControlCursor(CursorController.CursorState.Unlocked);
        pnlEndScreen.SetActive(true);
        StopAllCoroutines();

        string endReport = "";

        switch (gameOverState)
        {
            case MainManager.GameOverState.TIMER:
                tmpSubtitle.text = "TIMES UP";
                endReport += "Time Left: " + "00:00" + '\n';
                btnContinue.onClick.AddListener(() => {
                    SceneManager.LoadScene("WinScene");
                });
                AudioManager.instance.PlaySounds("Win");
                break;
            case MainManager.GameOverState.STABILITY:
                tmpSubtitle.text = "STABILITY OVER REACH: " + currentStablity;
                endReport += "Time Left: " + timer.GetCurrentTime() + '\n';
                btnContinue.onClick.AddListener(() => {
                    SceneManager.LoadScene("FailureScene");
                    AudioManager.instance.PlaySounds("Lose");
                });
                break;
            default:
                break;
        }
        
        endReport += "Total Wishes Granted: " + totalWishesGranted + '\n';
        endReport += "Total Correct Wishes Granted: " + correctWishesGranted + '\n';
        endReport += "Nuclear Wishes: " + nuclearWishesGranted + '\n';
        endReport += "Super Wishes: " + superWishesGranted + '\n';
        endReport += "Final Score: " + currentScore + '\n';

        tmpGameReport.text = endReport;

        animGameOverUi.SetTrigger(trigger);
    }

    private void OnGamePause(bool pauseState)
    {
        if (pauseState)
        {
            StopAllCoroutines();
        }
        else
        {
            StartCoroutine(ManipulateStability());
        }
        
    }

    #region Methods for EndReport buttons
    public void ReloadScene()
    {
        AudioManager.instance.StopAll();
        canvasGroupFade.gameObject.SetActive(true);
        canvasGroupFade.alpha = 0;
        LeanTween.alphaCanvas(canvasGroupFade, 1, 2f).setOnComplete(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); ; });
    }

    public void LoadMainMenu()
    {
        AudioManager.instance.StopAll();
        canvasGroupFade.gameObject.SetActive(true);
        canvasGroupFade.alpha = 0;
        LeanTween.alphaCanvas(canvasGroupFade, 1, 2f).setOnComplete(() => { SceneManager.LoadScene("Main Menu"); ; });
    }

    public void ExitGame()
    {
        AudioManager.instance.StopAll();
        Application.Quit();
    }
    #endregion
}
