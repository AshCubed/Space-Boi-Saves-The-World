using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IngredientSpawn : MonoBehaviour
{
    [Header("Wait Time")]
    [SerializeField] private float waitTimeTillSpawn;
    private float tempWaitTime;
    [SerializeField] private Slider sldrWaitTime;
    [Header("Actual Ingredient")]
    [SerializeField] private GameObject gOIngredient;
    private Ingredient.Emotion gOIngredientEmotion;
    private Ingredient.EmotionType gOIngredientEmotionType;
    [Header("Type of Spawn Point")]
    [SerializeField] private GameObject particleEffectCloud;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject spawnPointForAngle;
    [SerializeField] private bool shouldSpawnAtAngle;
    [SerializeField] private float force;
    [SerializeField] private float angle;
    private bool currentlySpawningItem;
    [Header("If has Anim")]
    [SerializeField] private bool doesHaveAnim;
    [SerializeField] private Animator animStation;
    [SerializeField] private string trigger;
    [SerializeField] private float waitTime;

    private void Start()
    {
        tempWaitTime = waitTimeTillSpawn;
        sldrWaitTime.minValue = 0;
        sldrWaitTime.maxValue = waitTimeTillSpawn;
        sldrWaitTime.value = waitTimeTillSpawn;
        sldrWaitTime.gameObject.SetActive(false);
        currentlySpawningItem = false;
        gOIngredientEmotion = gOIngredient.GetComponent<IngredientPickUp>().ingredient.emotion;
        gOIngredientEmotionType = gOIngredient.GetComponent<IngredientPickUp>().ingredient.emotionType;
    }

    private void SpawnIngredient()
    {
        if (shouldSpawnAtAngle)
        {
            GameObject newGo = Instantiate(gOIngredient.gameObject, spawnPointForAngle.transform.position, Quaternion.identity);
            AddForceAtAngle(newGo.GetComponent<Rigidbody>(), force, angle);
        }
        else
        {
            Instantiate(gOIngredient.gameObject, spawnPoint.transform.position, Quaternion.identity);
            Instantiate(particleEffectCloud.gameObject, spawnPoint.transform.position, Quaternion.identity);
        }
    }

    IEnumerator WaitTillSpawn()
    {
        if (tempWaitTime > 0)
        {
            tempWaitTime--;
            sldrWaitTime.value = tempWaitTime;
            yield return new WaitForSeconds(1f);
            StartCoroutine(WaitTillSpawn());
        }
        else
        {
            currentlySpawningItem = false;
            sldrWaitTime.gameObject.SetActive(false);
            sldrWaitTime.value = waitTimeTillSpawn;
            tempWaitTime = waitTimeTillSpawn;
            if (doesHaveAnim)
            {
                animStation.SetTrigger(trigger);
            }
            LeanTween.delayedCall(waitTime, () => {
                switch (gOIngredientEmotionType)
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
                SpawnIngredient();
            });
        }
    }

    public void AddForceAtAngle(Rigidbody newIng, float force, float angle)
    {
        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180) * force;
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180) * force;

        newIng.AddForce(xcomponent, ycomponent, 180);
    }

    public Ingredient.Emotion StartSpawningItem()
    {
        if (!currentlySpawningItem)
        {
            currentlySpawningItem = true;
            sldrWaitTime.gameObject.SetActive(true);
            StartCoroutine(WaitTillSpawn());
        }
        return gOIngredientEmotion;
    }
}
