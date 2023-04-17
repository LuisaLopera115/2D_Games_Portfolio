using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public GameObject[] ingredients;
    public bool postioncompleted;
    // Start is called before the first frame update
    void Start()
    {

    }


    public void ChechifComplete(){

        postioncompleted = true;

        foreach (var ingredient in ingredients)
        {
            if (!ingredient.GetComponent<IngredientProp>().completed)
            {
                postioncompleted = false;
            }
        }

        if (postioncompleted)
        {
            foreach (var ingredient in ingredients)
            {
                ingredient.GetComponent<IngredientProp>().cauldronIngredient = true;
                ingredient.GetComponent<SpriteRenderer>().sprite = null;
            }
            
            //CauldronManager.ShareInstance.RefillCauldron();
        }
    }
}
