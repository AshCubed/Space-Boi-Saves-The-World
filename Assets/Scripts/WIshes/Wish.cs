using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wish
{
    [SerializeField] private Ingredient.Emotion _emotion;
    [SerializeField] private Ingredient.EmotionType _emotionType;

    public List<Ingredient> _givenIngredients { get; private set; }

    public Wish(int emotion)
    {
        _givenIngredients = new List<Ingredient>();
        switch (emotion)
        {
            case 0:
                _emotion = Ingredient.Emotion.Angry;
                _emotionType = Ingredient.EmotionType.Negative;
                break;
            case 1:
                _emotion = Ingredient.Emotion.Sad;
                _emotionType = Ingredient.EmotionType.Negative;
                break;
            case 2:
                _emotion = Ingredient.Emotion.Happy;
                _emotionType = Ingredient.EmotionType.Positive;
                break;
            case 3:
                _emotion = Ingredient.Emotion.Inspired;
                _emotionType = Ingredient.EmotionType.Positive;
                break;
            default:
                break;
        }
    }

    public Ingredient.Emotion GetWishEmotion()
    {
        return _emotion;
    }

    public Ingredient.EmotionType GetWishEmotionType()
    {
        return _emotionType;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (_givenIngredients.Count < 2)
        {
            _givenIngredients.Add(ingredient);
        }
        else
        {
            Debug.Log("Ingredients Full for this wish");
        }
    }

    public bool HasMaxIngredientsReached()
    {
        if (_givenIngredients.Count < 2)
        {
            return false;
        }
        else
        {
            Debug.Log("Ingredients Full for this wish");
            return true;
        }
    }

    

    public void ResetIngredients()
    {
        _givenIngredients = new List<Ingredient>();
    }
}
