using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPickUp : MonoBehaviour
{
    public Ingredient ingredient;

    public void SetIngrident(Ingredient newIngredient)
    {
        ingredient = newIngredient;
    }
}
