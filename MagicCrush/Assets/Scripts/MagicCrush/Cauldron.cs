using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour
{
    public GameObject[] ingredients;
    public bool postioncompleted;
    public Sprite EmtyCircle;
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
            GUIManager.sharedInstance.Score += 100;
            foreach (var ingredient in ingredients)
            {
                ingredient.GetComponent<IngredientProp>().cauldronIngredient = true;
                ingredient.GetComponent<IngredientProp>().circle.color = new Color(ingredient.GetComponent<IngredientProp>().circle.color.r,
                                                                            ingredient.GetComponent<IngredientProp>().circle.color.g,
                                                                            ingredient.GetComponent<IngredientProp>().circle.color.g,
                                                                            0f);
                ingredient.GetComponent<Image>().sprite = EmtyCircle;
            }
            CauldronManager.ShareInstance.RefillCauldron();
        }
    }
}
