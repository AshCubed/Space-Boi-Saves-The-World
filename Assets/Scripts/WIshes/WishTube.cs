using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class WishTube : MonoBehaviour
{
    [Header("Wish Tube Data")]
    [SerializeField] private WishManager wishManager;
    [SerializeField] private Wish currentWish;
    [SerializeField] private Transform wishLaunchPos;
    [Header("Current Wish UI")]
    [SerializeField] private TextMeshProUGUI tmpIngredientCount;
    [SerializeField] private Image imgWish;
    [SerializeField] private Image imgWishEmpty;
    [SerializeField] private Sprite imgWishAngry;
    [SerializeField] private Sprite imgWishHappy;
    [SerializeField] private Sprite imgWishInspired;
    [SerializeField] private Sprite imgWishSad;
    [Header("Resource Floating Up Anim")]
    [SerializeField] private string animPlay;
    [SerializeField] private Vector3 wishGoingUpSpawnPos;
    [SerializeField] private GameObject wishGoingUp;


    [SerializeField] private float timeToWaitForNewIngredient;
    private bool canTakeNewIngredient;

    private void Start()
    {
        imgWish.gameObject.SetActive(false);
        imgWishEmpty.gameObject.SetActive(true);
        canTakeNewIngredient = true;
        TextIngredientAmntUpdate(0);
    }

    public void AssignWish(Wish incomingWish)
    {
        currentWish = incomingWish;
        TextIngredientAmntUpdate(currentWish._givenIngredients.Count);
        imgWishEmpty.gameObject.SetActive(false);
        imgWish.gameObject.SetActive(true);
        switch (incomingWish.GetWishEmotion())
        {
            case Ingredient.Emotion.Angry:
                imgWish.sprite = imgWishAngry;
                break;
            case Ingredient.Emotion.Sad:
                imgWish.sprite = imgWishSad;
                break;
            case Ingredient.Emotion.Happy:
                imgWish.sprite = imgWishHappy;
                break;
            case Ingredient.Emotion.Inspired:
                imgWish.sprite = imgWishInspired;
                break;
            case Ingredient.Emotion.Common:
                break;
            default:
                break;
        }
    }

    public Wish GetWish()
    {
        return currentWish;
    }

    public Transform GetWishLaunchPos()
    {
        return wishLaunchPos;
    }

    public void DestroyWish()
    {
        currentWish = null;
        imgWish.gameObject.SetActive(false);
        imgWishEmpty.gameObject.SetActive(true);
        tmpIngredientCount.text = "AWAITING NEW WISH";
    }

    public bool CanDropIngredient()
    {
        bool ndd = !currentWish.HasMaxIngredientsReached();
        if (ndd == false)
        {
            TextReadyForLaunch();
        }
        return ndd;
    }

    public void InsertIngredient(GameObject gOIngredient, Ingredient ingredient)
    {
        gOIngredient.SetActive(false);
        GameObject resourceParentAnim = Instantiate(wishGoingUp, transform.position + wishGoingUpSpawnPos, Quaternion.identity, gameObject.transform);
        gOIngredient.transform.SetParent(resourceParentAnim.transform.GetChild(0).transform);
        gOIngredient.transform.position = Vector3.zero;
        gOIngredient.transform.position = resourceParentAnim.transform.position;
        LeanTween.delayedCall(.5f, () => {
            gOIngredient.SetActive(true);
            resourceParentAnim.transform.GetChild(0).GetComponent<Animator>().SetTrigger(animPlay);
            LeanTween.delayedCall(2f, () =>
            {
                if (!currentWish.HasMaxIngredientsReached())
                {
                    if (canTakeNewIngredient)
                    {
                        Destroy(resourceParentAnim);
                        canTakeNewIngredient = false;
                        currentWish.AddIngredient(ingredient);
                        LeanTween.delayedCall(timeToWaitForNewIngredient, () => canTakeNewIngredient = true);
                        tmpIngredientCount.color = Color.green;
                        LeanTween.delayedCall(timeToWaitForNewIngredient, () => { tmpIngredientCount.color = Color.white; });
                        TextIngredientAmntUpdate(currentWish._givenIngredients.Count);
                        if (currentWish.HasMaxIngredientsReached())
                        {
                            TextReadyForLaunch();
                        }
                    }
                }
            });
        });
    }

    public void TextIngredientAmntUpdate(int ingredientAmnt)
    {
        tmpIngredientCount.text = "Ingredients: " + ingredientAmnt;
    }

    public void TextReadyForLaunch()
    {
        tmpIngredientCount.color = Color.green;
        tmpIngredientCount.text = "READY FOR LAUNCH";
        LeanTween.delayedCall(timeToWaitForNewIngredient, () => {
            tmpIngredientCount.color = Color.white;
            tmpIngredientCount.text = "Ingredients: " + currentWish._givenIngredients.Count;
        });
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + wishGoingUpSpawnPos, .5f);
    }*/
}
