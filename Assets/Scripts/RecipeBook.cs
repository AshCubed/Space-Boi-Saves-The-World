using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] public CanvasGroup recipeUi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            recipeUi.gameObject.SetActive(true);
            LeanTween.alphaCanvas(recipeUi, 1, 0.2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LeanTween.alphaCanvas(recipeUi, 0, 0.2f).setOnComplete(() => { recipeUi.gameObject.SetActive(false); });
        }
    }
}
